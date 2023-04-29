using Microsoft.Xna.Framework;
using sopra05_2223.Core.Entity;
using System;
using System.Collections.Generic;
using sopra05_2223.Core;

namespace sopra05_2223.Pathfinding
{
    internal sealed class Grid
    {
        private readonly PathNode[,] mGrid;
        private readonly int mGridWidth;
        private readonly int mGridHeight;
        private readonly int mGridTileDimension;
        private readonly Vector2 mWorldSize;


        public Grid(int gameWidth, int gameHeight, int gridTileDim)
        {
            mGridWidth = gameWidth / gridTileDim;
            mGridHeight = gameHeight / gridTileDim;
            mGridTileDimension = gridTileDim;
            mWorldSize = new Vector2(gameWidth, gameHeight);

            mGrid = new PathNode[mGridWidth, mGridHeight];

            for (var x = 0; x < mGridWidth; x++)
            {
                for (var y = 0; y < mGridHeight; y++)
                {
                    var newNode = new PathNode(null, new Point(x, y))
                    {
                        mG = int.MaxValue,
                        mIsObstacle = false
                    };
                    newNode.mF = newNode.mG + newNode.mH;
                    mGrid[x, y] = newNode;

                }
            }
        }

        public void ResetGrid()
        {
            for (var x = 0; x < mGridWidth; x++)
            {
                for (var y = 0; y < mGridHeight; y++)
                {
                    if (mGrid[x, y].mIsObstacle == false)
                    {
                        var newNode = new PathNode(null, new Point(x, y))
                        {
                            mG = int.MaxValue,
                            mIsObstacle = false
                        };
                        newNode.mF = newNode.mG + newNode.mH;
                        mGrid[x, y] = newNode;

                    }
                }
            }
        }

        internal void InsertPlanetIntoGrid(Entity planet)
        {
            var gridCoordinatesToObstacle = new List<Vector2>();
            var entityPosition = new Vector2(planet.GetX(), planet.GetY());
            var tileX = (int)Math.Floor(entityPosition.X / mGridTileDimension);
            var tileY = (int)Math.Floor(entityPosition.Y / mGridTileDimension);
            var entityRectangle = new Rectangle((int)entityPosition.X - (int)(mGridTileDimension * 1.5), (int)entityPosition.Y - (int)(mGridTileDimension * 1.5),
                planet.GetWidth() + (int)(mGridTileDimension * 2.0), planet.GetHeight() + (int)(mGridTileDimension * 2.0));

            if (mGridTileDimension < 400)
            {
                for (var x = tileX - 3; x <= (entityPosition.X + planet.GetWidth()) / mGridTileDimension + 3; x++)
                {
                    for (var y = tileY - 3; y <= (entityPosition.Y + planet.GetHeight()) / mGridTileDimension + 3; y++)
                    {
                        var tile = new Rectangle(x * mGridTileDimension, y * mGridTileDimension, mGridTileDimension,
                            mGridTileDimension);
                        var intersect = Rectangle.Intersect(entityRectangle, tile);
                        if (intersect.IsEmpty)
                        {
                            continue;
                        }

                        if (x < mGridWidth && y < mGridHeight && x > 0 && y > 0)
                        {
                            gridCoordinatesToObstacle.Add(new Vector2(x, y));
                        }
                    }
                }
            }
            if (mGridTileDimension is >= 400 and <= 500)
            {
                for (var x = tileX; x <= (entityPosition.X + planet.GetWidth()) / mGridTileDimension; x++)
                {
                    for (var y = tileY; y <= (entityPosition.Y + planet.GetHeight()) / mGridTileDimension; y++)
                    {
                        var tile = new Rectangle(x * mGridTileDimension, y * mGridTileDimension, mGridTileDimension,
                            mGridTileDimension);
                        var intersect = Rectangle.Intersect(entityRectangle, tile);
                        if (intersect.IsEmpty)
                        {
                            continue;
                        }

                        if (x < mGridWidth && y < mGridHeight && x > 0 && y > 0)
                        {
                            gridCoordinatesToObstacle.Add(new Vector2(x, y));
                        }
                    }
                }
            }
            else
            {
                var midEntityX = (planet.GetX() + planet.GetWidth() / 2) / mGridTileDimension;
                var midEntityY = (planet.GetY() + planet.GetHeight() / 2) / mGridTileDimension;
                if (midEntityX >= 0 && midEntityX < mGridWidth && midEntityY >= 0 && midEntityY < mGridHeight)
                {
                    gridCoordinatesToObstacle.Add(new Vector2(midEntityX, midEntityY));
                }
            }

            SetObstacles(gridCoordinatesToObstacle);
        }

