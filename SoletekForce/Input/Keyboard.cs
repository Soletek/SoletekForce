using System;
using OpenTK.Input;

namespace SoletekForce.Input
{
	public static class Keyboard
	{
		static KeyboardState state;
		static KeyboardState previousFrame;

		internal static void Update()
		{
			state = OpenTK.Input.Keyboard.GetState();
			previousFrame = state;
		}
	}
}
