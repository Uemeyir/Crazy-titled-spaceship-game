using sopra05_2223.Core.Components;
using sopra05_2223.Protagonists;
using Microsoft.Xna.Framework;

namespace sopra05_2223.Core.Entity.Ships
{
    internal sealed class ECollector : EShip
    {
        public ECollector(Vector2 pos, Player owner, ETransport theOwnerETransport, int num) : base(pos, owner)
        {
            if (owner is PlayerPlayer)
            {
                mTexture = Art.Sammler;
                AddComponent(new CSelect(true));
            }
            else
            {
                mTexture = Art.EnemySammler;
            }

            mWidth = mTexture.Width;
            mHeight = mTexture.Height;

            // Components:
            AddComponent(new CollectCollector(theOwnerETransport, num));
            AddComponent(new CTransform(0.3f, new Point(GetX(), GetY())));
            AddComponent(new CView(Globals.sViewRadius["Collector"]));
        }

        internal override void ChangeTexture()
        {
            if (mTexture == Art.Sammler)
            {
                mTexture = Art.EnemySammler;
            }
            else
            {
                mTexture = Art.Sammler;
            }
        }
    }
}
