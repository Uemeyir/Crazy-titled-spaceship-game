using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using sopra05_2223.Core.Entity.Ships;

namespace sopra05_2223.Core.Components;

internal sealed class CollectCollector : Component
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    private Entity.Entity mTargetResource;

    internal ETransport mOwnerTransport;

    [JsonRequired]
    private readonly int mNum;

    internal CollectCollector(ETransport owner, int num)
    {
        mOwnerTransport = owner;
        mNum = num;
    }

    [Newtonsoft.Json.JsonConstructor]
    internal CollectCollector(int num)
    {
        mNum = num;
    }

    internal void SetTargetResource(Entity.Entity r)
    {
        mTargetResource = r;
    }

    private bool CheckIfReachedResource()
    {
        return mEntity.IsColliding(mTargetResource);
    }

    internal void RemoveTarget()
    {
        mTargetResource = null;
    }

    internal Entity.Entity GetTarget()
    {
        return mTargetResource;
    }

    internal Entity.Entity GetTransporter()
    {
        return mOwnerTransport;
    }

    private void StoreInOwner()
    {
        var compResource = mTargetResource.GetComponent<CResource>();
        var compStorageOwner = mOwnerTransport.GetComponent<CStorage>();
        var oxygenStorageLeft = compStorageOwner.GetMaxOxygen() - compStorageOwner.GetOxygen();
        var metalStorageLeft = compStorageOwner.GetMaxMetal() - compStorageOwner.GetMetal();

        if (compResource.GetOxygen() <= oxygenStorageLeft)
        {
            Globals.mStatistics["ScrapResources"] += (int)compResource.GetOxygen();
            Globals.mRunStatistics["ScrapResources"] += (int)compResource.GetOxygen();
            compStorageOwner.AddToStorage(true, (int)compResource.Deplete((int)compResource.GetOxygen(), true));
        }
        else
        {
            Globals.mStatistics["ScrapResources"] += oxygenStorageLeft;
            Globals.mRunStatistics["ScrapResources"] += oxygenStorageLeft;
            compStorageOwner.AddToStorage(true, (int)compResource.Deplete(oxygenStorageLeft, true));
        }


        if (compResource.GetMetal() <= metalStorageLeft)
        {
            Globals.mStatistics["ScrapResources"] += (int)compResource.GetMetal();
            Globals.mRunStatistics["ScrapResources"] += (int)compResource.GetMetal();
            compStorageOwner.AddToStorage(false, (int)compResource.Deplete((int)compResource.GetMetal(), false));
        }
        else
        {
            Globals.mStatistics["ScrapResources"] += (int)compResource.GetMetal();
            Globals.mRunStatistics["ScrapResources"] += (int)compResource.GetMetal();
            compStorageOwner.AddToStorage(false, (int)compResource.Deplete(metalStorageLeft, false));
        }

        if (compResource.GetOxygen() == 0 && compResource.GetMetal() == 0)
        {
            mEntity.mEntityManager.Remove(mTargetResource);
        }

        mTargetResource = null;
    }

    internal void MoveToOwner(bool newBuild)
    {
        mEntity.GetComponent<CTransform>().SetPath(new List<Point>());

        var target = new Point(0, 0);

        switch (mNum)
        {

            case 0:
                target = new Vector2((mOwnerTransport.GetX() + mOwnerTransport.GetWidth() / 2 - (mOwnerTransport.GetWidth() / 2 + 150)),
                    (mOwnerTransport.GetY() + mOwnerTransport.GetHeight() / 2 - (mOwnerTransport.GetHeight() / 2 + 40))).ToPoint();
                break;

            case 1:
                target = new Vector2((mOwnerTransport.GetX() + mOwnerTransport.GetWidth() / 2 + (mOwnerTransport.GetWidth() / 2 + 150)),
                    (mOwnerTransport.GetY() + mOwnerTransport.GetHeight() / 2 - (mOwnerTransport.GetHeight() / 2 + 40))).ToPoint();
                break;

            case 2:
                target = new Vector2((mOwnerTransport.GetX() + mOwnerTransport.GetWidth() / 2 - (mOwnerTransport.GetWidth() / 2 + 150)),
                    (mOwnerTransport.GetY() + mOwnerTransport.GetHeight() / 2 + (mOwnerTransport.GetHeight() / 2 + 40))).ToPoint();
                break;

            case 3:
                target = new Vector2((mOwnerTransport.GetX() + mOwnerTransport.GetWidth() / 2 + (mOwnerTransport.GetWidth() / 2 + 150)),
                    (mOwnerTransport.GetY() + mOwnerTransport.GetHeight() / 2 + (mOwnerTransport.GetHeight() / 2 + 40))).ToPoint();
                break;

            case 4:
                target = new Vector2((mOwnerTransport.GetX() + mOwnerTransport.GetWidth() / 2 - (mOwnerTransport.GetWidth() / 2 + 190)),
                   (mOwnerTransport.GetY() + mOwnerTransport.GetHeight() / 2)).ToPoint();
                break;

            case 5:
                target = new Vector2((mOwnerTransport.GetX() + mOwnerTransport.GetWidth() / 2 + (mOwnerTransport.GetWidth() / 2) + 190),
                   (mOwnerTransport.GetY() + mOwnerTransport.GetHeight() / 2)).ToPoint();
                break;
        }

        var toMove = new List<Entity.Entity>
        {
            mEntity
        };

        if (newBuild)
        {
            mEntity.GetComponent<CTransform>().SetTarget(target);
        }
        else
        {
            Globals.mMoveEntities.Move(toMove, target, false);
        }
    }

    internal Vector2 GetTargetToMoveTo(Vector2 posOwner)
    {
        switch (mNum)
        {

            case 0:
                return new Vector2(
                    (int)(posOwner.X + mOwnerTransport.GetWidth() * 0.5f - (mOwnerTransport.GetWidth() / 2 + 300)),
                    (int)(posOwner.Y + mOwnerTransport.GetHeight() * 0.5f - (mOwnerTransport.GetHeight() / 2 + 80)));

            case 1:
                return new Vector2(
                    (int)(posOwner.X + mOwnerTransport.GetWidth() * 0.5f + (mOwnerTransport.GetWidth() / 2 + 300)),
                    (int)(posOwner.Y + mOwnerTransport.GetHeight() * 0.5f - (mOwnerTransport.GetHeight() / 2 + 80)));

            case 2:
                return new Vector2(
                    (int)(posOwner.X + mOwnerTransport.GetWidth() * 0.5f - (mOwnerTransport.GetWidth() / 2 + 300)),
                    (int)(posOwner.Y + mOwnerTransport.GetHeight() * 0.5f + (mOwnerTransport.GetHeight() / 2 + 80)));

            case 3:
                return new Vector2(
                    (int)(posOwner.X + mOwnerTransport.GetWidth() * 0.5f + (mOwnerTransport.GetWidth() / 2 + 300)),
                    (int)(posOwner.Y + mOwnerTransport.GetHeight() * 0.5f + (mOwnerTransport.GetHeight() / 2 + 80)));


            case 4:
                return new Vector2(
                    (int)(posOwner.X + mOwnerTransport.GetWidth() * 0.5f - (mOwnerTransport.GetWidth() / 2 + 380)),
                    (int)(posOwner.Y + mOwnerTransport.GetHeight() * 0.5f));

            case 5:
                return new Vector2((int)(posOwner.X + mOwnerTransport.GetWidth() * 0.5f + (mOwnerTransport.GetWidth() * 0.5f) + 380),
                    (int)(posOwner.Y + mOwnerTransport.GetHeight() * 0.5f));
        }

        return new Vector2(100, 100);
    }

    internal override void Update()
    {
        if (mTargetResource == null)
        {
            return;
        }

        if (mEntity.mEntityManager.Entities.Contains(mTargetResource) == false)
        {
            mTargetResource = null;
        }

        if (CheckIfReachedResource())
        {
            StoreInOwner();
        }

        if (mTargetResource == null)
        {
            MoveToOwner(false);
        }

    }
}
