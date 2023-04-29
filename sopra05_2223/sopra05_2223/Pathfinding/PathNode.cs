using Microsoft.Xna.Framework;

namespace sopra05_2223.Pathfinding
{
    internal sealed class PathNode
    {
        internal PathNode mParent;
        internal Point mPoint;
        internal bool mIsObstacle;
        internal int mG;
        internal int mH;
        internal int mF;


        internal PathNode(PathNode parent, Point point)
        {
            mParent = parent;
            mPoint = point;
        }
        internal bool IsEqual(PathNode that)
        {
            return this.mPoint.Equals(that.mPoint);
        }
    }
}
