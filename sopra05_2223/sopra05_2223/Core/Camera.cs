using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using sopra05_2223.Background;
using sopra05_2223.Core.Components;
using sopra05_2223.Core.Entity;
using sopra05_2223.Core.Entity.Base;
using sopra05_2223.Core.Entity.Base.PlanetBase;
using sopra05_2223.Core.Entity.Base.SpaceBase;
using sopra05_2223.Core.Entity.Resource;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace sopra05_2223.Core;

internal sealed class Camera
{
    [JsonRequired]
    private Vector2 mPos;
    private int Speed => (int)(140 / 14f / Zoom);
    private readonly int mMoveOffset;
    private readonly int mMiniMapSize;
    // false -> draw map
    internal bool mMinimapDisabled;

    [JsonRequired]
    public float Zoom
    {
        get; private set;
    }
    private Point mScreenSize;
    private readonly Point mWorldSize;
    internal readonly Parallax mParallax;
    private Point mOldMousePosition;
    private const float ParallaxMouseSensitivity = 0.03125f;
    private const float ParallaxMoveSensitivity = 1 / 3f;
    private readonly EntityManager mEntityManager;

    private float mMiniMapCameraBorderWidth;
    private float mMiniMapCameraBorderHeight;

    private float ScreenShakeLimit => 120 * Zoom;
    private float mScreenShakeAmount;
    private float mScreenShakeTimer;

    private HashSet<Entity.Entity> mMinimapEntities = new();

    private Point ScreenShakeOffset => new((int)(Math.Cos(mScreenShakeTimer * 60) * mScreenShakeAmount), (int)(Math.Cos(mScreenShakeTimer * 70) * mScreenShakeAmount));
    private Point Pos => mPos.ToPoint() + ScreenShakeOffset;
    private int Width => (int)((1 / Zoom) * mScreenSize.X);
    private int Height => (int)((1 / Zoom) * mScreenSize.Y);
    internal Rectangle Rectangle => new(Pos, new Point(Width, Height));

    internal Rectangle mMiniMapRectangle;
    

    internal Camera(Vector2 pos, Point screenSize, Point worldSize, EntityManager entityManager, float zoom = 0.25f)
    {
        mPos = pos;
        mScreenSize = screenSize;
        Zoom = zoom;
        mMiniMapSize = 200;
        const float baseScreenWidth = 1280f;
        const float cameraOffsetMargin = 25f;
        mMoveOffset = (int)(screenSize.X / baseScreenWidth * cameraOffsetMargin);
        mWorldSize = worldSize;
        mParallax = new Parallax(Art.Parallax);
        mEntityManager = entityManager;
        mMiniMapRectangle = new Rectangle(new Point(mMoveOffset, mScreenSize.Y - mMiniMapSize - mMoveOffset), new Point(mMiniMapSize, mMiniMapSize));


        mMiniMapCameraBorderWidth = (1f * Width) / mWorldSize.X * mMiniMapSize;
        mMiniMapCameraBorderHeight = (1f * Height) / mWorldSize.Y * mMiniMapSize;
    }

    // Used to translate between Screen- and MapCoordinates.
    internal Matrix TranslationMatrix =>
        Matrix.CreateTranslation(-Pos.X, -Pos.Y, 0) *
        Matrix.CreateScale(new Vector3(Zoom, Zoom, 1));

    internal Vector2 ScreenToWorld(Vector2 screenPosition)
    {
        return Vector2.Transform(screenPosition, Matrix.Invert(TranslationMatrix));
    }

    internal Vector2 ScreenToWorldClickInMiniMap(Vector2 screenPosition)
    {
        var x = screenPosition.X - mMiniMapRectangle.X;
        var y = screenPosition.Y - mMiniMapRectangle.Y;
        var propX = x / mMiniMapSize;
        var propY = y / mMiniMapSize;
        return new Vector2(propX * mWorldSize.X, propY * mWorldSize.Y);
    }


    internal void IncreaseShake()
    {
        mScreenShakeAmount += 10;
        
        if (mScreenShakeAmount > ScreenShakeLimit)
        {
            mScreenShakeAmount = ScreenShakeLimit;
        }
    }

    private void ReduceShake()
    {
        if (mScreenShakeAmount == 0)
        {
            mScreenShakeTimer = 0;
            return;
        }
        mScreenShakeAmount -= (float) (Globals.GameTime.ElapsedGameTime.TotalSeconds * 10 * 3);
        if (mScreenShakeAmount < 0)
        {
            mScreenShakeAmount = 0;
        }
    }

