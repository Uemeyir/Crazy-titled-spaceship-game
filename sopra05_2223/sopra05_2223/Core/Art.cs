using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using sopra05_2223.Core.Animation;

namespace sopra05_2223.Core;

internal static class Art
{
    public static AnimationManager AnimationManager
    {
        get;
        private set;
    }
    public static Texture2D Bomber
    {
        get; private set;
    }
    public static Texture2D Spy
    {
        get; private set;
    }
    public static Texture2D Buoy
    {
        get; private set;
    }
    public static Texture2D Transport
    {
        get; private set;
    }

    public static Texture2D Sammler
    {
        get; private set;
    }

    public static Texture2D Moerser
    {
        get; private set;
    }

    public static Texture2D Medic
    {
        get; private set;
    }

    public static Texture2D EnemyBomber
    {
        get; private set;
    }
    public static Texture2D EnemyTransport
    {
        get; private set;
    }

    public static Texture2D EnemySammler
    {
        get; private set;
    }
    public static Texture2D EnemySpy
    {
        get; private set;
    }
    public static Texture2D EnemyBuoy
    {
        get; private set;
    }

    public static Texture2D EnemyMoerser
    {
        get; private set;
    }

    public static Texture2D EnemyMedic
    {
        get; private set;
    }


    public static Texture2D ActionMenuBomber
    {
        get; private set;
    }
    public static Texture2D ActionMenuSpy
    {
        get; private set;
    }
    public static Texture2D ActionMenuTransport
    {
        get; private set;
    }

    public static Texture2D ActionMenuMoerser
    {
        get; private set;
    }

    public static Texture2D ActionMenuMedic
    {
        get; private set;
    }

    public static Texture2D MiniMapDot
    {
        get; private set;
    }

    public static Texture2D MiniMapPlus
    {
        get; private set;
    }
    public static Texture2D Parallax
    {
        get; private set;
    }
    public static Texture2D Projectile
    {
        get; private set;
    }

    public static Texture2D MedicProjectile
    {
        get; private set;
    }
    public static Texture2D Healthbar
    {
        get; private set;
    }

    public static Texture2D SpaceBasePlayer
    {
        get; private set;
    }

    public static Texture2D SpaceBaseKi
    {
        get; private set;
    }

    public static Texture2D SpaceBaseNeutral
    {
        get; private set;
    }

    public static readonly Texture2D[] sPlanets = new Texture2D[12];

    public static Texture2D Planetbase1Ki
    {
        get; private set;
    }

    public static Texture2D Planetbase1Neutral
    {
        get; private set;
    }

    public static Texture2D Planetbase1Player
    {
        get; private set;
    }

    public static Texture2D LightMask
    {
        get;
        private set;
    }

    public static Effect Glow
    {
        get; private set;
    }
    public static Effect Health
    {
        get; private set;
    }

    public static Effect OxygenEffect
    {
        get; private set;
    }

    public static Effect MetalEffect
    {
        get; private set;
    }
    public static Effect BuildShipEffect
    {
        get; private set;
    }

    public static Effect LightingEffect
    {
        get; private set;
    }
    public static SpriteFont MenuButtonFont
    {
        get; private set;
    }

    public static Texture2D MenuButtonTexture
    {
        get; private set;
    }

    public static SpriteFont Arial22
    {
        get; private set;
    }
    public static SpriteFont Arial12
    {
        get; private set;
    }

    public static Texture2D OptionsMenu
    {
        get; private set;
    }

    public static Texture2D Sauerstoffflasche
    {
        get; private set;
    }
    public static Texture2D Metall
    {
        get; private set;
    }
    public static Texture2D Ausrufezeichen
    {
        get; private set;
    }
    public static Texture2D Explosion1
    {
        get; private set;
    }
    public static Texture2D Explosion2
    {
        get; private set;
    }
    public static Texture2D PlanetAnimated1
    {
        get; private set;
    }
    public static Texture2D PurpleNebula
    {
        get; private set;
    }
    public static Texture2D MenuBackground1
    {
        get; private set;
    }

    public static Texture2D GameOverBackground
    {
        get; private set;
    }

    public static Texture2D GameWonBackground
    {
        get; private set;
    }

    public static Texture2D ShipsInfoScreenBackground
    {
        get; private set;
    }

