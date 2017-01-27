using System;
using System.Collections.Generic;

namespace SoletekForce.Graphics
{
	struct Particle<TData>
		where TData : struct
	{
		TData dataStore;
		Action<TData> update;
		ParticleRenderer holder;

		public Particle(ParticleRenderer holder)
		{
			this.holder = holder;
			dataStore = new TData();
			update = (o) => o.ToString();
		}

		public void Update()
		{
			update(dataStore);
		}

		public List<ushort> UpdateMesh(List<Vertex> vert)
		{
			return new List<ushort>();
		}
	}
}
