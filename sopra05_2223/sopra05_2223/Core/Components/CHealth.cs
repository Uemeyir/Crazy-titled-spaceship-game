using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using sopra05_2223.Core.Animation;
using sopra05_2223.Core.Entity.Resource;
using sopra05_2223.Core.Entity.Ships;
using sopra05_2223.Screens;

namespace sopra05_2223.Core.Components;

internal sealed class CHealth : Component
{
    [JsonRequired]
    // Stores the Maximum amount of Health
    private readonly int mMaxHealth;
    [JsonRequired]
    // Stores the current amount of Health
    private int mHealth;

    [JsonConstructor]
    internal CHealth(int maxHealth)
    {
        mMaxHealth = maxHealth;
        mHealth = maxHealth;
    }

    internal override void Update()
    {
    }

    internal void ChangeHealth(int healthChange)
    {
        // Keep Health value between 0 and mMaxHealth.
        if (healthChange < 0)
        {
            mHealth = mHealth + healthChange > 0 ? mHealth + healthChange : 0;
        }
        else
        {
            mHealth = mHealth + healthChange < mMaxHealth ? mHealth + healthChange : mMaxHealth;
        }

        if (mHealth <= 0) // Destroy this Entity, as it lost all HP.
        {
            if (mEntity.GetComponent<CTeam>().mTeam == Team.Player)
            {
                Globals.mStatistics["LostShips"] += 1;
                Globals.mRunStatistics["LostShips"] += 1;
            }
            else
            {
                Globals.mStatistics["DestroyedEnemyShips"] += 1;
                Globals.mRunStatistics["DestroyedEnemyShips"] += 1;
                if (Globals.mAchievements["EnemyShips1"] == false && Globals.mStatistics["DestroyedEnemyShips"] == 100)
                {
                    Globals.mAchievements["EnemyShips1"] = true;
                    Globals.ScreenManager.AddScreen(new AchievementGetScreen(3000, Globals.SoundManager, "Feldwebel", Globals.Resolution.mScreenSize));
                }
                else if (Globals.mAchievements["EnemyShips2"] == false &&
                         Globals.mStatistics["DestroyedEnemyShips"] == 500)
                {
                    Globals.mAchievements["EnemyShips2"] = true;
                    Globals.ScreenManager.AddScreen(new AchievementGetScreen(3000, Globals.SoundManager, "Leutnant", Globals.Resolution.mScreenSize));
                }
                else if (Globals.mAchievements["EnemyShips3"] == false &&
                         Globals.mStatistics["DestroyedEnemyShips"] == 2000)
                {
                    Globals.mAchievements["EnemyShips3"] = true;
                    Globals.ScreenManager.AddScreen(new AchievementGetScreen(3000, Globals.SoundManager, "Hauptmann", Globals.Resolution.mScreenSize));
                }
                else if (Globals.mAchievements["EnemyShips4"] == false &&
                         Globals.mStatistics["DestroyedEnemyShips"] == 5000)
                {
                    Globals.mAchievements["EnemyShips4"] = true;
                    Globals.ScreenManager.AddScreen(new AchievementGetScreen(3000, Globals.SoundManager, "Major", Globals.Resolution.mScreenSize));
                }
                else if (Globals.mAchievements["EnemyShips5"] == false &&
                         Globals.mStatistics["DestroyedEnemyShips"] == 15000)
                {
                    Globals.mAchievements["EnemyShips5"] = true;
                    Globals.ScreenManager.AddScreen(new AchievementGetScreen(3000, Globals.SoundManager, "General", Globals.Resolution.mScreenSize));
                }
            }

            if (mEntity is EShip)
            {
                Hud.ShipCounter(-1, mEntity);
                DestroyEntity();
            }
            else if (mEntity.GetComponent<CTeam>().mTeam != Team.Neutral)
            {
                mEntity.GetComponent<CTeam>().SetNeutral();
            }
        }
    }

    private void DestroyEntity()
    {
        Art.AnimationManager.AddAnimation(new Explosion2(new Vector2(mEntity.GetX() + mEntity.mTexture.Width, mEntity.GetY() + mEntity.mTexture.Height), 2f));
        // Spawn a resource when the ship is destroyed. Resource contains half of what has been stored on the ship + half the cost of the destroyed entity.
        // The other half got lost in the explosion.
        var res = new EResource(mEntity.GetPos(), Globals.sCosts[mEntity.GetType()].Y / 2 + mEntity.GetComponent<CStorage>().mStoredOxygen / 2,
            Globals.sCosts[mEntity.GetType()].X / 2 + mEntity.GetComponent<CStorage>().mStoredMetal / 2);
        res.GetComponent<CTeam>()?.SetNeutral();
        mEntity.mEntityManager.Add(res);
        mEntity.mEntityManager.Remove(mEntity);
        mEntity.mEntityManager?.mSelectedEntities?.Remove(mEntity);
    }

    internal void DrawHealthbar(SpriteBatch spriteBatch, Matrix translationMatrix)
    {
        // Draws Healthbar by using Effect HealthEffect
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, Art.Health, translationMatrix);
        Art.Health.Parameters["HealthPercent"].SetValue(mHealth / (float)mMaxHealth);
        spriteBatch.Draw(Art.Healthbar, new Vector2(mEntity.GetX() + mEntity.mTexture.Width * 0.5f - 50, mEntity.GetY() + 2 * mEntity.mTexture.Height * 0.3f), Color.White);
        spriteBatch.End();
        spriteBatch.Begin();
    }

    internal int GetHealth()
    {
        return mHealth;
    }
}