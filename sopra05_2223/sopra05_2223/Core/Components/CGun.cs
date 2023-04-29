using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using sopra05_2223.Core.Entity.Projectile;
using sopra05_2223.Core.Entity.Ships;
using sopra05_2223.Protagonists;
using sopra05_2223.SoundManagement;

namespace sopra05_2223.Core.Components;

internal sealed class CGun : Component, IAudible
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    private Entity.Entity mTarget;

    private bool mPlayShot;
    private double mShotTimer;
    [JsonRequired]
    private readonly double mFireRate;

    private double FireDelay => 1f / mFireRate;

    private string mShipType;
    private bool mFixedTarget;

   [JsonConstructor]
    internal CGun(double fireRate)
    {
        mFireRate = fireRate;
    }

    internal override void Update()
    {
        mShotTimer += Globals.GameTime.ElapsedGameTime.TotalSeconds;

        // this is temporary code to demonstrate how the creation of a projectile works.
        mShipType = mEntity switch
        {
            EAttacker => "Attacker",
            EMoerser => "Moerser",
            EMedic => "Medic",
            _ => mShipType
        };

        if (mShotTimer >= FireDelay)
        {
            mShotTimer -= FireDelay;

            var player = mEntity.GetComponent<CTeam>().GetProtagonist();
            var opponent = player.GetOpponent();

            if (!mFixedTarget)
            {
            }

            var actionRad = Globals.sActionRadius[mShipType];

            // Fire if Fixed Target within ActionRadius.
            if (mTarget != null
                && (int)Vector2.Distance(mTarget.GetPos(),
                    mEntity.GetPos()) <= actionRad)
            {
                FireGun(mTarget.GetPos());
            }
            else // target other Entities in Range
            {
                var closestEntity = mShipType == "Medic" ? player.GetClosestEntityForMedic(mEntity) : GetClosestTargetEntity(opponent);
                if (closestEntity != null && Vector2.Distance(closestEntity.GetPos(), mEntity.GetPos()) <= actionRad)
                {
                    FireGun(closestEntity.GetPos());
                }
            }
        }
    }

    private Entity.Entity GetClosestTargetEntity(Player opponent)
    {
        var ship = opponent.GetClosestShip(mEntity);
        var planetBase = opponent.GetClosestPlanetBaseForGun(mEntity);
        var spaceBase = opponent.GetClosestSpaceBaseForGun(mEntity);
        var closestEntity = ship;
        var distClosestEntity = float.MaxValue;

        if (ship != null)
        {
            var distShip = Vector2.Distance(new Vector2(ship.GetX() + ship.GetWidth() / 2, ship.GetY() + ship.GetHeight() / 2),
                new Vector2(mEntity.GetX() + mEntity.GetWidth() / 2, mEntity.GetY() + mEntity.GetHeight() / 2));
            distClosestEntity = distShip;
        }

        if (planetBase != null)
        {
            var distPlanetBase = Vector2.Distance(new Vector2(planetBase.GetX() + planetBase.GetWidth() / 2, planetBase.GetY() + planetBase.GetHeight() / 2),
                new Vector2(mEntity.GetX() + mEntity.GetWidth() / 2, mEntity.GetY() + mEntity.GetHeight() / 2));
            if (distPlanetBase < distClosestEntity)
            {
                closestEntity = planetBase;
                distClosestEntity = distPlanetBase;
            }
        }

        if (spaceBase != null)
        {
            var distSpaceBase = Vector2.Distance(new Vector2(spaceBase.GetX() + spaceBase.GetWidth() / 2, spaceBase.GetY() + spaceBase.GetHeight() / 2),
                new Vector2(mEntity.GetX() + mEntity.GetWidth() / 2, mEntity.GetY() + mEntity.GetHeight() / 2));
            if (distSpaceBase < distClosestEntity)
            {
                closestEntity = spaceBase;
            }
        }

        return closestEntity;
    }

    internal void SetTarget(Entity.Entity e)
    {
        mTarget = e;
        mFixedTarget = false;
    }

    internal void SetFixedTarget(Entity.Entity e)
    {
        mTarget = e;
        mFixedTarget = true;
    }

    internal void RemoveFixedTarget()
    {
        mFixedTarget = false;
    }

    internal Entity.Entity GetTarget()
    {
        return mTarget;
    }


    // this method should be used to fire projectiles.
    private void FireGun(Vector2 targetPos)
    {
        var temp = new EProjectile(mEntity.GetPos(), mEntity.GetComponent<CTeam>().mProtagonist, mEntity);
        temp.GetComponent<CTeam>().ChangeTeam(mEntity.GetComponent<CTeam>().GetProtagonist());

        var target = new Vector2(mEntity.GetPos().X - targetPos.X,mEntity.GetPos().Y - targetPos.Y);
        target *= -20;
        temp.AddTarget(mEntity.GetPos().ToPoint() + target.ToPoint());
        mEntity.mEntityManager.Add(temp);
        if (InCameraView())
        {
            mPlayShot = true;
        }
    }

    SoundEnum[] IAudible.GetQueuedSound()
    {
        var num = Globals.RandomNumber();
        num %= 3;
        SoundEnum sound;
        if (mShipType != "Medic")
        {
            switch (num)
            {
                case 1:
                    sound = SoundEnum.Shot;
                    break;
                case 2:
                    sound = SoundEnum.Shot2;
                    break;
                default:
                    sound = SoundEnum.Shot3;
                    break;
            }
        }
        else
        {
            switch (num)
            {
                case 1:
                    sound = SoundEnum.Heal;
                    break;
                case 2:
                    sound = SoundEnum.Heal2;
                    break;
                default:
                    sound = SoundEnum.Heal3;
                    break;
            }
        }
        if (mPlayShot)
        {
            var temp = new SoundEnum[1];
            temp[0] = sound;
            return temp;
        }
        return Array.Empty<SoundEnum>();
    }

    void IAudible.ResetSound()
    {
        mPlayShot = false;
    }

    bool IAudible.IsRemovable()
    {
        return mEntity.GetComponent<CHealth>()?.GetHealth() <= 0;
    }
    float IAudible.GetPan()
    {
        return (mEntity.GetX() - Globals.Camera.Rectangle.X) / (Globals.Camera.Rectangle.Width * 1f) * 2 - 1;
    }

}
