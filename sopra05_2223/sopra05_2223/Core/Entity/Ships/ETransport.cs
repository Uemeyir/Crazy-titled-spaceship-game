using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using sopra05_2223.Core.Components;
using sopra05_2223.Protagonists;

namespace sopra05_2223.Core.Entity.Ships;

internal sealed class ETransport : EShip
{
    private readonly int mAxShips;
    [JsonRequired]
    internal readonly List<ECollector> mCollectors = new();

    public ETransport(Vector2 pos, Player owner) : base(pos, owner)
    {
        if (owner is PlayerPlayer)
        {
            mTexture = Art.Transport;
            AddComponent(new CSelect(true));
            AddComponent(new CView(Globals.sViewRadius["Transport"]));
        }
        else
        {
            mTexture = Art.EnemyTransport;
        }

        mWidth = mTexture.Width;
        mHeight = mTexture.Height;

        mAxShips = 6;

        // Components:
        AddComponent(new CTransform(0.3f, new Point(GetX(), GetY())));
        AddComponent(new CHealth(100));
        AddComponent(new CStorage(Globals.sStorage["Transport"].X, Globals.sStorage["Transport"].Y, false));
        AddComponent(new CollectTransporter());
    }

    public List<ECollector> GetCollectors()
    {
        return mCollectors;
    }

    internal void AddCollectors(int n)
    {
        if (n > mAxShips)
        {
            return;
        }

        for (var i = 0; i < n; i++)
        {
            switch (i)
            {
                case 0:
                    {
                        var newCollector = new ECollector(new Vector2((this.GetX() + this.mWidth / 2 - (this.mWidth / 2 + 300)),
                            (this.GetY() + this.mHeight / 2 - (this.mHeight / 2 + 80))), this.GetComponent<CTeam>().mProtagonist, this, i);
                        mCollectors.Add(newCollector);
                        mEntityManager.Add(newCollector);
                        this.GetComponent<CTeam>().mProtagonist.AddEntity(newCollector);
                        break;
                    }
                case 1:
                    {
                        var newCollector = new ECollector(new Vector2((this.GetX() + this.mWidth / 2 + (this.mWidth / 2 + 300)),
                            (this.GetY() + this.mHeight / 2 - (this.mHeight / 2 + 80))), this.GetComponent<CTeam>().mProtagonist, this, i);
                        mCollectors.Add(newCollector);
                        mEntityManager.Add(newCollector);
                        this.GetComponent<CTeam>().mProtagonist.AddEntity(newCollector);
                        break;
                    }
                case 2:
                    {
                        var newCollector = new ECollector(new Vector2((this.GetX() + this.mWidth / 2 - (this.mWidth / 2 + 300)),
                            (this.GetY() + this.mHeight / 2 + (this.mHeight / 2 + 80))), this.GetComponent<CTeam>().mProtagonist, this, i);
                        mCollectors.Add(newCollector);
                        mEntityManager.Add(newCollector);
                        this.GetComponent<CTeam>().mProtagonist.AddEntity(newCollector);
                        break;
                    }
                case 3:
                    {
                        var newCollector = new ECollector(new Vector2((this.GetX() + this.mWidth / 2 + (this.mWidth / 2 + 300)),
                            (this.GetY() + this.mHeight / 2 + (this.mHeight / 2 + 80))), this.GetComponent<CTeam>().mProtagonist, this, i);
                        mCollectors.Add(newCollector);
                        mEntityManager.Add(newCollector);
                        this.GetComponent<CTeam>().mProtagonist.AddEntity(newCollector);
                        break;
                    }
                case 4:
                    {
                        var newCollector = new ECollector(new Vector2((this.GetX() + this.mWidth / 2 - (this.mWidth / 2 + 380)),
                            (this.GetY() + this.mHeight / 2)), this.GetComponent<CTeam>().mProtagonist, this, i);
                        mCollectors.Add(newCollector);
                        mEntityManager.Add(newCollector);
                        this.GetComponent<CTeam>().mProtagonist.AddEntity(newCollector);
                        break;
                    }
                case 5:
                    {
                        var newCollector = new ECollector(new Vector2((this.GetX() + this.mWidth / 2 + (this.mWidth / 2) + 380),
                            (this.GetY() + this.mHeight / 2)), this.GetComponent<CTeam>().mProtagonist, this, i);
                        mCollectors.Add(newCollector);
                        mEntityManager.Add(newCollector);
                        this.GetComponent<CTeam>().mProtagonist.AddEntity(newCollector);
                        break;
                    }
            }
        }

        this.GetComponent<CollectTransporter>()?.SetCollectors(mCollectors);
    }

    internal override void ChangeTexture()
    {
        if (mTexture == Art.Transport)
        {
            mTexture = Art.EnemyTransport;
        }
        else
        {
            mTexture = Art.Transport;
        }
    }
}