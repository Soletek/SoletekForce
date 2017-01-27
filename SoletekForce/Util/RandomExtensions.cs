using System;

namespace SoletekForce
{
	public static class RandomExtensions
	{
		public static float RangeF(this Random rng, float min, float max)
		{
			float diff = max - min;
			return (float)(rng.NextDouble() * diff + min);
		}
	}
}
