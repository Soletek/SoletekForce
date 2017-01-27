using System;
using OpenTK.Graphics.OpenGL;

namespace SoletekForce.Graphics
{
	public class Antialiasing : IFrameBuffer
	{
		static int msaa_texture;
		static int msaa_fbo;
		static int msaa_rbo;

		public Antialiasing(int samples)
		{
			var width = (int)Viewport.Size.X;
			var height = (int)Viewport.Size.Y;

			msaa_texture = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2DMultisample, msaa_texture);
			GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, samples, 
			                         PixelInternalFormat.Rgba8, width, height, true);
			GL.BindTexture(TextureTarget.Texture2D, 0);

			msaa_rbo = GL.GenRenderbuffer();
			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, msaa_rbo);
			GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, samples, 
			                                  RenderbufferStorage.Depth24Stencil8, width, height);
			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

			msaa_fbo = GL.GenFramebuffer();
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, msaa_rbo);
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, 
			                        TextureTarget.Texture2DMultisample, msaa_texture, 0);
			GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, 
			                           RenderbufferTarget.Renderbuffer, msaa_rbo);

			Console.WriteLine(GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer));
		}

		public void Bind()
		{
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, msaa_fbo);
		}

		public void Clear()
		{
			GL.ClearColor(System.Drawing.Color.MidnightBlue);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		}

		public void End()
		{
			var width = (int)Viewport.Size.X;
			var height = (int)Viewport.Size.Y;

			GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
			GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, msaa_fbo);
			GL.DrawBuffer(DrawBufferMode.Back);

			GL.BlitFramebuffer(0, 0, width, height, 0, 0, width, height, 
			                   ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);
		}

		public static void Enable()
		{
			GL.Enable(EnableCap.Multisample);
		}

		public static void Disable()
		{
			GL.Disable(EnableCap.Multisample);
		}
	}
}
