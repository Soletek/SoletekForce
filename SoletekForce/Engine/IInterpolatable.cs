using System;
namespace SoletekForce
{
	public interface IInterpolatable<T>
	{
		T Interpolate(T other, float blend);
	}

	public struct InterpolatableFloat : IInterpolatable<InterpolatableFloat>
	{
		float val;

		public static implicit operator InterpolatableFloat(float val)
		{
			return new InterpolatableFloat { val = val };
		}

		public static implicit operator float(InterpolatableFloat var)
		{
			return var.val;
		}

		public InterpolatableFloat Interpolate(InterpolatableFloat other, float blend)
		{
			return (this * (1 - blend)) + (other * blend);
		}
	}
}
