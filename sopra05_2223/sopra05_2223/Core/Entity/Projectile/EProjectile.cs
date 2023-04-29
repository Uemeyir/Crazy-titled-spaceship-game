using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using sopra05_2223.Core.Animation;
using sopra05_2223.Core.Components;
using sopra05_2223.Core.Entity.Base.PlanetBase;
using sopra05_2223.Core.Entity.Base.SpaceBase;
using sopra05_2223.Core.Entity.Ships;
using sopra05_2223.Protagonists;

namespace sopra05_2223.Core.Entity.Projectile;

internal sealed class EProjectile : Entity
{

    private Point mTarget;
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    internal readonly Entity mEntityWhoShot;

    [JsonConstructor]
    public EProjectile(Vector2 pos, Player owner, Entity whoShot) : base(pos, owner)
    {
        mEntityWhoShot = whoShot;
        
        switch (mEntityWhoShot)
        {
            case EMoerser:
                mTexture = Art.Projectile;
                AddComponent(new CLifespan(1500));
                break;
            case EMedic:
                mTexture = Art.MedicProjectile;
                AddComponent(new CLifespan(1000));
                break;
            default:
                mTexture = Art.Projectile;
                AddComponent(new CLifespan(1250));
                break;
        }
        mWidth = mTexture.Width;
        mHeight = mTexture.Height;
        AddComponent(new CTransform(1.5f, new Point(GetX(), GetY())));
    }

    public override void Collides(Entity other)
    {
        if (other == null || other == this || other is ECollector)
        {
            return;
        }

        if (IsColliding(other) && other != mEntityWhoShot)
        {
            if (other is EShip or EPlanetBase or ESpaceBase &&
                other.GetComponent<CTeam>().mTeam == GetComponent<CTeam>().mTeam &&
                mEntityWhoShot is EMedic)
            {
                var healingPower = Globals.sDestructionPower["Medic"];
                other.GetComponent<CHealth>()?.ChangeHealth(healingPower);
                GetComponent<CLifespan>().End();
            }
            if (other is not (EShip or EPlanetBase or ESpaceBase) || other.GetComponent<CTeam>().mProtagonist ==
                this.GetComponent<CTeam>().mProtagonist)
            {
                return;
            }

            if (mEntityWhoShot is EAttacker)
            {
                var destructionPower = Globals.sDestructionPower["Attacker"];

                var damage = other switch
                {
                    EPlanetBase => (int)(destructionPower * (1 - Globals.sArmor["PlanetBase"])),
                    ESpaceBase => (int)(destructionPower * (1 - Globals.sArmor["SpaceBase"])),
                    ESpy => (int)(destructionPower * (1 - Globals.sArmor["Spy"])),
                    ETransport => (int)(destructionPower * (1 - Globals.sArmor["Transporter"])),
                    EMoerser => (int)(destructionPower * (1 - Globals.sArmor["Moerser"])),
                    EMedic => (int)(destructionPower * (1 - Globals.sArmor["Medic"])),
                    EAttacker => (int)(destructionPower * (1 - Globals.sArmor["Attacker"])),
                    _ => 0
                };

                other.GetComponent<CHealth>()?.ChangeHealth(-damage);
                Art.AnimationManager.AddAnimation(new Explosion1(GetPos(), 0.5f));
            }
            else if (mEntityWhoShot is EMoerser)
            {
                var destructionPower = Globals.sDestructionPower["Moerser"];

                var damage = other switch
                {
                    EPlanetBase => (int)(destructionPower * (1 - Globals.sArmor["PlanetBase"])),
                    ESpaceBase => (int)(destructionPower * (1 - Globals.sArmor["SpaceBase"])),
                    ESpy => (int)(destructionPower * (1 - Globals.sArmor["Spy"])),
                    ETransport => (int)(destructionPower * (1 - Globals.sArmor["Transporter"])),
                    EMoerser => (int)(destructionPower * (1 - Globals.sArmor["Moerser"])),
                    EMedic => (int)(destructionPower * (1 - Globals.sArmor["Medic"])),
                    EAttacker => (int)(destructionPower * (1 - Globals.sArmor["Attacker"])),
                    _ => 0
                };

                other.GetComponent<CHealth>()?.ChangeHealth(-damage);
                Art.AnimationManager.AddAnimation(new Explosion1(GetPos(), 0.5f));
            }

            GetComponent<CLifespan>().End();
        }
    }

    internal override void ChangeTexture()
    {
    }

    internal void AddTarget(Point target)
    {
        mTarget = target;
        GetComponent<CTransform>().SetTarget(mTarget);
    }
}