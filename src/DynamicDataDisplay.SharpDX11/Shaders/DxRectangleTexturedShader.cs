using DynamicDataDisplay.SharpDX9.DataTypes;
using SharpDX;
using SharpDX.Direct3D9;
using System;

namespace DynamicDataDisplay.SharpDX9.Shaders
{
    public class DxRectangleTexturedShader : BaseDxTransformShader
    {
        private Texture _texture;

        public DxRectangleTexturedShader(Device device) : base(device)
        {
        }


        protected override void DoMultipassEffect(int width, int height, DataRect dataRect, float depth, DxColor color, Matrix dataTransform, Action<int> processPass)
        {
            _effect.SetTexture("g_MeshTexture", _texture);
            base.DoMultipassEffect(width, height, dataRect, depth, color, dataTransform, processPass);
        }

        public void SetTexture(Texture texture)
        {
            _texture = texture;
        }

    }
}