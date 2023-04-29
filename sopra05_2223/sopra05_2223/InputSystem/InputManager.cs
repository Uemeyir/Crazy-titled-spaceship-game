using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using sopra05_2223.SoundManagement;
using System;
using System.Collections.Generic;

namespace sopra05_2223.InputSystem
{
    internal sealed class InputManager : IAudible
    {

        private readonly Input mInput;

        private Keys[] mPrevKeys;
        private ButtonState[] mPrevMouseButtonStates;

        private Point mLeftMouseDown;

        private readonly List<SoundEnum> mSounds;

        private int mPrevMouseWheel;

        public InputManager()
        {
            mPrevMouseButtonStates = new ButtonState[5];
            mInput = new Input();
            mLeftMouseDown = new Point();

            mSounds = new List<SoundEnum>();
        }

        public Input GetInput()
        {
            return mInput;
        }

        public void Update(bool active)
        {

            if (active)
            {

                // ------ update keys --------
                var keys = Keyboard.GetState().GetPressedKeys();
                mInput.Update(Mouse.GetState().X, Mouse.GetState().Y);

                foreach (var key in keys)
                {
                    if (PositiveFlank(key))
                    {
                        mInput.AddKey(key);
                    }
                }

                mPrevKeys = keys;


                // ------- update Mouse ---------
                var buttons = new ButtonState[5];
                buttons[0] = Mouse.GetState().LeftButton;
                buttons[1] = Mouse.GetState().RightButton;
                buttons[2] = Mouse.GetState().MiddleButton;
                buttons[3] = Mouse.GetState().XButton1;
                buttons[4] = Mouse.GetState().XButton2;

                for (var i = 0; i < mPrevMouseButtonStates.Length; i++)
                {
                    // check mouse state. Pressed, Held, Released, Free
                    if (mPrevMouseButtonStates[i] == ButtonState.Released && buttons[i] == ButtonState.Pressed)
                    {
                        mInput.AddButton(i, MouseActionEnum.Pressed);
                        // Set mousedown position for select rect
                        mLeftMouseDown.X = Mouse.GetState().X;
                        mLeftMouseDown.Y = Mouse.GetState().Y;
                        mInput.SetRectangle(0, 0, 0, 0);

                        // play click sound
                        mSounds.Add(SoundEnum.Click1);
                    }
                    else if (mPrevMouseButtonStates[i] == ButtonState.Pressed && buttons[i] == ButtonState.Released)
                    {
                        mInput.AddButton(i, MouseActionEnum.Released);
                    }
                    else if (mPrevMouseButtonStates[i] == ButtonState.Pressed && buttons[i] == ButtonState.Pressed)
                    {
                        mInput.AddButton(i, MouseActionEnum.Held);
                    }
                    else
                    {
                        mInput.AddButton(i, MouseActionEnum.Free);
                    }
                }
                var amt = Mouse.GetState().ScrollWheelValue;
                // check for int overflow
                if (mPrevMouseWheel - amt > 500000 || mPrevMouseWheel - amt < -500000)
                {
                    amt = mPrevMouseWheel;
                }
                mInput.SetScrollAmount(mPrevMouseWheel - amt);
                mPrevMouseWheel = amt;

                // Create selection rect 
                if (mPrevMouseButtonStates[0] == ButtonState.Pressed)
                {

                    var state = Mouse.GetState();
                    var selectedRectangle = new Rectangle();
                    var selectedCorner = new Point(state.X, state.Y);

                    if (selectedCorner.X > mLeftMouseDown.X)
                    {
                        selectedRectangle.X = mLeftMouseDown.X;
                    }
                    else
                    {
                        selectedRectangle.X = selectedCorner.X;
                    }

                    if (selectedCorner.Y > mLeftMouseDown.Y)
                    {
                        selectedRectangle.Y = mLeftMouseDown.Y;
                    }
                    else
                    {
                        selectedRectangle.Y = selectedCorner.Y;
                    }

                    selectedRectangle.Width = Math.Abs(mLeftMouseDown.X - selectedCorner.X);
                    selectedRectangle.Height = Math.Abs(mLeftMouseDown.Y - selectedCorner.Y);

                    mInput.SetRectangle(selectedRectangle.X,
                        selectedRectangle.Y,
                        selectedRectangle.Width,
                        selectedRectangle.Height);
                }
                else
                {
                    mInput.SetRectangle(0, 0, 0, 0);
                }

                mPrevMouseButtonStates = buttons;
            }
        }

        private bool PositiveFlank(Keys key)
        {
            var returnVal = true;
            foreach (var prevKey in mPrevKeys)
            {
                if (key == prevKey)
                {
                    returnVal = false;
                }
            }

            return returnVal;
        }

        SoundEnum[] IAudible.GetQueuedSound()
        {
            return mSounds.ToArray();
        }

        void IAudible.ResetSound()
        {
            mSounds.Clear();
        }

        bool IAudible.IsRemovable()
        {
            // is always there
            return false;
        }
        float IAudible.GetPan()
        {
            return 0f;
        }

    }
}
