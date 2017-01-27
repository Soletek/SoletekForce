using System;
using System.Diagnostics;

namespace SoletekForce
{
	public static class Timer
	{
		static readonly Stopwatch timer = new Stopwatch();
		static long ticksInLastUpdate = 0;

		static float _deltaTime;
		public static float DeltaTime
		{
			get { return _deltaTime; }
			private set { _deltaTime = value; }
		}

		internal static void Start()
		{
			timer.Start();
			ticksInLastUpdate = timer.ElapsedTicks;
		}

		internal static void Stop()
		{
			timer.Stop();
		}

		internal static void Update()
		{
			long ticks1 = timer.ElapsedTicks;
			long ticks = ticks1 - ticksInLastUpdate;

			ticksInLastUpdate += ticks;

			DeltaTime = (float)ticks / (float)Stopwatch.Frequency;
		}

		internal static float Runtime
		{
			get { return (float)ticksInLastUpdate / (float)Stopwatch.Frequency; }
		}
	}
}