        internal void InsertPlanetsIntoGrid(List<Entity> obList)
        {
            foreach (var entity in obList)
            {
                InsertPlanetIntoGrid(entity);
            }
        }


        internal void InsertBaseIntoGrid(Entity baseBase)
        {
            var gridCoordinatesToObstacle = new List<Vector2>();
            var entityPosition = new Vector2(baseBase.GetX(), baseBase.GetY());
            var tileX = (int)Math.Floor(entityPosition.X / mGridTileDimension);
            var tileY = (int)Math.Floor(entityPosition.Y / mGridTileDimension);
            if (mGridTileDimension < 400)
            {
                for (var x = tileX - 3; x < (entityPosition.X + baseBase.GetWidth()) / mGridTileDimension + 3; x++)
                {
                    for (var y = tileY - 3;
                         y < (entityPosition.Y + baseBase.GetHeight()) / mGridTileDimension + 3;
                         y++)
                    {
                        if (x < mGridWidth && y < mGridHeight && x > 0 && y > 0)
                        {
                            gridCoordinatesToObstacle.Add(new Vector2(x, y));
                        }
                    }
                }
            }
            if (mGridTileDimension is >= 400 and <= 1000)
            {
                for (var x = tileX; x < (entityPosition.X + baseBase.GetWidth()) / mGridTileDimension; x++)
                {
                    for (var y = tileY;
                         y < (entityPosition.Y + baseBase.GetHeight()) / mGridTileDimension;
                         y++)
                    {
                        if (x < mGridWidth && y < mGridHeight && x > 0 && y > 0)
                        {
                            gridCoordinatesToObstacle.Add(new Vector2(x, y));
                        }
                    }
                }
            }
            else
            {
                var midEntityX = (baseBase.GetX() + baseBase.GetWidth() / 2) / mGridTileDimension;
                var midEntityY = (baseBase.GetY() + baseBase.GetHeight() / 2) / mGridTileDimension;
                if (midEntityX >= 0 && midEntityX < mGridWidth && midEntityY >= 0 && midEntityY < mGridHeight)
                {
                    gridCoordinatesToObstacle.Add(new Vector2(midEntityX, midEntityY));
                }
            }
            SetObstacles(gridCoordinatesToObstacle);
        }

        internal void InsertPlanetBasesIntoGrid(List<Entity> obList)
        {
            foreach (var entity in obList)
            {
                InsertBaseIntoGrid(entity);
            }
        }


        public int GetGridWidth()
        {
            return mGridWidth;
        }

        public int GetGridHeight()
        {
            return mGridHeight;
        }

        public bool IsObstacle(int x, int y)
        {
            x = Math.Max(0, x);
            x = Math.Min(x, mGridWidth - 1);
            y = Math.Max(0, y);
            y = Math.Min(y, mGridHeight - 1);
            return mGrid[x, y].mIsObstacle;
        }

        public int GetGridTileDimension()
        {
            return mGridTileDimension;
        }

        /* Unused
        public void RemoveObstacle(int x, int y)
        {
            mGrid[x, y].mIsObstacle = false;
        }
        */

        public PathNode GetPathNode(int x, int y)
        {
            x = Math.Max(0, x);
            x = Math.Min(x, mGridWidth - 1);
            y = Math.Max(0, y);
            y = Math.Min(y, mGridHeight - 1);
            return mGrid[x, y];
        }

        private void SetObstacle(int x, int y)
        {
            x = Math.Max(0, x);
            x = Math.Min(x, mGridWidth - 1);
            y = Math.Max(0, y);
            y = Math.Min(y, mGridHeight - 1);
            mGrid[x, y].mIsObstacle = true;
        }

        private void SetObstacles(IEnumerable<Vector2> objectPositions)
        {
            foreach (var k in objectPositions)
            {
                SetObstacle((int)k.X, (int)k.Y);
            }
        }

        public Vector2 GetGridCoordinatesToMapCoordinates(float xPos, float yPos)
        {
            return new Vector2((float)Math.Floor(xPos / mGridTileDimension),
                (float)Math.Floor(yPos / mGridTileDimension));
        }

