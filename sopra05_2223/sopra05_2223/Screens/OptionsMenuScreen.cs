using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using sopra05_2223.Controls;
using sopra05_2223.Core;
using sopra05_2223.InputSystem;
using sopra05_2223.ScreenManagement;
using sopra05_2223.Serializer;
using sopra05_2223.SoundManagement;


namespace sopra05_2223.Screens
{
    internal sealed class OptionsMenuScreen : IScreen
    {
        private List<Button> mComponents;
        private readonly GameTime mGameTime = Globals.GameTime;
        private readonly SoundManager mSoundManager;
        private int mSelectedRes;
        private Point mScreenSize;

        public ScreenManager ScreenManager { get; set; }
        bool IScreen.UpdateLower => true;

        bool IScreen.DrawLower => true;

        public OptionsMenuScreen(int screenWidth, int screenHeight, SoundManager soundManager)
        {
            mScreenSize = new Point(screenWidth, screenHeight);
            mSoundManager = soundManager;

            // Set the selected Resolution to the one the Game is currently running in.
            var idx = Globals.sResolutions.FindIndex(x => x == Globals.Resolution.mScreenSize);
            // Defaults to 1280x720 if the loaded Resolution can't be found.
            mSelectedRes = idx == -1 ? 1 : idx;

            CalculateButtons(mScreenSize);
        }

        private void CalculateButtons(Point screenSize)
        {
            var buttonLeftSide = mScreenSize.X / 2 - 100;
            var buttonMinus = buttonLeftSide - 73;
            var buttonPlus = buttonLeftSide + 280;
            var smallButtonHeight = Art.MenuButtonTexture.Height - 208;

            var position = new Vector2(buttonLeftSide, screenSize.Y / 2 + 100);
            String text = "Auflösung anwenden";
            var apply = new Button()
            {
                Position = position,
                Text = text
            };
            apply.Click += Apply_Click;

            position = new Vector2(buttonLeftSide, screenSize.Y / 2 + 50);
            text = $"{Globals.sResolutions[mSelectedRes].X} x {Globals.sResolutions[mSelectedRes].Y}";
            var res = new Button(false)
            {
                Position = position,
                Text = text
            };
            res.Click += res_Click;

            position = new Vector2(buttonPlus, screenSize.Y / 2 + 50);
            text = ">";
            var nextRes = new Button((int)position.X, (int)position.Y, smallButtonHeight, smallButtonHeight, text);
            nextRes.Click += NextRes_Click;

            position = new Vector2(buttonMinus, screenSize.Y / 2 + 50);
            text = "<";
            var prevRes = new Button((int)position.X, (int)position.Y, smallButtonHeight, smallButtonHeight, text);
            prevRes.Click += PrevRes_Click;

            var fullScreen = new Button()
            {
                Position = new Vector2(buttonLeftSide, screenSize.Y / 2 - 100),
                Text = "Vollbild"
            };
            fullScreen.Click += FullScreen_Click;

            position = new Vector2(buttonMinus, screenSize.Y / 2 - 50);
            text = "-";
            var lowerMusicVol = new Button((int)position.X, (int)position.Y, smallButtonHeight, smallButtonHeight, text);
            lowerMusicVol.Click += LowerMusicVol_Click;

            position = new Vector2(buttonPlus, screenSize.Y / 2 - 50);
            text = "+";
            var addMusicVol = new Button((int)position.X, (int)position.Y, smallButtonHeight, smallButtonHeight, text);
            addMusicVol.Click += AddMusicVol_Click;

            position = new Vector2(buttonMinus, screenSize.Y * 0.5f);
            text = "-";
            var lowerSoundVol = new Button((int)position.X, (int)position.Y, smallButtonHeight, smallButtonHeight, text);
            lowerSoundVol.Click += LowerSoundVol_Click;

            position = new Vector2(buttonPlus, screenSize.Y * 0.5f);
            text = "+";
            var addSoundVol = new Button((int)position.X, (int)position.Y, smallButtonHeight, smallButtonHeight, text);
            addSoundVol.Click += AddSoundVol_Click;

            var controls = new Button()
            {
                Position = new Vector2(buttonLeftSide, screenSize.Y / 2 - 200),
                Text = "Hotkeys anpassen"
            };
            controls.Click += Controls_Click;


            if (!Globals.Resolution.mIsBorderless)
            {
                text = ": an";
            }
            else
            {
                text = ": aus";
            }

            var borderless = new Button()
            {
                Position = new Vector2(buttonLeftSide, screenSize.Y / 2 - 150),
                Text = "Borderless" + text
            };
            borderless.Click += Borderless_Click;

            var volumeMusic = new Button(false)
            {
                Position = new Vector2(buttonLeftSide, screenSize.Y / 2 - 50),
                Text = $"Musik: {Math.Round(mSoundManager.GetMusicVolume() * 100)}%"
            };
            volumeMusic.Click += VolumeMusic_Click;

            var volumeSounds = new Button(false)
            {
                Position = new Vector2(buttonLeftSide, screenSize.Y * 0.5f),
                Text = $"Effekte: {Math.Round(mSoundManager.GetSoundVolume() * 100)}%"
            };
            volumeSounds.Click += VolumeSounds_Click;

            var backToPreviousScreen = new Button()
            {
                Position = new Vector2(buttonLeftSide, screenSize.Y / 2 + 200),
                Text = "Zurück"
            };
            // For the functionality
            backToPreviousScreen.Click += BackToPreviousScreen_Click;


            // store button and stats to draw it
            mComponents = new List<Button>()
            {
                backToPreviousScreen,
                fullScreen,
                volumeMusic,
                volumeSounds,
                addMusicVol,
                lowerMusicVol,
                addSoundVol,
                lowerSoundVol,
                apply,
                res,
                prevRes,
                nextRes,
                controls,
                borderless
            };
        }
        private void BackToPreviousScreen_Click(object sender, EventArgs e)
        {
            // Remove 2 Screens from Screen stack (StatisticScreen and MenuBackgroundScreen).
            ScreenManager.RemoveScreens();
            ScreenManager.RemoveScreens();
        }

