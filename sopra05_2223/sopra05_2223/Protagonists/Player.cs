using Microsoft.Xna.Framework;
using sopra05_2223.Core.Entity;
using sopra05_2223.Core.Entity.Base.PlanetBase;
using sopra05_2223.Core.Entity.Planet;
using sopra05_2223.Core.Entity.Ships;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using sopra05_2223.Core;
using sopra05_2223.Core.Components;
using sopra05_2223.Core.Entity.Base.SpaceBase;


namespace sopra05_2223.Protagonists;

internal class Player
{
    internal readonly List<Entity> mEntities = new();
    private readonly List<Entity> mPlanets = new();
    internal readonly List<Entity> mPlanetBases = new();
    internal readonly List<Entity> mSpaceBases = new();
    internal readonly List<Entity> mShips = new();
    [JsonRequired]
    internal int OxygenReserve { get; private protected set; }
    [JsonRequired]
    internal int IronReserve { get; private protected set; }
    protected Player mOpponent;
    internal EntityManager mEm;

    public virtual void Update()
    {
    }

    public virtual void AddOxygen(int toIncrease)
    {
        OxygenReserve += toIncrease;
    }

    public virtual void AddIron(int toIncrease)
    {
        IronReserve += toIncrease;
    }

    public void SetOpponent(Player opponent)
    {
        mOpponent = opponent;
    }

    public Player GetOpponent()
    {
        return mOpponent;
    }

    internal void SetEntityManager(EntityManager em)
    {
        mEm = em;
    }

    public virtual void AddEntity(Entity e)
    {
        mEntities.Add(e);
        switch (e)
        {
            case EPlanet:
                mPlanets.Add(e);
                break;

            case EShip:
                mShips.Add(e);
                break;

            case EPlanetBase:
                mPlanetBases.Add(e);
                break;

            case ESpaceBase:
                mSpaceBases.Add(e);
                break;
        }
    }

    public virtual void RemoveEntity(Entity e)
    {
        mEntities.Remove(e);
        switch (e)
        {
            case EPlanet:
                mPlanets.Remove(e);
                break;

            case EShip:
                mShips.Remove(e);
                break;

            case EPlanetBase:
                mPlanetBases.Remove(e);
                break;

            case ESpaceBase:
                mSpaceBases.Remove(e);
                break;
        }
    }

    private Entity GetClosestPlanet(Entity other)
    {
        var distClosest = float.MaxValue;
        Entity toReturn = null;
        var vec2 = new Vector2(other.GetX() + other.GetWidth() / 2f,
            other.GetY() + other.GetHeight() / 2f);

        foreach (var planet in mPlanets)
        {
            var vec1 = new Vector2(planet.GetX() + planet.GetWidth() / 2f,
                planet.GetY() + planet.GetHeight() / 2f);
            if (!(Vector2.Distance(vec1, vec2) <= distClosest))
            {
                continue;
            }

            distClosest = Vector2.Distance(vec1, vec2);
            toReturn = planet;
        }

        return toReturn;
    }

    public Entity GetClosestPlanetBase(Entity other)
    {
        var distClosest = float.MaxValue;
        Entity toReturn = null;
        var vec2 = new Vector2(other.GetX() + other.GetWidth() / 2f,
            other.GetY() + other.GetHeight() / 2f);

        foreach (var planetBase in mPlanetBases)
        {
            var vec1 = new Vector2(planetBase.GetX() + planetBase.GetWidth() / 2f,
                planetBase.GetY() + planetBase.GetHeight() / 2f);
            if (!(Vector2.Distance(vec1, vec2) <= distClosest))
            {
                continue;
            }

            distClosest = Vector2.Distance(vec1, vec2);
            toReturn = planetBase;
        }

        return toReturn;
    }

