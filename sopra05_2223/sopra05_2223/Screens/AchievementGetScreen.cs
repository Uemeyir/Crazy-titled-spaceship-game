using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using sopra05_2223.Core;
using sopra05_2223.InputSystem;
using sopra05_2223.ScreenManagement;
using sopra05_2223.Serializer;
using sopra05_2223.SoundManagement;

namespace sopra05_2223.Screens
{
    internal sealed class AchievementGetScreen : IScreen
    {
        private readonly string mAchievement;
        private Point mScreenSize;
        private int mElapsed;
        private readonly int mLength;
        private const float TopLeftX = 0.8f;
        private const float TopLeftY = 0.5f;
        private const float Width = 0.2f;
        private const float Height = Width / 3;


        public AchievementGetScreen(int length, SoundManager soundManager, string type, Point screenSize)
        {
            mScreenSize = screenSize;
            mAchievement = type;
            mLength = length;
            soundManager.PlaySound(SoundEnum.Woosh1);

            // Save the new Achievement immediately
            SopraSerializer.SerializeAchievements();
        }
        public ScreenManager ScreenManager { get; set; }

        bool IScreen.UpdateLower => true;

        bool IScreen.DrawLower => true;

        void IScreen.Update(Input input)
        {
            mElapsed += Globals.GameTime.ElapsedGameTime.Milliseconds;
            if (mElapsed >= mLength || input.GetKeys().Contains(Keys.Escape)) 
            {
                ScreenManager.RemoveScreens();
                input.GetKeys().Remove(Keys.Escape);
            }
        }

        void IScreen.Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Art.AchievementGet, new Rectangle((int)(TopLeftX * mScreenSize.X), (int)(TopLeftY * mScreenSize.Y), (int)(Width * mScreenSize.X), (int)(Height * mScreenSize.X)), Color.White);
            spriteBatch.DrawString(Art.Arial12, mAchievement, new Vector2((TopLeftX + 0.08f) * mScreenSize.X, (TopLeftY + 0.05f) * mScreenSize.Y), Color.White);
        }

        public void Resize(Point newSize)
        {
            mScreenSize = newSize;
        }

    }
}
