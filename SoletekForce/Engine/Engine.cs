using System;
using System.Collections.Generic;
using System.Linq;
using SoletekForce.Audio;
using SoletekForce.Backend;
using SoletekForce.Graphics;

// Soletek Force game engine
// TuukkaK

// GAME ENGINE
// TODO: get keyboard input
// TODO: optimize OpenGL and other stuff

// NOTE:
// openal32.dll might be missing from target device

namespace SoletekForce
{
	public static class Engine
	{
		static IBaseBackend backend;
		static List<Entity> entityBase = new List<Entity>();
		static public bool Running { get; private set; }
		static bool destroyedEntities;

		// General control

		public static void SetBackend(IBaseBackend newBackend)
		{
			if (backend != null) backend.Stop();

			if (!Running)
			{
				backend = newBackend;
			}
			else {
				Pause();
				backend = newBackend;
				Run();
			}
		}

		public static void Init()
		{
			if (backend != null) backend.Init();
			AudioEngine.Init();
		}

		public static void Run()
		{
			if (backend != null) backend.Start();
			Running = true;
			Timer.Start();
		}

		public static void Pause()
		{
			if (backend != null) backend.Stop();
			Running = false;
			Timer.Stop();
		}

		/// <summary>
		/// Render event handler.
		/// </summary>
		public static void OnRender(object sender, EventArgs e)
		{
			RenderPipeline.Execute(entityBase);
		}

		// Entity control

		public static void RegisterEntity(Entity entity)
		{
			entityBase.Add(entity);
		}

		public static void RemoveEntity(Entity entity)
		{
			destroyedEntities = true;
		}

		public static void ClearEntities()
		{
			entityBase.Clear();
		}

		// Internal methods

		internal static void ProcessInput()
		{
			Input.Mouse.Update();
			Input.Keyboard.Update();
		}

		internal static void Update()
		{
			Timer.Update();

			if (destroyedEntities)
			{
				entityBase = new List<Entity>(entityBase.Where(x => x.alive));
				destroyedEntities = false;
			}

			int elementCount = entityBase.Count;
			for (int i = 0; i < elementCount; i++)
			{
				entityBase[i].Update();
			}
		}
	}
}