    public Entity GetClosestPlanetBaseForGun(Entity other)
    {
        var distClosest = float.MaxValue;
        Entity toReturn = null;
        var vec2 = new Vector2(other.GetX() + other.GetWidth() / 2f,
            other.GetY() + other.GetHeight() / 2f);

        foreach (var planetBase in mPlanetBases.Where(
                     x => x.GetComponent<CTeam>()?.GetTeam() is not Team.Neutral
                     && x.GetComponent<CTeam>()?.GetProtagonist() != other.GetComponent<CTeam>()?.GetProtagonist()))
        {
            var vec1 = new Vector2(planetBase.GetX() + planetBase.GetWidth() / 2f,
                planetBase.GetY() + planetBase.GetHeight() / 2f);
            if (!(Vector2.Distance(vec1, vec2) <= distClosest))
            {
                continue;
            }

            distClosest = Vector2.Distance(vec1, vec2);
            toReturn = planetBase;
        }

        return toReturn;
    }


    internal Entity GetClosestSpaceBaseForAi(Entity other)
    {
        var distClosest = float.MaxValue;
        Entity toReturn = null;
        var vec2 = new Vector2(other.GetX() + other.GetWidth() / 2f,
            other.GetY() + other.GetHeight() / 2f);

        foreach (var spaceBase in mSpaceBases)
        {
            if ((spaceBase.GetX() == other.GetX() && spaceBase.GetY() == other.GetY()))
            {
                continue;
            }
            var vec1 = new Vector2(spaceBase.GetX() + spaceBase.GetWidth() / 2f,
                spaceBase.GetY() + spaceBase.GetHeight() / 2f);
            if (!(Vector2.Distance(vec1, vec2) <= distClosest))
            {
                continue;
            }

            distClosest = Vector2.Distance(vec1, vec2);
            toReturn = spaceBase;
        }

        return toReturn;
    }



    public Entity GetClosestSpaceBase(Entity other)
    {
        var distClosest = float.MaxValue;
        Entity toReturn = null;
        var vec2 = new Vector2(other.GetX() + other.GetWidth() / 2f,
            other.GetY() + other.GetHeight() / 2f);

        foreach (var spaceBase in mSpaceBases)
        {
            var vec1 = new Vector2(spaceBase.GetX() + spaceBase.GetWidth() / 2f,
                spaceBase.GetY() + spaceBase.GetHeight() / 2f);
            if (!(Vector2.Distance(vec1, vec2) <= distClosest))
            {
                continue;
            }

            distClosest = Vector2.Distance(vec1, vec2);
            toReturn = spaceBase;
        }

        return toReturn;
    }

    public Entity GetClosestSpaceBaseForGun(Entity other)
    {
        var distClosest = float.MaxValue;
        Entity toReturn = null;
        var vec2 = new Vector2(other.GetX() + other.GetWidth() / 2f,
            other.GetY() + other.GetHeight() / 2f);

        foreach (var spaceBase in mSpaceBases.Where(x => x.GetComponent<CTeam>()?.GetTeam() is not Team.Neutral
                 && x.GetComponent<CTeam>()?.GetProtagonist() != other.GetComponent<CTeam>()?.GetProtagonist()))
        {
            var vec1 = new Vector2(spaceBase.GetX() + spaceBase.GetWidth() / 2f,
                spaceBase.GetY() + spaceBase.GetHeight() / 2f);
            if (!(Vector2.Distance(vec1, vec2) <= distClosest))
            {
                continue;
            }

            distClosest = Vector2.Distance(vec1, vec2);
            toReturn = spaceBase;
        }

        return toReturn;
    }

    public Entity GetClosestShip(Entity other)
    {
        var distClosest = float.MaxValue;
        Entity toReturn = null;
        var vec2 = new Vector2(other.GetX() + other.GetWidth() / 2f,
            other.GetY() + other.GetHeight() / 2f);

        foreach (var ship in mShips.Where(s => s is not ECollector))
        {
            var vec1 = new Vector2(ship.GetX() + ship.GetWidth() / 2f,
                ship.GetY() + ship.GetHeight() / 2f);
            if (!(Vector2.Distance(vec1, vec2) <= distClosest))
            {
                continue;
            }

            distClosest = Vector2.Distance(vec1, vec2);
            toReturn = ship;
        }

        return toReturn;
    }

