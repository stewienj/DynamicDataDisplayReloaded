using DynamicDataDisplay.Common.Auxiliary;
using DynamicDataDisplay.Common.Palettes;
using DynamicDataDisplay.DataSources;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Windows;
using DataSource = DynamicDataDisplay.DataSources.IDataSource2D<double>;

namespace DynamicDataDisplay.SharpDX9
{
	public class SharpDXColorMap : SharpDXChartElement
	{
		#region Properties

		#region DataSource

		public IDataSource2D<double> DataSource
		{
			get { return (IDataSource2D<double>)GetValue(DataSourceProperty); }
			set { SetValue(DataSourceProperty, value); }
		}

		public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register(
		  "DataSource",
		  typeof(IDataSource2D<double>),
		  typeof(SharpDXColorMap),
		  new FrameworkPropertyMetadata(null, OnDataSourceReplaced));

		private static void OnDataSourceReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			SharpDXColorMap owner = (SharpDXColorMap)d;
			owner.OnDataSourceReplaced((DataSource)e.OldValue, (DataSource)e.NewValue);
		}

		private void OnDataSourceReplaced(IDataSource2D<double> prevDataSource, IDataSource2D<double> currDataSource)
		{
			if (prevDataSource != null)
			{
				prevDataSource.Changed -= OnDataSourceChanged;
			}
			if (currDataSource != null)
			{
				currDataSource.Changed += OnDataSourceChanged;
			}

			FillVertexBuffer();
		}

		private void OnDataSourceChanged(object sender, EventArgs e)
		{
			FillVertexBuffer();
		}

		#endregion

		#region Palette property

		public IPalette Palette
		{
			get { return (IPalette)GetValue(PaletteProperty); }
			set { SetValue(PaletteProperty, value); }
		}

		public static readonly DependencyProperty PaletteProperty = DependencyProperty.Register(
		  "Palette",
		  typeof(IPalette),
		  typeof(SharpDXColorMap),
		  new FrameworkPropertyMetadata(new HSBPalette(), OnPaletteChanged));

		private static void OnPaletteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			SharpDXColorMap owner = (SharpDXColorMap)d;
			// todo 
		}

		#endregion // end of Palette property

		#endregion // end of Properties

		protected override void OnDirectXRender()
		{
			if (vertexBuffer == null) FillVertexBuffer();
			if (vertexBuffer == null) return;

			//Device.SetTransform(TransformState.View, /*Matrix.Translation(100, 100, 0)* */
			//    Matrix.Translation(-0.05f, -0.05f, 0) *
			//    Matrix.Scaling(3.0f, 3.0f, 1.0f)
			//    );

			//Device.SetTransform(TransformState.View, Matrix.Translation(-0.1f, -0.1f, 0));
			//Device.SetTransform(TransformState.View, camera.ViewMatrix);
			//Device.SetTransform(TransformState.Projection, camera.ProjectionMatrix);

			Device.SetRenderState(global::SharpDX.Direct3D9.RenderState.Lighting, false);
			Device.SetRenderState<FillMode>(global::SharpDX.Direct3D9.RenderState.FillMode, FillMode.Wireframe);
			Device.SetRenderState(global::SharpDX.Direct3D9.RenderState.PointSize, 5.0f);
			Device.VertexFormat = VertexFormat.Position | VertexFormat.Diffuse;
			Device.SetStreamSource(0, vertexBuffer, 0, VertexPosition4Color.SizeInBytes);
			Device.Indices = indexBuffer;
			Device.DrawIndexedPrimitive(PrimitiveType.TriangleList, 0, 0, vertexCount, 0, indicesCount / 3);

			//Device.DrawIndexedUserPrimitives<int, VertexPosition4Color>(PrimitiveType.TriangleList, 0, vertexCount, indicesCount / 3, indicesArray, Format.Index32, verticesArray, VertexPosition4Color.SizeInBytes);
		}

		public override void OnPlotterAttached(Plotter plotter)
		{
			base.OnPlotterAttached(plotter);

			FillVertexBuffer();
		}

		private CoordinateTransform transform;
		private VertexPosition4Color[] verticesArray;
		private int[] indicesArray;
		private int indicesCount;
		private int vertexCount;
		private VertexBuffer vertexBuffer;
		private IndexBuffer indexBuffer;
		private void FillVertexBuffer()
		{
			if (DxHost == null) return;
			if (DataSource == null) return;
			if (Palette == null) return;
			if (Device == null) return;

			var dataSource = DataSource;
			var palette = Palette;
			var minMax = dataSource.GetMinMax();
			transform = Plotter.Transform;

			vertexCount = DataSource.Width * DataSource.Height;

			verticesArray = new VertexPosition4Color[vertexCount];
			for (int i = 0; i < verticesArray.Length; i++)
			{
				int ix = i % DataSource.Width;
				int iy = i / DataSource.Width;
				System.Windows.Point point = dataSource.Grid[ix, iy];
				double data = dataSource.Data[ix, iy];

				double interpolatedData = (data - minMax.Min) / minMax.GetLength();
				var color = palette.GetColor(interpolatedData);

				var pointInScreen = point;//.DataToScreen(transform);
				var position = new Vector4((float)pointInScreen.X, (float)pointInScreen.Y, 0.5f, 1);
				verticesArray[i] = new VertexPosition4Color
				{
					Position = position,
					Color =
						//System.Windows.Media.Colors.Blue.ToArgb()
						color.ToArgb()
				};
			}

			vertexBuffer = new VertexBuffer(Device, vertexCount * VertexPosition4Color.SizeInBytes, Usage.WriteOnly, VertexFormat.Position | VertexFormat.Diffuse, Pool.Default);
			using (var stream = vertexBuffer.Lock(0, vertexCount * VertexPosition4Color.SizeInBytes, LockFlags.None))
			{
				stream.WriteRange<VertexPosition4Color>(verticesArray);
			}
			vertexBuffer.Unlock();

			indicesCount = (dataSource.Width - 1) * (dataSource.Height - 1) * 2 * 3;

			indicesArray = new int[indicesCount];
			int index = 0;
			int width = dataSource.Width;
			for (int iy = 0; iy < dataSource.Height - 1; iy++)
			{
				for (int ix = 0; ix < dataSource.Width - 1; ix++)
				{
					indicesArray[index + 0] = ix + 0 + iy * width;
					indicesArray[index + 1] = ix + 1 + iy * width;
					indicesArray[index + 2] = ix + (iy + 1) * width;

					indicesArray[index + 3] = ix + 1 + iy * width;
					indicesArray[index + 4] = ix + (iy + 1) * width;
					indicesArray[index + 5] = ix + 1 + (iy + 1) * width;

					index += 6;
				}
			}

			indexBuffer = new IndexBuffer(Device, indicesCount * sizeof(int), Usage.WriteOnly, Pool.Default, false);
			using (var stream = indexBuffer.Lock(0, indicesCount * sizeof(int), LockFlags.None))
			{
				stream.WriteRange<int>(indicesArray);
			}
			indexBuffer.Unlock();
		}
	}
}
