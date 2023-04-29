using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using sopra05_2223.Core;
using sopra05_2223.Core.Components;
using sopra05_2223.Core.Entity.Ships;
using sopra05_2223.Core.Entity;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace sopra05_2223.Pathfinding;

internal sealed class MoveEntities
{
    /*
     * Will be used for short distances, has smaller grid tiles
     * with smaller grid tiles => pathfinder runs longer.
     * But with smaller grid tiles, ships have a more precise path around
     * planets and can fly closer to them.
    */ 
    /*
     * Will be used when entities have to fly longer distances,
     * uses significantly larger grid tiles and will allow us to receive
     * a path with less points (compared to mGridStandard) which also
     * means that this path is faster available.
     * (it could be important that obstacles like planets are placed
     * within coordinates relating to ONE grid tile)
     */
    /*
     * Grid with large tiles for tech demo (could be redundant later, could be
     * merged it mGridExtended)
     */
    //private Grid mTechDemoGrid;

    /*
     * represents the size used for GameScreen
     */
    private readonly Point mWorldSize;


    private int mXMid;

    private int mYMid;

    private Vector2 mTarget;

    /*
     * will receive gridS (smaller tiles) and gridE (larger tiles)
     * obstacles are already inserted into the grid
     */
    public MoveEntities(Point gameScreenWorldSize)
    {
        mWorldSize = gameScreenWorldSize;
    }

    /*
     * We want to set a "transitory target" so that we don't have to move all selected
     * entities in one move and add them to a "waiting list" toMove which is declared
     * in the EntityManager
     */
    internal bool SetTargetOfGroup(EntityManager entityManager, Point posTarget)
    {
        // Added check for CTransform on first Entity here,
        // cause we might call this method with a single static Entity in our selected Entities that we can't to move.
        if (entityManager.mSelectedEntities.Count == 0 
            || entityManager.mSelectedEntities[0].GetComponent<CTransform>() == null)
        {
            return false;
        }

        // if less than two entities are selected, move them immediately
        if (entityManager.mSelectedEntities.Count < 2)
        {
            Move(entityManager.mSelectedEntities, posTarget, false);
            return true;
        }

        mXMid = 0;
        mYMid = 0;

        CalculateMidAndSetSpeed(entityManager.mSelectedEntities, false);

        mXMid /= entityManager.mSelectedEntities.Count;
        mYMid /= entityManager.mSelectedEntities.Count;

        // Sort transport ships to front of selectedEntities, so the transport ships
        // will already contain the new target which collector ships can later use
        // to set the transporter adjusted target for themselves
        entityManager.mSelectedEntities = SortTransporterToFront(entityManager.mSelectedEntities);

        // entities which would relative to the mouse click have a target in an obstacle
        // will be deselected
        //var toRemove = new List<Entity>();

        foreach (var entity in entityManager.mSelectedEntities.
                     Where(entity => entity?.GetComponent<CTransform>() != null && entity is not ECollector))
        {

            var distToClick = Vector2.Distance(
                new Vector2(entity.GetX(), entity.GetY()), posTarget.ToVector2()
                );
            
            entity.GetComponent<CTransform>().SetDistanceToClick(distToClick);
            
            mTarget = (posTarget
                       + new Point(entity.GetX() - mXMid, entity.GetY() - mYMid)
                       ).ToVector2();

            // don't now if really necessary, but seems like you can click on larger coordinates
            // than the size of the map should allow, to reduce possibility of index error
            // if statement has been added
            if (posTarget.X > mWorldSize.X || posTarget.X < 0 
                                                   || posTarget.Y > mWorldSize.Y 
                                                   || posTarget.Y < 0)
            {
                continue;
            }

            if (entity is ETransport)
            {
                if (mTarget.X < 700)
                {
                    mTarget.X = 700;
                }

                if (mTarget.X > mWorldSize.X - 800)
                {
                    mTarget.X = mWorldSize.X - 800;
                }

                if (mTarget.Y < 700)
                {
                    mTarget.Y = 700;
                }

                if (mTarget.Y > mWorldSize.Y - 800)
                {
                    mTarget.Y = mWorldSize.Y - 800;
                }
            }
            /*
            if (Globals.mGridStandard.TestIfMouseClickInObstacle(posTarget.X, posTarget.Y))
            {
                continue;
            }
            */

            switch (entity)
            {
                case ETransport:
                    entity.GetComponent<CollectTransporter>().ResetAlreadyCollectingResources();
                    entity.GetComponent<CollectTransporter>().SetTarget(mTarget);
                    break;
                case ESpy when entity.GetComponent<CBuoyPlacer>().mBuoyCount > 0 &&
                               entity.GetComponent<CBuoyPlacer>().mShouldPlaceBuoy:
                    entity.GetComponent<CBuoyPlacer>().mNextBuoyPosition = new Point(posTarget.X, posTarget.Y);
                    break;
                // set target of collector in relation to target of transport ship
                case ECollector when entityManager.mSelectedEntities.Contains(entity
                    .GetComponent<CollectCollector>()?.GetTransporter()) && entity
                    .GetComponent<CollectCollector>()?.GetTarget() == null:
                    SetTargetCollector(entity);
                    break;
            }

            entity.GetComponent<CTransform>().SetTransitoryTarget(mTarget.ToPoint());
                
            // to reduce as many collisions as possible, we want to move the entities
            // which are closest to the target first

            entityManager.mToMovePlayerPlayer.Remove(entity);

            var indexToInsert = 0;
            for (var i = 0; i < entityManager.mToMovePlayerPlayer.Count; i++) 
            {
                if (entity.GetComponent<CTransform>().GetDistanceToClick() >
                    entityManager.mToMovePlayerPlayer[i].GetComponent<CTransform>().GetDistanceToClick())
                {
                    if (i == entityManager.mToMovePlayerPlayer.Count - 1)
                    {
                        indexToInsert = entityManager.mToMovePlayerPlayer.Count;
                    }
                    continue;
                }
                indexToInsert = i;
            }
            entityManager.mToMovePlayerPlayer.Insert(indexToInsert, entity);
        }
        return true;
    }

