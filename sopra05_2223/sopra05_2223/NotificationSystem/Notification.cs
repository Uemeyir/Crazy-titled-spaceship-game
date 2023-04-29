using sopra05_2223.Core;

namespace sopra05_2223.NotificationSystem;

internal sealed class Notification
{
    private const int Duration = 3000;
    private int mElapsed;
    internal bool Done { get; private set; }
    internal readonly string mText;

    internal Notification(string text)
    {
        mText = text;
    }

    internal void Update()
    {
            mElapsed += Globals.GameTime.ElapsedGameTime.Milliseconds;
            if (mElapsed >= Duration)
            {
                Done = true;
            }
    }
}