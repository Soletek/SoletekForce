using System;
using SoletekForce.Animation;
using System.Collections.Generic;

namespace SoletekForce.Graphics
{
	/// <summary>
	/// Particle renderer.
	/// </summary>
	/// <remarks> WIP </remarks>
	internal class ParticleRenderer : Sprite
	{
		List<Particle<int>> aliveParticles = new List<Particle<int>>();

		public ParticleRenderer(Entity holder) : base (holder)
		{
		}

		public void Update()
		{
		}
	}
}
