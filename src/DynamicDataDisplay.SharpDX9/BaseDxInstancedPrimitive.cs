using DynamicDataDisplay.ViewModelTypes;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DynamicDataDisplay.SharpDX9
{
	/// <summary>
	/// This implements instancing of geometries. Influenced by:
	/// https://docs.microsoft.com/en-us/windows/win32/direct3d9/efficiently-drawing-multiple-instances-of-geometry
	/// https://gamedev.stackexchange.com/questions/68529/implementing-geometry-instancing-in-directx
	/// https://www.gamedev.net/forums/topic/604231-instancing-not-working-for-me-slimdx-dx9/
	/// https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/shader-model-3
	/// </summary>
	/// <typeparam name="TDxPoint"></typeparam>
	/// <typeparam name="TDxInstance"></typeparam>
	public abstract class BaseDxInstancedPrimitive<TDxPoint, TDxInstance> : BaseDxPrimitive<TDxPoint> where TDxPoint : struct, ID3Point where TDxInstance : struct, ID3Point
	{
		private IndexBuffer _indexBuffer = null; 
		private VertexBuffer _instanceBuffer = null;
		private int _instanceBufferAllocated = 0;
		private int _instanceCount = 0;
		private TDxInstance[] _instanceList;

		public override void OnPlotterAttached(Plotter plotter)
		{
			base.OnPlotterAttached(plotter);

			var vertexElements =
				new TDxPoint().GetVertexElements().Where(ve => (!ve.Equals(VertexElement.VertexDeclarationEnd)))
				.Concat(new TDxInstance().GetVertexElements())
				.ToArray();
			_vertexDeclaration = new VertexDeclaration(Device, vertexElements);
		}

		public override void OnPlotterDetaching(Plotter plotter)
		{
			_instanceBuffer?.Dispose();
			base.OnPlotterDetaching(plotter);
		}

		protected override bool UpdateVertexBufferFromGeometrySource(IEnumerable<TDxPoint> newPoints)
		{
			var vertexBufferSizeChanged = base.UpdateVertexBufferFromGeometrySource(newPoints);
			if(DxHost.LockImage())
			{
				if (vertexBufferSizeChanged)
				{
					_indexBuffer?.Dispose();
					// Create a 16 bit index buffer
					_indexBuffer = new IndexBuffer(Device, Utilities.SizeOf<int>() * _vertexBufferAllocated, Usage.WriteOnly, Pool.Default, false);
				}
				// Now set the index buffer to match

				// Lock the buffer, so that we can access the data.
				DataStream indexStream = _indexBuffer.Lock(0, 0, LockFlags.Discard);
				indexStream.WriteRange(Enumerable.Range(0, _vertexBufferAllocated).ToArray());
				// Unlock the stream again, committing all changes.
				_indexBuffer.Unlock();
				DxHost.UnlockImage();
			}
			return vertexBufferSizeChanged;
		}

		protected void UpdateInstanceBufferFromPositions(IEnumerable<TDxInstance> newInstances)
		{
			_instanceList = newInstances.ToArray();
			var instanceCount = _instanceList.Length;
			if (_instanceBuffer == null || instanceCount > _instanceBufferAllocated || instanceCount < (_instanceBufferAllocated >> 1))
			{
				_instanceBuffer?.Dispose();
				var newSize = MathHelper.CeilingPow2(instanceCount);
				_instanceBuffer = new VertexBuffer(Device, Utilities.SizeOf<TDxInstance>() * newSize, Usage.WriteOnly, VertexFormat.None, Pool.Default);
				_instanceBufferAllocated = newSize;
			}
			// Lock the entire buffer by specifying 0 for the offset and size, throw away it's current contents
			var buffer = _instanceBuffer.Lock(0, 0, LockFlags.Discard);
			buffer.WriteRange(_instanceList);
			_instanceBuffer.Unlock();
			_instanceCount = instanceCount;
		}

		protected override void OnDirectXRender(int width, int height)
		{
			if (_vertexCount <= 0)
				return;
			Device.SetRenderState(global::SharpDX.Direct3D9.RenderState.Lighting, false);
			Device.SetRenderState(global::SharpDX.Direct3D9.RenderState.AntialiasedLineEnable, true);
			Device.SetStreamSourceFrequency(0, _instanceCount, StreamSource.IndexedData);
			Device.SetStreamSourceFrequency(1, 1, StreamSource.InstanceData);
			Device.SetStreamSource(0, _vertexBuffer, 0, Utilities.SizeOf<TDxPoint>());
			Device.SetStreamSource(1, _instanceBuffer, 0, Utilities.SizeOf<TDxInstance>());
			Device.VertexDeclaration = _vertexDeclaration;
			Device.Indices = _indexBuffer;

			_transformEffect.DoMultipassEffect(width, height, this, passNo =>
			{
				// Draw everything, I can't work out what effect the 4th parameter has,
				// doesn't seem to affect anything, so I left it at zero, maybe comes into
				// effect for certain primitive types
				Device.DrawIndexedPrimitive(GetPrimitiveType(), 0, 0, 0, 0, _vertexCount - 1);
			});

			Device.ResetStreamSourceFrequency(1);
			Device.ResetStreamSourceFrequency(0);
		}

		public IEnumerable<TDxInstance> Positions
		{
			get { return (IEnumerable<TDxInstance>)GetValue(PositionsProperty); }
			set { SetValue(PositionsProperty, value); }
		}

		// Using a DependencyProperty as the backing store for DataSource.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty PositionsProperty =
			DependencyProperty.Register("Positions", typeof(IEnumerable<TDxInstance>), typeof(BaseDxInstancedPrimitive<TDxPoint, TDxInstance>), new PropertyMetadata(null, (s, e) =>
			{
				if (s is BaseDxInstancedPrimitive<TDxPoint, TDxInstance> control && e.NewValue is IEnumerable<TDxInstance> newPositions)
				{
					control.UpdateInstanceBufferFromPositions(newPositions);
				}
			}));
	}
}
