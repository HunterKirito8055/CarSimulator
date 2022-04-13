using System;
using UnityEngine;
using Unity.Notifications.Android;


public class LocalNotifications : MonoBehaviour
{

    private AndroidNotificationChannel channel;
    private AndroidNotification notification;
    public static LocalNotifications localNotificationsInstance;

    private void Awake()
    {
        if (localNotificationsInstance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            localNotificationsInstance = this;
        }
        InitializeNotificationChannel();
    }
    public void SendNotification(TimeSpan delay)
    {
        CancelAllNotifications();
        SendNotification(delay, "Fuels", "Restored", "icon_0");
    }
    void InitializeNotificationChannel()
    {
        if (channel.Id != null)
        {
            AndroidNotificationCenter.DeleteNotificationChannel(channel.Id);
        }
        channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }
    void SendNotification(TimeSpan delay, string _notificationTitle, string _notificationMessage, string _icon)
    {
        notification = new AndroidNotification();
        notification.Title = _notificationTitle;
        notification.Text = _notificationMessage;
        notification.FireTime = DateTime.Now + delay;
        notification.LargeIcon = _icon;
        // int id = notification.Number;
        AndroidNotificationCenter.SendNotification(notification, channel.Id);
    }
    public void CancelAllNotifications()
    {
        AndroidNotificationCenter.CancelAllScheduledNotifications();
    }
}