    public static Texture2D ScreenplayScreenBackground
    {
        get; private set;
    }

    public static Texture2D ControlsHelpScreenBackground
    {
        get; private set;
    }

    public static Texture2D Title
    {
        get; private set;
    }
    public static Texture2D Resource1
    {
        get; private set;
    }
    public static Texture2D ShipBaseBg
    {
        get; private set;
    }

    public static Texture2D Cursor
    {
        get; private set;
    }
    public static Texture2D Circle
    {
        get; private set;
    }

    public static Texture2D TakeBaseButton
    {
        get; private set;
    }

    public static Texture2D AnimatedArrow
    {
        get; private set;
    }

    public static Texture2D LoadButton
    {
        get; private set;
    }
    public static Texture2D UnloadButton
    {
        get; private set;
    }
    public static Texture2D MinusButton
    {
        get; private set;
    }
    public static Texture2D PlusButton
    {
        get; private set;
    }
    public static Texture2D PrevStorage
    {
        get; private set;
    }
    public static Texture2D Achievement
    {
        get; private set;
    }
    public static Texture2D AchievementGet
    {
        get; private set;
    }

    public static Texture2D MiniMapX
    {
        get; private set;
    }

    public static Texture2D MiniMapSquare
    {
        get; private set;
    }

    public static Texture2D MiniMapBg
    {
        get; private set;
    }

