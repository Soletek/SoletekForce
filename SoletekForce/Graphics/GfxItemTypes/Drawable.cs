using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SoletekForce.Graphics
{
	public delegate void MouseEventHandler(object sender, EventArgs e);

	public enum ElementLayer
	{
		Scene = 0,
		BillboardedGui = 1,
		Gui = 2,
	}

	public class Drawable
	{
		public bool Pickable { get; set; }
		public ElementLayer DrawLayer { get; set; }
		public bool AlphaBlend { get; set; }
		public float Depth { get; set; }
		public float Alpha { get; set; }

		bool active;
		protected Entity holder;
		internal int id;

		public Vertex[] verticles;
		public ushort[] indices;

		public event MouseEventHandler Clicked;
		public event MouseEventHandler Hovered;

		Texture texture;
		protected int textureHandle;

		public Drawable(Entity holder)
		{
			this.holder = holder;
			SetTexture(Texture.NoTexture);
		}

		public bool Active
		{
			get { return active; }
		}

		public void Activate()
		{
			if (!active)
			{
				if (Pickable)
				{
					id = RenderPipeline.GetNewItemID();
				}

				RenderPipeline.AddItem(this);
				active = true;
			}
			else
			{
				// Move to end
				RenderPipeline.RemoveItem(this);
				RenderPipeline.AddItem(this);
			}
		}

		public void Deactivate()
		{
			if (active)
			{
				RenderPipeline.RemoveItem(this);
				active = false;
			}
		}

		/// <summary>
		/// Overrides the color with new one
		/// </summary>
		public void SetColor(Color4 color)
		{
			for (int i = 0; i < verticles.Length; i++)
			{
				verticles[i].color = color;
			}
		}

		public void SetTexture(Texture texture)
		{
			this.texture = texture;
			RenderPipeline.QueueGLAction(() => textureHandle = (int)texture);
		}

		public Texture GetTexture()
		{
			return texture;
		}

		public void BakeTransform(Transform trans)
		{
			for (int i = 0; i < verticles.Length; i++)
			{
				var vec4 = new Vector4(verticles[i].position, 1);
				verticles[i].position =  new Vector3(vec4 * trans.ModelMatrix);
			}
		}

		internal virtual void Render(GraphicsBuffer buffer, Shader shader)
		{
			GL.BindTexture(TextureTarget.Texture2D, textureHandle);
			shader.LoadUniformMatrix("modelMatrix", holder.Transform.ModelMatrix);
			shader.LoadUniformFloat("alpha", Alpha);
			if (DrawLayer == ElementLayer.BillboardedGui)
			{
				var scale = holder.Transform.WorldScale;
				shader.LoadUniformFloat("scaleX", scale.X);
				shader.LoadUniformFloat("scaleY", scale.Y);
			}
			if (Pickable)
			{
				shader.LoadUniformFloat("id", id);
			}
			buffer.SetIndices(indices);
			buffer.SetVertices(verticles);
			buffer.BufferData();
			buffer.Draw();
		}

		internal void OnHover()
		{
			if (Hovered != null) Hovered(this, null);
		}

		internal void OnClick()
		{
			if (Clicked != null) Clicked(this, null);
		}
	}
}
