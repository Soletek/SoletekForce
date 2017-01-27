using System.Collections.Generic;

namespace SoletekForce.Pathfinding
{
	public interface IPathfindResult
	{
		IEnumerable<PathNode> Nodes();
		IEnumerable<TCellType> Nodes<TCellType>() where TCellType : IPathfindNode;
		IPathfindable Network { get; }
	}
}
