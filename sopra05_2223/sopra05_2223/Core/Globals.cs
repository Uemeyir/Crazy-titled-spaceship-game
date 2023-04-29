using Microsoft.Xna.Framework;
using sopra05_2223.ScreenManagement;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.Xna.Framework.Input;
using sopra05_2223.Core.Entity.Ships;
using sopra05_2223.NotificationSystem;
using sopra05_2223.Pathfinding;
using sopra05_2223.Protagonists;
using sopra05_2223.SoundManagement;

namespace sopra05_2223.Core
{
    internal static class Globals
    {
        private static readonly string sGameFolder =
            System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CUSSG");

        public static string SaveGamePath(int i)
        {
            return System.IO.Path.Combine(sGameFolder, "SavedData", "Slot" + i, "GameScreen.json");
        }

        public static string SaveSettingsPath(string settingName)
        {
            return System.IO.Path.Combine(sGameFolder, "SavedData", "Settings", settingName + ".json");
        }

        public static readonly string sAchievementsPath = System.IO.Path.Combine(sGameFolder, "SavedData", "Achievements_and_Statistics", "Achievements.json");
        public static readonly string sStatisticsPath = System.IO.Path.Combine(sGameFolder, "SavedData", "Achievements_and_Statistics", "Statistics.json");

        public static GameTime GameTime
        {
            get;
            set;
        }

        public static ScreenManager ScreenManager
        {
            get;
            set;
        }

        public static SoundManager SoundManager
        {
            get;
            set;
        }

        // Set in HUD-Screen.
        public static NotificationManager NotificationManager
        {
            get;
            set;
        }

        public static readonly Dictionary<string, int> sLastNotifiedStats = new();

        public static GraphicsDeviceManager GraphicsDevice
        {
            get;
            set;
        }

        public static Resolution Resolution
        {
            get;
            set;
        }

        public static Camera Camera
        {
            get;
            set;
        }

        public static Player Player
        {
            get;
            set;
        }

        public static Grid mGridStandard;
        public static Grid mGridExtended;
        public static Grid mGridTechDemo;

        public static MoveEntities mMoveEntities;

        /* Unused
        public static int GameScreenTime
        {
            get;
            set;
        }
        */

        public static readonly Point sWorldSize = new (32000, 32000);

        public static int RandomNumber()
        {
            var r = RandomNumberGenerator.Create();
            byte[] data = new byte[16];
            r.GetBytes(data);
            return (ushort) BitConverter.ToInt16(data);
        }

        public static readonly List<Point> sResolutions = new()
        {
            new Point(1280, 960),
            new Point(1280, 720),
            new Point(1920, 1080),
            new Point(1920, 823),
            new Point(2560, 1440)
        };

       
        public static readonly Dictionary<Type, Point> sCosts = new()
        {
            // x: metal, y: oxygen
            { typeof(ETransport), new Point(300, 350) },
            { typeof(EAttacker),  new Point(100, 50)},
            { typeof(ESpy),  new Point(150, 250)},
            { typeof(EMedic),  new Point(230, 350)},
            { typeof(EMoerser),  new Point(180, 150)}
        };

        public static readonly Dictionary<string, Point> sStorage = new()
        {
            { "Attacker", new Point(200, 100) },
            { "Moerser", new Point(900, 900) },
            { "Medic", new Point(200, 100) },
            { "Spy", new Point(300, 500) },
            { "Transport", new Point(10000, 10000) },
        };

        public static readonly Dictionary<Type, int> sBuildTime = new()
        {
            { typeof(ETransport), 10 },
            { typeof(EAttacker), 5 },
            { typeof(ESpy), 3 },
            { typeof(EMedic), 7},
            { typeof(EMoerser), 12}
        };

        public static readonly Dictionary<string, int> sActionRadius = new()
        {
            { "Attacker", 1500 },
            { "Moerser", 2000 },
            { "Transport", 4000 },
            { "Medic", 2000},
            { "Base", 2000 }
        };

        public static readonly Dictionary<string, int> sViewRadius = new()
        {
            {"Transport", sActionRadius["Transport"]},
            {"Collector", 200},
            {"Attacker", (int)(sActionRadius["Attacker"] * 1.5)},
            {"Moerser", (int)(sActionRadius["Moerser"] * 1.5)},
            {"Spy", 4000},
            {"PlanetBase", 2000 },
            {"SpaceBase", 3000 },
            {"Buoy", 4000 },
            {"Medic", (int)(sActionRadius["Medic"] * 1.1)}
        };

