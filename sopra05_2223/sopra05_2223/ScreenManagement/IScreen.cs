using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sopra05_2223.InputSystem;

namespace sopra05_2223.ScreenManagement;

internal interface IScreen
{
    ScreenManager ScreenManager
    {
        set;
    }

    public bool UpdateLower
    {
        get;
    }
    public bool DrawLower
    {
        get;
    }

    public void Resize(Point newSize);
    public void Update(Input input);
    public void Draw(SpriteBatch spriteBatch);
}