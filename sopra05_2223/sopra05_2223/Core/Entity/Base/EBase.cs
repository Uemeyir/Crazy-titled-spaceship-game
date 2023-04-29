using Microsoft.Xna.Framework;
using sopra05_2223.Core.Components;
using sopra05_2223.Core.Entity.Ships;
using sopra05_2223.Protagonists;

namespace sopra05_2223.Core.Entity.Base;

internal abstract class EBase : Entity
{
    protected EBase(Vector2 pos, Player player) : base(pos, player)
    {
    }

    public override void Collides(Entity other)
    {
        if (other == null || other == this)
        {
            return;
        }

        if (!IsColliding(other))
        {
            return;
        }

        if (other is EShip ship)
        {
            CollidesWithShip(ship);
        }
    }


    private void CollidesWithShip(EShip other)
    {
        // calculate direction
        var center = new Vector2(GetX() + GetWidth() / 2f, GetY() + GetHeight() / 2f);
        var otherCenter = new Vector2(other.GetX() + other.GetWidth() / 2f, other.GetY() + other.GetHeight() / 2f);
        var pushDir = center - otherCenter;

        pushDir.Normalize();
        other.GetComponent<CTransform>().AddOutsideForce(-pushDir);
    }

}