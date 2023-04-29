using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using sopra05_2223.Controls;
using sopra05_2223.Core;
using sopra05_2223.InputSystem;
using sopra05_2223.ScreenManagement;
using System;
using System.Collections.Generic;
using sopra05_2223.Serializer;

namespace sopra05_2223.Screens
{
    internal sealed class InputOptionsScreen : IScreen
    {
        private Point mScreenSize;
        // Buttons
        private List<Button> mComponents;
        private readonly GameTime mGameTime = Globals.GameTime;
        // Key that will overwrite previous Key
        private Keys mNewButton;
        // Selected Control Option that wants to be changed
        private int mSelected = -1;
        // Used to check if Button is selected, doesn't allow more than one button selected at a time
        private bool mChanged;

        // Defines Local Key Sets. Used to let Keys be assigned more than once (if Controls are not overlapping)
        private readonly List<int> mKeySet1 = new() { 0,1,2,3,4,5,6,7,8,9,10 }; 
        private readonly List<int> mKeySet2 = new() { 5,6 };
        public InputOptionsScreen(Point screenSize)
        {
            mScreenSize = screenSize;
            CalculateButtons(screenSize);
        }

        public ScreenManager ScreenManager { get; set; }
        bool IScreen.UpdateLower => true;
        bool IScreen.DrawLower => true;

        void IScreen.Draw(SpriteBatch spriteBatch)
        {
            // Draw everything in component (all Buttons).
            foreach (var component in mComponents)
            {
                component.Draw(mGameTime, spriteBatch);
            }
        }

        // Used to Display the Standard Text on Buttons
        private string GetText(int key)
        {
            List<string> defaultText = new()
            {
                $"Basis: Ressourcen importieren {Globals.mKeys["MoveResourcesOut"]}",
                $"Basis: Ressourcen exportieren   {Globals.mKeys["MoveResourcesIn"]}",
                $"Basenmenü: Vorheriges Schiff   {Globals.mKeys["PrevShip"]}",
                $"Basenmenü: Nächstes Schiff   {Globals.mKeys["NextShip"]}",
                $"Sound aus/an   {Globals.mKeys["Mute"]}",
                $"Basenmenü: Metall übertragen   {Globals.mKeys["MoveMetal"]}",
                $"Basenmenü: Sauerstoff übertragen  {Globals.mKeys["MoveOxygen"]}",
                $"Pausemenü   {Globals.mKeys["Pause"]}",
                $"Basenmenü: Ressourcenanzahl {Globals.mKeys["InputMode"]}",
                $"Spickzettel zeigen: {Globals.mKeys["Help"]}",
                $"Mininmap ausblenden: {Globals.mKeys["ToggleMiniMap"]}"
            };
            return defaultText[key];
        }

        // Returns currently used Key of Option 
        private Keys GetKey(int key)
        {
            List<Keys> keys = new()
            {
                Globals.mKeys["MoveResourcesOut"],
                Globals.mKeys["MoveResourcesIn"],
                Globals.mKeys["PrevShip"],
                Globals.mKeys["NextShip"],
                Globals.mKeys["Mute"],
                Globals.mKeys["MoveMetal"],
                Globals.mKeys["MoveOxygen"],
                Globals.mKeys["Pause"],
                Globals.mKeys["InputMode"],
                Globals.mKeys["Help"],
                Globals.mKeys["ToggleMiniMap"]
            };
            return keys[key];
        }

