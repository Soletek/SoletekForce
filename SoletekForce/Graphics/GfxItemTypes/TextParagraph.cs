using System;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using OpenTK.Graphics.OpenGL;

namespace SoletekForce.Graphics
{
	/// <summary>
	/// Text paragraph rendered to texture using System.Drawing
	/// </summary>
	public class TextParagraph : Sprite, IDisposable
	{
		readonly Bitmap bitmap;
		readonly System.Drawing.Graphics graphics;
		StringFormat format;

		Font font;
		Brush brush;

		bool disposed;

		public TextParagraph(Entity holder, Font font, Brush brush, int maxWidth, int maxHeight)
			: base(holder, 0)
		{
			this.font = font;
			this.brush = brush;

			bitmap = new Bitmap(maxWidth, maxHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			graphics = System.Drawing.Graphics.FromImage(bitmap);
			graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			format = new StringFormat();
			format.Alignment = StringAlignment.Center;

			RenderPipeline.QueueGLAction(GenerateTexture);
		}

		public void GenerateTexture()
		{
			textureHandle = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, textureHandle);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0,
				OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
		}

		public void ChangeText(string str)
		{
			graphics.Clear(Color.Transparent);
			graphics.DrawString(str, font, brush, bitmap.Width / 2, 0, format);
			RenderPipeline.QueueGLAction(UpdateTexture);
		}

		public void UpdateTexture()
		{
			var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
			var data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			GL.BindTexture(TextureTarget.Texture2D, textureHandle);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
				OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
			//GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
			
			bitmap.UnlockBits(data);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		void Dispose(bool disposing)
		{
			if (disposed) return;

			if (disposing)
			{
				bitmap.Dispose();
				graphics.Dispose();
			}

			RenderPipeline.QueueGLAction(() => GL.DeleteTexture(textureHandle));

			disposed = true;
		}
	}
}
