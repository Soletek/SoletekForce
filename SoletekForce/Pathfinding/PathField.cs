using System;
using System.Collections.Generic;

namespace SoletekForce.Pathfinding
{
	/// <summary>
	/// Contains a saved field of cells discovered in Path Search
	/// </summary>
	public class PathField : IPathfindResult
	{
		public readonly List<PathNode> field;
		public IPathfindable Network { get; private set; }

		public int NodeCount
		{
			get
			{
				return field.Count;
			}
		}

		public PathField(IPathfindable network, int minRadius = 0, int maxRadius = -1)
		{
			field = new List<PathNode>();
			Network = network;

			foreach (var cell in network.AllNodes())
			{
				if (cell.Node != null 
				    && cell.Node.data >= minRadius 
				    && (maxRadius == -1 || cell.Node.data <= maxRadius))
				{
					field.Add(cell.Node);
				}
			}
		}

		internal PathField(IPathfindable network, IPathfindResult other)
		{
			field = new List<PathNode>();
			Network = network;

			foreach (var node in other.Nodes())
			{
				if (node.Host.Node.data != -1)
				{
					field.Add(node);
				}
			}
		}

		public IEnumerable<PathNode> Nodes()
		{
			foreach (var node in field)
			{
				yield return node;
			}
		}

		/// <summary>
		/// Enumerates through all nodes in the field and yields them
		/// casted as TCellType
		/// </summary>
		public IEnumerable<TCellType> Nodes<TCellType>()
			where TCellType : IPathfindNode
		{
			foreach (var node in field)
			{
				yield return (TCellType)(node.Host);
			}
		}
	}
}
