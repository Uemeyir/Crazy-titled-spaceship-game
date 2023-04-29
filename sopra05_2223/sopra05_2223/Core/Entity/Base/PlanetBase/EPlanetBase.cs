using Microsoft.Xna.Framework;
using sopra05_2223.Protagonists;

namespace sopra05_2223.Core.Entity.Base.PlanetBase;

internal abstract class EPlanetBase : EBase
{
    protected EPlanetBase(Vector2 pos, Player player) : base(pos, player)
    {
    }

}