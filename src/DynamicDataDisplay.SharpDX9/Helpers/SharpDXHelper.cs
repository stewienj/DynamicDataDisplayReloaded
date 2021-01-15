﻿using DynamicDataDisplay.SharpDX9.DataTypes;
using SharpDX;
using System.Collections.Generic;

namespace DynamicDataDisplay.SharpDX9.Helpers
{
    public static class SharpDXHelper
    {
        // Calculate the texture coordinates for a vertex of a quadrilateral.
        public static Vector2 CalculateRectangleUV(float x, float y, float texWidth, float texHeight)
        {
            if (texWidth == 1 && texHeight == 1)
            {
                return Vector2.Zero;
            }

            return new Vector2(x / texWidth, y / texHeight);
        }

        // Generate the vertex positions and texture coordinates of two triangles to form a rectangle.
        public static List<DxVertex> MakeRectangle(float x, float y, float width, float height)
        {
            var geometry = new List<DxVertex>();

            // Calculate texture coordinates.
            var rect1 = new DxRectangle(0f, 0f, width, height);
            var uv4 = CalculateRectangleUV(rect1.Left, rect1.Top, width, height);
            var uv3 = CalculateRectangleUV(rect1.Right, rect1.Top, width, height);
            var uv2 = CalculateRectangleUV(rect1.Right, rect1.Bottom, width, height);
            var uv1 = CalculateRectangleUV(rect1.Left, rect1.Bottom, width, height);

            // Generate a rectangle at the point's position.
            var rect2 = new DxRectangle(x, y, width, height);

            // Rectangle is made from two triangles. First is top/left half
            geometry.Add(new DxVertex(rect2.Left, rect2.Top, uv1));
            geometry.Add(new DxVertex(rect2.Right, rect2.Top, uv2));
            geometry.Add(new DxVertex(rect2.Left, rect2.Bottom, uv4));

            // Second is bottom/right half
            geometry.Add(new DxVertex(rect2.Right, rect2.Bottom, uv3));
            geometry.Add(new DxVertex(rect2.Right, rect2.Top, uv2));
            geometry.Add(new DxVertex(rect2.Left, rect2.Bottom, uv4));

            return geometry;
        }
    }
}
