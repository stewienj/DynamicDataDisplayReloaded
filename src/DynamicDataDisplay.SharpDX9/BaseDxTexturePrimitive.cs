using DynamicDataDisplay.Common.Auxiliary;
using DynamicDataDisplay.ViewModelTypes;
using DynamicDataDisplay.SharpDX9.Helpers;
using DynamicDataDisplay.SharpDX9.Shaders;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace DynamicDataDisplay.SharpDX9
{
    public abstract class BaseDxTexturePrimitive<DxVertex> : BaseDxChartElement where DxVertex : struct, ID3Point
	{
		protected VertexBuffer _vertexBuffer = null;
		protected int _vertexBufferAllocated = 0;
		protected int _vertexCount = 0;
		private SynchronizationContext _syncContext = null;
		protected VertexDeclaration _vertexDeclaration;
		protected DxRectangleTexturedShader _transformEffect;
		private DxVertex[] _pointList;
		private Texture _texture;
		// Limit updates to 100 times per second. This also schedules updates to another thread.
		private ThrottledAction _throttledAction = new ThrottledAction(TimeSpan.FromMilliseconds(10));

		public override void OnPlotterAttached(Plotter plotter)
		{
			_syncContext = SynchronizationContext.Current;

			base.OnPlotterAttached(plotter);

			_transformEffect = GetTransformEffect(Device) as DxRectangleTexturedShader;

			// Creates and sets the Vertex Declaration
			_vertexDeclaration = new VertexDeclaration(Device, new DxVertex().GetVertexElements());

			_texture = BaseDxHelpers.GetDVDImageTexture(Device);
		}

		public override void OnPlotterDetaching(Plotter plotter)
		{
			_vertexBuffer?.Dispose();
			base.OnPlotterDetaching(plotter);
		}

		protected virtual bool UpdateVertexBufferFromGeometrySource(IEnumerable<DxVertex> newPoints)
		{
			bool vertexBufferSizeChanged = false;

			// Vertices will be resized to the next power of 2, saves on resizing too much
			_pointList = newPoints.ToArray();
			var pointCount = _pointList.Length;

			if (DxHost.LockImage())
			{
				if (_vertexBuffer == null || pointCount > _vertexBufferAllocated || pointCount < (_vertexBufferAllocated >> 1))
				{
					_vertexBuffer?.Dispose();
					var newSize = MathHelper.CeilingPow2(pointCount);
					var size = Utilities.SizeOf<DxVertex>();
					_vertexBuffer = new VertexBuffer(Device, size * newSize, Usage.WriteOnly, VertexFormat.None, Pool.Default);
					_vertexBufferAllocated = newSize;
					vertexBufferSizeChanged = true;
				}
				// Lock the entire buffer by specifying 0 for the offset and size, throw away it's current contents
				var vertexStream = _vertexBuffer.Lock(0, 0, LockFlags.Discard);
				vertexStream.WriteRange(_pointList);
				_vertexBuffer.Unlock();
				DxHost.UnlockImage();
			}
			_vertexCount = pointCount;

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
			return vertexBufferSizeChanged;
		}

		protected override void OnDirectXRender(int width, int height)
		{
			var test = Utilities.SizeOf<DxVertex>();
			if (_vertexCount <= 0)
				return;
			Device.SetRenderState(global::SharpDX.Direct3D9.RenderState.Lighting, false);
			Device.SetRenderState(global::SharpDX.Direct3D9.RenderState.AntialiasedLineEnable, true);
			Device.SetStreamSource(0, _vertexBuffer, 0, Utilities.SizeOf<DxVertex>());
			Device.VertexDeclaration = _vertexDeclaration;

			_transformEffect.SetTexture(_texture);
			_transformEffect.DoMultipassEffect(width, height, this, passNo =>
			{
				Device.DrawPrimitives(GetPrimitiveType(), 0, _vertexCount - 1);
			});
		}

		protected abstract BaseDxTransformShader GetTransformEffect(Device device);

		protected abstract PrimitiveType GetPrimitiveType();

		public IEnumerable<DxVertex> GeometrySource
		{
			get { return (IEnumerable<DxVertex>)GetValue(GeometrySourceProperty); }
			set { SetValue(GeometrySourceProperty, value); }
		}

		// Using a DependencyProperty as the backing store for DataSource.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty GeometrySourceProperty =
			DependencyProperty.Register("GeometrySource", typeof(IEnumerable<DxVertex>), typeof(BaseDxTexturePrimitive<DxVertex>), new PropertyMetadata(null, (s, e) =>
			{
				if (s is BaseDxTexturePrimitive<DxVertex> control && e.NewValue is IEnumerable<DxVertex> newData)
				{
					control.UpdateVertexBufferFromGeometrySource(newData);
				}
			}));
	}
}
