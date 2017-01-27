using System;
using System.Collections.Generic;

namespace SoletekForce.Graphics
{
	internal class RenderStep
	{
		readonly IFrameBuffer frameBuffer;
		readonly GraphicsBuffer dataBuffer;

		List<RenderPhase> phases = new List<RenderPhase>();
		RenderPhase activePhase;

		public RenderStep(IFrameBuffer frameBuffer, GraphicsBuffer dataBuffer)
		{
			this.frameBuffer = frameBuffer;
			this.dataBuffer = dataBuffer;
		}

		public IFrameBuffer GetFrameBuffer()
		{
			return frameBuffer;
		}

		public Shader GetShader()
		{
			return activePhase.shader;
		}

		public void AddPhase(Action<RenderStep> phaseStart, Shader shader, IEnumerable<Drawable> enumeratorClause,
		                     Action<RenderStep> phaseEnd = null)
		{
			phases.Add(new RenderPhase(phaseStart, phaseEnd, enumeratorClause, shader));
		}

		public void Execute()
		{
			frameBuffer.Bind();
			frameBuffer.Clear();

			foreach (var phase in phases)
			{
				activePhase = phase;

				phase.shader.Begin();
					
				if (phase.start != null) phase.start(this);

				foreach (var item in phase.objects)
				{
					item.Render(dataBuffer, phase.shader);
				}

				if (phase.end != null) phase.end(this);

				phase.shader.Disable();
			}
		}

		struct RenderPhase
		{
			public Action<RenderStep> start;
			public Action<RenderStep> end;
			public IEnumerable<Drawable> objects;
			public Shader shader;

			public RenderPhase(Action<RenderStep> phaseStart, Action<RenderStep> phaseEnd,
			                   IEnumerable<Drawable> enumeratorClause, Shader activeShader)
			{
				start = phaseStart;
				end = phaseEnd;
				objects = enumeratorClause;
				shader = activeShader;
			}
		}
	}
}