        private void FullScreen_Click(object sender, EventArgs e)
        {
            Globals.Resolution.FullscreenCall();
            SopraSerializer.SerializeSettings();
        }

        private void Borderless_Click(object sender, EventArgs e)
        {
            Globals.Resolution.ToggleBorderless(); 
            string text;
            if (!Globals.Resolution.mIsBorderless)
            {
                text = ": an";
            }
            else
            {
                text = ": aus";
            }
            mComponents[13].Text = "Borderless" + text;
            SopraSerializer.SerializeSettings();
        }

        private void res_Click(object sender, EventArgs e)
        {

        }

        int Mod(int x, int m)
        {
            return (x % m + m) % m;
        }

        private void PrevRes_Click(object sender, EventArgs e)
        {
            mSelectedRes -= 1;
            mSelectedRes = Mod(mSelectedRes, Globals.sResolutions.Count);
            mComponents[9].Text = $"{Globals.sResolutions[mSelectedRes].X} x {Globals.sResolutions[mSelectedRes].Y}";
        }

        private void NextRes_Click(object sender, EventArgs e)
        {
            mSelectedRes += 1;
            mSelectedRes = Mod(mSelectedRes, Globals.sResolutions.Count);
            mComponents[9].Text = $"{Globals.sResolutions[mSelectedRes].X} x {Globals.sResolutions[mSelectedRes].Y}";
        }

        private void Controls_Click(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new MenuBackgroundScreen(mScreenSize));
            ScreenManager.AddScreen(new InputOptionsScreen(mScreenSize));
        }
        private void Apply_Click(object sender, EventArgs e)
        {
            Globals.Resolution.UseResolution(false, Globals.sResolutions[mSelectedRes]);
            Globals.Resolution.ResizeScreenManager();
            SopraSerializer.SerializeSettings();
        }
        private void AddMusicVol_Click(object sender, EventArgs e)
        {
            if (mSoundManager != null)
            {
                // If game is muted, unmute it now (usability!)
                if (mSoundManager.mSoundMuted)
                {
                    mSoundManager.mSoundMuted = false;
                }
            }
            mSoundManager?.SetMusicVolume(mSoundManager.GetMusicVolume() + 0.1f);
            if (mSoundManager != null)
            {
                mComponents[2].Text = $"Musik: {Math.Round(mSoundManager.GetMusicVolume() * 100)}%";
            }
            // Save the changes.
            SopraSerializer.SerializeSettings();
        }

        private void LowerMusicVol_Click(object sender, EventArgs e)
        {
            if (mSoundManager != null)
            {
                // If game is muted, unmute it now (usability!)
                if (mSoundManager.mSoundMuted)
                {
                    mSoundManager.mSoundMuted = false;
                }
            }
            mSoundManager?.SetMusicVolume(mSoundManager.GetMusicVolume() - 0.1f);
            if (mSoundManager != null)
            {
                mComponents[2].Text = $"Musik: {Math.Round(mSoundManager.GetMusicVolume() * 100)}%";
            }
            SopraSerializer.SerializeSettings();
        }
        private void AddSoundVol_Click(object sender, EventArgs e)
        {
            if (mSoundManager != null)
            {
                // If game is muted, unmute it now (usability!)
                if (mSoundManager.mSoundMuted)
                {
                    mSoundManager.mSoundMuted = false;
                }
            }
            mSoundManager?.SetSoundVolume(mSoundManager.GetSoundVolume() + 0.1f);
            if (mSoundManager != null)
            {
                mComponents[3].Text = $"Effekte: {Math.Round(mSoundManager.GetSoundVolume() * 100)}%";
            }
            // Save the changes.
            SopraSerializer.SerializeSettings();
        }
        private void LowerSoundVol_Click(object sender, EventArgs e)
        {
            if (mSoundManager != null)
            {
                // If game is muted, unmute it now (usability!)
                if (mSoundManager.mSoundMuted)
                {
                    mSoundManager.mSoundMuted = false;
                }
            }
            mSoundManager?.SetSoundVolume(mSoundManager.GetSoundVolume() - 0.1f);
            if (mSoundManager != null)
            {
                mComponents[3].Text = $"Effekte: {Math.Round(mSoundManager.GetSoundVolume() * 100)}%";
            }
            SopraSerializer.SerializeSettings();
        }
        private void VolumeMusic_Click(object sender, EventArgs e)
        {
        }
        private void VolumeSounds_Click(object sender, EventArgs e)
        {
        }

        void IScreen.Resize(Point newSize)
        {
            mScreenSize = newSize;
            CalculateButtons(newSize);
        }
        void IScreen.Update(Input input)
        {
            if (input.GetKeys().Contains(Keys.Escape))
            {
                ScreenManager.RemoveScreens();
                ScreenManager.RemoveScreens();
            }

            foreach (var component in mComponents)
            {
                // Update all Buttons.
                component.Update(mGameTime);
            }
        }

        void IScreen.Draw(SpriteBatch spriteBatch)
        {
            // Draw everything in component (all Buttons).
            foreach (var component in mComponents)
            {
                component.Draw(mGameTime, spriteBatch);
            }
        }
    }
}
