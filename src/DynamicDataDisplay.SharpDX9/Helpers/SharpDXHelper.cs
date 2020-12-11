using SharpDX;

namespace DynamicDataDisplay.SharpDX9.Helpers
{
    public static class SharpDXHelper
    {
        // Calculate the texture coordinates for a vertex of a quadrilateral.
        public static Vector2 CalculateUV(float x, float y, float texWidth, float texHeight)
        {
            if (texWidth == 1 && texHeight == 1)
            {
                return Vector2.Zero;
            }

            return new Vector2(x / texWidth, y / texHeight);
        }
    }
}