    internal void SetTargetForKi(EntityManager entityManager, Entity entity, Point posTarget)
    {

        if (entityManager.mToMovePlayerKi.Count > 15 && Globals.mGridTechDemo is not null)
        {
            return;
        }
        if (entityManager.mToMovePlayerKi.Count > 40)
        {
            return;
        }

        var distToClick = Vector2.Distance(
                new Vector2(entity.GetX(), entity.GetY()), posTarget.ToVector2()
                );

        entity.GetComponent<CTransform>().SetDistanceToClick(distToClick);

        mTarget = posTarget.ToVector2();

        // don't now if really necessary, but seems like you can click on larger coordinates
        // than the size of the map should allow, to reduce possibility of index error
        // if statement has been added
        if (posTarget.X > mWorldSize.X || posTarget.X < 0
                                               || posTarget.Y > mWorldSize.Y
                                               || posTarget.Y < 0)
        {
            return;
        }

        /*
        if (Globals.mGridStandard.TestIfMouseClickInObstacle(posTarget.X, posTarget.Y))
        {
            return;
        }
        */

        else if (entity is ESpy)
        {
            var x = entity.GetComponent<CBuoyPlacer>();
            if (x.mBuoyCount > 0 && x.mShouldPlaceBuoy)
            {
                x.mNextBuoyPosition = new Point(posTarget.X, posTarget.Y);
            }

        }

        // set target of collector in relation to target of transport ship
        if (entity is ECollector)
        {
            if (entityManager.mSelectedEntities.Contains(entity
                    .GetComponent<CollectCollector>()?.GetTransporter()) && entity
                    .GetComponent<CollectCollector>()?.GetTarget() == null)
            {
                SetTargetCollector(entity);
            }
        }

        else
        {
            entity.GetComponent<CTransform>().SetTransitoryTarget(mTarget.ToPoint());

            // to reduce as many collisions as possible, we want to move the entities
            // which are closest to the target first

            entityManager.mToMovePlayerKi.Remove(entity);

            var indexToInsert = 0;
            for (var i = 0; i < entityManager.mToMovePlayerKi.Count; i++)
            {
                if (entity.GetComponent<CTransform>().GetDistanceToClick() >
                    entityManager.mToMovePlayerKi[i].GetComponent<CTransform>().GetDistanceToClick())
                {
                    if (i == entityManager.mToMovePlayerKi.Count - 1)
                    {
                        indexToInsert = entityManager.mToMovePlayerKi.Count;
                    }
                    continue;
                }
                indexToInsert = i;
            }
            entityManager.mToMovePlayerKi.Insert(indexToInsert, entity);
        }
    }


