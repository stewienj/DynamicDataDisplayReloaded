using DynamicDataDisplay.ViewModelTypes;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDataDisplay.SharpDX9
{
    public static class BaseDxExtensions
    {
        public static VertexElement[] GetVertexElements(this ID3Point dxPoint)
        {
            // Allocate Vertex Elements
            return dxPoint switch
            {
                D3InstancedPointAndColor => new[] {
                    new VertexElement(1, 0, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.Position, 1),
                    new VertexElement(1, 8, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 1),
                    VertexElement.VertexDeclarationEnd
                },

                D3InstancePoint => new[] {
                    new VertexElement(1, 0, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.Position, 1),
                    VertexElement.VertexDeclarationEnd
                },

                D3Point => new[] {
                    new VertexElement(0, 0, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.Position, 0),
                    VertexElement.VertexDeclarationEnd
                },

                D3PointAndColor => new[] {
                    new VertexElement(0, 0, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.Position, 0),
                    new VertexElement(0, 8, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                    VertexElement.VertexDeclarationEnd
                },

                D3Vertex => new[] {
                    new VertexElement(0, 0, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.Position, 0),
                    new VertexElement(0, 8, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
                    VertexElement.VertexDeclarationEnd
                },

                _ => throw new NotSupportedException()
            };
        }
    }
}
