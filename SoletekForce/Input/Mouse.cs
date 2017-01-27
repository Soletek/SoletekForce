using System;
using OpenTK;
using SoletekForce.Graphics;

namespace SoletekForce.Input
{
	public static class Mouse
	{
		static Vector2 position;
		static Drawable activeItem;
		static uint hoverID;

		public static float X { get { return position.X; } }
		public static float Y { get { return position.Y; } }
		public static Action ClickOnNothing { get; set; }
		public static Action HoverOnNothing { get; set; }
		static int _x;
		static int _y;

		public static void Hover(float x, float y)
		{
			UpdateActiveItem((int)x, (int)y);
			_x = (int)x;
			_y = (int)y;
			position = new Vector2(x, y).NormalizeToScreen();

			if (activeItem != null) activeItem.OnHover();
			else if (HoverOnNothing != null) HoverOnNothing.Invoke();
		}

		public static Vector3 ScreenPosition
		{
			get 
			{
				return new Vector3((X * 2 - 1) * Viewport.Aspect, -(Y * 2 - 1), 0);
			}
		}

		public static void Click(float x, float y)
		{
			UpdateActiveItem((int)x, (int)y);
			if (activeItem != null) activeItem.OnClick();
			else if (ClickOnNothing != null) ClickOnNothing.Invoke();
		}

		internal static void Update()
		{
			UpdateActiveItem(_x, _y);
		}

		static void UpdateActiveItem(int x, int y)
		{
			uint id = RenderPipeline.ReadPickmask(x, y);

			if (id != hoverID)
			{
				hoverID = id;
				activeItem = RenderPipeline.GetGfxItem(id);
			}
		}

		public static Drawable GetActiveItem()
		{
			return activeItem;
		}

		static Vector2 NormalizeToScreen(this Vector2 inPixels)
		{
			inPixels.X /= Viewport.Size.X;
			inPixels.Y /= Viewport.Size.Y;
			return inPixels;
		}
	}
}
