using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sopra05_2223.Background
{
    internal class ParallaxLayer
    {
        public Vector2 mPos;
        public readonly int mSpeed;
        private readonly Texture2D mTexture;

        public ParallaxLayer(Vector2 pos, int speed, Texture2D texture)
        {
            this.mPos = pos;
            this.mSpeed = speed;
            this.mTexture = texture;
        }

        internal void Draw(SpriteBatch spriteBatch, float zoom, Point worldSize)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.LinearWrap);
            spriteBatch.Draw(mTexture, mPos, new Rectangle(0, 0, worldSize.X * 3, worldSize.Y * 3), Color.White, 0, Vector2.Zero, zoom, SpriteEffects.None, 1);
            spriteBatch.End();
            spriteBatch.Begin();
        }
    }
}
