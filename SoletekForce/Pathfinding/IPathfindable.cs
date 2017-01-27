using System.Collections.Generic;

namespace SoletekForce.Pathfinding
{
	public interface IPathfindable
	{
		IEnumerable<IPathfindNode> AllNodes();
		List<IPathfindNode> GetNeighbours(IPathfindNode cell);
	}
}
