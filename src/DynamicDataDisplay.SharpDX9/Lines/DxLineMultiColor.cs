using DynamicDataDisplay.Common.Auxiliary;
using DynamicDataDisplay.SharpDX9.DataSources;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DynamicDataDisplay.SharpDX9.Lines
{
	/// <summary>
	/// This takes a collection of PointAndColor objects and renders them in a continuous line
	/// </summary>
	public class DxLineMultiColor : SharpDXChartElement
	{
		private VertexBuffer _vertices = null;
		private int _vecticesCount = 0;
		private VertexDeclaration _vertexDecl;
		private DxLineMultiColorShader _transformEffect;
		private DxPointAndColor[] _pointList;
		private int _lastVertexLength = 0;
		// Limit updates to 100 times per second. This also schedules updates to another thread.
		private ThrottledAction _throttledAction = new ThrottledAction(TimeSpan.FromMilliseconds(10));
		private SynchronizationContext _syncContext = null;

		public override void OnPlotterAttached(Plotter plotter)
		{
			_syncContext = SynchronizationContext.Current;

			base.OnPlotterAttached(plotter);

			_transformEffect = new DxLineMultiColorShader(Device);

			// Allocate Vertex Elements
			var vertexElems = new[] {
				new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Position, 0),
				new VertexElement(0, 16, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Color, 0),
				VertexElement.VertexDeclarationEnd
			};

			// Creates and sets the Vertex Declaration
			_vertexDecl = new VertexDeclaration(Device, vertexElems);
		}

		public override void OnPlotterDetaching(Plotter plotter)
		{
			_vertices.Dispose();
			base.OnPlotterDetaching(plotter);
		}

		private void UpdateFromSourceChange(IEnumerable<DxPointAndColor> newList)
		{
			// Vertices will be resized to the next power of 2, saves on resizing too much
			_pointList = newList.ToArray();
			var pointCount = _pointList.Length;
			if (_vertices == null || pointCount > _lastVertexLength || pointCount < (_lastVertexLength >> 1))
			{
				_vertices?.Dispose();
				var newSize = MathHelper.CeilingPow2(pointCount);
				_vertices = new VertexBuffer(Device, Utilities.SizeOf<Vector4>() * 2 * newSize, Usage.WriteOnly, VertexFormat.None, Pool.Default);
				_lastVertexLength = newSize;
			}
			// Lock the entire buffer by specifying 0 for the offset and size, throw away it's current contents
			var buffer = _vertices.Lock(0, 0, LockFlags.Discard);
			buffer.WriteRange(_pointList);
			_vertices.Unlock();
			_vecticesCount = pointCount;

			// Calculate the bounds of the list on a background thread
			var localPointList = _pointList;
			var dataTransform = Plotter.Viewport.Transform.DataTransform;
			if (localPointList.Any())
			{
				Action resizeAction = () =>
				{
					var minX = localPointList[0].X;
					var maxX = localPointList[0].X;
					var minY = localPointList[0].Y;
					var maxY = localPointList[0].Y;
					foreach (var point in localPointList)
					{
						minX = Math.Min(minX, point.X);
						maxX = Math.Max(maxX, point.X);
						minY = Math.Min(minY, point.Y);
						maxY = Math.Max(maxY, point.Y);
					}
					var bounds = BoundsHelper.GetViewportBounds(new[] { new System.Windows.Point(minX, minY), new System.Windows.Point(maxX, maxY) }, dataTransform);
					_syncContext.Send(s =>
					{
						Viewport2D.SetContentBounds(this, bounds);
					}, null);
				};
				// Spawn action on throttled update thread
				_throttledAction.InvokeAction(resizeAction);
			}
		}

		protected override void OnDirectXRender()
		{
			if (_vecticesCount <= 0)
				return;
			Device.SetRenderState(global::SharpDX.Direct3D9.RenderState.Lighting, false);
			Device.SetRenderState(global::SharpDX.Direct3D9.RenderState.AntialiasedLineEnable, true);
			Device.SetStreamSource(0, _vertices, 0, Utilities.SizeOf<Vector4>() * 2);
			Device.VertexDeclaration = _vertexDecl;
			_transformEffect.BeginEffect(Plotter.Viewport.Visible, DxDataTransform);
			Device.DrawPrimitives(PrimitiveType.LineStrip, 0, _vecticesCount - 1);
			_transformEffect.EndEffect();
		}

		public IEnumerable<DxPointAndColor> DataSource
		{
			get { return (IEnumerable<DxPointAndColor>)GetValue(DataSourceProperty); }
			set { SetValue(DataSourceProperty, value); }
		}

		// Using a DependencyProperty as the backing store for DataSource.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty DataSourceProperty =
			DependencyProperty.Register("DataSource", typeof(IEnumerable<DxPointAndColor>), typeof(DxLineMultiColor), new PropertyMetadata(null, (s,e)=> 
			{ 
				if (s is DxLineMultiColor control && e.NewValue is IEnumerable<DxPointAndColor> newData)
				{
					control.UpdateFromSourceChange(newData);
				}
			}));
	}
}
