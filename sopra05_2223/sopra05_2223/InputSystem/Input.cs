using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace sopra05_2223.InputSystem;

public sealed class Input
{
    private Point mMousePos;

    private Dictionary<int, MouseActionEnum> mMousePressed;

    private List<Keys> mKeys;

    private Rectangle mPressedRectangle;

    private int mWheelScroll;

    public Input()
    {
        mMousePos = new Point(0, 0);
        mMousePressed = new Dictionary<int, MouseActionEnum>();

        mKeys = new List<Keys>();

        mPressedRectangle = new Rectangle(0, 0, 0, 0);
    }

    internal void AddButton(int i, MouseActionEnum v)
    {
        mMousePressed.Add(i, v);
    }

    internal void AddKey(Keys key)
    {
        mKeys.Add(key);
    }

    // deprecated use GetKey instead
    public List<Keys> GetKeys()
    {
        return mKeys;
    }

    // deprecated use GetMouseButton instead

    // Method will be used in the future
    /* Unused
    public List<KeyValuePair<int, MouseActionEnum>> GetMouseButtons()
    {

        var kvPairs = new List<KeyValuePair<int, MouseActionEnum>>();
        foreach (var kvpair in mMousePressed)
        {
            kvPairs.Add(kvpair);
        }
        return kvPairs;
    }
    */

    internal void Update(int x, int y)
    {
        mMousePos.X = x;
        mMousePos.Y = y;

        mMousePressed = new Dictionary<int, MouseActionEnum>();

        mKeys = new List<Keys>();
    }

    internal Point GetMousePosition()
    {
        return mMousePos;
    }

    internal void SetRectangle(int x, int y, int width, int height)
    {
        mPressedRectangle.X = x;
        mPressedRectangle.Y = y;
        mPressedRectangle.Width = width;
        mPressedRectangle.Height = height;
    }

    internal Rectangle GetSelectRect()
    {
        return mPressedRectangle;
    }

    private void RemoveButtons(int button)
    {
        mMousePressed.Remove(button);
    }

    private void RemoveKeys(Keys key)
    {
        mKeys.Remove(key);
    }

    // returns if a key is in the input and removes it from the input.
    internal bool GetKey(Keys otherKey)
    {
        foreach (var key in mKeys)
        {
            if (key == otherKey)
            {
                RemoveKeys(otherKey);
                return true;
            }
        }

        return false;
    }

    // returns if a MouseButton is in the input and removes it from the input.
    internal MouseActionEnum GetMouseButton(int mouseButton)
    {
        if (!mMousePressed.ContainsKey(mouseButton))
        {
            return MouseActionEnum.Free;
        }
        var returnVal = mMousePressed[mouseButton];
        RemoveButtons(mouseButton);
        return returnVal;
    }

    internal void SetScrollAmount(int amt)
    {
        mWheelScroll = amt;
    }

    internal int GetScrollAmount()
    {
        return mWheelScroll;
    }
}