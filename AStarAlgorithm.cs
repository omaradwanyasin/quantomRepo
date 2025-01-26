using System;
using System.Collections.Generic;
using System.Linq;
using backend_api.Models;

public class AStarPathfinder
{
    
    public List<int> FindPath(Dictionary<int, Node> nodesMap, int startNodeId, int targetNodeId)
    {
        var path = new List<int>();

        if (!nodesMap.TryGetValue(startNodeId, out var startNode) ||
            !nodesMap.TryGetValue(targetNodeId, out var targetNode))
            return path;

        var openSet = new PriorityQueue<Node, double>();
        var cameFrom = new Dictionary<int, int>();
        var gScore = new Dictionary<int, double>();
        var fScore = new Dictionary<int, double>();

        foreach (var node in nodesMap.Values)
        {
            gScore[node.NodeId] = double.MaxValue;
            fScore[node.NodeId] = double.MaxValue;
        }

        gScore[startNodeId] = 0;
        fScore[startNodeId] = CalculateHeuristic(startNode, targetNode);
        openSet.Enqueue(startNode, fScore[startNodeId]);

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();

            if (current.NodeId == targetNodeId)
                return ReconstructPath(cameFrom, current.NodeId);

            foreach (var neighborId in current.ConnectedIds)
            {
                if (!nodesMap.TryGetValue(neighborId, out var neighbor))
                    continue;

                var tentativeGScore = gScore[current.NodeId] + CalculateDistance(current, neighbor);

                if (tentativeGScore < gScore[neighborId])
                {
                    cameFrom[neighborId] = current.NodeId;
                    gScore[neighborId] = tentativeGScore;
                    fScore[neighborId] = tentativeGScore + CalculateHeuristic(neighbor, targetNode);

                    if (!openSet.UnorderedItems.Any(x => x.Element.NodeId == neighborId))
                        openSet.Enqueue(neighbor, fScore[neighborId]);
                }
            }
        }

        return path; // Return empty path if no route found
    }

    private List<int> ReconstructPath(Dictionary<int, int> cameFrom, int currentNodeId)
    {
        var path = new List<int> { currentNodeId };

        while (cameFrom.ContainsKey(currentNodeId))
        {
            currentNodeId = cameFrom[currentNodeId];
            path.Insert(0, currentNodeId);
        }

        return path;
    }

    private double CalculateHeuristic(Node a, Node b)
    {
        return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
    }

    private double CalculateDistance(Node a, Node b)
    {
        return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
    }
}