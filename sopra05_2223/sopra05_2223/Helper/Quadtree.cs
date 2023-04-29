using sopra05_2223.Core.Entity;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace sopra05_2223.Helper;

public sealed class Quadtree
{
    private Quadtree mQtNorthWest;
    private Quadtree mQtNorthEast;
    private Quadtree mQtSouthWest;
    private Quadtree mQtSouthEast;

    private bool mSplit;
    private List<Entity> mEntities;
    // scale up by * 2 if stackoverflow
    private const int Limit = 32;
    private readonly bool mActivateLimit = true;
    private const int SizeMinimum = 100;


    private readonly int mPosx;
    private readonly int mPosy;
    private readonly int mWidth;
    private readonly int mHeight;

    [JsonConstructor]
    internal Quadtree(int posx, int posy, int width, int height)
    {
        if (width <= SizeMinimum || height <= SizeMinimum)
        {
            mActivateLimit = false;
        }
        mSplit = false;
        this.mPosx = posx;
        this.mPosy = posy;
        this.mWidth = width;
        this.mHeight = height;
        mEntities = new List<Entity>();
    }

    // adds entity to quadtree.
    // if quadtree is split, tries to add entity to subtrees. one of these have to intersect with the entity and not be split
    internal void AddEntity(Entity e)
    {
        if (!this.Intersects(e.GetX(), e.GetY(), e.GetWidth(), e.GetHeight()))
        {
            return;
        }

        if (mSplit)
        {
            mQtNorthWest.AddEntity(e);
            mQtNorthEast.AddEntity(e);
            mQtSouthWest.AddEntity(e);
            mQtSouthEast.AddEntity(e);
        }
        else
        {
            if (mActivateLimit && Limit <= mEntities.Count)
            {
                mEntities.Add(e);
                Split();
            }
            else
            {
                mEntities.Add(e);
            }
        }
    }

    private bool Intersects(int x, int y, int width, int height)
    {
        // left of one another
        if (this.mPosx > x + width || x > this.mPosx + this.mWidth)
        {
            return false;
        }
        // right of one another
        if (this.mPosy > y + height || y > this.mPosy + this.mHeight)
        {
            return false;
        }

        return true;
    }


    // this function returns the Entities, against which collision should be checked
    internal List<Entity> Query(int x, int y, int width, int height)
    {
        if (this.Intersects(x, y, width, height))
        {
            if (!mSplit)
            {
                return mEntities;
            }

            var entitiesNw = mQtNorthWest.Query(x, y, width, height);
            var entitiesNe = mQtNorthEast.Query(x, y, width, height);
            var entitiesSw = mQtSouthWest.Query(x, y, width, height);
            var entitiesSe = mQtSouthEast.Query(x, y, width, height);

            var entities =new List<Entity>();

            foreach (var e in entitiesNw.Where(e => e != null))
            {
                entities.Add(e);
            }

            foreach (var e in entitiesNe.Where(e => e != null))
            {
                entities.Add(e);
            }

            foreach (var e in entitiesSw.Where(e => e != null))
            {
                entities.Add(e);
            }

            foreach (var e in entitiesSe.Where(e => e != null))
            {
                entities.Add(e);
            }
            return entities;
        }
        
        return new List<Entity>();
    }

    // splits the quadtree into 4 sub trees. only used once limit is reached
    private void Split()
    {
        mQtNorthWest = new Quadtree(
                this.mPosx,
                this.mPosy,
                this.mWidth / 2,
                this.mHeight / 2);
            mQtNorthEast = new Quadtree(
                this.mPosx + this.mWidth / 2,
                this.mPosy,
                this.mWidth / 2,
                this.mHeight / 2);
            mQtSouthWest = new Quadtree(
                this.mPosx,
                this.mPosy + this.mHeight / 2,
                this.mWidth / 2,
                this.mHeight / 2);
            mQtSouthEast = new Quadtree(
                this.mPosx + this.mWidth / 2,
                this.mPosy + this.mHeight / 2,
                this.mWidth / 2,
                this.mHeight / 2);

            foreach (var e in mEntities)
            {
                if (e == null)
                {
                    return;
                }

                mQtNorthWest.AddEntity(e);
                mQtNorthEast.AddEntity(e);
                mQtSouthWest.AddEntity(e);
                mQtSouthEast.AddEntity(e);
            }

            mSplit = true;
            mEntities = null;
    }
}