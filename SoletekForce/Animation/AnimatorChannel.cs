using System;
namespace SoletekForce.Animation
{
	public abstract class AnimatorChannel
	{
		protected bool running;
		protected Animator hostAnimator { get; private set; }

		internal void SetAnimator(Animator animator)
		{
			hostAnimator = animator;
		}

		public virtual void Start()
		{
			running = true;
		}

		public virtual void Stop()
		{
			running = false;
		}

		public abstract void Reset();
		public abstract bool Update(float phase);
	}
}
