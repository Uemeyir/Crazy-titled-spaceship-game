using Microsoft.Xna.Framework;
using sopra05_2223.Core.Components;
using sopra05_2223.Protagonists;

namespace sopra05_2223.Core.Entity.Ships
{
    internal sealed class EMedic : EShip
    {
        public EMedic(Vector2 pos, Player owner) : base(pos, owner)
        {
            if (owner is PlayerPlayer)
            {
                mTexture = Art.Medic;
                AddComponent(new CSelect(true));
                AddComponent(new CView(Globals.sViewRadius["Medic"]));
            }
            else
            {
                mTexture = Art.EnemyMedic;
            }
            mWidth = mTexture.Width;
            mHeight = mTexture.Height;

            // Components:
            AddComponent(new CTransform(1.0f, new Point(GetX(), GetY())));
            AddComponent(new CHealth(100));
            AddComponent(new CGun(5));
            AddComponent(new CStorage(Globals.sStorage["Medic"].X, Globals.sStorage["Medic"].Y, false));
        }

        internal override void ChangeTexture()
        {
            if (mTexture == Art.Medic)
            {
                mTexture = Art.EnemyMedic;
            }
            else
            {
                mTexture = Art.Medic;
            }
        }
    }
}
