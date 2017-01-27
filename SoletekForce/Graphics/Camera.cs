using System;
using OpenTK;

namespace SoletekForce.Graphics
{
	public static class Camera
	{
		static Matrix4 viewMatrix = Matrix4.Identity;
		static Vector3 position = Vector3.One;
		static Vector3 lookAt = Vector3.Zero;
		static readonly Vector3 up = new Vector3(0, 1f, 0);

		static void UpdateViewMatrix() 
		{ 
			viewMatrix = Matrix4.LookAt(position, lookAt, up);
			Audio.AudioEngine.UpdateListenerLocation();
		}

		public static Vector3 Position
		{
			get { return position; }
			set
			{
				position = value;
				UpdateViewMatrix();
			}
		}

		public static Vector3 LookAt
		{
			get { return lookAt; }
			set
			{
				lookAt = value;
				UpdateViewMatrix();
			}
		}

		public static Vector3 Up { get { return up; } }

		public static Matrix4 ViewMatrix
		{
			get 
			{
				return viewMatrix; 
			}
		}
	}
}
