using Microsoft.Xna.Framework;
using sopra05_2223.Core.Components;
using sopra05_2223.Protagonists;

namespace sopra05_2223.Core.Entity.Ships;

internal sealed class ESpy : EShip
{
    internal Vector2 mStartPos; 
    public ESpy(Vector2 pos, Player owner) : base(pos, owner)
    {
        if (owner is PlayerPlayer)
        {
            mTexture = Art.Spy;
            AddComponent(new CSelect(true));
            AddComponent(new CView(Globals.sViewRadius["Spy"]));
        }
        else
        {
            mTexture = Art.EnemySpy;
        }
        mWidth = mTexture.Width;
        mHeight = mTexture.Height;

        AddComponent(new CTransform(1f, new Point(GetX(), GetY())));
        AddComponent(new CHealth(100));
        AddComponent(new CBuoyPlacer());
        AddComponent(new CStorage(Globals.sStorage["Spy"].X, Globals.sStorage["Spy"].Y, false));
    }

    internal override void ChangeTexture()
    {
        if (mTexture == Art.Spy)
        {
            mTexture = Art.EnemySpy;
        }
        else
        {
            mTexture = Art.Spy;
        }
    }
}