using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using sopra05_2223.Core.Components;
using sopra05_2223.Core;
using sopra05_2223.Core.Entity;
using sopra05_2223.InputSystem;
using sopra05_2223.ScreenManagement;

namespace sopra05_2223.Screens
{
    internal sealed class ResourceScreen : IScreen
    {
        private Point mScreenSize;
        private readonly Entity mEntity;
        private static readonly float sYOffset = 0.15f;
        private static readonly int sAmount = 50;
        public ResourceScreen(Entity entity, Point screenSize)
        {
            mScreenSize = screenSize;
            mEntity = entity;
        }
        bool IScreen.UpdateLower => true;
        bool IScreen.DrawLower => true;

        public ScreenManager ScreenManager { get; set; }

        void IScreen.Draw(SpriteBatch spriteBatch)
        {
            mEntity.GetComponent<CMoveResource>()?.Draw(spriteBatch, mScreenSize);
        }

        void IScreen.Update(Input input)
        {
            var mousepos = input.GetMousePosition();
            var mousetoworld = Globals.Camera.ScreenToWorld(new Vector2(mousepos.X, mousepos.Y));
            var x = mEntity.GetComponent<CMoveResource>();

            if (input.GetMouseButton(0) == MouseActionEnum.Released)
            {
                ShipBaseScreen shipbasescreen = (ShipBaseScreen)Globals.ScreenManager.GetScreen(typeof(ShipBaseScreen));
                PlanetBaseScreen planetbasescreen = (PlanetBaseScreen)Globals.ScreenManager.GetScreen(typeof(PlanetBaseScreen));
                
                if (shipbasescreen != null && mousepos.X <= mScreenSize.X * 0.4f)
                {
                    shipbasescreen.mLeftMouse = true;
                }
                if (mousepos.Y >= mScreenSize.Y * (0.9f - sYOffset) && mousepos.Y <= mScreenSize.Y * (0.9f- sYOffset) + mScreenSize.X * 0.04f && mousepos.X >= mScreenSize.X * 0.82f && mousepos.X <= mScreenSize.X * 0.86f)
                {
                    Load();
                }
                else if (mousepos.Y >= mScreenSize.Y * (0.9f - sYOffset) && mousepos.Y <= mScreenSize.Y * (0.9f- sYOffset) + mScreenSize.X * 0.04f && mousepos.X >= mScreenSize.X * 0.88f && mousepos.X <= mScreenSize.X * 0.92f)
                {
                    Unload();
                }
                else if (mousepos.Y >= mScreenSize.Y * (0.79f - sYOffset) && mousepos.Y <= mScreenSize.Y * (0.84f - sYOffset) && mousepos.X >= mScreenSize.X * 0.92f && mousepos.X <= mScreenSize.X * 0.97f)
                {
                    x.mSelectedType = false; // Metal
                }
                else if (mousepos.Y >= mScreenSize.Y * (0.74f - sYOffset) && mousepos.Y <= mScreenSize.Y * (0.79f - sYOffset) && mousepos.X >= mScreenSize.X * 0.92f && mousepos.X <= mScreenSize.X * 0.97f)
                {
                    x.mSelectedType = true; // Oxygen
                }
                else if (mousepos.Y >= mScreenSize.Y * (0.75f - sYOffset) && mousepos.Y <= mScreenSize.Y * (0.75f - sYOffset) + mScreenSize.X * 0.1f && mousepos.X >= mScreenSize.X * 0.62f && mousepos.X <= mScreenSize.X * 0.64f)
                {
                    x.ShowPrev();
                }
                else if (mousepos.Y >= mScreenSize.Y * (0.75f - sYOffset) && mousepos.Y <= mScreenSize.Y * (0.75f - sYOffset) + mScreenSize.X * 0.1f && mousepos.X >= mScreenSize.X * 0.76f && mousepos.X <= mScreenSize.X * 0.78f)
                {
                    x.ShowNext();
                }
                else if (mousepos.Y >= mScreenSize.Y * (0.95f - sYOffset) && mousepos.Y <= mScreenSize.Y * (0.95f - sYOffset) + mScreenSize.X * 0.025f && mousepos.X >= mScreenSize.X * 0.76f && mousepos.X <= mScreenSize.X * 0.785f)
                {
                    AddAmount();
                }
                else if (mousepos.Y >= mScreenSize.Y * (0.95f - sYOffset) && mousepos.Y <= mScreenSize.Y * (0.95f - sYOffset) + mScreenSize.X * 0.025f && mousepos.X >= mScreenSize.X * 0.62f && mousepos.X <= mScreenSize.X * 0.645f)
                {
                    RemoveAmount();
                }
                else if ((mousepos.X > 0.4f * mScreenSize.X && mousepos.X <= mScreenSize.X * 0.6 || mousepos.X > mScreenSize.X * 0.6f && mousepos.Y < mScreenSize.Y * (0.7f - sYOffset) || mousepos.X > mScreenSize.X * 0.6f && mousepos.Y >= mScreenSize.Y * (1.0f - sYOffset)) && !new Rectangle((int)mousetoworld.X, (int)mousetoworld.Y, 1, 1).Intersects(new Rectangle(mEntity.GetX(), mEntity.GetY(), mEntity.GetWidth(), mEntity.GetHeight())))
                {
                    CloseWindow();
                }
                else if (planetbasescreen != null && (mousepos.Y > mScreenSize.Y * (0.5f- sYOffset) || mousepos.Y < mScreenSize.Y * (0.2f - sYOffset)) && mousepos.X <= mScreenSize.X * 0.25f)
                {
                    CloseWindow();
                }
            }

            if (input.GetKeys().Contains(Keys.Escape))
            {
                input.GetKeys().Remove(Keys.Escape);
                CloseWindow();
            }

            if (x.mUserInput.Length < 5)
            { 
                UserInput(input);
            }
            
            if (input.GetKeys().Contains(Keys.Back) && x.mUserInput.Length > 0)
            {
                x.mUserInput = mEntity.GetComponent<CMoveResource>().mUserInput.Remove(x.mUserInput.Length - 1);
            }

            if (input.GetKeys().Contains(Globals.mKeys["InputMode"]))
            {
                x.mUserInput = "";
            }

            if (x.mUserInput != "")
            {
                x.SetMoveMetal(Int32.Parse(mEntity.GetComponent<CMoveResource>().mUserInput));
                x.SetMoveOxygen(Int32.Parse(mEntity.GetComponent<CMoveResource>().mUserInput));
            }

            if (input.GetKeys().Contains(Globals.mKeys["MoveResourcesIn"]))
            {
                Load();
            }
            else if (input.GetKeys().Contains(Globals.mKeys["MoveResourcesOut"]))
            {
                Unload();
            }

            if (input.GetKeys().Contains(Globals.mKeys["MoveMetal"]))
            {
                x.mSelectedType = false;
            }
            else if (input.GetKeys().Contains(Globals.mKeys["MoveOxygen"]))
            {
                x.mSelectedType = true;
            }

            if (input.GetKeys().Contains(Globals.mKeys["NextShip"]))
            {
                x.ShowNext();
            }
            else if (input.GetKeys().Contains(Globals.mKeys["PrevShip"]))
            {
                x.ShowPrev();
            }
        }

