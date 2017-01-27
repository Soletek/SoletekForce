using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SoletekForce.Graphics
{
	public static class Viewport
	{
		internal static bool Initialized { get; set; }
		public static ViewportOptions Options { get; private set; }

		public static Vector2 Size
		{
			get { return Options.Size; }
		}

		public static void Resize(ViewportOptions options)
		{
			Options = options;
		}

		public static float Aspect
		{
			get { return Size.X / Size.Y; }
		}
	}

	public struct ViewportOptions
	{
		public Vector2 Size;
	}
}
