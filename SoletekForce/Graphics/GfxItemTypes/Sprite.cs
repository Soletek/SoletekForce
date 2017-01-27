using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SoletekForce.Graphics
{
	public class Sprite : Drawable
	{
		protected Sprite(Entity holder) : base(holder) 
		{
			SetTexture(Texture.NoTexture);
		}

		public Sprite(Entity holder, Texture texture)
			: base(holder)
		{
			SetTexture(texture);
			GenerateMesh();
		}

		protected Sprite(Entity holder, int texture)
			: base(holder)
		{
			textureHandle = texture;
			GenerateMesh();
		}

		protected void GenerateMesh()
		{
			indices = new ushort[] { 0, 1, 2, 0, 2, 3 };

			verticles = new Vertex[]{
				new Vertex(new Vector3(-1.0f, -1.0f, 0.0f), Color4.White, new Vector2(0.0f, 1.0f)),
				new Vertex(new Vector3( 1.0f, -1.0f, 0.0f), Color4.White, new Vector2(1.0f, 1.0f)),
				new Vertex(new Vector3( 1.0f,  1.0f, 0.0f), Color4.White, new Vector2(1.0f, 0.0f)),
				new Vertex(new Vector3(-1.0f,  1.0f, 0.0f), Color4.White, new Vector2(0.0f, 0.0f))
			};
		}

		protected void GenerateMesh(List<ushort> indexList, List<Vertex> vertexList)
		{
			GenerateMesh(indexList, vertexList, Vector3.Zero, Color4.White);
		}

		protected void GenerateMesh(List<ushort> indexList, List<Vertex> vertexList, Vector3 offset, Color4 color)
		{
			vertexList.AddRange(new Vertex[]{
				new Vertex(new Vector3(-1.0f, -1.0f, 0.0f) + offset, color, new Vector2(0.0f, 1.0f)),
				new Vertex(new Vector3( 1.0f, -1.0f, 0.0f) + offset, color, new Vector2(1.0f, 1.0f)),
				new Vertex(new Vector3( 1.0f,  1.0f, 0.0f) + offset, color, new Vector2(1.0f, 0.0f)),
				new Vertex(new Vector3(-1.0f,  1.0f, 0.0f) + offset, color, new Vector2(0.0f, 0.0f))
			});

			var b = vertexList.Count - 4;

			indexList.AddRange(new ushort[] { (ushort)(b + 0), (ushort)(b + 1), (ushort)(b + 2), 
											  (ushort)(b + 0), (ushort)(b + 2), (ushort)(b + 3)});
		}

		public void ClipTexture(float x, float y, float w, float h, int index = 0)
		{
			float xs = x / GetTexture().Width;
			float ys = y / GetTexture().Height;
			float xe = (x + w) / GetTexture().Width;
			float ye = (y + h) / GetTexture().Height;

			verticles[0 + index].uv = new Vector2(xs, ye);
			verticles[1 + index].uv = new Vector2(xe, ye);
			verticles[2 + index].uv = new Vector2(xe, ys);
			verticles[3 + index].uv = new Vector2(xs, ys);
		}
	}
}
