using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using sopra05_2223.Core.Components;
using sopra05_2223.Protagonists;
using sopra05_2223.Screens;
using System;
using System.Collections.Generic;
using sopra05_2223.Core.Entity.Projectile;
using sopra05_2223.Core.Entity.Ships;

// using sopra05_2223.Core.Entity.Ships;

namespace sopra05_2223.Core.Entity;

[JsonObject(IsReference = true)]
internal abstract class Entity
{
    [JsonRequired]
    private Vector2 mPos;

    protected int mWidth, mHeight;
    // mTexture has to be set in the constructor of every individual entity because
    // serialization of Texture2D does not work (and would be stupid (large file size)).
    [JsonIgnore]
    public Texture2D mTexture;
    // Ignore needed to tackle self referencing loop.
    [JsonIgnore]
    public EntityManager mEntityManager;
    [JsonRequired]
    internal readonly Dictionary<Type, Component> mComponents = new();

    [JsonIgnore]
    internal List<Entity> mCloseEntities = new();

    public Rectangle Rectangle => new(new Point((int)mPos.X - mWidth / 2, (int)mPos.Y - mHeight / 2), new Point(mWidth, mHeight));

    protected Entity(Vector2 pos, Player owner)
    {
        mPos = pos;
        AddComponent(new CTeam(owner));
    }

    public void Update()
    {
        foreach (var component in mComponents.Values)
        {
            component.Update();
        }

        foreach (var e in mCloseEntities)
        {
            Collides(e);
        }
    }

    public void Draw(SpriteBatch spriteBatch, Matrix translationMatrix)
    {
        // get angle if available
        var cTrans = GetComponent<CTransform>();
        var rotation = (float)Math.PI;
        var origin = new Vector2(mWidth / 2f, mHeight / 2f);
        if (cTrans != null)
        {
            rotation = -cTrans.GetAngle();
        }

        var selected = false;
        if (GetComponent<CSelect>() != null)
        {
            selected = GetComponent<CSelect>().mSelected;
        }

        if (selected)
        {
            Hud.ShipCounter(1, this);
            if (GetComponent<CEffect>() == null)
            {
                AddComponent(new CEffect(Art.Glow));
                GetComponent<CEffect>().RemoveEffect(Art.Glow);
                GetComponent<CEffect>().AddEffect(Art.Glow);
            }
            GetComponent<CEffect>().DrawGlow(spriteBatch, translationMatrix, rotation, origin, mPos);
        }
        else
        {
            Hud.ShipCounter(-1, this);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, translationMatrix);
            if (GetComponent<CStorage>() != null && GetComponent<CStorage>().GetSelected())
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, Art.Glow, translationMatrix);
                spriteBatch.Draw(mTexture, mPos, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 1f);
                spriteBatch.End();
                spriteBatch.Begin();
            }
            else
            {
                var col = Color.White;
                var scale = 1f;
                if (this is EProjectile ep)
                {
                    if (ep.mEntityWhoShot is EMedic)
                    {
                        col = GetComponent<CTeam>().mTeam == Team.Ki ? Color.Orange : Color.Cyan;
                    }
                    else
                    {
                        col = GetComponent<CTeam>().mTeam == Team.Ki ? Color.Red : Color.Lime;
                        scale = ep.mEntityWhoShot is EMoerser ? 1.5f : 1f;
                    }
                }
                spriteBatch.Draw(mTexture, mPos, null, col, rotation, origin, scale, SpriteEffects.None, 1f);
            }
            spriteBatch.End();
            spriteBatch.Begin();
        }

        GetComponent<CHealth>()?.DrawHealthbar(spriteBatch, translationMatrix);

        GetComponent<CResource>()?.DrawAmount(spriteBatch, translationMatrix);

        GetComponent<CTakeBase>()?.ShowTakeBaseButton(spriteBatch, translationMatrix);
    }

    public void AddComponent(Component component)
    {
        mComponents.TryAdd(component.GetType(), component);
        component.mEntity = this;
    }

    public T GetComponent<T>() where T : Component
    {
        //TODO: not sure if this is ideal
        return mComponents.GetValueOrDefault(typeof(T), defaultValue: null) as T;
    }

    public int GetX()
    {
        // returns x of top left corner of the entity
        return (int)(mPos.X - (mWidth >> 1));
    }

    public int GetY()
    {
        // returns y of top left corner of the entity
        return (int)(mPos.Y - (mHeight >> 1));
    }
    public Vector2 GetPos()
    {
        return mPos;
    }
    public void SetY(int y)
    {
        mPos.Y = y + mHeight / 2f;
    }
    public void SetX(int x)
    {
        mPos.X = x + mWidth / 2f;
    }

    public int GetWidth()
    {
        return mWidth;
    }

    public int GetHeight()
    {
        return mHeight;
    }

    public abstract void Collides(Entity other);

    internal bool IsColliding(Entity other)
    {
        // collision check

        var center = new Vector2(GetX() + GetWidth() / 2, GetY() + GetHeight() / 2);
        var otherCenter = new Vector2(other.GetX() + other.GetWidth() / 2, other.GetY() + other.GetHeight() / 2);
        var collisionDistance = Math.Max(GetWidth(), GetHeight()) / 2f + Math.Max(other.GetHeight(), other.GetHeight()) / 2f;
        return Vector2.Distance(center, otherCenter) <= collisionDistance;
    }


    internal abstract void ChangeTexture();

    internal void RemoveComponent(Component component)
    {
        if (component != null)
        {
            mComponents.Remove(component.GetType());
        }
    }

    protected bool InCameraView()
    {
        return Globals.Camera.Rectangle.Intersects(Rectangle);
    }

}
