using System;
using OpenTK;
using SoletekForce.Graphics;

namespace SoletekForce
{
	// TODO: move to gui class
	public enum Corner
	{
		BottomLeft = 0,
		BottomRight = 1,
		TopRight = 2,
		TopLeft = 3
	}

	public abstract class Entity
	{
		protected Entity()
		{
			Engine.RegisterEntity(this);
		}

		public readonly Transform Transform = new Transform();

		public virtual void PreRender() { }

		public virtual void Update() { }

		internal bool alive = true;

		public bool Alive { get { return alive; } }

		public virtual void Destroy()
		{
			alive = false;
			Engine.RemoveEntity(this);
		}

		// TODO: move to gui class
		public void SetRectPos(float x, float y, Corner corner)
		{
			float xStart = (corner == Corner.BottomLeft || corner == Corner.TopLeft) ? -1.0f : 1.0f;
			float yStart = (corner == Corner.BottomLeft || corner == Corner.BottomRight) ? -1.0f : 1.0f;

			Action action = delegate 
			{
				Transform.Position = new Vector3((Viewport.Aspect - x) * xStart, (1.0f - y) * yStart, 0f);
			};

			if (Viewport.Initialized)
			{
				action.Invoke();
			}
			else
			{
				RenderPipeline.QueueGLAction(action);
			}
		}
	}
}
