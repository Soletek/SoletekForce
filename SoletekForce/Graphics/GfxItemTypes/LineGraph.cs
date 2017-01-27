using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SoletekForce.Graphics
{
	public class LineGraph : Drawable
	{
		float lineWidth;

		public LineGraph(Entity holder, float lineWidth)
			: base(holder)
		{
			this.lineWidth = lineWidth;
		}

		internal override void Render(GraphicsBuffer buffer, Shader shader)
		{
			GL.LineWidth(lineWidth);
			GL.BindTexture(TextureTarget.Texture2D, (int)Texture.NoTexture);
			shader.LoadUniformMatrix("modelMatrix", holder.Transform.ModelMatrix);
			buffer.SetVertices(verticles);
			buffer.BufferData();
			buffer.DrawLines();
		}
	}
}
