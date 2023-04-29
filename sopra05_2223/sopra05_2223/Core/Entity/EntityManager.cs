using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sopra05_2223.Helper;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using sopra05_2223.Core.Components;
using sopra05_2223.Core.Entity.Base;
using sopra05_2223.Core.Entity.Ships;
using sopra05_2223.Core.ShipBase;
using sopra05_2223.Core.Entity.Base.SpaceBase;
using sopra05_2223.Core.Entity.Misc;
using sopra05_2223.Core.Entity.Resource;
using System.Threading;

namespace sopra05_2223.Core.Entity;

internal sealed class EntityManager
{
    public List<Entity> Entities { get; } = new();
    // Entities that will be added/removed until next frame.
    [JsonRequired]
    private readonly List<Entity> mEntitiesToAdd = new();
    [JsonRequired]
    private readonly List<Entity> mEntitiesToRemove = new();
    [JsonRequired]
    internal List<Entity> mSelectedEntities = new();

    [JsonRequired]
    internal readonly List<Entity> mToMovePlayerPlayer = new();
    [JsonRequired]
    internal readonly List<Entity> mToMovePlayerKi = new();
    [JsonRequired]
    private readonly List<Entity> mBackgroundEntities = new();

    private int mCounter;
    internal bool mIsMinimapUpdateFrame;
    private float mElapsed = 1;

    [JsonRequired]
    private Quadtree mQuadTree;
    [JsonRequired]
    private bool mUpdating;
    [JsonRequired]
    internal readonly Point mWorldSize;

    private Rectangle mCameraPosition;
    private Matrix mTranslationMatrix;

    public EntityManager(Point worldSize)
    {
        mWorldSize = worldSize;
        mCameraPosition = new Rectangle(0, 0, 0, 0);
    }

    public void Add(Entity entity)
    {
        entity.mEntityManager = this;
        if (!mUpdating)
        {
            AddEntity(entity);
        }
        else
        {
            mEntitiesToAdd.Add(entity);
        }
    }

    private void AddEntity(Entity entity)
    {
        Entities.Add(entity);

        if (entity is EResource or EBase)
        {
            mBackgroundEntities.Add(entity);
        }
    }

    public void Remove(Entity entity)
    {
        if (!mUpdating)
        {
            RemoveEntity(entity);
        }
        else
        {
            mEntitiesToRemove.Add(entity);
        }
    }

