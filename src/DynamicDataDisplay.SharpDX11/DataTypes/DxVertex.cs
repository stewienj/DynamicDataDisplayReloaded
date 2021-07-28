using SharpDX;
using SharpDX.Direct3D11;

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
            var vertexElems = new[] {
                new VertexElement(0, 0, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.Position, 0),
                new VertexElement(0, 8, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
                VertexElement.VertexDeclarationEnd
            };
            return vertexElems;
        }

        public float X => x;

        public float Y => y;

        public float UVX => uvX;

        public float UVY => uvY;
    }
}
