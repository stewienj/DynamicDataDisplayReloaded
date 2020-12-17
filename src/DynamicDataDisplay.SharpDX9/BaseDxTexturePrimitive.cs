﻿using DynamicDataDisplay.Common.Auxiliary;
using DynamicDataDisplay.SharpDX9.DataTypes;
using DynamicDataDisplay.SharpDX9.Helpers;
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
    public abstract class BaseDxTexturePrimitive<DxVertex> : BaseDxChartElement where DxVertex : struct, IDxPoint
	{
		protected VertexBuffer _vertexBuffer = null;
		protected int _vertexBufferAllocated = 0;
		protected int _vertexCount = 0;
		private SynchronizationContext _syncContext = null;
		protected VertexDeclaration _vertexDeclaration;
		protected BaseDxTransformShader _transformEffect;
		private DxVertex[] _pointList;
		private Texture _texture;
		// Limit updates to 100 times per second. This also schedules updates to another thread.
		private ThrottledAction _throttledAction = new ThrottledAction(TimeSpan.FromMilliseconds(10));

		public override void OnPlotterAttached(Plotter plotter)
		{
			_syncContext = SynchronizationContext.Current;

			base.OnPlotterAttached(plotter);

			_transformEffect = GetTransformEffect(Device);

			// Creates and sets the Vertex Declaration
			_vertexDeclaration = new VertexDeclaration(Device, new DxVertex().GetVertexElements());

			this._texture = D3D9Helper.GetDVDImageTexture(Device);
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

			// There's an issue with nVidia cards that the rendering pipeline locks up if we try to reuse
			// Vertex buffers allocated on the default pool. AMD cards seem to be ok. Work around is to use
			// the system pool, which is slow, or lock the back buffer via the target image.

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
			if (_texture != null)
			{
				_transformEffect.SetTexture(_texture);
			}
			Device.SetStreamSource(0, _vertexBuffer, 0, Utilities.SizeOf<DxVertex>());
			Device.VertexDeclaration = _vertexDeclaration;
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
