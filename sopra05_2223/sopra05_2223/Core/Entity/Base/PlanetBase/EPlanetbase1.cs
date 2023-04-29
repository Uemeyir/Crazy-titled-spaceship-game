using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using sopra05_2223.Core.Components;
using sopra05_2223.Pathfinding;
using sopra05_2223.Protagonists;

namespace sopra05_2223.Core.Entity.Base.PlanetBase
{
    internal sealed class EPlanetBase1 : EPlanetBase
    {
        internal EPlanetBase1(Vector2 pos, Player owner, Grid grid) : base(pos, owner)
        {
            mTexture = Art.Planetbase1Ki;
            mWidth = mTexture.Width;
            mHeight = mTexture.Height;
            
            
            AddComponent(new CStorage(10000, 10000, false));
            AddComponent(new CResourceGenerator(20, 10));
            AddComponent(new CSelect(false));
            AddComponent(new CPlanetBase());
            AddComponent(new CHealth(100));
            AddComponent(new CTakeBase(owner));
            AddComponent(new CMoveResource());
            owner.AddEntity(this);
            grid.InsertBaseIntoGrid(this);
        }

        [JsonConstructor]
        internal EPlanetBase1(Vector2 pos, Player owner) : base(pos, owner)
        {
            mTexture = Art.Planetbase1Ki;
            mWidth = mTexture.Width;
            mHeight = mTexture.Height;
        }

        internal override void ChangeTexture()
        {
            if (this.GetComponent<CTeam>().mTeam == Team.Neutral)
            {
                mTexture = Art.Planetbase1Neutral;
            }
            if (this.GetComponent<CTeam>().mTeam == Team.Ki)
            {
                mTexture = Art.Planetbase1Ki;
            }
            if (this.GetComponent<CTeam>().mTeam == Team.Player)
            {
                mTexture = Art.Planetbase1Player;
            }
        }
    }
}
