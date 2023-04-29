using Microsoft.Xna.Framework;
using sopra05_2223.Core.Components;

namespace sopra05_2223.Core.Entity.Resource
{
    internal class EResource : Entity
    {
        public EResource(Vector2 pos, int maxOxygen, int maxMetal) : base(pos, null)
        {
            mTexture = Art.Resource1;
            mWidth = mTexture.Width;
            mHeight = mTexture.Height;
            AddComponent(new CResource(maxMetal, maxOxygen));
        }

        public override void Collides(Entity other)
        {
        }

        internal override void ChangeTexture()
        {
        }
    }
}
