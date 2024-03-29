﻿using DynamicDataDisplay.ViewModelTypes;
using SharpDX;
using SharpDX.Direct3D9;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DynamicDataDisplay.SharpDX9
{
    public abstract class BaseDxInstancedTexturePrimitive<DxVertex, TDxInstance> : BaseDxTexturePrimitive<DxVertex> where DxVertex : struct, ID3Point where TDxInstance : struct, ID3Point
    {
        private IndexBuffer _indexBuffer = null; 
        private VertexBuffer _instanceBuffer = null;
        private int _instanceBufferAllocated = 0;
        private int _instanceCount = 0;
        private TDxInstance[] _instanceList;
        private Texture _texture;

        public override void OnPlotterAttached(Plotter plotter)
        {
            base.OnPlotterAttached(plotter);

            var tdx = new DxVertex();
            var vel = tdx.GetVertexElements();
            var vee = vel.Where(v => (!v.Equals(VertexElement.VertexDeclarationEnd)));
            var tdxve = new TDxInstance().GetVertexElements();

            var vertexElements =
                new DxVertex().GetVertexElements().Where(ve => (!ve.Equals(VertexElement.VertexDeclarationEnd)))
                .Concat(new TDxInstance().GetVertexElements())
                .ToArray();
            _vertexDeclaration = new VertexDeclaration(Device, vertexElements);
            this._texture = SharedTexture?.GetTexture(Device);
        }

        public override void OnPlotterDetaching(Plotter plotter)
        {
            _instanceBuffer?.Dispose();
            base.OnPlotterDetaching(plotter);
        }

        protected override bool UpdateVertexBufferFromGeometrySource(IEnumerable<DxVertex> newPoints)
        {
            var vertexBufferSizeChanged = base.UpdateVertexBufferFromGeometrySource(newPoints);

            if (DxHost == null)
            {
                return vertexBufferSizeChanged;
            }

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
            _instanceList = (newInstances ?? Enumerable.Empty<TDxInstance>()).ToArray();
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
            Device.SetStreamSource(0, _vertexBuffer, 0, Utilities.SizeOf<DxVertex>());
            Device.SetStreamSource(1, _instanceBuffer, 0, Utilities.SizeOf<TDxInstance>());
            Device.VertexDeclaration = _vertexDeclaration;
            Device.Indices = _indexBuffer;

            _transformEffect.SetTexture(_texture);
            _transformEffect.DoMultipassEffect(width, height, this, passNo =>
            {
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
            DependencyProperty.Register("Positions", typeof(IEnumerable<TDxInstance>), typeof(BaseDxInstancedTexturePrimitive<DxVertex, TDxInstance>), new PropertyMetadata(null, (s, e) =>
            {
                if (s is BaseDxInstancedTexturePrimitive<DxVertex, TDxInstance> control && e.NewValue is IEnumerable<TDxInstance> newPositions)
                {
                    control.UpdateInstanceBufferFromPositions(newPositions);
                }
            }));

    }
}