    private void UpdateShake()
    {
        mScreenShakeTimer += (float)Globals.GameTime.ElapsedGameTime.TotalSeconds;
        ReduceShake();
    }

    // Move the Camera to the given Map Position.
    internal void MiniMapMove(Vector2 pos)
    {
        var posOld = mPos;
        var offset = new Vector2(mMoveOffset * 1f / mMiniMapRectangle.Width * mWorldSize.X, mMoveOffset * 1f / mMiniMapSize * mWorldSize.Y);
        pos.X -= offset.X;
        pos.Y += offset.Y;
        if (pos.X - Width / 2f < 0)
        {
            mPos.X = 0;
            mParallax.Move(0, 0);
        }
        else if (pos.X + Width / 2f > mWorldSize.X)
        {
            mPos.X = mWorldSize.X - Width;
        }
        else
        {
            mPos.X = pos.X - Width / 2f;
            mParallax.Move(mPos.X - (pos.X - Width / 2f), 0);
        }

        if (pos.Y - Height / 2f < 0)
        {
            mPos.Y = 0;
            mParallax.Move(0, 0);
        }
        else if (pos.Y + Height / 2f > mWorldSize.Y)
        {
            mPos.Y = mWorldSize.Y - Height;
        }
        else
        {
            mPos.Y = pos.Y - Height / 2f;
            mParallax.Move(0, mPos.Y - (pos.Y - Height / 2f));
        }

        // not to sure what this does, as posOld = mPos. pls fix
        // I know what you mean but somehow this moves it, so posOld.X - mPos.X /= 0 somehow. 
        mParallax.Move((posOld.X - mPos.X) * ((float)Speed / 45), (posOld.Y - mPos.Y) * ((float)Speed / 45));
    }
    internal void Resize(Point newSize)
    {
        mScreenSize = newSize;
        mMiniMapCameraBorderWidth = (1f * Width) / mWorldSize.X * mMiniMapSize;
        mMiniMapCameraBorderHeight = (1f * Height) / mWorldSize.Y * mMiniMapSize;
        mMiniMapRectangle = new Rectangle(new Point(mMoveOffset, mScreenSize.Y - mMiniMapSize - mMoveOffset), new Point(mMiniMapSize, mMiniMapSize));
    }
    internal void Update(Point pos)
    {
        UpdateShake();
        // Move the Camera when Cursor (almost) hits Edges.
        if (pos.X < mMoveOffset && pos.X >= 0 && pos.Y < mScreenSize.Y)
        {
            MoveLeft();
        }
        else if (pos.X > mScreenSize.X - mMoveOffset && pos.X <= mScreenSize.X)
        {
            MoveRight(mWorldSize.X);
        }

        if (pos.Y < mMoveOffset && pos.Y >= 0)
        {
            MoveUp();
        }
        else if (pos.Y > mScreenSize.Y - mMoveOffset && pos.Y <= mScreenSize.Y)
        {
            MoveDown(mWorldSize.Y);
        }

        if (mPos.X == 0 && mPos.Y == 0)
        {
            Globals.mExplored["TopLeft"] = true;
        }
        else if (mPos.X >= mWorldSize.X - Width && mPos.Y == 0)
        {
            Globals.mExplored["TopRight"] = true;
        }
        else if (mPos.X == 0 && mPos.Y >= mWorldSize.Y - Height)
        {
            Globals.mExplored["BottomLeft"] = true;
        }
        else if (mPos.X >= mWorldSize.X - Width && mPos.Y >= mWorldSize.Y - Height)
        {
            Globals.mExplored["BottomRight"] = true;
        }

        mParallax.Move(-(mScreenSize.X * 0.5f - mOldMousePosition.X) * ParallaxMouseSensitivity, -(mScreenSize.Y * 0.5f - mOldMousePosition.Y) * ParallaxMouseSensitivity);
        mParallax.Move((mScreenSize.X * 0.5f - pos.X) * ParallaxMouseSensitivity, (mScreenSize.Y * 0.5f - pos.Y) * ParallaxMouseSensitivity);

        mOldMousePosition = pos;
        mEntityManager.SetCameraRectangle(Rectangle);
        mEntityManager.SetTranslationMatrix(TranslationMatrix);

        if(mEntityManager.mIsMinimapUpdateFrame){
            UpdateMinimapEntities();
        }
    }

