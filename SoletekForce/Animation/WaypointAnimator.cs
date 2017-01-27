using System;
using System.Collections.Generic;

namespace SoletekForce.Animation
{
	/// <summary>
	/// An animator that animates a value in time by interpolating it between two 
	/// </summary>
	public class WaypointAnimator<TData> : AnimatorChannel
		where TData : IInterpolatable<TData>
	{
		Waypoint<TData> active;
		Waypoint<TData> next;
		float lastTiming;

		readonly Action<TData> setTarget;

		readonly Queue<Waypoint<TData>> waypoints = new Queue<Waypoint<TData>>();

		public WaypointAnimator(Action<TData> setter)
		{
			setTarget = setter;
		}

		public void AddWaypoint(float timing, TData waypointData)
		{
			var waypoint = new Waypoint<TData>(waypointData, timing + hostAnimator.Head);
			waypoints.Enqueue(waypoint);
		}

		public override void Reset()
		{
			lastTiming = 0;
			waypoints.Clear();
			active = null;
			next = null;
		}

		Waypoint<TData> DequeueWaypoint()
		{
			if (waypoints.Count == 0) return null;
			return waypoints.Dequeue();
		}

		public override bool Update(float phase)
		{
			if (running)
			{
				if (active == null) active = DequeueWaypoint();
				if (next == null) {
					if (active != null && phase >= active.Timing)
					{
						lastTiming = active.Timing;
						next = DequeueWaypoint();
					}
				}

				if (next != null && phase >= next.Timing)
				{
					active = next;
					next = DequeueWaypoint();
					lastTiming = active.Timing;
				}

				if (next != null)
				{
					float blend = (phase - lastTiming) / (next.Timing - lastTiming);
					blend = Math.Max(0f, Math.Min(blend, 1f));
					setTarget(active.WaypointData.Interpolate(next.WaypointData, blend));
				}
				else if (waypoints.Count == 0)
				{
					if (active != null) setTarget(active.WaypointData);
					Stop();
				}
			}

			return running;
		}

		class Waypoint<TDataInner>
			where TDataInner : IInterpolatable<TDataInner>
		{
			public TDataInner WaypointData { get; set; }
			public float Timing { get; set; }

			public Waypoint(TDataInner waypointData, float timing)
			{
				WaypointData = waypointData;
				Timing = timing;
			}
		}
	}
}
