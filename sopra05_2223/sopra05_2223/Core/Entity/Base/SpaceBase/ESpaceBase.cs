using Microsoft.Xna.Framework;
using sopra05_2223.Core.Components;
using sopra05_2223.Protagonists;

namespace sopra05_2223.Core.Entity.Base.SpaceBase;

internal abstract class ESpaceBase : EBase
{
    protected ESpaceBase(Vector2 pos, Player player) : base(pos, player)
    {
        if (player is PlayerPlayer)
        {
            AddComponent(new CView(Globals.sViewRadius["SpaceBase"]));
        }
    }
}