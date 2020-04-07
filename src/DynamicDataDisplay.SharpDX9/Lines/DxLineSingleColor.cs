using DynamicDataDisplay.Common.Auxiliary;
using DynamicDataDisplay.SharpDX9.DataTypes;
using DynamicDataDisplay.SharpDX9.Shaders;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DynamicDataDisplay.SharpDX9.Lines
{
	/// <summary>
	/// This takes a collection of points and renders a continous line in a single color
	/// </summary>
	public class DxLineSingleColor : SharpDxPrimitive<DxPoint>
	{
		private DxPointGlobalColorShader _shader;

		protected override TransformShader GetTransformEffect(Device device)
		{
			_shader = new DxPointGlobalColorShader(Device);
			_shader.DxColor = new DxColor(LineColor);
			return _shader;
		}

		protected override VertexElement[] GetVertexElements()
		{
			// Allocate Vertex Elements
			var vertexElems = new[] {
				new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Position, 0),
				VertexElement.VertexDeclarationEnd
			};
			return vertexElems;
		}

		protected override PrimitiveType GetPrimitiveType()
		{
			return PrimitiveType.LineStrip;
		}

		public System.Windows.Media.Color LineColor
		{
			get { return (System.Windows.Media.Color)GetValue(LineColorProperty); }
			set { SetValue(LineColorProperty, value); }
		}

		// Using a DependencyProperty as the backing store for LineColor.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty LineColorProperty =
			DependencyProperty.Register("LineColor", typeof(System.Windows.Media.Color), typeof(DxLineSingleColor), new PropertyMetadata(System.Windows.Media.Colors.Black, (s,e)=> 
			{
				if (s is DxLineSingleColor control && e.NewValue is System.Windows.Media.Color newColor)
				{
					control._shader.DxColor = new DxColor(newColor);
				}
			}));
	}
}
