using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SoletekForce.Graphics
{
	public struct Vertex
	{
		public const int Size = (3 + 4 + 2 + 1) * 4; // size of struct in bytes

		public Vector3 position;
		public Color4 color;
		public Vector2 uv;
		internal float id;

		public Vertex(Vector3 position, Color4 color, Vector2 uv)
		{
			this.position = position;
			this.color = color;
			this.uv = uv;
			id = 0;
		}

		public readonly static VertexAttribute[] fieldMetadata = {
			new VertexAttribute("vPosition", 3, VertexAttribPointerType.Float, false, 40, 0),
			new VertexAttribute("vColor", 4, VertexAttribPointerType.Float, false, 40, 12),
			new VertexAttribute("vUV", 2, VertexAttribPointerType.Float, false, 40, 28),
			new VertexAttribute("vID", 1, VertexAttribPointerType.Float, false, 40, 36)
		};
	}

	public struct VertexAttribute
	{
		public readonly string name;
		public readonly int size;
		public readonly VertexAttribPointerType type;
		public readonly bool normalized;
		public readonly int stride;
		public readonly int offset;

		public VertexAttribute(string name, int size, VertexAttribPointerType type,
							  bool normalized, int stride, int offset)
		{
			this.name = name;
			this.size = size;
			this.type = type;
			this.normalized = normalized;
			this.stride = stride;
			this.offset = offset;
		}

		public static VertexAttribute FindByName(string name, VertexAttribute[] data)
		{
			for (int i = 0; i < data.Length; i++)
			{
				if (data[i].name == name) return data[i];
			}

			return new VertexAttribute();
		}
	}
}
