using System;
using OpenTK.Graphics.OpenGL;

// Handles the vertex and index buffers to GPU

namespace SoletekForce.Graphics
{
	sealed class GraphicsBuffer
	{
		Buffer<ushort> indexBuffer;
		Buffer<Vertex> vertexBuffer;

		//Shader shader;

		public GraphicsBuffer(int vertexSize)
		{
			indexBuffer = new Buffer<ushort>(sizeof(ushort), BufferTarget.ElementArrayBuffer);
			vertexBuffer = new Buffer<Vertex>(vertexSize, BufferTarget.ArrayBuffer);
		}

		public void SetIndices(ushort[] indices)
		{
			indexBuffer.SetData(indices);
		}

		public void SetVertices(Vertex[] vertices)
		{
			vertexBuffer.SetData(vertices);
		}

		public void BufferData()
		{
			indexBuffer.Bind();
			indexBuffer.BufferData();

			vertexBuffer.Bind();
			vertexBuffer.BufferData();
		}

		public void Draw()
		{
			indexBuffer.Bind();
			GL.DrawElements(PrimitiveType.Triangles, indexBuffer.ElementCount, 
				DrawElementsType.UnsignedShort, IntPtr.Zero);
		}

		public void DrawLines()
		{
			GL.DrawArrays(PrimitiveType.Lines, 0, vertexBuffer.ElementCount);
		}

		public int GetVBOHandle()
		{
			return vertexBuffer.handle;
		}

		class Buffer<TData>
			where TData : struct
		{
			internal readonly int size;
			internal readonly int handle;
			int count;
			internal TData[] data = new TData[4];
			readonly BufferTarget bufferType;

			public int ElementCount
			{
				get { return count; }
			}

			public Buffer(int size, BufferTarget target)
			{
				handle = GL.GenBuffer();
				bufferType = target;
				this.size = size;
			}

			public void SetData(TData[] data)
			{
				this.data = data;
				count = data.Length;
			}

			public void Bind()
			{
				GL.BindBuffer(bufferType, handle);
			}

			public void BufferData()
			{
				GL.BufferData(bufferType, (IntPtr)(count * size),
					data, BufferUsageHint.StreamDraw);
			}
		}	
	}
}
