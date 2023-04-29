using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;

namespace sopra05_2223.InputSystem
{
    internal sealed class MouseCatcher
    {
        [DllImport("user32.dll")]
        static extern void ClipCursor(ref Rectangle rect);
        [DllImport("user32.dll")]
        static extern void GetClipCursor(ref Rectangle rect);

        private readonly Rectangle mOldCursorSpace;

        internal MouseCatcher()
        {
            GetClipCursor(ref mOldCursorSpace);
        }

        public void Update(bool isActive, GameWindow window, bool topIsMenu)
        {
            if (isActive && !topIsMenu)
            {
                Rectangle rect = window.ClientBounds;
                rect.Width += rect.X;
                rect.Height += rect.Y;

                ClipCursor(ref rect);
            }
            else
            {
                var rect = mOldCursorSpace;
                ClipCursor(ref rect);
            }
        }
    }
}
