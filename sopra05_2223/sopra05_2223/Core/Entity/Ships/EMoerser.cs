using Microsoft.Xna.Framework;
using sopra05_2223.Core.Components;
using sopra05_2223.Protagonists;

namespace sopra05_2223.Core.Entity.Ships;

internal sealed class EMoerser : EShip
{
    public EMoerser(Vector2 pos, Player owner) : base(pos, owner)
    {
        if (owner is PlayerPlayer)
        {
            mTexture = Art.Moerser;
            AddComponent(new CSelect(true));
            AddComponent(new CView(Globals.sViewRadius["Moerser"]));
        }
        else
        {
            mTexture = Art.EnemyMoerser;
        }
        mWidth = mTexture.Width;
        mHeight = mTexture.Height;

        // Components:
        AddComponent(new CTransform(0.3f, new Point(GetX(), GetY())));
        AddComponent(new CHealth(100));
        AddComponent(new CGun(1));
        AddComponent(new CStorage(Globals.sStorage["Moerser"].X, Globals.sStorage["Moerser"].Y, false));
    }

    internal override void ChangeTexture()
    {
        if (mTexture == Art.Moerser)
        {
            mTexture = Art.EnemyMoerser;
        }
        else
        {
            mTexture = Art.Moerser;
        }
    }

}