        private void Load()
        {
            mEntity.GetComponent<CMoveResource>().Load();
        }

        private void Unload()
        {
            mEntity.GetComponent<CMoveResource>().Unload();
        }
        private void AddAmount()
        {
            var x = mEntity.GetComponent<CMoveResource>();
            if (x.GetMoveMetal() + sAmount <= 99999)
            {
                x.SetMoveMetal(x.GetMoveMetal() + sAmount);
                x.SetMoveOxygen(x.GetMoveOxygen() + sAmount);
                x.mUserInput = (StringToInt(x.mUserInput) + sAmount).ToString();
            }
        }
        private void RemoveAmount()
        {
            var x = mEntity.GetComponent<CMoveResource>();
            if (x.GetMoveMetal() - sAmount >= 0)
            {
                x.SetMoveMetal(x.GetMoveMetal() - sAmount);
                x.SetMoveOxygen(x.GetMoveOxygen() - sAmount);
                x.mUserInput = (StringToInt(x.mUserInput) - sAmount).ToString();
            }
            else
            {
                mEntity.GetComponent<CMoveResource>().mUserInput = "";
            }
        }

        private int StringToInt(string str)
        {
            if (str == "")
            {
                return 0;
            }

            return int.Parse(mEntity.GetComponent<CMoveResource>().mUserInput);
        }
        private void CloseWindow()
        {
            mEntity.GetComponent<CMoveResource>().Clear();
            var x = mEntity.GetComponent<CSelect>();
            if (x != null)
            {
                x.mSelected = false;
            }
            ScreenManager.ReturnToGameScreen();
        }

        // Ich bekomme keinen funktionierenden Switch hin. Ich weiß, das sieht nicht gut aus. Wenn jemand einen funktionierenden Switch weiß, gerne ersetzen!
        private void UserInput(Input input)
        {
            var x = mEntity.GetComponent<CMoveResource>();
            if (input.GetKeys().Contains(Keys.D1) || input.GetKeys().Contains(Keys.NumPad1))
            {
                x.mUserInput += '1';
            }
            else if (input.GetKeys().Contains(Keys.D2) || input.GetKeys().Contains(Keys.NumPad2))
            {
                x.mUserInput += '2';
            }
            else if (input.GetKeys().Contains(Keys.D3) || input.GetKeys().Contains(Keys.NumPad3))
            {
                x.mUserInput += '3';
            }
            else if (input.GetKeys().Contains(Keys.D4) || input.GetKeys().Contains(Keys.NumPad4))
            {
                x.mUserInput += '4';
            }
            else if (input.GetKeys().Contains(Keys.D5) || input.GetKeys().Contains(Keys.NumPad5))
            {
                x.mUserInput += '5';
            }
            else if (input.GetKeys().Contains(Keys.D6) || input.GetKeys().Contains(Keys.NumPad6))
            {
                x.mUserInput += '6';
            }
            else if (input.GetKeys().Contains(Keys.D7) || input.GetKeys().Contains(Keys.NumPad7))
            {
                x.mUserInput += '7';
            }
            else if (input.GetKeys().Contains(Keys.D8) || input.GetKeys().Contains(Keys.NumPad8))
            {
                x.mUserInput += '8';
            }
            else if (input.GetKeys().Contains(Keys.D9) || input.GetKeys().Contains(Keys.NumPad9))
            {
                x.mUserInput += '9';
            }
            else if (input.GetKeys().Contains(Keys.D0) || input.GetKeys().Contains(Keys.NumPad0))
            {
                x.mUserInput += '0';
            }
        }

        void IScreen.Resize(Point newRes)
        {
            mScreenSize = newRes;
        }
    }
}
