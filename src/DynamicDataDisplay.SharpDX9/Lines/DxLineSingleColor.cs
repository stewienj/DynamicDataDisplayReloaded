using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.SharpDX9.DataSources;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.SharpDX9.Lines
{
	/// <summary>
	/// This takes a collection of points and renders a continous line in a single color
	/// </summary>
	public class DxLineSingleColor : SharpDXChartElement
	{
		private VertexBuffer _vertices = null;
		private int _vecticesCount = 0;
		private VertexDeclaration _vertexDecl;
		private DxLineSingleColorShader _transformEffect;
		private DxPoint[] _pointList;
		private int _lastVertexLength = 0;
		// Limit updates to 100 times per second. This also schedules updates to another thread.
		private ThrottledAction _throttledAction = new ThrottledAction(TimeSpan.FromMilliseconds(10));
		private SynchronizationContext _syncContext = null;

		public override void OnPlotterAttached(Plotter plotter)
		{
			_syncContext = SynchronizationContext.Current;

			base.OnPlotterAttached(plotter);

			_transformEffect = new DxLineSingleColorShader(Device);

			// Allocate Vertex Elements
			var vertexElems = new[] {
				new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Position, 0),
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

		private void UpdateFromSourceChange(IEnumerable<DxPoint> newList)
		{
			// Vertices will be resized to the next power of 2, saves on resizing too much
			_pointList = newList.ToArray();
			var pointCount = _pointList.Length;
			if (_vertices == null || pointCount > _lastVertexLength || pointCount < (_lastVertexLength >> 1))
			{
				_vertices?.Dispose();
				var newSize = MathHelper.CeilingPow2(pointCount);
				_vertices = new VertexBuffer(Device, Utilities.SizeOf<Vector4>() * newSize, Usage.WriteOnly, VertexFormat.None, Pool.Default);
				_lastVertexLength = newSize;
			}
			// Lock the entire buffer by specifying 0 for the offset and size
			var buffer = _vertices.Lock(0, 0, LockFlags.None);
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
			//Device.SetRenderState<FillMode>(global::SharpDX.Direct3D9.RenderState.FillMode, FillMode.Wireframe);
			//Device.SetRenderState(global::SharpDX.Direct3D9.RenderState.PointSize, 5.0f);

			Device.SetRenderState(global::SharpDX.Direct3D9.RenderState.Lighting, false);
			Device.SetRenderState(global::SharpDX.Direct3D9.RenderState.AntialiasedLineEnable, true);
			Device.SetStreamSource(0, _vertices, 0, Utilities.SizeOf<Vector4>());
			Device.VertexDeclaration = _vertexDecl;
			_transformEffect.BeginEffect(Plotter.Viewport.Visible, _lineColor, DxDataTransform);
			Device.DrawPrimitives(PrimitiveType.LineStrip, 0, _vecticesCount - 1);
			_transformEffect.EndEffect();
		}

		public IEnumerable<DxPoint> DataSource
		{
			get { return (IEnumerable<DxPoint>)GetValue(DataSourceProperty); }
			set { SetValue(DataSourceProperty, value); }
		}

		// Using a DependencyProperty as the backing store for DataSource.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty DataSourceProperty =
			DependencyProperty.Register("DataSource", typeof(IEnumerable<DxPoint>), typeof(DxLineSingleColor), new PropertyMetadata(null, (s, e) =>
			{
				if (s is DxLineSingleColor control && e.NewValue is IEnumerable<DxPoint> newData)
				{
					control.UpdateFromSourceChange(newData);
				}
			}));

		private DxColor _lineColor = new DxColor(System.Windows.Media.Colors.Black);
		public System.Windows.Media.Color LineColor
		{
			get { return (System.Windows.Media.Color)GetValue(LineColorProperty); }
			set { SetValue(LineColorProperty, value); }
		}

		// Using a DependencyProperty as the backing store for LineColor.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty LineColorProperty =
			DependencyProperty.Register("LineColor", typeof(System.Windows.Media.Color), typeof(DxLineSingleColor), new PropertyMetadata(System.Windows.Media.Colors.Black, (s,e)=> 
			{
				if (s is DxLineSingleColor control && e.NewValue is System.Windows.Media.Color newColor)
				{
					control._lineColor = new DxColor(newColor);
				}
			}));
	}
}