        public static readonly Dictionary<string, double> sArmor = new()
        {
            { "Attacker", 0.10 },
            { "Moerser", 0.33 },
            { "Collector", 1.00 },
            { "Transporter", 0.33 },
            { "Spy", 0.66 },
            { "Medic", 0.10},
            { "PlanetBase", 0.80 },
            { "SpaceBase", 0.66 }
        };

        public static readonly Dictionary<string, int> sDestructionPower = new()
        {
            { "Attacker", 10 },
            { "Moerser", 40 },
            { "Medic", 10}

        };

        public static float mGameSpeed = 1f;

        // The following Dictionaries are persistent.
        // To add a new Item to a Dictionary, add it to the respective Default-Dictionary.

        public static Dictionary<string, int> mStatistics = new();
        public static Dictionary<string, int> mRunStatistics = new();

        public static readonly Dictionary<string, int> sStatisticsDefault = new()
        {
            { "RunningSeconds", 0 },
            { "BuiltAttack", 0 },
            { "BuiltMoerser", 0},
            { "BuiltSpy", 0 },
            { "BuiltTransport", 0 },
            { "BuiltMedic", 0 },
            { "DestroyedEnemyShips", 0 },
            { "LostShips", 0 },
            { "TotalResources", 0 },
            { "NaturalResources", 0 },
            { "ScrapResources", 0 },
            { "BasesTaken", 0 },
            { "FastestWin", 0 },
            { "FewestLosses", 0 },
            { "MostDestroyed", 0 },
            { "LeastDestroyed", 0 },
            { "MostLosses", 0 },
            { "TimesWon", 0 }
        };

        public static readonly Dictionary<string, int> sRunStatisticsDefault = new()
        {
            { "RunningSeconds", 0 },
            { "BuiltAttack", 0 },
            { "BuiltMoerser", 0},
            { "BuiltSpy", 0 },
            { "BuiltTransport", 0 },
            { "BuiltMedic", 0 },
            { "DestroyedEnemyShips", 0 },
            { "LostShips", 0 },
            { "TotalResources", 0 },
            { "NaturalResources", 0 },
            { "ScrapResources", 0 },
            { "BasesTaken", 0 }
        };

        public static Dictionary<string, bool> mAchievements = new();
        
        public static readonly Dictionary<string, bool> sAchievementsDefault = new()
        {
            {"EnemyShips1", false},
            {"EnemyShips2", false},
            {"EnemyShips3", false},
            {"EnemyShips4", false},
            {"EnemyShips5", false},
            {"Resource1", false},
            {"Resource2", false},
            {"Resource3", false},
            {"Resource4", false},
            {"Resource5", false},
            {"AllBases1", false},
            {"AllBases2", false},
            {"AllBases3", false},
            {"AllCorners", false},
            {"GameOver", false},
            {"OneShipType", false},
            {"5000EnemyShips", false},
            {"500EnemyShips", false},
            {"1000LostShips", false}
        };

        public static Dictionary<string, bool> mBuilt = new();

        public static readonly Dictionary<string, bool> sBuiltDefault = new()
        {
            { "Attacker", false },
            { "Spy", false },
            { "Moerser", false },
            { "Medic", false }
        };

        public static Dictionary<string, bool> mExplored = new();

        public static readonly Dictionary<string, bool> sExploredDefault = new()
        {
            { "TopLeft", false },
            { "TopRight", false },
            { "BottomLeft", false },
            { "BottomRight", false }
        };

        public static Dictionary<string, Keys> mKeys = new();

        internal static readonly Dictionary<string, Keys> sKeysDefault = new()
        {
            {"MoveResourcesOut", Keys.Up},
            {"MoveResourcesIn", Keys.Down},
            {"PrevShip", Keys.Left},
            {"NextShip", Keys.Right},
            {"MoveOxygen", Keys.O},
            {"MoveMetal", Keys.I},
            {"Mute", Keys.M},
            {"InputMode", Keys.Enter},
            {"NextText", Keys.Right},
            {"PrevText", Keys.Left},
            {"Pause", Keys.P},
            {"Help", Keys.H},
            {"ToggleMiniMap", Keys.K},
        };

        /* Unused
        public static Dictionary<string, int> Speed = new Dictionary<string, int>
        {
            {"Transport", 2000},
            {"Bomber", 1000}
        };

        public static Dictionary<string, int> mHealth = new Dictionary<string, int>
        {
            {"Transport", 2000},
            {"Bomber", 1000}
        };
        */

    }
}
