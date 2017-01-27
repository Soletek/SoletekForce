using System;
using System.Collections.Generic;

namespace SoletekForce.Animation
{
	public sealed class Animator
	{
		readonly List<AnimatorChannel> channels = new List<AnimatorChannel>();

		Entity holder;
		bool running;
		float phase;
		float head;

		public float SpeedMultiplier { get; set; } = 1.0f;

		public Animator(Entity holder)
		{
			this.holder = holder;
		}

		public void AddChannel(AnimatorChannel channel)
		{
			channel.SetAnimator(this);
			channels.Add(channel);
		}

		public float Head
		{
			get { return head; }
			set { head = value; } 
		}

		public bool Running
		{
			get { return running; }
		}

		public void Start()
		{
			running = true;

			foreach (var channel in channels) channel.Start();
		}

		public void Reset()
		{
			phase = 0;
			head = 0;
			Stop();

			foreach (var channel in channels) channel.Reset();
		}

		public void Stop()
		{
			running = false;

			foreach (var channel in channels) channel.Stop();
		}

		public void Update()
		{
			bool someRunning = false;

			if (running)
			{
				phase += Timer.DeltaTime * SpeedMultiplier;
				if (phase >= head) head = phase;

				foreach (var channel in channels) someRunning |= channel.Update(phase);
			}

			if (!someRunning)
			{
				running = false;
				Reset();
			}
		}

		public float TimeToHead()
		{
			return head - phase;
		}
	}
}