    /*
     * receives list of entities which have to be moved and 
     * the position of destination (map coords.
     * Returns Success of the Route Calculation.
     */
    internal void Move(List<Entity> selectedEntities, Point posTarget, bool byEm)
    {
        /*
         * Move function will reach out to Global to access Grids
         * removed all class internal usage of grids which where initialized
         * with MoveSelection(), because there was an Issue with GridExtended and
         * GridTechDemo
         */
        if (Globals.mGridExtended == null || Globals.mGridStandard == null)
        {
            return;
        }
        
        mXMid = 0;
        mYMid = 0;

        if (!selectedEntities.Any())
        {
            return;
        }


        CalculateMidAndSetSpeed(selectedEntities, byEm);

        mXMid /= selectedEntities.Count;
        mYMid /= selectedEntities.Count;

        // Sort transport ships to front of selectedEntities, so the transport ships
        // will already contain the new target which collector ships can later use
        // to set the transporter adjusted target for themselves
        selectedEntities = SortTransporterToFront(selectedEntities);


        foreach (var entity in selectedEntities.Where(entity => entity?.GetComponent<CTransform>() != null))
        {

            mTarget = (posTarget
                          + new Point(entity.GetX() - mXMid, entity.GetY() - mYMid)).ToVector2();


            if (posTarget.X > mWorldSize.X || posTarget.X < 0 || posTarget.Y > mWorldSize.Y ||
                posTarget.Y < 0)
            {
                continue;
            }

            if (entity is ETransport)
            {
                if (mTarget.X < 700)
                {
                    mTarget.X = 700;
                }

                if (mTarget.X > mWorldSize.X - 800)
                {
                    mTarget.X = mWorldSize.X - 800;
                }

                if (mTarget.Y < 700)
                {
                    mTarget.Y = 700;
                }

                if (mTarget.Y > mWorldSize.Y - 800)
                {
                    mTarget.Y = mWorldSize.Y - 800;
                }
            }

            if (entity is ETransport)
            {
                entity.GetComponent<CollectTransporter>().ResetAlreadyCollectingResources();
                entity.GetComponent<CollectTransporter>().SetTarget(mTarget);
            }
            else if (entity is ESpy)
            {
                if (entity.GetComponent<CBuoyPlacer>().mBuoyCount > 0 &&
                    entity.GetComponent<CBuoyPlacer>().mShouldPlaceBuoy)
                {
                    entity.GetComponent<CBuoyPlacer>().mNextBuoyPosition = new Point(posTarget.X, posTarget.Y);
                    ESpy spy = (ESpy)entity;
                    spy.mStartPos = entity.GetPos();
                }
            }


            // set target of collector in relation to target of transport ship
            
            if (entity is ECollector)
            {
                if (selectedEntities.Contains(entity.GetComponent<CollectCollector>()?.GetTransporter()) && entity.GetComponent<CollectCollector>()?.GetTarget() == null)
                {
                    SetTargetCollector(entity);
                }
            }
            

            var distance = Vector2.Distance(new Vector2(entity.GetX(), entity.GetY()),
                new Vector2(mTarget.X, mTarget.Y));


            List<Point> pathPoints = new();

            if (Globals.mGridTechDemo == null || (Globals.mGridTechDemo != null && distance < 3000))
            {
                if (mTarget.X < 0)
                {
                    mTarget.X = 0;
                }

                if (posTarget.X / Globals.mGridStandard.GetGridTileDimension() > Globals.mGridStandard.GetGridWidth()
                    - (int)Math.Ceiling(entity.GetWidth() / (double)Globals.mGridStandard.GetGridTileDimension()))
                {
                    mTarget.X = mWorldSize.X - (entity.GetWidth() + 1);
                }

                if (mTarget.Y < 0)
                {
                    mTarget.Y = 0;
                }

                if (posTarget.Y / Globals.mGridStandard.GetGridTileDimension() > Globals.mGridStandard.GetGridHeight()
                    - (int)Math.Ceiling(entity.GetHeight() / (double)Globals.mGridStandard.GetGridTileDimension()))
                {
                    mTarget.Y = mWorldSize.Y - (entity.GetHeight() + 1);

                }

                if (Globals.mGridStandard.TestIfDirectPathIsAvailable(entity, mTarget) 
                    && Globals.mGridStandard.TestIfMouseClickInObstacle(entity.GetX(), entity.GetY()) == false
                    && Globals.mGridStandard.TestIfMouseClickInObstacle((int)(entity.GetX() + entity.GetWidth() / 2f), 
                        (int)(entity.GetY() + entity.GetHeight() /2f)) == false 
                    && Globals.mGridStandard.TestIfMouseClickInObstacle(entity.GetX() + entity.GetWidth(), 
                        entity.GetY() + entity.GetHeight()) == false) 
                {
                    // there is a direct path to target. take it
                    entity.GetComponent<CTransform>()?.SetPath(new List<Point>());

                    // set Target Relative to middle of pack
                    entity.GetComponent<CTransform>()?.SetTarget(mTarget.ToPoint());
                }
                else
                {
                    if (distance < 1500)
                    {
                        pathPoints = GetPathToGrid(Globals.mGridStandard, entity);
                    }
                    else
                    {
                        pathPoints = GetPathToGrid(Globals.mGridExtended, entity);
                    }
                }
            }
            else
            {
                if (mTarget.X < 0)
                {
                    mTarget.X = 0;
                }
                if (posTarget.X / Globals.mGridTechDemo.GetGridTileDimension() > Globals.mGridTechDemo.GetGridWidth()
                    - (int)Math.Ceiling(entity.GetWidth() / (double)Globals.mGridTechDemo.GetGridTileDimension()))
                {
                    mTarget.X = mWorldSize.X - (entity.GetWidth() + 1);
                }
                if (mTarget.Y < 0)
                {
                    mTarget.Y = 0;
                }
                if (posTarget.Y / Globals.mGridTechDemo.GetGridTileDimension() > Globals.mGridTechDemo.GetGridHeight()
                    - (int)Math.Ceiling(entity.GetHeight() / (double)Globals.mGridTechDemo.GetGridTileDimension()))
                {
                    mTarget.Y = mWorldSize.Y - (entity.GetHeight() + 1);
                }

                if (Globals.mGridStandard.TestIfDirectPathIsAvailable(entity, mTarget))
                {
                    // there is a direct path to target. take it
                    entity.GetComponent<CTransform>()?.SetPath(new List<Point>());

                    // set Target Relative to middle of pack
                    entity.GetComponent<CTransform>()?.SetTarget(mTarget.ToPoint());
                }
                else
                {
                    pathPoints = GetPathToGrid(Globals.mGridTechDemo, entity);
                }
            }

            if (pathPoints.Count >= 1){
                SetTargetAndPath(pathPoints, posTarget, entity);
            }
        }
    }

    
    private void CalculateMidAndSetSpeed(List<Entity> selectedEntities, bool byEm)
    {
        var minVelocity = float.MaxValue;

        // Calculate Center of Movement and minimum velocity
        foreach (var entity in selectedEntities.Where(entity => entity?.GetComponent<CTransform>() != null))
        {

            var maxSpeedEntity = entity.GetComponent<CTransform>().GetMaxVelocity();

            if ((maxSpeedEntity < minVelocity) && (maxSpeedEntity != 0))
            {
                minVelocity = maxSpeedEntity;
            }

            mXMid += entity.GetX() + entity.GetWidth() / 2;
            mYMid += entity.GetY() + entity.GetHeight() / 2;
        }

        if (byEm == false)
        {
            SetMaxSpeed(minVelocity, selectedEntities);
        }
    }

    
    private static void SetMaxSpeed(float maxSpeed, List<Entity> selectedEntities)
    {
        foreach (var entity in selectedEntities)
        {
            entity.GetComponent<CTransform>()?.SetSquadVelocity(maxSpeed);
        }
    }

    
    private void SetTargetCollector(Entity collector)
    {
        var targetTransport = collector.GetComponent<CollectCollector>().GetTransporter()
            .GetComponent<CollectTransporter>().GetTarget();


        mTarget = collector.GetComponent<CollectCollector>().GetTargetToMoveTo(targetTransport);
        collector.GetComponent<CollectCollector>().RemoveTarget();
    }

    
    private static List<Entity> SortTransporterToFront(List<Entity> selectedEntities)
    {
        var list = new List<Entity>();
        foreach (var e in selectedEntities)
        {
            if (e is ETransport)
            {
                list.Insert(0, e);
            }
            else
            {
                list.Add(e);
            }
        }

        return list;
    }


