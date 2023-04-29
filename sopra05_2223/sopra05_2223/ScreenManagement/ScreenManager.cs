using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sopra05_2223.InputSystem;
using System.Collections.Generic;
using System.Linq;
using sopra05_2223.Core;
using sopra05_2223.Screens;

namespace sopra05_2223.ScreenManagement;

internal sealed class ScreenManager
{
    private readonly List<IScreen> mScreenStack = new();
    private readonly List<IScreen> mScreensToAdd = new();
    private int mScreensToRemove;

    internal void AddScreen(IScreen screen)
    {
        screen.ScreenManager = this;
        mScreensToAdd.Add(screen);
    }

    internal void RemoveScreens(int n=1)
    {
        mScreensToRemove += n;
    }

    // So far only used in MouseCatcher which is only implemented for Windows Version.
    internal bool IsTopScreenMenu()
    {
        var topScreen = mScreenStack.Last();
        // add screenTypes which are considered a menu
        return topScreen is MenuScreen or PauseMenuScreen or StatisticScreen or StatisticScreenTwo or AchievementsScreen or SaveGameScreen or LoadGameScreen or ShipBaseScreen or YouSureScreen or MenuBackgroundScreen or OptionsMenuScreen or InputOptionsScreen or ShipsInfoScreen or ScreenplayScreen or GameOverScreen or GameWonScreen;
    }

    internal void ReturnToMainMenu()
    {
        Globals.SoundManager.RemoveEntities();
        // MenuScreen should always be at position i=1, just wanna make sure this never breaks.
        for (var i = 0; i < mScreenStack.Count; ++i)
        {
            if (mScreenStack[i] is MenuScreen)
            {
                // Remove all screens on top of MainMenu during next Update.
                RemoveScreens(mScreenStack.Count - i - 1);
                return;
            }
        }
    }

    internal void ReturnToGameScreen()
    {
        // MenuScreen should always be at position i=1, just wanna make sure this never breaks.
        for (var i = 0; i < mScreenStack.Count; ++i)
        {
            if (mScreenStack[i] is Hud)
            {
                // Remove all screens on top of MainMenu during next Update.
                RemoveScreens(mScreenStack.Count - i - 1);
                return;
            }
        }
    }

    internal IScreen GetTopScreen()
    {
        return mScreenStack.Last();
    }

    internal void ResizeScreens(Point newSize)
    {
        foreach (var k in mScreenStack)
        {
            k.Resize(newSize);
        }
    }

    internal void Draw(SpriteBatch spriteBatch)
    {
        // Lower Screens have to be drawn first, so we have to all screens that have to be drawn first.
        var idx = mScreenStack.Count - 1;
        List<IScreen> drawQueue = new();
        while (idx != -1)
        {
            drawQueue.Add(mScreenStack[idx]);
            idx = mScreenStack[idx].DrawLower ? --idx : -1;
        }

        idx = drawQueue.Count - 1;
        while (idx != -1)
        {
            drawQueue[idx].Draw(spriteBatch);
            --idx;
        }
    }

    internal void Update(Input input)
    {

        // Update ScreenStack.
        for (var i = 0; i < mScreensToRemove; ++i)
        {
            if (mScreenStack.Count > 0)
            {
                mScreenStack.RemoveAt(mScreenStack.Count - 1);
            }
        }
        foreach (var s in mScreensToAdd)
        {
            mScreenStack.Add(s);
        }
        mScreensToAdd.Clear();
        mScreensToRemove = 0;

        // Update Screens.
        var idx = mScreenStack.Count - 1;
        while (idx != -1)
        {
            mScreenStack[idx].Update(input);
            idx = mScreenStack[idx].UpdateLower ? --idx : -1;
        }
    }
    internal List<IScreen> GetStack()
    {
        return mScreenStack;
    }

    internal IScreen GetScreen(Type type)
    {
        foreach (var k in mScreenStack)
        {
            if (k.GetType() == type)
            {
                return k;
            }
        }
        return null;
    }
}
