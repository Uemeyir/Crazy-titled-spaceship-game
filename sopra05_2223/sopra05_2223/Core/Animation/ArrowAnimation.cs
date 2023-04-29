using Microsoft.Xna.Framework;

namespace sopra05_2223.Core.Animation
{
    internal class ArrowAnimation : Animation
    {
        public ArrowAnimation(Vector2 pos, float scale) : base(Art.AnimatedArrow, 8, 6, 1, scale, pos)
        {
        }
    }
}
