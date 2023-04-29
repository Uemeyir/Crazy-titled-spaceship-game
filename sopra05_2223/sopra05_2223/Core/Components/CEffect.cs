using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace sopra05_2223.Core.Components
{
    internal sealed class CEffect : Component
    {
        // Saves the List of active Effects
        private readonly List<Effect> mEffects;

        [JsonConstructor]
        public CEffect()
        {
            mEffects = new List<Effect>();
        }

        // Constructor which allows addition of a new effect immediately
        public CEffect(Effect effect)
        {
            mEffects = new List<Effect>
            {
                effect
            };
        }

        internal override void Update()
        {
        }

        // Draws Effect which Highlights selected Entities
        internal void DrawGlow(SpriteBatch spriteBatch, Matrix translationMatrix, float rotation, Vector2 origin, Vector2 position)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, Art.Glow, translationMatrix);
            spriteBatch.Draw(mEntity.mTexture, position, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 1f);
            spriteBatch.End();
            spriteBatch.Begin();
        }

        // Adds Effect to List of Active Effects, only if Effect is not active yet 
        internal void AddEffect(Effect effect)
        {
            if (!mEffects.Contains(effect))
            {
                mEffects.Add(effect);
            }
        }

        // Removes Effect from List of Active Effects
        internal void RemoveEffect(Effect effect)
        {
            if (mEffects.Contains(effect))
            {
                mEffects.Remove(effect);
            }
        }
    }
}