    private List<Point> GetPathToGrid(Grid grid, Entity entity)
    {
        List<Point> pathPoints = new List<Point>();
        var newPathSolver = new PathSolver(grid);
        // calculate path and send ship on its way
        // TODO: refactor to own function

        // Set target, get start and end coordinates for path solver
        var gridStartCoordinates =
            grid.GetGridCoordinatesToMapCoordinates(entity.GetX() + entity.GetWidth() / 2f, 
                entity.GetY() + entity.GetHeight() / 2f);

        if (gridStartCoordinates.X > grid.GetGridWidth() - 1)
        {
            gridStartCoordinates.X = grid.GetGridWidth() - 1;
        }
        if (gridStartCoordinates.Y > grid.GetGridHeight() - 1)
        {
            gridStartCoordinates.Y = grid.GetGridHeight() - 1;
        }

        var gridEndCoordinates =
            grid.GetGridCoordinatesToMapCoordinates(mTarget.X + entity.GetWidth() / 2f, mTarget.Y + entity.GetHeight() / 2f);

        if (gridEndCoordinates.X > grid.GetGridWidth() - 1)
        {
            gridEndCoordinates.X = grid.GetGridWidth() - 1;
        }
        if (gridEndCoordinates.Y > grid.GetGridHeight() - 1)
        {
            gridEndCoordinates.Y = grid.GetGridHeight() - 1;
        }

        var path = newPathSolver.GetPath((int)gridStartCoordinates.X,
            (int)gridStartCoordinates.Y,
            (int)gridEndCoordinates.X,
            (int)gridEndCoordinates.Y);
        grid.ResetGrid();

        if (path == null)
        {
            return pathPoints;
        }

        foreach (var p in path)
        {
            var coordinates = new Point(
                p.mPoint.X * grid.GetGridTileDimension() + grid.GetGridTileDimension() / 2 - entity.GetWidth() / 2,
                p.mPoint.Y * grid.GetGridTileDimension() + grid.GetGridTileDimension() / 2 - entity.GetHeight() / 2);
            pathPoints.Add(coordinates);
        }

        return pathPoints;
    }

    
    private void SetTargetAndPath(List<Point> pathPoints, Point posTarget , Entity entity)
    {
        bool setOldTarget = true;
        
        if (pathPoints.Count > 1)
        {
            if (pathPoints[0] == pathPoints[pathPoints.Count - 1])
            {
                setOldTarget = false;
            }
        }

        // set target around click pos and not around grid nodes
        if (pathPoints.Count != 0)
        {
            pathPoints.RemoveAt(pathPoints.Count - 1);
        }

        //pathPoints.Add(new Point((int)mTarget.X, (int)mTarget.Y));

        // Remove first point. why?
        // - if we don't remove the first point, the entity will fly to the middle of the coordinates
        //   relative to the grid tile in which itself is positioned, so it will fly "Zick Zack" at the beginning
        if (setOldTarget)
        {
            pathPoints.Add(new Point((int)mTarget.X, (int)mTarget.Y));
        }

        pathPoints.RemoveAt(0);

        //entity.mPath = pathPoints;
        if (pathPoints.Count > 0)
        {
            entity.GetComponent<CTransform>()?.SetPath(pathPoints);
        }
        else
        {
            var p1 = posTarget.ToVector2();
            var p2 = mTarget;
            if (Vector2.Distance(p1, p2) < Globals.mGridStandard.GetGridTileDimension())
            {
                // no path nodes, fly direct
                entity.GetComponent<CTransform>()?.SetPath(new List<Point>());
                entity.GetComponent<CTransform>()?.SetTarget(mTarget.ToPoint());
            }
        }
    }
}