        private void CalculateButtons(Point screenSize)
        {
            var buttonLeftSide = mScreenSize.X * 0.25f;
            var buttonLeftSide2 = mScreenSize.X * 0.5f;
            var verticalSpace = 50;


            var resourcesUp = new Button()
            {
                Position = new Vector2(buttonLeftSide2, screenSize.Y / 4f),
                Text = GetText(0)
            };
            resourcesUp.Click += ResourcesUp_Click;

            var resourcesDown = new Button()
            {
                Position = new Vector2(buttonLeftSide2, screenSize.Y / 4f + verticalSpace),
                Text = GetText(1)
            };
            resourcesDown.Click += ResourcesDown_Click;

            var nextShip = new Button()
            {
                Position = new Vector2(buttonLeftSide2, screenSize.Y / 4f + 2 * verticalSpace),
                Text = GetText(3)
            };
            nextShip.Click += NextShip_Click;

            var prevShip = new Button()
            {
                Position = new Vector2(buttonLeftSide2, screenSize.Y / 4f + 3 * verticalSpace),
                Text = GetText(2)
            };
            prevShip.Click += PrevShip_Click;

            var moveOxy = new Button()
            {
                Position = new Vector2(buttonLeftSide2, screenSize.Y / 4f + 4 * verticalSpace),
                Text = GetText(6)
            };
            moveOxy.Click += Oxygen_Click;

            var moveMetal = new Button()
            {
                Position = new Vector2(buttonLeftSide2, screenSize.Y / 4f + 5 * verticalSpace),
                Text = GetText(5)
            };
            moveMetal.Click += Metal_Click;

            var inputMode = new Button()
            {
                Position = new Vector2(buttonLeftSide2, screenSize.Y / 4f + 6 * verticalSpace),
                Text = GetText(8)
            };
            inputMode.Click += InputMode_Click;

            var helpButton = new Button()
            {
                Position = new Vector2(buttonLeftSide, screenSize.Y / 4f),
                Text = GetText(9)
            };
            helpButton.Click += HelpButton_Click;

            var mute = new Button()
            {
                Position = new Vector2(buttonLeftSide, screenSize.Y / 4f + verticalSpace),
                Text = GetText(4)
            };
            mute.Click += Mute_Click;

            var pause = new Button()
            {
                Position = new Vector2(buttonLeftSide, screenSize.Y / 4f + 2 * verticalSpace),
                Text = GetText(7)
            };
            pause.Click += Pause_Click;

            var minimapButton = new Button()
            {
                Position = new Vector2(buttonLeftSide, screenSize.Y / 4f + 3 * verticalSpace),
                Text = GetText(10)
            };
            minimapButton.Click += MinimapButton_Click;

            var back = new Button()
            {
                Position = new Vector2(screenSize.X * 0.5f - 100, screenSize.Y * 0.8f),
                Text = "Zurück"
            };
            back.Click += Back_Click;

            var reset = new Button()
            {
                Position = new Vector2(buttonLeftSide, screenSize.Y / 4f + 6 * verticalSpace),
                Text = "Auf Standardeinstellungen zurücksetzen"
            };
            reset.Click += Reset_Click;

            mComponents = new List<Button>()
            {
                resourcesUp,
                resourcesDown,
                prevShip,
                nextShip,
                mute,
                moveMetal,
                moveOxy,
                pause,
                inputMode,
                helpButton,
                minimapButton,
                back,
                reset
            };
        }

        private void MinimapButton_Click(object sender, EventArgs e)
        {
            if (!mChanged)
            {
                mSelected = 10;
                mComponents[10].Text = "< >";
                mChanged = true;
            }
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            if (!mChanged)
            {
                mSelected = 9;
                mComponents[9].Text = "< >";
                mChanged = true;
            }
        }

        private void ResourcesUp_Click(object sender, EventArgs e)
        {
            if (!mChanged)
            {
                mSelected = 0;
                mComponents[0].Text = "< >";
                mChanged = true;
            }
        }

        private void ResourcesDown_Click(object sender, EventArgs e)
        {
            if (!mChanged)
            {
                mSelected = 1;
                mComponents[1].Text = "< >";
                mChanged = true;
            }
        }

        private void Mute_Click(object sender, EventArgs e)
        {
            if (!mChanged)
            {
                mSelected = 4;
                mComponents[4].Text = "< >";
                mChanged = true;
            }
        }

        private void PrevShip_Click(object sender, EventArgs e)
        {
            if (!mChanged)
            {
                mSelected = 2;
                mComponents[2].Text = "< >";
                mChanged = true;
            }
        }

        private void NextShip_Click(object sender, EventArgs e)
        {
            if (!mChanged)
            {
                mSelected = 3;
                mComponents[3].Text = "< >";
                mChanged = true;
            }
        }
        

        private void Pause_Click(object sender, EventArgs e)
        {
            if (!mChanged)
            {
                mSelected = 7;
                mComponents[7].Text = "< >";
                mChanged = true;
            }
        }
        private void Reset_Click(object sender, EventArgs e)
        {
            // Set Keybindings to default
            Globals.mKeys = Globals.sKeysDefault;
            CalculateButtons(mScreenSize);
            SopraSerializer.Serialize(Globals.SaveSettingsPath("Keybindings"), Globals.mKeys, typeof(Dictionary<string, int>));
        }


