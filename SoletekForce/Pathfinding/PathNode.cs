using System;

namespace SoletekForce.Pathfinding
{
	public class PathNode
	{
		public int data = -1;
		public readonly IPathfindNode Host;

		public PathNode(IPathfindNode host)
		{
			Host = host;
		}
	}
}
