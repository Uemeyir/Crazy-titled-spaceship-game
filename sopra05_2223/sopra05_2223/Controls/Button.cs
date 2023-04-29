using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using sopra05_2223.Core;
using System;

namespace sopra05_2223.Controls
{
    internal sealed class Button
    {
        #region Fields

        private MouseState mCurrentMouse;
        private Rectangle mDrawRectangle;

        #endregion

        #region Properties

        public event EventHandler Click;

        private readonly Color mHoverColor;
        private readonly Color mNoHoverColor;
        private readonly Color mTextColor;
        private Color mBackColor;
        private readonly bool mCustom;

        public Vector2 Position
        {
            get; init;
        }

        // To determine if the mouse is on top of the button
        private Rectangle Rectangle => new((int)Position.X, (int)Position.Y, Art.MenuButtonTexture.Width - 550, Art.MenuButtonTexture.Height - 208);

        public string Text
        {
            get; set;
        }

        #endregion

        #region Methods

        public Button(bool clickAble = true)
        {
            if (clickAble)
            {
                mHoverColor = Color.Coral; // Turquoise / Gray
                mNoHoverColor = Color.DimGray; // DarkSlateGrey / DarkTurquoise
            }
            else
            {
                // these colors are not final
                mHoverColor = Color.DarkSlateGray;
                mNoHoverColor = mHoverColor;
            }
            mTextColor = Color.White;

            mBackColor = mNoHoverColor;
        }

        public Button(int posx, int posy, int width, int height, string text, bool clickAble = true)
        {
            mDrawRectangle = new(posx, posy, width, height);
            mCustom = true;
            Position = new(posx, posy);
            Text = text;
            if (clickAble)
            {
                mHoverColor = Color.Coral; // Turquoise / Gray
                mNoHoverColor = Color.DimGray; // DarkSlateGrey / DarkTurquoise
            }
            else
            {
                // these colors are not final
                mHoverColor = Color.DarkSlateGray;
                mNoHoverColor = mHoverColor;
            }
            mTextColor = Color.White;

            mBackColor = mNoHoverColor;
        }

        internal void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (mCustom)
            {
                spriteBatch.Draw(Art.MenuButtonTexture, mDrawRectangle, mBackColor);
            }
            else
            {
                spriteBatch.Draw(Art.MenuButtonTexture, Rectangle, mBackColor);
            }
            

            if (!string.IsNullOrEmpty(Text))
            {
                Vector2 center;
                if (mCustom)
                {
                    center = mDrawRectangle.Center.ToVector2() - Art.MenuButtonFont.MeasureString(Text) / 2;
                }
                else
                {
                    center = Rectangle.Center.ToVector2() - Art.MenuButtonFont.MeasureString(Text) / 2;
                }
                
                spriteBatch.DrawString(Art.MenuButtonFont, Text, center, mTextColor);
            }
        }

        internal void Update(GameTime gameTime)
        {
            var previousMouse = mCurrentMouse;
            mCurrentMouse = Mouse.GetState();

            if (mCustom)
            {
                if (!mDrawRectangle.Contains(mCurrentMouse.Position))
                {
                    mBackColor = mNoHoverColor;
                    return;
                }
            }
            else
            {
                if (!Rectangle.Contains(mCurrentMouse.Position))
                {
                    mBackColor = mNoHoverColor;
                    return;
                }
            }
            

            // Hovering
            mBackColor = mHoverColor;
            if (mCurrentMouse.LeftButton == ButtonState.Released && previousMouse.LeftButton == ButtonState.Pressed)
            {
                Click?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}