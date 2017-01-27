using System;
using System.Text;
using System.Collections.Generic;

namespace SoletekForce.Pathfinding
{
	/// <summary>
	/// Contains a saved route discovered in Path Search
	/// </summary>
	public class PathRoute : IPathfindResult
	{
		readonly List<PathNode> route;
		public IPathfindable Network { get; private set; }

		public int Length
		{
			get
			{
				return route.Count;
			}
		}

		public PathRoute(IPathfindable network, IPathfindNode targetPoint)
		{
			route = new List<PathNode>();
			Network = network;

			if (targetPoint.Node != null && targetPoint.Node.data != -1)
			{
				PathNode current = targetPoint.Node;
				route.Add(current);

				while (current.data != 0)
				{
					PathNode closest = null;

					foreach (var neighbourCell in network.GetNeighbours(current.Host))
					{
						PathNode neighbour = neighbourCell.Node;
						if (neighbour.data != -1)
						{
							if (closest == null || neighbour.data < closest.data)
							{
								closest = neighbour;
							}
						}
					}

					current = closest;
					route.Add(current);
				}
			}
		}

		public override string ToString()
		{
			var sb = new StringBuilder();

			if (route.Count > 0)
			{
				foreach (var node in route)
				{
					sb.Append(node.Host + " ");
				}
			}
			else
			{
				sb.Append("No route.");
			}

			return sb.ToString();
		}

		public IEnumerable<PathNode> Nodes()
		{
			foreach (var node in route)
			{
				yield return node;
			}
		}

		/// <summary>
		/// Enumerates through all nodes in the route from end to start 
		/// and yields them casted as TCellType
		/// </summary>
		public IEnumerable<TCellType> Nodes<TCellType>()
			where TCellType : IPathfindNode
		{
			foreach (var node in route)
			{
				yield return (TCellType)(node.Host);
			}
		}
	}
}