    public static void Load(ContentManager content, GraphicsDevice graphicsDevice)
    {
        AnimationManager = new AnimationManager();

        Bomber = content.Load<Texture2D>("00Bomber2");
        EnemyBomber = content.Load<Texture2D>("01Bomber2");
        Transport = content.Load<Texture2D>("TransportschiffPlayer");
        Sammler = content.Load<Texture2D>("SammlerPlayer");
        Buoy = content.Load<Texture2D>("sQfoJee");
        Moerser = content.Load<Texture2D>("00Moerser1");
        Medic = content.Load<Texture2D>("00Medic1");

        EnemyTransport = content.Load<Texture2D>("TransportschiffKi");
        EnemySammler = content.Load<Texture2D>("SammlerKi");
        Spy = content.Load<Texture2D>("00Spionage1");
        EnemySpy = content.Load<Texture2D>("01Spionage1");
        EnemyBuoy = content.Load<Texture2D>("sQfoJee");
        EnemyMoerser = content.Load<Texture2D>("01Moerser1");
        EnemyMedic = content.Load<Texture2D>("01Medic1");


        ActionMenuBomber = content.Load<Texture2D>("sAttacker");
        ActionMenuTransport = content.Load<Texture2D>("sTransport");
        ActionMenuSpy = content.Load<Texture2D>("sSpy");
        ActionMenuMoerser = content.Load<Texture2D>("sMoerser");
        ActionMenuMedic = content.Load<Texture2D>("sMedic");

        MiniMapDot = content.Load<Texture2D>("minimap_dot");
        MiniMapPlus = content.Load<Texture2D>("MiniMap/plus-16");
        MiniMapX = content.Load<Texture2D>("MiniMap/x-mark-16");
        MiniMapSquare = content.Load<Texture2D>("MiniMap/square-16");
        MiniMapBg = content.Load<Texture2D>("MiniMap/exp");

        Parallax = content.Load<Texture2D>("stars_parallax");
        Projectile = content.Load<Texture2D>("Particle/Projectile");
        Healthbar = content.Load<Texture2D>("healthbar");
        MedicProjectile = content.Load<Texture2D>("Particle/HealingBullet");

        MenuButtonFont = content.Load<SpriteFont>("Fonts/Font");
        MenuButtonTexture = content.Load<Texture2D>("Controls/button3");

        SpaceBasePlayer = content.Load<Texture2D>("SpaceBases/playerSpaceBase01_3");
        SpaceBaseKi = content.Load<Texture2D>("SpaceBases/kiSpaceBase01_3");
        SpaceBaseNeutral = content.Load<Texture2D>("SpaceBases/spaceBase01_3");

        // Neutral planets
        sPlanets[0] = content.Load<Texture2D>("Planets/planet1");
        sPlanets[1]= content.Load<Texture2D>("Planets/planet2");
        sPlanets[2]= content.Load<Texture2D>("Planets/planet3");
        sPlanets[3] = content.Load<Texture2D>("Planets/planet4");
        sPlanets[4] = content.Load<Texture2D>("Planets/planet5");
        sPlanets[5] = content.Load<Texture2D>("Planets/planet6");
        sPlanets[6] = content.Load<Texture2D>("Planets/planet7");
        sPlanets[7] = content.Load<Texture2D>("Planets/planet8");
        sPlanets[8] = content.Load<Texture2D>("Planets/planet9");
        sPlanets[9] = content.Load<Texture2D>("Planets/planet11");
        sPlanets[10] = content.Load<Texture2D>("Planets/planet12");
        sPlanets[11] = content.Load<Texture2D>("Planets/planet13");

        Arial22 = content.Load<SpriteFont>("Fonts/Arial22");
        Arial12 = content.Load<SpriteFont>("Fonts/Arial12");

        OptionsMenu = content.Load<Texture2D>("actionMenu");

        Sauerstoffflasche = content.Load<Texture2D>("Sauerstoffflasche");
        Metall = content.Load<Texture2D>("Metall");

        Ausrufezeichen = content.Load<Texture2D>("Ausrufezeichen");

        Explosion1 = content.Load<Texture2D>("Explosion1");
        Explosion2 = content.Load<Texture2D>("explosion2");
        PlanetAnimated1 = content.Load<Texture2D>("AnimatedPlanet1");
        AnimatedArrow = content.Load<Texture2D>("Arrow");

        MenuBackground1 = content.Load<Texture2D>("Backgrounds/menubackground1");

        ShipsInfoScreenBackground = content.Load<Texture2D>("Backgrounds/shipsInfoScreen");
        ScreenplayScreenBackground = content.Load<Texture2D>("Backgrounds/screenplayScreen");
        ControlsHelpScreenBackground = content.Load<Texture2D>("Backgrounds/controlsHelpScreen");

        GameOverBackground = content.Load<Texture2D>("Backgrounds/GameOverBackground");
        GameWonBackground = content.Load<Texture2D>("Backgrounds/GameWonBackground");

        PurpleNebula = content.Load<Texture2D>("TempF/exp");

        Planetbase1Ki = content.Load<Texture2D>("Planetbases/pb03");
        Planetbase1Neutral = content.Load<Texture2D>("Planetbases/neutralpb03");
        Planetbase1Player = content.Load<Texture2D>("Planetbases/playerpb03");

        Resource1 = content.Load<Texture2D>("resource2_small");

        Title = content.Load<Texture2D>("logo");

        ShipBaseBg = content.Load<Texture2D>("shipbasescreenbg");
        Cursor = content.Load<Texture2D>("cursor");
        Circle = content.Load<Texture2D>("circle");

        TakeBaseButton = content.Load<Texture2D>("TakeBaseButton");
        LoadButton = content.Load<Texture2D>("Load");
        PlusButton = content.Load<Texture2D>("plus");
        MinusButton = content.Load<Texture2D>("minus");
        UnloadButton = content.Load<Texture2D>("Unload");
        PrevStorage = content.Load<Texture2D>("Resource_left");
        LightMask = content.Load<Texture2D>("lightmask");

        Achievement = content.Load<Texture2D>("achievement");
        AchievementGet = content.Load<Texture2D>("achievementget");

        byte[] bytecode = File.ReadAllBytes("Content/Glow.mgfx");
        Art.Glow = new Effect(graphicsDevice, bytecode);
        bytecode = File.ReadAllBytes("Content/HealthEffect.mgfx");
        Art.Health = new Effect(graphicsDevice, bytecode);
        bytecode = File.ReadAllBytes("Content/OxygenEffect.mgfx");
        Art.OxygenEffect = new Effect(graphicsDevice, bytecode);
        bytecode = File.ReadAllBytes("Content/MetalEffect.mgfx");
        Art.MetalEffect = new Effect(graphicsDevice, bytecode);
        bytecode = File.ReadAllBytes("Content/BuildShipEffect.mgfx");
        Art.BuildShipEffect = new Effect(graphicsDevice, bytecode);
        bytecode = File.ReadAllBytes("Content/lightingEffect.mgfx");
        Art.LightingEffect = new Effect(graphicsDevice, bytecode);
    }
}
