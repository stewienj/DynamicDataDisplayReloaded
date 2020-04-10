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

namespace DynamicDataDisplay.SharpDX9.Prototypes
{
	/// <summary>
	/// Based on code from https://www.gamedev.net/forums/topic/604231-instancing-not-working-for-me-slimdx-dx9/
	/// </summary>
	public class InstancingPrototype : BaseDxChartElement
	{
		private VertexBuffer _vertexBuffer = null;
		private VertexBuffer _instanceBuffer = null;
		private IndexBuffer _indexBuffer = null;

		private int _lastVertexLength = 0;
		private int _vecticesCount = 0;
		private SynchronizationContext _syncContext = null;
		private VertexDeclaration _vertexDecl;
		private BaseDxTransformShader _transformEffect;
		//private TDxPoint[] _pointList;
		// Limit updates to 100 times per second. This also schedules updates to another thread.
		private ThrottledAction _throttledAction = new ThrottledAction(TimeSpan.FromMilliseconds(10));

		int vertexSize = 32;
		int instanceSize = 64;
		int numberOfObjects = 10;

		public override void OnPlotterAttached(Plotter plotter)
		{
			_syncContext = SynchronizationContext.Current;

			base.OnPlotterAttached(plotter);

			_transformEffect = GetTransformEffect(Device);

			// Creates and sets the Vertex Declaration
			//_vertexDecl = new VertexDeclaration(Device, new TDxPoint().GetVertexElements());

			VertexElement[] vertexElementsArray = new VertexElement[]
			{
				new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Position, 0),
				new VertexElement(0, 12, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
				new VertexElement(1, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
				new VertexElement(1, 16, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 1),
				new VertexElement(1, 32, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 2),
				new VertexElement(1, 48, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 3),
				VertexElement.VertexDeclarationEnd
			};
			_vertexDecl = new VertexDeclaration(Device, vertexElementsArray);
			CreateIndexBuffer();
			CreateInstanceBuffer();
			CreateVertexBuffer();
		}

		public override void OnPlotterDetaching(Plotter plotter)
		{
			_vertexBuffer?.Dispose();
			base.OnPlotterDetaching(plotter);
		}

		private void CreateVertexBuffer()
		{
			// Positions have W = 1, directions have W = 0 so they aren't affected by translations
			Vector4[] vectors = new Vector4[]
			{
				new Vector4(-1f, 0f, 0f, 1f),
				new Vector4( 0f, 1f, 0f, 1f),
				new Vector4( 1f, 0f, 0f, 1f)
			};

			// Normal is direction, so W is 0
			Vector4 normal = new Vector4(0f, 0f, -1f, 0f);

			_vertexBuffer = new VertexBuffer(Device, vertexSize * vectors.Length, Usage.WriteOnly, VertexFormat.None, Pool.Default);
			DataStream vertexBufferStream = _vertexBuffer.Lock(0, vertexSize * vectors.Length, LockFlags.Discard);
			
			// Copy the vertex data to the Vertex Buffer memory block.
			for (int nCount = 0; nCount < vectors.Length; nCount++)
			{
				vertexBufferStream.Write<float>(vectors[nCount].X);
				vertexBufferStream.Write<float>(vectors[nCount].Y);
				vertexBufferStream.Write<float>(vectors[nCount].Z);
				vertexBufferStream.Write<float>(vectors[nCount].W);

				vertexBufferStream.Write<float>(normal.X);
				vertexBufferStream.Write<float>(normal.Y);
				vertexBufferStream.Write<float>(normal.Z);
				vertexBufferStream.Write<float>(normal.W);
			}

			// Unlock the Vertex Buffer again, to allow rendering of the Vertex Buffer data.
			_vertexBuffer.Unlock();
		}

		private void CreateInstanceBuffer()
		{
			// Create the interleaved Vertex Buffer.
			_instanceBuffer = new VertexBuffer(Device, instanceSize * numberOfObjects, Usage.WriteOnly, VertexFormat.None, Pool.Default);

			DataStream instanceBufferStream = _instanceBuffer.Lock(0, instanceSize * numberOfObjects, LockFlags.Discard);

			// Create identity matrix.
			Matrix matrix = new Matrix();

			// Copy the matrix data to the Vertex Buffer memory block.
			for (int count = 0; count < numberOfObjects; count++)
			{
				// Translate the matrix along the X axis.
				matrix.M41 = (float)count;
				matrix.M42 = (float)0;
				matrix.M43 = (float)10;
				instanceBufferStream.Write<float>(matrix[0, 0]);
				instanceBufferStream.Write<float>(matrix[0, 1]);
				instanceBufferStream.Write<float>(matrix[0, 2]);
				instanceBufferStream.Write<float>(matrix[0, 3]);

				instanceBufferStream.Write<float>(matrix[1, 0]);
				instanceBufferStream.Write<float>(matrix[1, 1]);
				instanceBufferStream.Write<float>(matrix[1, 2]);
				instanceBufferStream.Write<float>(matrix[1, 3]);

				instanceBufferStream.Write<float>(matrix[2, 0]);
				instanceBufferStream.Write<float>(matrix[2, 1]);
				instanceBufferStream.Write<float>(matrix[2, 2]);
				instanceBufferStream.Write<float>(matrix[2, 3]);

				instanceBufferStream.Write<float>(matrix[3, 0]);
				instanceBufferStream.Write<float>(matrix[3, 1]);
				instanceBufferStream.Write<float>(matrix[3, 2]);
				instanceBufferStream.Write<float>(matrix[3, 3]);
			}
			// Unlock the Vertex Buffer again, to allow rendering of the Vertex Buffer data.
			_instanceBuffer.Unlock();
		}

		private void CreateIndexBuffer()
		{
			int numberOfSurfaces = 1;

			IndexBuffer indexBuffer = new IndexBuffer(Device, numberOfSurfaces * sizeof(uint) * 3, Usage.None, Pool.Default, false);

			// Lock the buffer, so that we can access the data.
			DataStream indexBufferStream = indexBuffer.Lock(0, numberOfSurfaces * sizeof(uint) * 3, LockFlags.None);

			indexBufferStream.Write<uint>(0);
			indexBufferStream.Write<uint>(1);
			indexBufferStream.Write<uint>(2);

			// Unlock the stream again, committing all changes.
			indexBuffer.Unlock();
		}

		/*
		private void UpdateFromSourceChange(IEnumerable<TDxPoint> newList)
		{
			// Vertices will be resized to the next power of 2, saves on resizing too much
			_pointList = newList.ToArray();
			var pointCount = _pointList.Length;
			if (_vertices == null || pointCount > _lastVertexLength || pointCount < (_lastVertexLength >> 1))
			{
				_vertices?.Dispose();
				var newSize = MathHelper.CeilingPow2(pointCount);
				_vertices = new VertexBuffer(Device, Utilities.SizeOf<TDxPoint>() * newSize, Usage.WriteOnly, VertexFormat.None, Pool.Default);
				_lastVertexLength = newSize;
			}
			// Lock the entire buffer by specifying 0 for the offset and size, throw away it's current contents
			var buffer = _vertices.Lock(0, 0, LockFlags.Discard);
			buffer.WriteRange(_pointList);
			_vertices.Unlock();
			_vecticesCount = pointCount;

			// Calculate the bounds of the list on a background thread
			var localPointList = _pointList;
			var dataTransform = Plotter.Viewport.Transform.DataTransform;
			if (localPointList.Any())
			{
				Action resizeAction = () =>
				{
					var minX = localPointList[0].X;
					var maxX = localPointList[0].X;
					var minY = localPointList[0].Y;
					var maxY = localPointList[0].Y;
					foreach (var point in localPointList)
					{
						minX = Math.Min(minX, point.X);
						maxX = Math.Max(maxX, point.X);
						minY = Math.Min(minY, point.Y);
						maxY = Math.Max(maxY, point.Y);
					}
					var bounds = BoundsHelper.GetViewportBounds(new[] { new System.Windows.Point(minX, minY), new System.Windows.Point(maxX, maxY) }, dataTransform);
					_syncContext.Send(s =>
					{
						Viewport2D.SetContentBounds(this, bounds);
					}, null);
				};
				// Spawn action on throttled update thread
				_throttledAction.InvokeAction(resizeAction);
			}
		}
		*/

		/*
		protected override void OnDirectXRender()
		{
			if (_vecticesCount <= 0)
				return;
			Device.SetRenderState(global::SharpDX.Direct3D9.RenderState.Lighting, false);
			Device.SetRenderState(global::SharpDX.Direct3D9.RenderState.AntialiasedLineEnable, true);
			Device.SetStreamSource(0, _vertices, 0, Utilities.SizeOf<TDxPoint>());
			Device.VertexDeclaration = _vertexDecl;
			_transformEffect.BeginEffect(Plotter.Viewport.Visible, DxDataTransform);
			Device.DrawPrimitives(GetPrimitiveType(), 0, _vecticesCount - 1);
			_transformEffect.EndEffect();
		}
		*/
		protected override void OnDirectXRender()
		{
			Device.Indices = _indexBuffer;
			Device.SetStreamSource(0, _vertexBuffer, 0, vertexSize);
			Device.SetStreamSource(1, _instanceBuffer, 0, instanceSize);

			// Specify how many times the vertex stream source and the instance stream source should be rendered.
			Device.SetStreamSourceFrequency(0, 10, StreamSource.IndexedData);
			Device.SetStreamSourceFrequency(1, 1, StreamSource.InstanceData);

			Action<int> perPass = passNo =>
			{
				Device.DrawIndexedPrimitive(PrimitiveType.TriangleList, 0, 0, 3, 0, 1);
				//Device.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
			};

			_transformEffect.DoMultipassEffect(Plotter.Viewport.Visible, perPass);
				
			Device.ResetStreamSourceFrequency(1);
			Device.ResetStreamSourceFrequency(0);
			// Add these later
			//Device.SetStreamSource(1, null, 0, 0);
			//Device.SetStreamSource(0, null, 0, 0);
		}


		protected BaseDxTransformShader GetTransformEffect(Device device)
		{
			return new InstancingPrototypeShader(device);
		}

		//protected abstract PrimitiveType GetPrimitiveType();
	}
}