    private void RemoveEntity(Entity entity)
    {
        entity.GetComponent<CTeam>()?.GetProtagonist()?.RemoveEntity(entity);

        if (entity is EResource or EBase)
        {
            mBackgroundEntities.Remove(entity);
        }
        Entities.Remove(entity);
    }
    private void CalculateCallBack(object f)
    {
        if (f == null) { return; }
        var e = (Entity)f;
        var xd = e.GetWidth();
        var yd = e.GetHeight();
        var x = e.GetComponent<CView>();
        // Bad detection of TechDemo
        if (mIsMinimapUpdateFrame && mWorldSize == new Point(80000, 80000))
        {
            xd = x == null ? e.GetWidth() : x.mViewRadius;
            yd = x == null ? e.GetHeight() : x.mViewRadius;
        }

        e.mCloseEntities = mQuadTree.Query((int)e.GetPos().X - xd, (int)e.GetPos().Y - yd, xd * 2, yd * 2);
    }
    public void Update()
    {
        mUpdating = true;
        mCounter += 1;
        mCounter %= 5000;

        if (mElapsed >= 1)
        {
            mIsMinimapUpdateFrame = true;
            mElapsed -= 1;
        }
        else
        {
            mIsMinimapUpdateFrame = false;
        }

        mElapsed += (float)Globals.GameTime.ElapsedGameTime.TotalSeconds;

        // ----------Collision Detection-----------------
        // Build qt
        mQuadTree = new Quadtree(0, 0, mWorldSize.X, mWorldSize.Y);
        foreach (var entity in Entities)
        {
            mQuadTree.AddEntity(entity);
        }
        foreach (var e in Entities)
        {
            ThreadPool.QueueUserWorkItem(CalculateCallBack, e);
        }
        // ------------Collision Detection End ----------
        
        // set path for one entity at the time, wait ten
        // updates till next one moves

        if (mToMovePlayerPlayer.Count > 0) //&& mCounter % 10 == 0)
        {
            var entityToMove = mToMovePlayerPlayer[0];
            mToMovePlayerPlayer.Remove(entityToMove);

            var lstToMove = new List<Entity>()
            {
                entityToMove
            };

            if (entityToMove is ETransport e)
            {
                foreach (var collector in e.GetCollectors())
                {
                    lstToMove.Add(collector);
                }
            }

            Globals.mMoveEntities.Move(lstToMove, 
                entityToMove.GetComponent<CTransform>().GetTransitoryTarget(), true);
        }
        // -------one entity of PlayerPlayer will start moving now -------

        if (mToMovePlayerKi.Count > 0 && (mCounter + 3) % 6 == 0)
        {
            var entityToMove = mToMovePlayerKi[0];
            mToMovePlayerKi.Remove(entityToMove);

            var lstToMove = new List<Entity>()
            {
                entityToMove
            };

            if (entityToMove is ETransport e)
            {
                foreach (var collector in e.GetCollectors())
                {
                    lstToMove.Add(collector);
                }
            }

            Globals.mMoveEntities.Move(lstToMove,
                entityToMove.GetComponent<CTransform>().GetTransitoryTarget(), true);
        }
        // -------one entity of PlayerKi will start moving now -------



        // target closest entity of opponent, if  within action radius of shooter => fire gun
        foreach (var entity in Entities.Where(x => x is EAttacker or EMoerser or EMedic))
        {
            var gun = entity.GetComponent<CGun>();
            var targetEntity = gun.GetTarget();

            if (targetEntity == null)
            {
                continue;
            }

            if (entity is EAttacker or EMoerser)
            {
                if (Entities.Contains(targetEntity) == false || mEntitiesToRemove.Contains(targetEntity) ||
                    targetEntity.GetComponent<CTeam>()?.GetProtagonist()
                    == entity.GetComponent<CTeam>()?.GetProtagonist() ||
                    targetEntity.GetComponent<CHealth>()?.GetHealth() == 0
                    || entity.GetComponent<CTeam>()?.GetTeam() is Team.Neutral)
                {
                    entity.GetComponent<CGun>()?.SetTarget(null);
                }
            }
            if (entity is EMedic)
            {
                if (targetEntity.GetComponent<CHealth>()?.GetHealth() == 100 ||
                    Entities.Contains(targetEntity) == false || mEntitiesToRemove.Contains(targetEntity) ||
                    targetEntity.GetComponent<CHealth>()?.GetHealth() == 0
                    || entity.GetComponent<CTeam>()?.GetTeam() is Team.Neutral)
                {
                    entity.GetComponent<CGun>()?.SetTarget(null);
                }
            }
        }

        foreach (var entity in Entities)
        {
            entity.Update();
        }

        mUpdating = false;

        // Add/Remove the collected Entities from the list.

        foreach (var entity in mEntitiesToAdd)
        {
            AddEntity(entity);
        }

        mEntitiesToAdd.Clear();


        foreach (var entity in mEntitiesToRemove)
        {
            var player = entity.GetComponent<CTeam>().GetProtagonist();

            if (entity is EResource)
            {
                foreach (var c in Entities.Where(x => x is ECollector))
                {
                    var x = c.GetComponent<CollectCollector>();
                    if (x.GetTarget() == entity)
                    {
                        x.RemoveTarget();
                        x.MoveToOwner(false);
                    }
                }
            }

            player?.RemoveEntity(entity);

            ETransport toRemove = null;
            if (entity is ETransport et)
            {
                toRemove = et;
            }

            if (toRemove is not null)
            {
                foreach (var c in toRemove.GetCollectors())
                {
                    Remove(c);
                }
            }

            RemoveEntity(entity);

        }

        mEntitiesToRemove.Clear();
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle cameraRect, Matrix translationMatrix)
    {
        foreach (var e in mBackgroundEntities.Where(x => cameraRect.Intersects(x.Rectangle)))
        {
            e.Draw(spriteBatch, translationMatrix);
            if (e is ESpaceBase)
            {
                DrawBuildingShips(spriteBatch, e);
            }
        }
        // Draw only visible entities.
        foreach (var entity in Entities.Where(x => cameraRect.Intersects(x.Rectangle)))
        {
            switch (entity)
            {
                case EResource or EBase: // Skip BackgroundEntities, as they're drawn above
                    continue;
                    /*
                case ESpaceBase1:
                    DrawBuildingShips(spriteBatch, entity);
                    break;
                    */
                case EBuoy:
                    entity.GetComponent<CLifespan>().Draw(spriteBatch, translationMatrix);
                    break;
            }

            entity.Draw(spriteBatch, translationMatrix);
        }
    }

