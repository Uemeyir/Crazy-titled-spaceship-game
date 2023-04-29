using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace sopra05_2223.Background
{
    internal sealed class Parallax
    {
        private readonly List<ParallaxLayer> mLayers;
        private const float BaseSpeed = 0.2f;

        public Parallax(Texture2D texture)
        {
            mLayers = new List<ParallaxLayer>
            {
                new (new Vector2(-3000, -3000), 1, texture),
                new (new Vector2(-1500, -1500), 2, texture),
                new (new Vector2(-2500, -2500), 4, texture)
            };
        }

        internal void Draw(SpriteBatch spriteBatch, float zoom, Point worldsize)
        {
            foreach (var k in this.mLayers)
            {
                k.Draw(spriteBatch, zoom, worldsize);
            }
        }

        internal void Move(float distanceX, float distanceY)
        {
            foreach (var k in this.mLayers)
            {
                k.mPos += new Vector2((float)Math.Round(distanceX * BaseSpeed * k.mSpeed), (float)Math.Round(distanceY * BaseSpeed * k.mSpeed));
            }

        }
    }
}
