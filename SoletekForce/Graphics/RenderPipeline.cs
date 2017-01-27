using System;
using System.Collections.Generic;
using System.Linq;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SoletekForce.Graphics
{
	public static class RenderPipeline
	{
		static readonly List<Drawable> items = new List<Drawable>();
		static readonly Dictionary<string, Shader> shaders = new Dictionary<string, Shader>();
		static readonly Queue<Action> gfxContextQueue = new Queue<Action>();

		static Matrix4 sceneProjection;
		static Matrix4 guiProjection;

		static GraphicsBuffer buffer;
		static Pickmask pickmaskBuffer;
		static Antialiasing msaaBuffer;

		static RenderStep pickmaskStep;
		static RenderStep mainStep;

		public static void Execute(List<Entity> entities)
		{
			if (!Viewport.Initialized)
			{
				Initialize();
				Viewport.Initialized = true;
			}
			else
			{
				foreach (var entity in entities)
				{
					entity.PreRender();
				}

				while (gfxContextQueue.Count > 0)
				{
					gfxContextQueue.Dequeue().Invoke();
				}

				Draw();
			}
		}

		public static void Unload()
		{
			// TODO: dispose everything!!

			items.Clear();
			shaders.Clear();
			gfxContextQueue.Clear();
			Texture.textures.Clear();
		}

		static void Initialize()
		{
			var width = (int)Viewport.Size.X;
			var height = (int)Viewport.Size.Y;
			float aspect = Viewport.Size.X / Viewport.Size.Y;
			float fov = 1.1F;

			GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
			GL.Hint(HintTarget.GenerateMipmapHint, HintMode.Nicest);

			sceneProjection = Matrix4.CreatePerspectiveFieldOfView(fov, aspect, 1.0f, 128.0f);
			guiProjection = Matrix4.CreateOrthographic(2 * aspect, 2, -128.0f, 128.0f);
			buffer = new GraphicsBuffer(Vertex.Size);


			//shaders.Add("default", new Shader("Graphics/Shaders/default", "Graphics/Shaders/default"));
			//shaders.Add("pickmask", new Shader("Graphics/Shaders/pickmask", "Graphics/Shaders/pickmask"));
			//shaders.Add("alpha", new Shader("Graphics/Shaders/alpha", "Graphics/Shaders/pickmask"));
			//shaders.Add("billboard", new Shader("Graphics/Shaders/billboard"));

			shaders.Add("default", new Shader("Graphics/Shaders/default", "Graphics/Shaders/default"));
			shaders.Add("pickmask", new Shader("Graphics/Shaders/default", "Graphics/Shaders/pickmask"));
			shaders.Add("pickmask-billboard", new Shader("Graphics/Shaders/billboard", "Graphics/Shaders/pickmask"));
			shaders.Add("alpha", new Shader("Graphics/Shaders/alpha", "Graphics/Shaders/alpha"));
			shaders.Add("billboard", new Shader("Graphics/Shaders/billboard", "Graphics/Shaders/billboard"));
			InitiateShaders();

			shaders["alpha"].SetGLActions(() => {
				GL.Enable(EnableCap.Blend);
				GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);			}, () => { 
				GL.Disable(EnableCap.Blend);
			});

			shaders["billboard"].SetGLActions(() => {
				GL.Enable(EnableCap.Blend);
				GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
				GL.Disable(EnableCap.DepthTest);
			}, () => {
				GL.Disable(EnableCap.Blend);
				GL.Enable(EnableCap.DepthTest);
			});

			shaders["pickmask-billboard"].SetGLActions(() =>
			{
				GL.Disable(EnableCap.DepthTest);
			}, () =>
			{
				GL.Enable(EnableCap.DepthTest);
			});

			pickmaskBuffer = new Pickmask();
			msaaBuffer = new Antialiasing(4);

			// TODO: transfer projection swap logic to uniform buffer object, at least when
			// using multiple shades per renderstep phase is supported

			var activateSceneBindings = new Action<RenderStep>((step) =>
			{
				step.GetShader().LoadUniformMatrix("viewMatrix", Camera.ViewMatrix);
				step.GetShader().LoadUniformMatrix("projectionMatrix", sceneProjection);
			});

			var activateGuiBindings = new Action<RenderStep>((step) =>
			{
				step.GetShader().LoadUniformMatrix("viewMatrix", Matrix4.Identity);
				step.GetShader().LoadUniformMatrix("projectionMatrix", guiProjection);
				GL.Disable(EnableCap.DepthTest);
			});

			var endRenderStep = new Action<RenderStep>((step) =>
			{
				step.GetFrameBuffer().End();
				GL.Enable(EnableCap.DepthTest);
			});

			// TODO: cache drawable item lists and update only when dirty

			pickmaskStep = new RenderStep(pickmaskBuffer, buffer);
			pickmaskStep.AddPhase(activateSceneBindings, shaders["pickmask"], 
			                      items.Where(x => x.DrawLayer == ElementLayer.Scene && x.Pickable));
			pickmaskStep.AddPhase(activateSceneBindings, shaders["pickmask-billboard"],
								  items.Where(x => x.DrawLayer == ElementLayer.BillboardedGui && x.Pickable));
			pickmaskStep.AddPhase(activateGuiBindings, shaders["pickmask"], 
			                      items.Where(x => x.DrawLayer == ElementLayer.Gui && x.Pickable), endRenderStep);

			mainStep = new RenderStep(msaaBuffer, buffer);
			mainStep.AddPhase(activateSceneBindings, shaders["default"], 
			                  items.Where(x => x.DrawLayer == ElementLayer.Scene && !x.AlphaBlend));
			mainStep.AddPhase(activateSceneBindings, shaders["alpha"], 
			                  items.Where(x => x.DrawLayer == ElementLayer.Scene && x.AlphaBlend).OrderBy(x => x.Depth));
			mainStep.AddPhase(activateSceneBindings, shaders["billboard"],
							  items.Where(x => x.DrawLayer == ElementLayer.BillboardedGui));
			mainStep.AddPhase(activateGuiBindings, shaders["alpha"], 
			                  items.Where(x => x.DrawLayer == ElementLayer.Gui), endRenderStep);
			        
			GL.Viewport(0, 0, width, height);
			GL.Ortho(-1.0f, 1.0f, -1.0f, 1.0f, 1.0f, 128.0f);
			GL.Enable(EnableCap.Multisample);
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.Texture2D);

			Console.WriteLine("GL Initializated: " + GL.GetError());
		}

		static void InitiateShaders()
		{
			foreach (var shader in shaders.Values)
			{
				shader.Load();
				shader.Use();
				shader.GenerateVAO(buffer, Vertex.fieldMetadata);
			}
		}

		public static void QueueGLAction(Action action)
		{
			gfxContextQueue.Enqueue(action);
		}

		static void Draw()
		{
			mainStep.Execute();
			pickmaskStep.Execute();

			//GL.Flush();
			//Console.WriteLine(GL.GetError());
		}

		public static void AddItem(Drawable item)
		{
			if (!items.Contains(item))
			{
				items.Add(item);
			}
		}

		public static void RemoveItem(Drawable item)
		{
			items.Remove(item);
		}

		public static int GetNewItemID()
		{
			// TODO: this can be optimized, no need to always iterate
			int id = 1;

			foreach (var item in items)
			{
				if (item.id >= id) id = item.id + 1;
			}

			return id;
		}

		internal static Drawable GetGfxItem(uint id)
		{
			if (id == 0) return null;

			foreach (var item in items)
			{
				if (item.id == id) return item;
			}

			return null;
		}

		public static uint ReadPickmask(int x, int y)
		{
			if (Viewport.Initialized)
			{
				return pickmaskBuffer.Read(x, y);
			}
			return 0;
		}
	}
}