    private void DrawBuildingShips(SpriteBatch spriteBatch, Entity entity)
    {
        foreach (var k in entity.GetComponent<BuildSlotManager>().GetBuildSlots())
        {
            if (k.IsUsed())
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, Art.BuildShipEffect, mTranslationMatrix);
                Art.BuildShipEffect.Parameters["BuildPercent"].SetValue(k.GetElapsedTime() / k.GetTotalDuration());

                var j = k.GetType();
                if (j == typeof(ETransport))
                {
                    spriteBatch.Draw(Art.Transport, k.GetDestination() - new Vector2(Art.Transport.Width * 0.5f, Art.Transport.Height * 0.5f), Color.White);
                }
                else if (j == typeof(ESpy))
                {
                    spriteBatch.Draw(Art.Spy, k.GetDestination() - new Vector2(Art.Spy.Width * 0.5f, Art.Spy.Height * 0.5f), Color.White);
                }
                else if (j == typeof(EMedic))
                {
                    spriteBatch.Draw(Art.Medic, k.GetDestination() - new Vector2(Art.Spy.Width * 0.5f, Art.Spy.Height * 0.5f), Color.White);
                }
                else if (j == typeof(EMoerser))
                {
                    spriteBatch.Draw(Art.Moerser, k.GetDestination() - new Vector2(Art.Spy.Width * 0.5f, Art.Spy.Height * 0.5f), Color.White);
                }
                else
                {
                    spriteBatch.Draw(Art.Bomber, k.GetDestination() - new Vector2(Art.Bomber.Width * 0.5f, Art.Bomber.Height * 0.5f), Color.White);
                }
                spriteBatch.End();
                spriteBatch.Begin();
            }
        }
    }


    internal Rectangle GetCameraRectangle()
    {
        return mCameraPosition;
    }

    public void RemoveForTechDemo()
    {
        foreach (var e in Entities)
        {
            mEntitiesToRemove.Add(e);
        }
    }

    internal List<Entity> EntitiesInRadius(int radius, Entity entity)
    {
        var inRange = new List<Entity>();
        foreach (var k in Entities)
        {
            if (entity == k || k.GetComponent<CTeam>()?.mTeam != Team.Player || k is not EShip)
            {
                continue;
            }
            if (Vector2.Distance(entity.GetPos(), k.GetPos()) <= radius)
            {
                inRange.Add(k);
            }
        }
        return inRange;
    }

    internal void SetCameraRectangle(Rectangle camRect)
    {
        mCameraPosition = camRect;
    }

    internal void SetTranslationMatrix(Matrix matrix)
    {
        mTranslationMatrix = matrix;
    }
}
