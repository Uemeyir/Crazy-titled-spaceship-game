using Microsoft.Xna.Framework;
using sopra05_2223.Core.Components;
using sopra05_2223.Protagonists;

namespace sopra05_2223.Core.Entity.Ships;

internal sealed class EAttacker : EShip
{
    public EAttacker(Vector2 pos, Player owner) : base(pos, owner)
    {
        if (owner is PlayerPlayer)
        {
            mTexture = Art.Bomber;
            AddComponent(new CSelect(true));
            AddComponent(new CView(Globals.sViewRadius["Attacker"]));
        }
        else
        {
            mTexture = Art.EnemyBomber;
        }
        mWidth = mTexture.Width;
        mHeight = mTexture.Height;

        // Components:
        AddComponent(new CTransform(0.6f, new Point(GetX(), GetY())));
        AddComponent(new CHealth(100));
        AddComponent(new CGun(1.5));
        AddComponent(new CStorage(Globals.sStorage["Attacker"].X, Globals.sStorage["Attacker"].Y, false));
    }

    internal override void ChangeTexture()
    {
        if (mTexture == Art.Bomber)
        {
            mTexture = Art.EnemyBomber;
        }
        else
        {
            mTexture = Art.Bomber;
        }
    }

}