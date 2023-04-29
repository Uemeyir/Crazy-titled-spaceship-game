using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace sopra05_2223
{
    internal static class SelectRectangle
    {
        private static Texture2D sTexture2D;
        private const int BorderThickness = 3;

        public static void LoadContent(ContentManager content)
        {
            sTexture2D = content.Load<Texture2D>("pixel");
        }

        public static void Draw(SpriteBatch spriteBatch, Rectangle rectangle)
        {
            if (sTexture2D == null)
            {
                return;
            }
            spriteBatch.Draw(sTexture2D, new Rectangle(rectangle.Location.X, rectangle.Location.Y, rectangle.Width, BorderThickness), Color.Red);
            spriteBatch.Draw(sTexture2D, new Rectangle(rectangle.Location.X, rectangle.Location.Y, BorderThickness, rectangle.Height), Color.Red);
            spriteBatch.Draw(sTexture2D, new Rectangle(rectangle.Location.X, rectangle.Location.Y + rectangle.Height, rectangle.Width + BorderThickness, BorderThickness), Color.Red);
            spriteBatch.Draw(sTexture2D, new Rectangle(rectangle.Location.X + rectangle.Width, rectangle.Location.Y, BorderThickness, rectangle.Height + BorderThickness), Color.Red);
        }
    }
}
