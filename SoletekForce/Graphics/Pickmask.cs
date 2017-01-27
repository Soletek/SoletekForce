using System;
using OpenTK.Graphics.OpenGL;

namespace SoletekForce.Graphics
{
	public class Pickmask : IFrameBuffer
	{
		static int texture;
		static int fbo;
		static int rbo;

		public Pickmask()
		{
			var width = (int)Viewport.Size.X;
			var height = (int)Viewport.Size.Y;

			texture = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, texture);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, width, height, 
			              0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
			GL.BindTexture(TextureTarget.Texture2D, 0);

			rbo = GL.GenRenderbuffer();
			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
			GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, 
			                       width, height);
			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

			fbo = GL.GenFramebuffer();
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, rbo);
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
									TextureTarget.Texture2D, texture, 0);
			GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment,
									   RenderbufferTarget.Renderbuffer, rbo);

			Console.WriteLine(GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer));

			var id = new uint();
			GL.ReadPixels(0, 0, 1, 1, PixelFormat.Rgb, PixelType.UnsignedByte, ref id);
		}

		public void Bind()
		{
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
		}

		public void Clear()
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		}

		public void End() { }

		public uint Read(int x, int y)
		{
			if (Viewport.Initialized)
			{
				var id = new uint();
				GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
				GL.ReadPixels(x, (int)Viewport.Size.Y - y, 1, 1, PixelFormat.Rgb, PixelType.UnsignedByte, ref id);
				return id;
			}

			return 0;
		}
	}
}
