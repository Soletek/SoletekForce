using System;
using System.Collections.Generic;

namespace SoletekForce.Animation
{
	/// <summary>
	/// An animator that animates a value in time by interpolating it between two 
	/// </summary>
	public class InvokeAnimator : AnimatorChannel
	{
		Invocation nextInvocation;
		readonly Queue<Invocation> timedInvocations = new Queue<Invocation>();

		public void AddInvocation(float timing, Action function)
		{
			timedInvocations.Enqueue(new Invocation(function, timing + hostAnimator.Head));
		}

		public override void Reset()
		{
			timedInvocations.Clear();
			nextInvocation = null;
		}

		Invocation DequeueInvocation()
		{
			if (timedInvocations.Count == 0) return null;
			return timedInvocations.Dequeue();
		}

		public override bool Update(float phase)
		{
			if (running)
			{
				if (nextInvocation == null) nextInvocation = DequeueInvocation();

				while (nextInvocation != null && phase >= nextInvocation.Timing)
				{
					nextInvocation.Function.Invoke();
					nextInvocation = DequeueInvocation();
				}

				if (nextInvocation == null) Stop();
			}

			return running;
		}

		class Invocation
		{
			public Action Function;
			public float Timing;

			public Invocation(Action function, float timing)
			{
				Function = function;
				Timing = timing;
			}
		}
	}
}