        private Vector2 GridAdjustedCoordinates(int x, int y)
        {
            var vec = GetGridCoordinatesToMapCoordinates(x, y);
            if (vec.X > mGridWidth - 1)
            {
                vec.X = mGridWidth - 1;
            }
            if (vec.X < 0)
            {
                vec.X = 0;
            }
            if (vec.Y < 0)
            {
                vec.Y = 0;
            }
            if (vec.Y > mGridHeight - 1)
            {
                vec.Y = mGridHeight - 1;
            }

            return vec;
        }

        public bool TestIfMouseClickInObstacle(int x, int y)
        {
            if (x < 0 || y < 0 || x >= mWorldSize.X
                || y >= mWorldSize.Y)
            {
                return true;
            }
            var vec = GridAdjustedCoordinates(x, y);
            return mGrid[(int)vec.X, (int)vec.Y].mIsObstacle;
        }


        private bool TestIfPathIsAvailable(Vector2 start, Vector2 end)
        {
            var startGrid = GridAdjustedCoordinates((int)start.X, (int)start.Y);
            startGrid.X = Math.Max(0, startGrid.X);
            startGrid.Y = Math.Max(0, startGrid.Y);
            startGrid.X = Math.Min(startGrid.X, mGridWidth - 1);
            startGrid.Y = Math.Min(startGrid.Y, mGridHeight - 1);
            var endGrid = GridAdjustedCoordinates((int)end.X, (int)end.Y);
            
            if (mGrid[(int)endGrid.X, (int)endGrid.Y].mIsObstacle ||
                mGrid[(int)startGrid.X, (int)startGrid.Y].mIsObstacle)
            {
                return false;
            }

            if (startGrid.Y - endGrid.Y < 0)
            {
                (startGrid, endGrid) = (endGrid, startGrid);
            }

            double anK = endGrid.X - startGrid.X;
            /*
            will always be positive because we swap
            start, end if deltaY < 0
            */
            double geK = startGrid.Y - endGrid.Y;

            /*
            we want the abs value, direction is already
            which way we move over the x-axis is given by
            anK (if anK is negative we are in the second quadrant
            or quadrant four
            */
            var gradient = Math.Abs(geK / anK);

            /*
             * if anK is null, this means we have to check
             * a path which is a corresponding vertical move
             * in the grid, so x stays the same and y-increases
             */
            if (anK == 0)
            {
                for (var y = 0; y <= geK; y++)
                {
                    var xCoords = startGrid.X;
                    var yCoords = startGrid.Y - y;

                    xCoords = Math.Max(0, xCoords);
                    xCoords = Math.Min(xCoords, mGridWidth - 1);
                    yCoords = Math.Max(0, yCoords);
                    yCoords = Math.Min(yCoords, mGridHeight - 1);

                    if (mGrid[(int)xCoords, (int)yCoords].mIsObstacle)
                    {
                        return false;
                    }
                }
            }

            /*
             * if geK is null, this means we have to check
             * a path which is a corresponding horizontal move
             * in the grid, so y stays the same and x-increases
            */
            if (geK == 0)
            {
                for (var x = 0; x <= Math.Abs(anK); x++)
                {
                    var yCoords = startGrid.Y;
                    float xCoords;

                    if (anK > 0)
                    {
                        xCoords = startGrid.X + x;
                    }
                    else
                    {
                        xCoords = startGrid.X - x;
                    }

                    xCoords = Math.Max(0, xCoords);
                    xCoords = Math.Min(xCoords, mGridWidth - 1);
                    yCoords = Math.Max(0, yCoords);
                    yCoords = Math.Min(yCoords, mGridHeight - 1);


                    if (mGrid[(int)xCoords, (int)yCoords].mIsObstacle)
                    {
                        return false;
                    }
                }
            }



            /*
             * we have to introduce a last value checked to know
             * from which y value we have to start checking to our
             * current x and corresponding deltaY (gradient * x)
             */
            var lastCheckedTo = 0.00;
            for (var x = 0; x <= Math.Abs(anK); x++)
            {
                var deltaY = gradient * x;
                //lastCheckedTo = deltaY - 1;

                for (var y = lastCheckedTo; y <= deltaY; y++)
                {
                    float xCoords;
                    if (anK > 0)
                    {
                        /*
                         * if anK is greater 0, we move right,
                         * our coordinates to check are in the third or first
                         * quadrant
                         */
                        xCoords = startGrid.X + x;
                    }
                    else
                    {
                        /*
                         * if anK is smaller 0, we move left,
                         * our coordinates to check are in quadrant second or four
                         * thus end.X < start.X
                        */
                        xCoords = startGrid.X - x;
                    }
                    var yCoords = startGrid.Y - y;

                    /*
                     * Just in case we want to make sure we get legitimate
                     * coordinate values to our grid, not really necessary
                     * but could possibly safe us from index errors while
                     * gaming
                    */
                    xCoords = Math.Max(0, xCoords);
                    xCoords = Math.Min(xCoords, mGridWidth - 1);
                    yCoords = Math.Max(0, yCoords);
                    yCoords = Math.Min(yCoords, mGridHeight - 1);


                    if (yCoords <= Math.Ceiling(yCoords) && yCoords >= Math.Floor(yCoords))
                    {
                        if (mGrid[(int)xCoords, (int)Math.Ceiling(yCoords)].mIsObstacle)
                        {
                            return false;
                        }
                        if (mGrid[(int)xCoords, (int)Math.Ceiling(yCoords)].mIsObstacle)
                        {
                            return false;
                        }
                    }

                    if (mGrid[(int)xCoords, (int)yCoords].mIsObstacle)
                    {
                        return false;
                    }
                }
                lastCheckedTo = deltaY;

            }
            return true;

        }

