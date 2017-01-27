using System;
using System.Linq;
using System.Collections.Generic;

namespace SoletekForce.Pathfinding
{
	/// <summary>
	/// A Breath first based algorithm for path search and distance calculation.
	/// </summary>
	public class PathSearch<TNetwork>
		where TNetwork : IPathfindable
	{
		readonly TNetwork map;
		readonly Queue<PathNode> frontier;

		CellBlockingDelegate CheckCell;
		IPathfindNode targetLimit;
		int depth;
		int depthLimit;

		/// <summary>
		/// True, if the specified target node was reached, otherwise false.
		/// </summary>
		public bool TargetFound { get; private set; }

		public delegate bool CellBlockingDelegate(IPathfindNode cell);
		static PathSearch<TNetwork> latest;
		public static PathSearch<TNetwork> Latest { get { return latest; } }

		/// <summary>
		/// Executes a breath first search until either depthLimit has reached, target Cell
		/// has been found or all accessible cells has been iterated
		/// </summary>
		public PathSearch(TNetwork map, IPathfindNode initial, CellBlockingDelegate checkFunction = null,
						  int depthLimit = -1, IPathfindNode targetLimit = null)
			: this(map, new List<IPathfindNode> {initial}, checkFunction, depthLimit, targetLimit)
		{}

		/// <summary>
		/// Executes a breath first search until either depthLimit has reached, target Cell
		/// has been found or all accessible cells has been iterated
		/// </summary>
		public PathSearch(TNetwork map, IEnumerable<IPathfindNode> initialSet, CellBlockingDelegate checkFunction = null, 
		                  int depthLimit = -1, IPathfindNode targetLimit = null)
		{
			this.map = map;
			this.depthLimit = depthLimit;
			this.targetLimit = targetLimit;
			frontier = new Queue<PathNode>();

			SetBlockingDelegate(checkFunction);
			PopulateNodes(map);

			foreach (var node in initialSet)
			{
				node.Node.data = 0;
				frontier.Enqueue(node.Node);
			}

			BreathFirstSearch();

			latest = this;
		}

		void SetBlockingDelegate(CellBlockingDelegate checkFunction)
		{
			CheckCell = checkFunction;
			if (CheckCell == null)
			{
				CheckCell = ((cell) => true);
			}
		}

		public TNetwork ReferencedMap
		{
			get { return map; }
		}

		/// <summary>
		/// Saves a field of nodes with the search distance between min and max (both inclusive)
		/// </summary>
		public PathField SaveAsField(int min = 0, int max = -1)
		{
			if (this != latest) return null;

			return new PathField(ReferencedMap, min, max);
		}

		/// <summary>
		/// Saves the route from search start node to 
		/// target node into a PathRoute object. (in reversed order)
		/// </summary>
		/// <param name="targetNode">Target node.</param>
		public PathRoute SaveAsRouteTo(IPathfindNode targetNode)
		{
			if (this != latest) return null;

			return new PathRoute(ReferencedMap, targetNode);
		}

		static void PopulateNodes(TNetwork map)
		{
			foreach (var cell in map.AllNodes())
			{
				cell.Node = new PathNode(cell);
			}
		}

		/// <summary>
		/// Recalls an existing search result and overlays the map with it. Overwrites the latest search.
		/// </summary>
		/// <returns><c>true</c>, if target cell was found, <c>false</c> otherwise.</returns>
		public static void Overlay(TNetwork map, IPathfindResult priorResult)
		{
			latest = null;

			PopulateNodes(map);

			foreach (var node in priorResult.Nodes())
			{
				node.Host.Node.data = node.data;
			}
		}

		/// <summary>
		/// Return the intersectiong area of two Pathfind results, containing
		/// the data values from first. Overwrites the latest search.
		/// </summary>
		public static PathField Intersect(IPathfindResult first, IPathfindResult second)
		{
			Overlay((TNetwork)second.Network, second);

			return new PathField(second.Network, first);
		}

		/// <summary>
		/// Continues a existing search based on new criteria
		/// </summary>
		/// <param name="depthIncrement">Depth increment.</param>
		/// <param name="targetLimit">Target limit.</param>
		/// <param name="checkFunction">Check function.</param>
		/// <param name="resetFrontier">If set to <c>true</c> reset the frontier to all cells with data.</param>
		public void Expand(int depthIncrement, IPathfindNode targetLimit = null, 
		                   CellBlockingDelegate checkFunction = null, bool resetFrontier = false)
		{
			TargetFound = false;

			if (resetFrontier || frontier.Count == 0)
			{
				frontier.Clear();

				foreach (var cell in map.AllNodes())
				{
					if (cell.Node.data != -1)
					{
						frontier.Enqueue(cell.Node);
					}
				}
			}

			depthLimit = depth + depthIncrement;
			this.targetLimit = targetLimit;
			SetBlockingDelegate(checkFunction);

			BreathFirstSearch();
		}

		/// <summary>
		/// Executes a breath first search until either depthLimit has reached, target Cell
		/// has been found or all accessible cells has been iterated
		/// </summary>
		/// <returns><c>true</c>, if target cell was found, <c>false</c> otherwise.</returns>
		bool BreathFirstSearch()
		{
			while (frontier.Count > 0)
			{
				var current = frontier.Dequeue();

				if (current.data + 1 > depth)
				{
					if (TargetFound) return true;
					depth++;
				}

				if (depthLimit != -1 && depth > depthLimit)
				{
					frontier.Enqueue(current);
					return false;
				}

				foreach (var neighbourCell in map.GetNeighbours(current.Host))
				{
					PathNode neighbour = neighbourCell.Node;
					if (neighbour.data == -1 && CheckCell(neighbourCell))
					{
						neighbour.data = depth;
						TargetFound |= neighbourCell == targetLimit;
						frontier.Enqueue(neighbour);
					}
				}
			}

			return false;
		}
	}
}
