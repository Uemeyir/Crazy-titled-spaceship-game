using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using sopra05_2223.Core.Components;
using sopra05_2223.Core.ShipBase;
using sopra05_2223.Pathfinding;
using sopra05_2223.Protagonists;

namespace sopra05_2223.Core.Entity.Base.SpaceBase;

internal sealed class ESpaceBase1 : ESpaceBase
{
    internal ESpaceBase1(Vector2 pos, Player owner, Grid grid) : base(pos, owner)
    {
        mTexture = Art.SpaceBaseKi;
        mWidth = mTexture.Width;
        mHeight = mTexture.Height;
        
        AddComponent(new CStorage(10000, 10000, false));
        AddComponent(new CSelect(false));
        AddComponent(new BuildSlotManager());
        AddComponent(new CShipBase());
        AddComponent(new CHealth(100));
        AddComponent(new CTakeBase(owner));
        AddComponent(new CMoveResource());
        owner.AddEntity(this);
        grid.InsertBaseIntoGrid(this);
    }

    [JsonConstructor]
    internal ESpaceBase1(Vector2 pos, Player owner) : base(pos, owner)
    {
        mTexture = Art.SpaceBaseKi;
        mWidth = mTexture.Width;
        mHeight = mTexture.Height;
    }

    internal override void ChangeTexture()
    {
        switch (GetComponent<CTeam>().mTeam)
        {
            case Team.Neutral:
                mTexture = Art.SpaceBaseNeutral;
                break;
            case Team.Player:
                mTexture = Art.SpaceBasePlayer;
                break;
            case Team.Ki:
                mTexture = Art.SpaceBaseKi;
                break;
        }
    }
}