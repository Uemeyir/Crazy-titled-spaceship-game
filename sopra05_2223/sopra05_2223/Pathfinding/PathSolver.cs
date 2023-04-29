using System;
using System.Collections.Generic;

namespace sopra05_2223.Pathfinding
{
    internal sealed class PathSolver
    {
        private const int DiagonalWeight = 14;
        private const int DirectWeight = 10;
        private readonly Grid mGrid;

        public PathSolver(Grid grid)
        {
            mGrid = grid;
        }

        //GCost: Distance between start node (a) an current node (b)
        private static int GCost(PathNode a, PathNode b)
        {
            if (!a.IsEqual(b))
            {
                return DiagonalWeight;
            }
            return DirectWeight;
        }

        //HCost: Heuristic cost between current node (a) and target node (b)
        private static int HCost(PathNode a, PathNode end)
        {
            var deltaX = (int)MathF.Abs(end.mPoint.X - a.mPoint.X);
            var deltaY = (int)MathF.Abs(end.mPoint.Y - a.mPoint.Y);
            var straight = (int)MathF.Abs(deltaX - deltaY);
            var diagonal = (int)MathF.Min(deltaX, deltaY);
            return straight * DirectWeight + DiagonalWeight * diagonal;
        }

        //FCost: The total cost for path node (n), add gCost + hCost
        private static int FCost(PathNode n)
        {
            return (n.mG + n.mH);
        }

        private static PathNode LowestFCost(IReadOnlyList<PathNode> list)
        {
            var bestNode = list[0];
            for (var i = 1; i < list.Count; i++)
            {
                if (list[i].mF < bestNode.mF)
                {
                    bestNode = list[i];
                }
            }

            return bestNode;
        }

        private static List<PathNode> ReturnPath(PathNode end, bool to)
        {
            var path = new List<PathNode>();
            var parentNode = end;
            while (parentNode.mParent != null)
            {
                path.Add(parentNode);
                parentNode = parentNode.mParent;
            }

            path.Reverse();

            if (to && path.Count > 0)
            {
                path.Add(path[0]);
            }

            return path;
        }

        private List<PathNode> GetAdjacentNodes(PathNode n)
        {
            var adjacentNodes = new List<PathNode>();
            for (var x = (n.mPoint.X - 1); x < (n.mPoint.X + 2); x++)
            {
                for (var y = (n.mPoint.Y - 1); y < (n.mPoint.Y + 2); y++)
                {
                    if (((x == n.mPoint.X) && (y == n.mPoint.Y)) || x < 0 ||
                         x >= mGrid.GetGridWidth() || y < 0 ||
                         y >= mGrid.GetGridHeight() || mGrid.IsObstacle(x, y))
                    {
                        continue;
                    }
                    adjacentNodes.Add(mGrid.GetPathNode(x, y));
                }
            }

            return adjacentNodes;
        }


        public List<PathNode> GetPath(int startX, int startY, int endX, int endY)
        {

            var startNode = mGrid.GetPathNode(startX, startY);
            var targetNode = mGrid.GetPathNode(endX, endY);
            bool to = false;

            if (startNode.mIsObstacle)
            {
                var newStart = mGrid.
                    GetClosestFreeGridCoordinateGeneral(startNode.mPoint.ToVector2());
                startNode = mGrid.GetPathNode((int)newStart.X, (int)newStart.Y);
            }

            if (targetNode.mIsObstacle)
            {
                to = true;
                var newTarget = mGrid.
                    GetClosestFreeGridCoordinateGeneral(targetNode.mPoint.ToVector2());
                targetNode = mGrid.GetPathNode((int)newTarget.X, (int)newTarget.Y);
            }

            startNode.mIsObstacle = false;
            targetNode.mIsObstacle = false;

            startNode.mG = 0;
            startNode.mH = HCost(startNode, targetNode);
            startNode.mF = FCost(startNode);

            var openList = new List<PathNode>();
            var closedList = new List<PathNode>();

            openList.Add(startNode);

            var count = 0;

            while (openList.Count > 0)
            {
                count += 1;

                if (count > 2000)
                {
                    return null;
                }

                var currentNode = LowestFCost(openList);
                if (currentNode.IsEqual(targetNode))
                {
                    return ReturnPath(currentNode, to);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (var adjacentNode in GetAdjacentNodes(currentNode))
                {
                    if (closedList.Contains(adjacentNode))
                    {
                        continue;
                    }

                    var adjacentNodeGCost = currentNode.mG +
                                            GCost(currentNode, adjacentNode);
                    if (adjacentNodeGCost >= adjacentNode.mG)
                    {
                        continue;
                    }

                    adjacentNode.mParent = currentNode;
                    adjacentNode.mG = adjacentNodeGCost;
                    adjacentNode.mH = HCost(adjacentNode, targetNode);
                    adjacentNode.mF = FCost(adjacentNode);

                    if (!openList.Contains(adjacentNode))
                    {
                        openList.Add(adjacentNode);
                    }
                }
            }

            return openList;
        }
    }
}
