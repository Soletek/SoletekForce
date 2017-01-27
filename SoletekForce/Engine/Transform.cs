using System;
using OpenTK;

namespace SoletekForce
{
	public class Transform : IInterpolatable<Transform>
	{
		public Vector3 Position { get; set; }
		public Vector3 Scale { get; set; }
		public Quaternion Rotation { get; set; }

		WeakReference parent;

		public Transform() : this(Vector3.Zero)
		{}

		public Transform(Vector3 position) : this(position, Vector3.One, Quaternion.Identity)
		{}

		public Transform(Vector3 position, float scale) : this (position, new Vector3(scale), Quaternion.Identity)
		{}

		public Transform(Vector3 position, Vector3 scale, Quaternion rotation)
		{
			Position = position;
			Scale = scale;
			Rotation = rotation;
		}

		public Transform Parent
		{
			get {
				if (parent == null || parent.IsAlive == false) return null;
				return parent.Target as Transform; 
			}
			set { SetParent(value); }
		}

		public Matrix4 ModelMatrix
		{
			get
			{
				var matrix = Matrix4.CreateFromQuaternion(Rotation) * Matrix4.CreateScale(Scale)
									* Matrix4.CreateTranslation(Position);

				if (parent != null && parent.IsAlive)
				{
					var parentTransform = parent.Target as Transform;
					matrix = matrix * parentTransform.ModelMatrix;
				}

				return matrix;
			}
		}

		internal Vector3 WorldScale
		{
			get
			{
				var scale = Scale;

				if (parent != null && parent.IsAlive)
				{
					var parentTransform = parent.Target as Transform;
					scale = scale * parentTransform.WorldScale;
				}

				return scale;
			}
		}

		public bool SetParent(Transform newParent)
		{
			var parentIterator = new WeakReference(newParent);

			// Check that no circular references are created
			while (parentIterator != null && parentIterator.IsAlive)
			{
				var parentTransform = parentIterator.Target as Transform;
				if (parentTransform == this) return false;
				parentIterator = parentTransform.parent;
			}

			parent = new WeakReference(newParent);

			return true;
		}

		// TODO: how should this method react to transform parent?
		public Transform Interpolate(Transform other, float blend)
		{
			var output = new Transform();
			output.Position = Vector3.Lerp(this.Position, other.Position, blend);
			output.Scale = Vector3.Lerp(this.Scale, other.Scale, blend);
			output.Rotation = Quaternion.Slerp(this.Rotation, other.Rotation, blend);
			return output;
		}

		// TODO: how should this method react to transform parent?
		public void SetVectors(Transform other)
		{
			this.Position = other.Position;
			this.Rotation = other.Rotation;
			this.Scale = other.Scale;
		}

		// TODO: how should this method react to transform parent?
		public Transform CombineWith(Transform other)
		{
			var output = new Transform();
			output.Position = this.Position + other.Position;
			output.Scale = this.Scale * other.Scale;
			output.Rotation = this.Rotation * other.Rotation;
			return output;
		}
	}
}