    // Move without leaving the boundary of the Map.
    private void MoveRight(int xLimit)
    {
        mPos.X += 1 * Speed;
        mParallax.Move(-(float)Speed * ParallaxMoveSensitivity, 0);
        if (xLimit - Width < mPos.X)
        {
            mPos.X = xLimit - Width;
            mParallax.Move(Speed * ParallaxMoveSensitivity, 0);
        }
    }

    private void MoveLeft()
    {
        mPos.X -= 1 * Speed;
        mParallax.Move(Speed * ParallaxMoveSensitivity, 0);
        if (mPos.X < 0)
        {
            mPos.X = 0;
            mParallax.Move(-(float)Speed * ParallaxMoveSensitivity, 0);
        }
    }

    private void MoveUp()
    {
        mPos.Y -= 1 * Speed;
        mParallax.Move(0, Speed * ParallaxMoveSensitivity);
        if (mPos.Y < 0)
        {
            mPos.Y = 0;
            mParallax.Move(0, -(float)Speed * ParallaxMoveSensitivity);
        }
    }

    private void MoveDown(int yLimit)
    {
        mPos.Y += 1 * Speed;
        mParallax.Move(0, -(float)Speed * ParallaxMoveSensitivity);
        if (yLimit - Height < mPos.Y)
        {
            mPos.Y = yLimit - Height;
            mParallax.Move(0, Speed * ParallaxMoveSensitivity);
        }
    }

    public void ChangeZoom(int amount)
    {
        var center = new Vector2(mPos.X + Rectangle.Width / 2f, mPos.Y + Rectangle.Height / 2f);

        float zoomFloat = -amount / 10000f;
        Zoom += zoomFloat;
        if (Zoom < 0.05f)
        {
            Zoom = 0.05f;
        }

        if (Zoom > 0.5f)
        {
            Zoom = 0.5f;
        }

        mMiniMapCameraBorderWidth = (1f * Width) / mWorldSize.X * mMiniMapSize;
        mMiniMapCameraBorderHeight = (1f * Height) / mWorldSize.Y * mMiniMapSize;

        var newCenter = new Vector2(mPos.X + Rectangle.Width / 2f, mPos.Y + Rectangle.Height / 2f);
        mPos.X -= newCenter.X - center.X;
        mPos.Y -= newCenter.Y - center.Y;


        MoveUp();
        MoveDown(mWorldSize.Y);
        MoveRight(mWorldSize.X);
        MoveLeft();
    }

    private void UpdateMinimapEntities()
    {
        mMinimapEntities = new HashSet<Entity.Entity>(Globals.Player.mEntities);
        foreach (var e in Globals.Player.mEntities)
        {
            // Also draw Enemies and neutral entities within viewRadius.
            var rad = e.GetComponent<CView>()?.mViewRadius;
            if (rad != null)
            {
                foreach (var e2 in e.mCloseEntities.Where(el =>
                             el.GetComponent<CTeam>().mTeam == Team.Ki ||
                             el is EBase && el.GetComponent<CTeam>().mTeam == Team.Neutral))
                {
                    if (Vector2.DistanceSquared(e.GetPos(), e2.GetPos()) <= rad * rad)
                    {
                        mMinimapEntities.Add(e2);
                    }
                }
            }
        }
    }

