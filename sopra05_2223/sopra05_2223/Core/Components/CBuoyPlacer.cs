using System.Collections.Generic;
using Microsoft.Xna.Framework;
using sopra05_2223.Core.Entity.Misc;
using sopra05_2223.Core.Entity.Ships;

namespace sopra05_2223.Core.Components;

internal sealed class CBuoyPlacer : Component
{
    internal int mBuoyCount = 2;
    internal bool mShouldPlaceBuoy;
    internal Point mNextBuoyPosition;
    internal readonly List<EBuoy> mBuoyList = new();
    private int mNext;
    internal override void Update()
    {
        if (mShouldPlaceBuoy && mBuoyCount > 0 && Vector2.Distance(new Vector2(mEntity.GetPos().X, mEntity.GetPos().Y), new Vector2(mNextBuoyPosition.X, mNextBuoyPosition.Y)) <= 50)
        {
            var buoy = new EBuoy(new Vector2(mEntity.GetX() + mEntity.GetWidth(),
                mEntity.GetY() + mEntity.GetHeight()), mEntity.GetComponent<CTeam>().mProtagonist, (ESpy)mEntity, mNext);
            if (mNext == 0)
            {
                if (mBuoyList.Count == 0)
                {
                    mBuoyList.Add(buoy);
                }
                mBuoyList[mNext] = buoy;
                mNext = 1;
            }
            else
            {
                if (mBuoyList.Count == 1)
                {
                    mBuoyList.Add(buoy);
                }
                mBuoyList[mNext] = buoy;
                mNext = 0;
            }
            mEntity.mEntityManager.Add(buoy);
            mEntity.GetComponent<CTeam>()?.mProtagonist.AddEntity(buoy);
            mBuoyCount -= 1;
            mShouldPlaceBuoy = false;
            mNextBuoyPosition = Point.Zero;
            ESpy x = (ESpy)mEntity; 
            mEntity.GetComponent<CTransform>().SetTarget(x.mStartPos.ToPoint());
        }
    }

    internal void PlaceBuoy()
    {
        mShouldPlaceBuoy = true;
    }
}