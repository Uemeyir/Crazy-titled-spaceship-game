using Microsoft.Xna.Framework;
using sopra05_2223.Core.Components;
using sopra05_2223.Core.Entity.Ships;
using sopra05_2223.Protagonists;

namespace sopra05_2223.Core.Entity.Misc;

internal sealed class EBuoy : Entity
{
    internal int mBuoyList;
    public EBuoy(Vector2 pos, Player owner, ESpy spy, int buoyList) : base(pos, owner)
    {
        mTexture = Art.EnemyBuoy;
        mWidth = mTexture.Width;
        mHeight = mTexture.Height;
        mBuoyList = buoyList;

        // 2 minutes in millis
        AddComponent(new CLifespan(120000, spy));
        AddComponent(new CView(Globals.sViewRadius["Buoy"]));
    }

    public override void Collides(Entity other)
    {
        // this is not collidbale yet
    }

    internal override void ChangeTexture()
    {
        if (mTexture == Art.Buoy)
        {
            mTexture = Art.EnemyBuoy;
        }
        else
        {
            mTexture = Art.Buoy;
        }
    }
}