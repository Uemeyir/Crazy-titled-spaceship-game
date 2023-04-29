using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace sopra05_2223.Core.Components
{
    internal sealed class CResource : Component
    {
        [JsonRequired]
        // Stores Maximum Amount of Metal in Resource
        private readonly float mMaxMetal; // type = 0
        [JsonRequired]
        // Stores Maximum Amount of Oxygen
        private readonly float mMaxOxygen; // type = 1
        [JsonRequired]
        // Stores Amount of Metal left
        private float mMetalLeft;
        [JsonRequired]
        // Stores Amount of Oxygen left
        private float mOxygenLeft;

        private readonly Texture2D mTexture;

        internal CResource(float maxMetal, float maxOxygen)
        {
            mMaxMetal = maxMetal;
            mMaxOxygen = maxOxygen;
            mMetalLeft = maxMetal;
            mOxygenLeft = maxOxygen;

            mTexture = Art.Resource1;
        }

        [JsonConstructor]
        internal CResource(float maxMetal, float maxOxygen, float metalLeft, float oxygenLeft)
        {
            mMaxMetal = maxMetal;
            mMaxOxygen = maxOxygen;
            mMetalLeft = metalLeft;
            mOxygenLeft = oxygenLeft;

            mTexture = Art.Resource1;
        }

        internal override void Update()
        {
        }

        // Method Deplete is used to take Metal/Oxygen from the resource
        // It calculates, how much of the resource is actually taken and returns this value
        // this return value will be used to add the amount of resources to the ship which took the resources
        //
        // int amount: Amount of Resource a ship wants to take
        // bool type: Specifies, which type of resource is taken: Metal = false, Oxygen = true  

        // return value will be used soon
        internal float Deplete(int amount, bool type)
        {

            var oldOxygen = mOxygenLeft;
            var oldMetal = mMetalLeft;
            if (!type)
            {
                if (amount >= mMetalLeft)
                {
                    mMetalLeft = 0;
                    return oldMetal;
                }
                else
                {
                    mMetalLeft -= amount;
                    return amount;
                }
            }
            else
            {
                if (amount >= mOxygenLeft)
                {
                    mOxygenLeft = 0;
                    return oldOxygen;
                }
                else
                {
                    mOxygenLeft -= amount;
                    return amount;
                }
            }
        }

        // Draws the Amount of Oxygen and Metal left in resource
        public void DrawAmount(SpriteBatch spriteBatch, Matrix translationMatrix)
        {
            DrawOxygen(spriteBatch, translationMatrix);
            DrawMetal(spriteBatch, translationMatrix);
        }

        // Draws the Amount of Oxygen left in resource by using Effect OxygenEffect
        private void DrawOxygen(SpriteBatch spriteBatch, Matrix translationMatrix)
        {
            // Check for division by zero needed at least until serialization works rightfully.
            // Might be helpful not only for serialization?!
            if (mMaxOxygen != 0)
            {
                Art.OxygenEffect.Parameters["OxygenPercent"].SetValue(mOxygenLeft / mMaxOxygen);
            }
            else
            {
                Art.OxygenEffect.Parameters["OxygenPercent"].SetValue(0);
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, Art.OxygenEffect, translationMatrix);
            spriteBatch.Draw(Art.Healthbar, new Rectangle((int)(mEntity.GetX() + mTexture.Width * 0.5f - Art.Healthbar.Width * 0.5f), (int)(mEntity.GetY() + mTexture.Height * 0.8f), mTexture.Width / 2, Art.Healthbar.Height), Color.White);
            spriteBatch.End();
            spriteBatch.Begin();
        }

        // Draws the Amount of Metal left in resource by using Effect OxygenEffect
        private void DrawMetal(SpriteBatch spriteBatch, Matrix translationMatrix)
        {
            // Check for division by zero needed at least until serialization works rightfully.
            // Might be helpful not only for serialization?!
            if (mMaxMetal != 0)
            {
                Art.MetalEffect.Parameters["MetalPercent"].SetValue(mMetalLeft / mMaxMetal);
            }
            else
            {
                Art.MetalEffect.Parameters["MetalPercent"].SetValue(0);
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, Art.MetalEffect, translationMatrix);
            spriteBatch.Draw(Art.Healthbar, new Rectangle((int)(mEntity.GetX() + mTexture.Width * 0.5f - Art.Healthbar.Width * 0.5f), (int)(mEntity.GetY() + mTexture.Height * 0.9f), mTexture.Width / 2, Art.Healthbar.Height), Color.White);
            spriteBatch.End();
            spriteBatch.Begin();
        }

        public float GetMetal()
        {
            return mMetalLeft;
        }

        public float GetOxygen()
        {
            return mOxygenLeft;
        }
    }
}
