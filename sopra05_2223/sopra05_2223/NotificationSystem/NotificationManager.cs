using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sopra05_2223.Core;

namespace sopra05_2223.NotificationSystem;

internal sealed class NotificationManager
{

    private readonly List<Notification> mNotifications = new();
    private const int NotificationLimit = 5;
    private int mElapsed;
    private const int UpdateStatisticsNotificationInterval = 5000;
    private int mMaxStrLen = 250;

    internal void AddNotification(Notification notification)
    {
        // FiFo-Queue logic
        mNotifications.Add(notification);
        if (mNotifications.Count > NotificationLimit)
        {
            mNotifications.RemoveAt(0);
        }
    }

    internal void Update()
    {
        mMaxStrLen = 250;
        foreach (var notification in mNotifications)
        {
            if (!notification.Done)
            {
                notification.Update();
            }

            var strLen = (int)Art.Arial12.MeasureString(notification.mText).X;
            mMaxStrLen = mMaxStrLen < strLen ? strLen : mMaxStrLen;
        }

        mElapsed += Globals.GameTime.ElapsedGameTime.Milliseconds;
        if (mElapsed > UpdateStatisticsNotificationInterval)
        {
            UpdateStatisticsNotifications();
            mElapsed -= UpdateStatisticsNotificationInterval;
        }
    }

    internal void DrawNewNotifications(SpriteBatch spriteBatch, Vector2 textPosition)
    {
        // Notifications are added to the back of the List, so draw notifications until first is done.
        var yOffset = 0;
        for (var i=mNotifications.Count-1; i >= 0; --i)
        {
            if (mNotifications[i].Done)
            {
                break;
            }
            spriteBatch.DrawString(Art.Arial12, mNotifications[i].mText, textPosition + yOffset * new Vector2(0, 30) - new Vector2(mMaxStrLen - 250, 0), Color.White);
            --yOffset;
        }
    }

    internal void DrawNotificationsMenu(SpriteBatch spriteBatch, Vector2 textPosition)
    {
        // This is drawn based on the position of the lowest string position.
        Color borderColor = Color.Black;
        const int border = 5;
        const int padding = 10;

        var topLeftMenu = textPosition - new Vector2(border + padding + mMaxStrLen - 250, border + padding + (NotificationLimit - 1) * (12 + Art.Arial12.LineSpacing));
        int width = 2 * (border + padding) + mMaxStrLen;
        const int height = 2 * (border + padding) - 18 + NotificationLimit * 30;

        // Draw Background
        spriteBatch.Draw(Art.MiniMapBg, new Rectangle((int)topLeftMenu.X, (int)topLeftMenu.Y, width, height), Color.White);

        // Draw Border
        spriteBatch.Draw(Art.MiniMapDot, new Rectangle(topLeftMenu.ToPoint(), new Point(width, border)), borderColor);
        spriteBatch.Draw(Art.MiniMapDot, new Rectangle(topLeftMenu.ToPoint(), new Point(border, height)), borderColor);
        spriteBatch.Draw(Art.MiniMapDot, new Rectangle((int)topLeftMenu.X, (int)topLeftMenu.Y + height - border, width, border), borderColor);
        spriteBatch.Draw(Art.MiniMapDot, new Rectangle((int)topLeftMenu.X + width - border, (int)topLeftMenu.Y, border, height), borderColor);

        for (var i = mNotifications.Count - 1; i >= 0; --i)
        {
            spriteBatch.DrawString(Art.Arial12, mNotifications[i].mText, topLeftMenu + new Vector2(border + padding, border + padding + i * 30), Color.White);
        }
    }

    private static void UpdateStatisticsNotifications()
    {
        UpdateStatisticsNotification("DestroyedEnemyShips", singular: " gegnerisches Schiff zerstört!", plural: " gegnerische Schiffe zerstört!");
        UpdateStatisticsNotification("LostShips", singular: " Schiff verloren!", plural: " Schiffe verloren!");
    }

    private static void UpdateStatisticsNotification(string statisticName, string singular="", string plural="")
    {
        var diff = Globals.mStatistics[statisticName] - Globals.sLastNotifiedStats[statisticName];

        if (diff > 0)
        {
            var str = diff > 1 ? plural : singular;
            Globals.NotificationManager.AddNotification(new Notification(diff + str));
            switch (statisticName)
            {
                case "DestroyedEnemyShips":
                    if (diff > 25)
                    {
                        Globals.NotificationManager.AddNotification(new Notification("nice"));
                    }

                    break;
                case "LostShips":
                    if (diff > 24 && diff <= 49)
                    {
                        Globals.NotificationManager.AddNotification(new Notification("du müsstest jetzt wütend sein"));
                    } else if (diff > 49)
                    {
                        Globals.NotificationManager.AddNotification(new Notification("git gud ist kein git Befehl"));
                    }
                    break;
            }
        }
        Globals.sLastNotifiedStats[statisticName] = Globals.mStatistics[statisticName];
    }

}