    internal void DrawMiniMap(SpriteBatch spriteBatch)
    {
        
        // -------------- Drawing a black outline --------------------------
        spriteBatch.Draw(Art.MiniMapSquare,
            new Vector2(mMoveOffset - 6.5f, mScreenSize.Y  - mMiniMapSize -  mMoveOffset - 6.5f),
            null,
            Color.White,
            0f,
            new Vector2(0, 0),
            new Vector2(mMiniMapSize / 15f, mMiniMapSize / 15f),
            SpriteEffects.None,
            0f);
        
        // --------------- MiniMap Background draw ------------------------
        spriteBatch.Draw(Art.MiniMapBg,
            new Vector2(mMoveOffset, mScreenSize.Y - mMiniMapSize - mMoveOffset),
            null,
            Color.White,
            0f,
            new Vector2(0, 0),
            new Vector2(mMiniMapSize / 8f, mMiniMapSize / 8f),
            SpriteEffects.None,
            0f);


        for (var i = 1; i < 4; i += 1)
        {

            var color = Color.FromNonPremultiplied(122, 122, 122, 200);
            spriteBatch.Draw(Art.MiniMapDot,
                new Vector2(mMoveOffset + i * mMiniMapSize / 4, mScreenSize.Y - mMiniMapSize - mMoveOffset),
                null,
                color,
                0f,
                new Vector2(0, 0),
                new Vector2(1, mMiniMapSize),
                SpriteEffects.None,
                0f);

            spriteBatch.Draw(Art.MiniMapDot,
                new Vector2(mMoveOffset, mScreenSize.Y - mMiniMapSize - mMoveOffset + i * mMiniMapSize / 4),
                null,
                color,
                0f,
                new Vector2(0, 0),
                new Vector2(mMiniMapSize, 1),
                SpriteEffects.None,
                0f);
        }
        // --------------- MiniMap Background draw end --------------------

        // MiniMap dots draw
        // Bad detection of TechDemo.
        if (mWorldSize == new Point(80000, 80000))
        {
            mMinimapEntities  = new HashSet<Entity.Entity>(mEntityManager.Entities.Where(e => e is not EResource));
        }
        foreach (var e in mMinimapEntities)
        {
            var color = e.GetComponent<CTeam>().mTeam switch
            {
                Team.Player => Color.CornflowerBlue,
                Team.Ki => Color.Orange,
                _ => Color.Gray
            };
            DrawOneMiniMapEntity(spriteBatch, e, color, new Vector2(2, 2));
        }

        var positions = new Vector2[]
        {
            new ((1f * mPos.X) / mWorldSize.X * mMiniMapSize + mMoveOffset, ((1f * mPos.Y) / mWorldSize.Y * mMiniMapSize + mScreenSize.Y - mMiniMapSize - mMoveOffset)),
            new ((1f * mPos.X) / mWorldSize.X * mMiniMapSize + mMoveOffset, ((1f * mPos.Y + Height) / mWorldSize.Y * mMiniMapSize + mScreenSize.Y - mMiniMapSize - mMoveOffset)),
            new ((1f * mPos.X) / mWorldSize.X * mMiniMapSize + mMoveOffset, ((1f * mPos.Y) / mWorldSize.Y * mMiniMapSize + mScreenSize.Y - mMiniMapSize - mMoveOffset)),
            new ((1f * mPos.X + Width) / mWorldSize.X * mMiniMapSize + mMoveOffset, ((1f * mPos.Y) / mWorldSize.Y * mMiniMapSize + mScreenSize.Y - mMiniMapSize - mMoveOffset)),
        };

        var scales = new Vector2[]
        {
            new (mMiniMapCameraBorderWidth, 1f),
            new (mMiniMapCameraBorderWidth, 1f),
            new (1f, mMiniMapCameraBorderHeight),
            new (1f, mMiniMapCameraBorderHeight + 1),
        };

        for (var i = 0; i < 4; ++i)
        {
            spriteBatch.Draw(Art.MiniMapDot,
                positions[i],
                null,
                Color.White,
                0f,
                new Vector2(0, 0),
                scales[i],
                SpriteEffects.None,
                0f);
        }
    }

    private void DrawOneMiniMapEntity(SpriteBatch spriteBatch, Entity.Entity e, Color color, Vector2 scale)
    {
        switch (e)
        {
            case ESpaceBase:
                spriteBatch.Draw(Art.MiniMapPlus,
                    new Vector2((e.GetX() + e.Rectangle.Width / 2f) / (mWorldSize.X * 1.0f) * mMiniMapSize + mMoveOffset - 4,
                        ((e.GetY() + e.Rectangle.Height / 2f) / (mWorldSize.Y * 1.0f) * mMiniMapSize) + mScreenSize.Y - mMiniMapSize - mMoveOffset - 4 ),
                    color
                );
                break;
            case EPlanetBase:
                spriteBatch.Draw(Art.MiniMapX,
                    new Vector2((e.GetX() + e.Rectangle.Width / 2f) / (mWorldSize.X * 1.0f) * mMiniMapSize + mMoveOffset - 4,
                        ((e.GetY() + e.Rectangle.Height / 2f) / (mWorldSize.Y * 1.0f) * mMiniMapSize) + mScreenSize.Y - mMiniMapSize - mMoveOffset - 4),
                    color
                );
                break;
            default:
                spriteBatch.Draw(Art.MiniMapDot,
                    new Vector2((e.GetX() + e.Rectangle.Width / 2f) / (mWorldSize.X * 1.0f) * mMiniMapSize +
                        mMoveOffset - 4,
                        ((e.GetY() + e.Rectangle.Height / 2f) / (mWorldSize.Y * 1.0f) * mMiniMapSize) +
                        mScreenSize.Y - mMiniMapSize - mMoveOffset - 4),
                    null,
                    color,
                    0f,
                    new Vector2(0, 0),
                    scale,
                    SpriteEffects.None,
                    0f
                );
                break;
        }
    }
}
