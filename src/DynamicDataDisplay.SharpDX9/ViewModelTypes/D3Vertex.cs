using System.Numerics;

namespace DynamicDataDisplay.ViewModelTypes
{
    // A 2D data type containing x and y coordinates and texture coordinates.
    public struct D3Vertex : ID3Point
    {
        private float x;
        private float y;
        private float uvX;
        private float uvY;

        public D3Vertex(float x, float y, Vector2 uv)
        {
            this.x = x;
            this.y = y;
            this.uvX = uv.X;
            this.uvY = uv.Y;
        }

        public float X => x;

        public float Y => y;

        public float UVX => uvX;

        public float UVY => uvY;
    }
}
