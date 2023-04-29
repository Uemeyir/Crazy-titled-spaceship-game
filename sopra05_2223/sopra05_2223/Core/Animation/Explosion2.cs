using System;
using Microsoft.Xna.Framework;
using sopra05_2223.SoundManagement;

namespace sopra05_2223.Core.Animation
{
    internal sealed class Explosion2 : Animation, IAudible
    {

        private bool mSoundPlayed;

        public Explosion2(Vector2 pos, float scale) : base(Art.Explosion2, 9, 6, 1, scale, pos)
        {
            Globals.SoundManager.AddIAudible(this);
            Globals.Camera.IncreaseShake();
        }

        SoundEnum[] IAudible.GetQueuedSound()
        {
            if (mSoundPlayed)
            {
                return Array.Empty<SoundEnum>();
            }
            mSoundPlayed = true;
            if (!Globals.Camera.Rectangle.Intersects(new Rectangle(new Point( (int) mPosition.X, (int) mPosition.Y), new Point(GetFrameHeight(), GetFrameWidth()))))
            {
                return Array.Empty<SoundEnum>();
            }

            SoundEnum[] sound = new SoundEnum[1];
            var rand = Globals.RandomNumber() % 3;
            if (rand == 0)
            {
                sound[0] = SoundEnum.ShipExplosion2;
            } else if (rand == 1)
            {
                sound[0] = SoundEnum.ShipExplosion;
            }
            else
            {
                sound[0] = SoundEnum.ShipExplosion3;
            }
            return sound;
        }

        bool IAudible.IsRemovable()
        {
            return mSoundPlayed;
        }

        void IAudible.ResetSound()
        {
        }

        float IAudible.GetPan()
        {
            return (mPosition.X - Globals.Camera.Rectangle.X) / (Globals.Camera.Rectangle.Width * 1f) * 2 - 1;
        }
    }
}
