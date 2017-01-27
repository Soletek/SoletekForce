using System.Diagnostics;
using OpenTK;
using GLib;

// Backend for GLib + GTK#

namespace SoletekForce.Backend
{
	public class GLib : IBaseBackend
	{
		public delegate void RenderDelegate();
		static RenderDelegate OnRender;

		public GLib(RenderDelegate renderer)
		{
			OnRender = renderer;
		}

		public void Init()
		{
		}

		public void Start()
		{
			if (!Engine.Running)
			{
				// TODO adjust timeout to framerate
				Timeout.Add(3, UpdateTimeoutHandler);
			}
		}

		public void Stop()
		{
		}

		public void Unload()
		{
			Graphics.Viewport.Initialized = false;
			Graphics.RenderPipeline.Unload();
		}

		bool UpdateTimeoutHandler()
		{
			if (!Engine.Running) return false;

			if (true)
			{
				Engine.ProcessInput();
				Engine.Update();
				OnRender();
			}

			return Engine.Running;
		}
	}
}
