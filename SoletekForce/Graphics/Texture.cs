using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace SoletekForce.Graphics
{
	public class Texture : IDisposable
	{
		internal static SortedDictionary<string, Texture> textures = new SortedDictionary<string, Texture>();

		Bitmap bitmap;
		int texture = 0;

		public int Width { get; private set; }
		public int Height { get; private set; }

		public Bitmap Bitmap
		{
			get {
				return bitmap;
			}
			set {
				bitmap = value;
			}
		}

		public Texture(string filename)
		{
			bitmap = new Bitmap(filename);
			Width = bitmap.Width;
			Height = bitmap.Height;

			RenderPipeline.QueueGLAction(Load);
		}

		Texture() 
		{
			bitmap = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			bitmap.SetPixel(0, 0, Color.White);
			Height = 1;
			Width = 1;

			RenderPipeline.QueueGLAction(Load);
		}

		// Executed inside GL context
		public void Load()
		{
			var rect = new Rectangle(0, 0, Width, Height);
			var data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			float maxAniso;
			GL.GetFloat((GetPName)ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out maxAniso);

			texture = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, texture);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, 
			                (int)TextureMagFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, 
			                (int)TextureMinFilter.LinearMipmapLinear);
			GL.TexParameter(TextureTarget.Texture2D, 
			                (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, maxAniso);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
				OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

			bitmap.UnlockBits(data);
			bitmap.Dispose();
		}

		public static explicit operator int(Texture self)
		{
			return self.texture;
		}

		bool disposed;
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
			}

			RenderPipeline.QueueGLAction(() => GL.DeleteTexture(texture));

			disposed = true;
		}

		// Texture listing

		public static void Add(string name, string path)
		{
			if (textures.ContainsKey(name)) return;
			textures.Add(name, new Texture(path));
		}

		public static Texture Get(string name)
		{
			return textures[name];
		}

		public static Texture NoTexture
		{
			get {
				if (!textures.ContainsKey("none_texture"))
				{
					textures.Add("none_texture", new Texture());
				}
				return textures["none_texture"]; 
			}
		}
	}
}
