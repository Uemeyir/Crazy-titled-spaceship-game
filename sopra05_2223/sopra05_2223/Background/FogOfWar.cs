using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sopra05_2223.Core;
using sopra05_2223.Core.Components;
using sopra05_2223.Core.Entity;

namespace sopra05_2223.Background;

internal sealed class FogOfWar
{
    private readonly Texture2D mLightMask;
    internal RenderTarget2D mLightsTarget;

    internal bool mEnabled = true;

    private readonly EntityManager mEntityManager;
    private readonly Camera mCamera;

    internal FogOfWar(EntityManager entityManager, Camera camera)
    {
        mEntityManager = entityManager;
        mCamera = camera;
        mLightMask = Art.LightMask;

        var pp = Globals.GraphicsDevice.GraphicsDevice.PresentationParameters;
        mLightsTarget = new RenderTarget2D(Globals.GraphicsDevice.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
    }

   internal void Draw(SpriteBatch spriteBatch)
    {
        if (!mEnabled)
        {
            return;
        }
        Globals.GraphicsDevice.GraphicsDevice.SetRenderTarget(mLightsTarget);
        Globals.GraphicsDevice.GraphicsDevice.Clear(Color.Black);
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

        // all entities that are in range of the camera (with their radius)
        foreach(var entity in mEntityManager.Entities.Where(e => e.GetComponent<CView>() != null))
        {
            var center = Vector2.Transform(entity.Rectangle.Center.ToVector2(), mCamera.TranslationMatrix).ToPoint();
            var screenRadius = (int)(entity.GetComponent<CView>().mViewRadius * mCamera.Zoom * 1.2);

            spriteBatch.Draw(mLightMask,
                new Rectangle(center - new Point(screenRadius, screenRadius), new Point(screenRadius * 2, screenRadius * 2)),
                Color.White);
        }

        spriteBatch.End();
    }
}