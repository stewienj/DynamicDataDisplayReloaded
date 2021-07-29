using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace DynamicDataDisplay.SharpDX11.DataTypes
{
    // A 2D data type containing x and y coordinates and texture coordinates.
    public struct DxVertex : IDxPoint
    {
        private float x;
        private float y;
        private float uvX;
        private float uvY;

        public DxVertex(float x, float y, Vector2 uv)
        {
            this.x = x;
            this.y = y;
            this.uvX = uv.X;
            this.uvY = uv.Y;
        }

        public InputElement[] GetInputElements()
        {
            // Allocate Vertex Elements
            var inputElements = new[]
            {
				// TODO check names below correspond to names in the shader file
				new InputElement("POSITION", 0, Format.R32G32_Float, 0, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 8, 0)
            };
            return inputElements;
        }

        public float X => x;

        public float Y => y;

        public float UVX => uvX;

        public float UVY => uvY;
    }
}
