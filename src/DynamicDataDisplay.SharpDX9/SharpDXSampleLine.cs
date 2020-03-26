using Microsoft.Research.DynamicDataDisplay.DataSources;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Linq;
using System.Windows.Threading;

namespace Microsoft.Research.DynamicDataDisplay.SharpDX9
{
	public class SharpDXSampleLine : SharpDXChart
	{
		private static int VERTEX_COUNT = 100;
		private IPointDataSource animatedDataSource;
		private readonly double[] animatedX = new double[VERTEX_COUNT];
		private readonly double[] animatedY = new double[VERTEX_COUNT];
		private readonly Vector4[] pointList = new Vector4[VERTEX_COUNT * 2];
		private double phase = 0;
		private VertexBuffer _vertices;
		private readonly DispatcherTimer timer = new DispatcherTimer();
		private TransformShader _transformEffect;

		public SharpDXSampleLine()
		{
			EnumerableDataSource<double> xSrc = new EnumerableDataSource<double>(animatedX);
			xSrc.SetXMapping(x => x);
			var yDS = new EnumerableDataSource<double>(animatedY);
			yDS.SetYMapping(y => y);
			animatedDataSource = new CompositeDataSource(xSrc, yDS);

			timer.Interval = TimeSpan.FromMilliseconds(10);
			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();
		}

		void timer_Tick(object sender, EventArgs e)
		{
			phase += 0.01;
			if (phase > 2 * Math.PI)
				phase -= 2 * Math.PI;
			for (int i = 0; i < animatedX.Length; i++)
			{
				animatedX[i] = 2 * Math.PI * i / animatedX.Length;

				if (i % 2 == 0)
					animatedY[i] = Math.Sin(animatedX[i] + phase);
				else
					animatedY[i] = -Math.Sin(animatedX[i] + phase);
			}

		}

		public override void OnPlotterAttached(Plotter plotter)
		{
			base.OnPlotterAttached(plotter);

			var transform = Plotter.Viewport.Transform;
			var bounds = BoundsHelper.GetViewportBounds(new[] { new System.Windows.Point(100, 250), new System.Windows.Point(1350, 750) }, transform.DataTransform);
			Viewport2D.SetContentBounds(this, bounds);

			_transformEffect = new TransformShader(Device);
			_vertices = new VertexBuffer(Device, Utilities.SizeOf<Vector4>() * 2 * VERTEX_COUNT, Usage.WriteOnly, VertexFormat.None, Pool.Default);

			// Allocate Vertex Elements
			var vertexElems = new[] {
				new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Position, 0),
				new VertexElement(0, 16, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Color, 0),
				VertexElement.VertexDeclarationEnd
			};

			// Creates and sets the Vertex Declaration
			var vertexDecl = new VertexDeclaration(Device, vertexElems);
			Device.SetStreamSource(0, _vertices, 0, Utilities.SizeOf<Vector4>() * 2);
			Device.VertexDeclaration = vertexDecl;
		}

		public override void OnPlotterDetaching(Plotter plotter)
		{
			_vertices.Dispose();
			base.OnPlotterDetaching(plotter);
		}

		protected override void OnDirectXRender()
		{
			var points = animatedDataSource.GetPoints().ToArray();
			for (int i = 0; i < points.Length; i++)
			{
				var color = System.Drawing.Color.Blue;
				//pointList[i * 2] = new Vector4(100 + 200 * (float)points[i].X, 500 + 250 * (float)points[i].Y, 0.5f, 1);
				pointList[i * 2] = new Vector4((float)points[i].X, (float)points[i].Y, 0.5f, 1);
				pointList[i * 2 + 1] = new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
			}

			var dataTransform = Matrix.Scaling(200f, 250f, 1f) * Matrix.Translation(100f, 500f, 0f);

			_vertices.Lock(0, 0, LockFlags.None).WriteRange(pointList);
			_vertices.Unlock();

			Device.SetRenderState(global::SharpDX.Direct3D9.RenderState.AntialiasedLineEnable, true);
			_transformEffect.BeginEffect(Plotter.Viewport.Visible, dataTransform);
			Device.DrawPrimitives(PrimitiveType.LineStrip, 0, VERTEX_COUNT-1);
			_transformEffect.EndEffect();
		}
	}
}
