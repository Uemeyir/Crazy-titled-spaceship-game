using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using sopra05_2223.InputSystem;

namespace sopra05_2223.Core.Animation
{
    internal sealed class AnimationManager
    {
        private readonly List<Animation> mAnimations;
        internal AnimationManager()
        {
            mAnimations = new List<Animation>();
        }

        internal void AddAnimation(Animation anim)
        {
            if (anim is ArrowAnimation)
            {
                for (int i = mAnimations.Count - 1; i >= 0; i--)
                {
                    if (mAnimations[i] is ArrowAnimation)
                    {
                        mAnimations.Remove(mAnimations[i]);
                    }
                }
            }
            mAnimations.Add(anim);
        }

        internal void UpdateAnimations(Input input)
        {
            for (int i = mAnimations.Count - 1; i >= 0; i--)
            {
                mAnimations[i].Update();
                if (mAnimations[i].IsDone() && mAnimations[i] is Explosion1 or ArrowAnimation or Explosion2)
                {
                    mAnimations[i].Reset();
                    mAnimations.Remove(mAnimations[i]);
                }
            }

            for (int i = mAnimations.Count - 1; i >= 0; i--)
            {
                if (mAnimations[i] is PlanetAnimated1 && input.GetKeys().Contains(Keys.Space))
                {
                    if (mAnimations[i].mActive)
                    {
                        mAnimations[i].Stop();
                    }
                    else
                    {
                        mAnimations[i].Start();
                    }
                }
            }


        }

        internal void DrawAnimations(SpriteBatch spriteBatch, Matrix translationMatrix)
        {
            foreach (var k in mAnimations)
            {
                k.Draw(spriteBatch, translationMatrix);
            }
        }
    }
}
