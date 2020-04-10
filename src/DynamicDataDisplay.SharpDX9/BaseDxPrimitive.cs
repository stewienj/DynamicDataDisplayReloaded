using DynamicDataDisplay.Common.Auxiliary;
using DynamicDataDisplay.SharpDX9.DataTypes;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DynamicDataDisplay.SharpDX9
{
	public abstract class BaseDxPrimitive<TDxPoint> : BaseDxChartElement where TDxPoint : struct, IDxPoint
	{
		protected VertexBuffer _vertexBuffer = null;
		private int _vertexBufferAllocated = 0;
		protected int _vectexCount = 0;
		private SynchronizationContext _syncContext = null;
		protected VertexDeclaration _vertexDeclaration;
		protected BaseDxTransformShader _transformEffect;
		private TDxPoint[] _pointList;
		// Limit updates to 100 times per second. This also schedules updates to another thread.
		private ThrottledAction _throttledAction = new ThrottledAction(TimeSpan.FromMilliseconds(10));

		public override void OnPlotterAttached(Plotter plotter)
		{
			_syncContext = SynchronizationContext.Current;

			base.OnPlotterAttached(plotter);

			_transformEffect = GetTransformEffect(Device);

			// Creates and sets the Vertex Declaration
			_vertexDeclaration = new VertexDeclaration(Device, new TDxPoint().GetVertexElements());
		}

		public override void OnPlotterDetaching(Plotter plotter)
		{
			_vertexBuffer?.Dispose();
			base.OnPlotterDetaching(plotter);
		}


		private void UpdateVertexBufferFromDataSource(IEnumerable<TDxPoint> newPoints)
		{
			// Vertices will be resized to the next power of 2, saves on resizing too much
			_pointList = newPoints.ToArray();
			var pointCount = _pointList.Length;
			if (_vertexBuffer == null || pointCount > _vertexBufferAllocated || pointCount < (_vertexBufferAllocated >> 1))
			{
				_vertexBuffer?.Dispose();
				var newSize = MathHelper.CeilingPow2(pointCount);
				_vertexBuffer = new VertexBuffer(Device, Utilities.SizeOf<TDxPoint>() * newSize, Usage.WriteOnly, VertexFormat.None, Pool.Default);
				_vertexBufferAllocated = newSize;
			}
			// Lock the entire buffer by specifying 0 for the offset and size, throw away it's current contents
			var buffer = _vertexBuffer.Lock(0, 0, LockFlags.Discard);
			buffer.WriteRange(_pointList);
			_vertexBuffer.Unlock();
			_vectexCount = pointCount;

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
			if (_vectexCount <= 0)
				return;
			Device.SetRenderState(global::SharpDX.Direct3D9.RenderState.Lighting, false);
			Device.SetRenderState(global::SharpDX.Direct3D9.RenderState.AntialiasedLineEnable, true);
			Device.SetStreamSource(0, _vertexBuffer, 0, Utilities.SizeOf<TDxPoint>());
			Device.VertexDeclaration = _vertexDeclaration;
			_transformEffect.BeginEffect(Plotter.Viewport.Visible, DxDataTransform);
			Device.DrawPrimitives(GetPrimitiveType(), 0, _vectexCount - 1);
			_transformEffect.EndEffect();
		}

		protected abstract BaseDxTransformShader GetTransformEffect(Device device);

		protected abstract PrimitiveType GetPrimitiveType();

		public IEnumerable<TDxPoint> DataSource
		{
			get { return (IEnumerable<TDxPoint>)GetValue(DataSourceProperty); }
			set { SetValue(DataSourceProperty, value); }
		}

		// Using a DependencyProperty as the backing store for DataSource.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty DataSourceProperty =
			DependencyProperty.Register("DataSource", typeof(IEnumerable<TDxPoint>), typeof(BaseDxPrimitive<TDxPoint>), new PropertyMetadata(null, (s, e) =>
			{
				if (s is BaseDxPrimitive<TDxPoint> control && e.NewValue is IEnumerable<TDxPoint> newData)
				{
					control.UpdateVertexBufferFromDataSource(newData);
				}
			}));


		protected virtual void SetColor(DxColor color)
		{

		}

		public System.Windows.Media.Color Color
		{
			get { return (System.Windows.Media.Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}

		// Using a DependencyProperty as the backing store for LineColor.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ColorProperty =
			DependencyProperty.Register("Color", typeof(System.Windows.Media.Color), typeof(BaseDxPrimitive<TDxPoint>), new PropertyMetadata(System.Windows.Media.Colors.Black, (s, e) =>
			{
				if (s is BaseDxPrimitive<TDxPoint> control && e.NewValue is System.Windows.Media.Color newColor)
				{
					control.SetColor(new DxColor(newColor));
				}
			}));
	}
}
