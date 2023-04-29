using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace sopra05_2223.Core.Animation
{
    internal class Animation
    {
        private readonly Texture2D mSpriteSheet;
        // List of Rectangles that define each Frame
        private readonly List<Rectangle> mSourceRectangles = new();
        private readonly int mTotalFrames;
        private readonly int mFrameWidth;
        private readonly int mFrameHeight;
        // Current Frame
        private int mFrame;
        // Frame length
        private readonly int mFrameTime;
        // Remaining time for frame
        private readonly float mScale;
        public bool mActive = true;
        private bool mPlayedOnce;
        private readonly int mRepeats;
        private int mPlayedFrames;
        private int mElapsed;

        protected readonly Vector2 mPosition;

        protected Animation(Texture2D texture, int framesX, int framesY, float frameTime, float scale, Vector2 pos, int repeats = 1)
        {
            mSpriteSheet = texture;
            mFrameTime = (int)frameTime * 17;
            mTotalFrames = framesX * framesY;
            mScale = scale;
            mRepeats = repeats;
            mFrameWidth = mSpriteSheet.Width / framesX;
            mFrameHeight = mSpriteSheet.Height / framesY;

            // Creates List of Frame Rectangles by using dimension of SpriteSheet
            for (int y = 0; y < framesY; y++)
            {
                for (int x = 0; x < framesX; x++)
                {
                    mSourceRectangles.Add(new(x * mFrameWidth, y * mFrameHeight, mFrameWidth, mFrameHeight));
                }
            }

            mPosition = new Vector2(pos.X - mSpriteSheet.Width * 1 / 8f * 0.5f * scale, pos.Y - (mSpriteSheet.Height) * 1 / 5f * 0.5f * scale);
        }

        // Stops Animation
        public void Stop()
        {
            mActive = false;
        }

        // Starts Animation
        public void Start()
        {
            mActive = true;
        }

        // Resets Animation
        public void Reset()
        {
            mFrame = 0;
        }

        // returns, if Animation played (at least) once
        public bool IsDone()
        {
            return mPlayedOnce;
        }

        public void Update()
        {
            if (!mActive)
            {
                return;
            }

            mElapsed += Globals.GameTime.ElapsedGameTime.Milliseconds; 

            while (mElapsed >= mFrameTime)
            {
                if (mPlayedFrames + 1 == mTotalFrames * mRepeats)
                {
                    mPlayedOnce = true;
                }
                mPlayedFrames += 1;
                mFrame = (mFrame + 1) % mTotalFrames;
                mElapsed -= mFrameTime;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Matrix translationMatrix)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, translationMatrix);
            spriteBatch.Draw(mSpriteSheet, mPosition, mSourceRectangles[mFrame], Color.White, 0, Vector2.Zero, new Vector2(mScale, mScale), SpriteEffects.None, 1);
            spriteBatch.End();
            spriteBatch.Begin();
        }

        protected int GetFrameWidth()
        {
            return mFrameWidth;
        }

        protected int GetFrameHeight()
        {
            return mFrameHeight;
        }

    }

    
}