        private void InputMode_Click(object sender, EventArgs e)
        {
            if (!mChanged)
            {
                mSelected = 8;
                mComponents[8].Text = "< >";
                mChanged = true;
            }
        }
        private void Oxygen_Click(object sender, EventArgs e)
        {
            if (!mChanged)
            {
                mSelected = 6;
                mComponents[6].Text = "< >";
                mChanged = true;
            }
        }
        private void Metal_Click(object sender, EventArgs e)
        {
            if (!mChanged)
            {
                mSelected = 5;
                mComponents[5].Text = "< >";
                mChanged = true;
            }
        }
        private void Back_Click(object sender, EventArgs e)
        {
            CloseScreen();
        }

        // Looks up correct entry in Globals and changes to new key 
        private void ChangeButton(Keys key)
        {
            if (key == Keys.None)
            {
                return;
            }
            switch (mSelected)
            {
                case 0:
                    Globals.mKeys["MoveResourcesOut"] = mNewButton;
                    mComponents[0].Text = GetText(0);
                    break;
                case 1:
                    Globals.mKeys["MoveResourcesIn"] = mNewButton;
                    mComponents[1].Text = GetText(1);
                    break;
                case 2:
                    Globals.mKeys["PrevShip"] = mNewButton;
                    mComponents[2].Text = GetText(2);
                    break;
                case 3:
                    Globals.mKeys["NextShip"] = mNewButton;
                    mComponents[3].Text = GetText(3);
                    break;
                case 4:
                    Globals.mKeys["Mute"] = mNewButton;
                    mComponents[4].Text = GetText(4);
                    break;
                case 5:
                    Globals.mKeys["MoveMetal"] = mNewButton;
                    mComponents[5].Text = GetText(5);
                    break;
                case 6:
                    Globals.mKeys["MoveOxygen"] = mNewButton;
                    mComponents[6].Text = GetText(6);
                    break;
                case 7:
                    Globals.mKeys["Pause"] = mNewButton;
                    mComponents[7].Text = GetText(7);
                    break;
                case 8:
                    Globals.mKeys["InputMode"] = mNewButton;
                    mComponents[8].Text = GetText(8);
                    break;
                case 9:
                    Globals.mKeys["Help"] = mNewButton;
                    mComponents[9].Text = GetText(9);
                    break;
                case 10:
                    Globals.mKeys["ToggleMiniMap"] = mNewButton;
                    mComponents[10].Text = GetText(10);
                    break;
            }
            mChanged = false;
            mNewButton = Keys.None;
            mSelected = -1;

            // Save changed Keys.
            SopraSerializer.Serialize(Globals.SaveSettingsPath("Keybindings"), Globals.mKeys, typeof(Dictionary<string, Keys>));
        }

        // Stops current Change Process. Restores Text, sets to start configuration
        private void StopChange(int key)
        {
            mComponents[key].Text = GetText(key);
            mChanged = false;
            mNewButton = Keys.None;
            mSelected = -1;
        }

        private void CloseScreen()
        {
            ScreenManager.RemoveScreens(2);
        }
        void IScreen.Update(Input input)
        {
            var keys = input.GetKeys();
            if (keys.Count != 0 && mSelected != -1)
            {
                mNewButton = keys[0];

                if (keys.Contains(Keys.Escape))
                {
                    StopChange(mSelected);
                    keys.Remove(Keys.Escape);
                }

                // Checks Local Control Sets for which controls can be used multiple times and which don't 
                var stage = true;
                if (mKeySet1.Contains(mSelected))
                {
                    foreach (var k in mKeySet1)
                    {
                        if (GetKey(k) == mNewButton && k != mSelected)
                        {
                            stage = false;
                            break;
                        }
                    }
                }
                else if (mKeySet2.Contains(mSelected))
                {
                    foreach (var k in mKeySet2)
                    {
                        if (GetKey(k) == mNewButton && k != mSelected)
                        {
                            stage = false;
                        }
                    }
                }
                else
                {
                    if (Globals.mKeys.ContainsValue(mNewButton))
                    {
                        stage = false;
                    }
                }
                if (stage)
                {
                    ChangeButton(mNewButton);
                }
                // Local Set check end

            }
            if (keys.Contains(Keys.Escape))
            {
                CloseScreen();
            }
            foreach (var component in mComponents)
            {
                // Update all Buttons.
                component.Update(mGameTime);
            }
        }

        void IScreen.Resize(Point newres)
        {
            mScreenSize = newres;
            CalculateButtons(newres);
        }
    }
}