        public Vector2 GetClosestFreeGridCoordinateGeneral(Vector2 pos)
        {
            var tileCoordinate = pos;

            var distY = 0;
            var distX = 0;

            while (true)
            {
                if ((int)tileCoordinate.Y - distY >= 0)
                {
                    if (mGrid[(int)tileCoordinate.X, (int)tileCoordinate.Y - distY].mIsObstacle == false)
                    {
                        distY = -distY;
                        break;
                    }
                }

                if ((int)tileCoordinate.Y + distY < mGridHeight)
                {
                    if (mGrid[(int)tileCoordinate.X, (int)tileCoordinate.Y + distY].mIsObstacle == false)
                    {
                        break;
                    }
                }

                distY += 1;

                if (distY > 100)
                {
                    break;
                }
            }

            while (true)
            {
                if ((int)tileCoordinate.X + distX < mGridWidth)
                {
                    if (mGrid[(int)tileCoordinate.X + distX, (int)tileCoordinate.Y].mIsObstacle == false)
                    {
                        break;
                    }
                }

                if ((int)tileCoordinate.X - distX >= 0)
                {
                    if (mGrid[(int)tileCoordinate.X - distX, (int)tileCoordinate.Y].mIsObstacle == false)
                    {
                        distX = -distX;
                        break;
                    }
                }

                distX += 1;

                if (distX > 100)
                {
                    break;
                }
            }

            if (Math.Abs(distX) < Math.Abs(distY))
            {
                tileCoordinate = new Vector2((int)tileCoordinate.X + distX, (int)tileCoordinate.Y);
            }
            else
            {
                tileCoordinate = new Vector2((int)tileCoordinate.X, (int)tileCoordinate.Y + distY);
            }

            return tileCoordinate;
        }


        public bool TestIfDirectPathIsAvailable(Entity e, Vector2 end)
        {

            var vec1Start = new Vector2(e.GetX(), e.GetY());
            var vec2Start = new Vector2(e.GetX() + e.GetWidth() / 2, e.GetY() + e.GetHeight() / 2);
            var vec3Start = new Vector2(e.GetX() + e.GetWidth(), e.GetY() + e.GetHeight());

            var vec1Target = new Vector2(end.X - e.GetWidth() / 2f, end.Y - e.GetHeight() / 2f);
            var vec2Target = end;
            var vec3Target = new Vector2(end.X + e.GetWidth() / 2f, end.Y + e.GetHeight() / 2f);


            if (TestIfPathIsAvailable(vec1Start, vec1Target) == false ||
                TestIfPathIsAvailable(vec2Start, vec2Target) == false ||
                TestIfPathIsAvailable(vec3Start, vec3Target) == false
                )
            {
                return false;
            }

            if (Globals.mGridExtended.TestIfPathIsAvailable(vec1Start, vec1Target) == false ||
                Globals.mGridExtended.TestIfPathIsAvailable(vec2Start, vec2Target) == false ||
                Globals.mGridExtended.TestIfPathIsAvailable(vec3Start, vec3Target) == false)
            {
                    return false;
            }
            
            return true;
        }
    }
}
