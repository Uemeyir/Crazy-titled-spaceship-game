using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using sopra05_2223.Core.Entity.Resource;
using sopra05_2223.Core.Entity.Ships;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace sopra05_2223.Core.Components;

internal sealed class CollectTransporter : Component
{
    private readonly int mActionRadius;

    private List<ECollector> mCollectors;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    private Vector2 mTarget;

    private List<Entity.Entity> mAlreadyCollectingResources = new();

    internal CollectTransporter()
    {
        mActionRadius = Globals.sActionRadius["Transport"];
    }

    [JsonConstructor]
    internal CollectTransporter(Vector2 target)
    {
        mTarget = target;
    }

    internal void SetTarget(Vector2 t)
    {
        mTarget = t;
    }

    internal void ResetAlreadyCollectingResources()
    {
        mAlreadyCollectingResources = new List<Entity.Entity>();

        if (mEntity is ETransport et)
        {
            foreach (var c in et.GetCollectors())
            {
                c.GetComponent<CollectCollector>()?.RemoveTarget();
            }
        }
    }

    internal Vector2 GetTarget()
    {
        return mTarget;
    }

    internal void SetCollectors(List<ECollector> cs)
    {
        mCollectors = cs;
    }

    private bool IsCollectible(Entity.Entity resource)
    {
        var posResource = new Vector2(resource.GetX() + resource.GetWidth() / 2, resource.GetY() + resource.GetHeight() / 2);
        var posTransporter = new Vector2(mEntity.GetX() + mEntity.GetWidth() / 2,
            mEntity.GetY() + mEntity.GetHeight() / 2);
        return Vector2.Distance(posTransporter, posResource) <= mActionRadius;
    }


    private ECollector GetClosestToResource(Vector2 posRes)
    {
        ECollector closest = null;
        var dist = float.MaxValue;

        foreach (var eCollector in mCollectors)
        {
            if (eCollector.GetComponent<CollectCollector>()?.GetTarget() != null)
            {
                continue;
            }

            var posTransporter = new Vector2(eCollector.GetX() + eCollector.GetWidth() / 2,
                eCollector.GetY() + eCollector.GetHeight() / 2);

            if (Vector2.Distance(posTransporter, posRes) < dist)
            {
                closest = eCollector;
                dist = Vector2.Distance(posTransporter, posRes);
            }


        }
        return closest;
    }


    internal override void Update()
    {

        if (mEntity.GetComponent<CTransform>().GetDistanceToTarget().Length() > 40)
        {
            return;
        }

        var neutralResources = mEntity.mEntityManager.Entities
            .Where(r => r is EResource && r.GetComponent<CTeam>().IsNeutral()).ToList();

        foreach (var r in neutralResources)
        {
            if (mCollectors is null)
            {
                return;
            }

            if (mCollectors.Count == 0)
            {
                break;
            }

            if (IsCollectible(r) == false || mAlreadyCollectingResources.Contains(r))
            {
                continue;
            }

            if (Globals.mGridStandard == null)
            {
                continue;
            }

            var posResource = new Vector2(r.GetX() + r.GetWidth() / 2, r.GetY() + r.GetHeight() / 2);
            var c = GetClosestToResource(posResource);

            if (c is null)
            {
                return;
            }

            mAlreadyCollectingResources.Add(r);
            c.GetComponent<CollectCollector>()?.SetTargetResource(r);
            
            var toMove = new List<Entity.Entity>()
            {
                c
            };
            
            Globals.mMoveEntities.Move(toMove ,posResource.ToPoint(), false);
            
        }
    }
}