    public Entity GetClosestEntityForMedic(Entity other)
    {
        var minHealth = int.MaxValue;
        Entity toReturn = null;
        var i = 0;

        /*
         * medic still only selects the entity based on least amount of health,
         * thus ships and bases are treated the equally, could make sense later on
         * to prefer the bases if there current health < 10 or something
         */

        while (i < mSpaceBases.Count || i < mPlanetBases.Count || i < mShips.Count)
        {
            if (i < mSpaceBases.Count)
            {
                if (mSpaceBases[i].GetComponent<CHealth>()?.GetHealth() <= minHealth 
                    && mSpaceBases[i].GetComponent<CHealth>()?.GetHealth() < 100
                    && mSpaceBases[i].GetComponent<CHealth>() != null)
                {
                    if ((int)Vector2.Distance(new Vector2(mSpaceBases[i].GetX() + mSpaceBases[i].GetWidth() / 2,
                                mSpaceBases[i].GetY() + mSpaceBases[i].GetHeight() / 2),
                            new Vector2(other.GetX() + other.GetWidth() / 2,
                                other.GetY() + other.GetHeight() / 2)) < Globals.sActionRadius["Medic"])
                    {
                        minHealth = mSpaceBases[i].GetComponent<CHealth>().GetHealth();
                        toReturn = mSpaceBases[i];
                    }
                }
            }
            if (i < mPlanetBases.Count)
            {
                if (mPlanetBases[i].GetComponent<CHealth>()?.GetHealth() <= minHealth
                    && mPlanetBases[i].GetComponent<CHealth>()?.GetHealth() < 100 
                    && mPlanetBases[i].GetComponent<CHealth>() != null)
                {
                    if ((int)Vector2.Distance(new Vector2(mPlanetBases[i].GetX() + mPlanetBases[i].GetWidth() / 2,
                                mPlanetBases[i].GetY() + mPlanetBases[i].GetHeight() / 2),
                            new Vector2(other.GetX() + other.GetWidth() / 2,
                                other.GetY() + other.GetHeight() / 2)) < Globals.sActionRadius["Medic"])
                    {
                        minHealth = mPlanetBases[i].GetComponent<CHealth>().GetHealth();
                        toReturn = mPlanetBases[i];
                    }
                }
            }
            if (i < mShips.Count)
            {
                if (mShips[i].GetComponent<CHealth>()?.GetHealth() <= minHealth 
                    && mShips[i].GetComponent<CHealth>()?.GetHealth() < 100 
                    && mShips[i] is not ECollector && mShips[i] != other 
                    && mShips[i].GetComponent<CHealth>() != null)
                {
                    if ((int)Vector2.Distance(new Vector2(mShips[i].GetX() + mShips[i].GetWidth() / 2,
                                mShips[i].GetY() + mShips[i].GetHeight() / 2),
                            new Vector2(other.GetX() + other.GetWidth() / 2,
                                other.GetY() + other.GetHeight() / 2)) < Globals.sActionRadius["Medic"])
                    {
                        minHealth = mShips[i].GetComponent<CHealth>().GetHealth();
                        toReturn = mShips[i];
                    }
                }
            }
            i++;
        }

        return toReturn;
    }

    public Entity GetClosestEntity(Entity e)
    {
        var vecE = new Vector2(e.GetX(), e.GetY());
        var ship = GetClosestShip(e);
        var planet = GetClosestPlanet(e);
        var planetBase = GetClosestPlanetBase(e);
        var spaceBase = GetClosestSpaceBase(e);
        var closestEntity = ship;
        var minDist = float.MaxValue;

        if (ship != null)
        {
            minDist = Vector2.Distance(new Vector2(ship.GetX(), ship.GetY()), vecE);
        }

        if (planet != null && minDist > Vector2.Distance(new Vector2(planet.GetX(), planet.GetY()), vecE))
        {
            minDist = Vector2.Distance(new Vector2(planet.GetX(), planet.GetY()), vecE);
            closestEntity = planet;
        }

        if (spaceBase != null && minDist > Vector2.Distance(new Vector2(spaceBase.GetX(), spaceBase.GetY()), vecE))
        {
            minDist = Vector2.Distance(new Vector2(spaceBase.GetX(), spaceBase.GetY()), vecE);
            closestEntity = spaceBase;
        }

        if (planetBase != null && minDist > Vector2.Distance(new Vector2(planetBase.GetX(), planetBase.GetY()), vecE))
        {
            closestEntity = planetBase;
        }

        return closestEntity;

    }
}
