using System;
using System.Collections.Generic;
#if UNITY_IOS
using System.Collections;
using Unity.Notifications.iOS;
#else
using Unity.Notifications.Android;
#endif
using UnityEngine;
using Random = UnityEngine.Random;

public class NotificationManager : MonoBehaviour
{
    #region Variables

    private int dayInterval = 1;
    private const int NotificationCount = 30;
    private const int StarDayDaley = 1;

    private readonly List<string> titles = new()
    {
        "🔔 EyeColorMix Alert!",
        "✨ New Effect Unlocked!",
        "👋 We've Missed You!",
        "🎨 We Can't Wait To Have You Back!",
        "⏰ It's Time To Make Lens!",
        "💪🏻 Keep Up The Great Progress!",
        "🧐 Hey, Eye Color Master!",
        "🎉 Keep Your Streak Alive!",
        "👁️ Eye-mazing Creations!",
        "🖌️ Express Your Style!",
        "🧿 Pupils In Focus!",
        "👁️ Missing Your Eye-Coloring Skills!",
        "🌈 Feeling Creative? Try A New Pattern!",
        "🧿 Create The Perfect Lens!"
    };

    private readonly List<string> messages = new()
    {
        "EyeColorMix awaits you.",
        "All new effects has been unlocked! Let's check it out.",
        "It's time to come back to the excitement you've been missing.",
        "Let's make new memories together in the world of eye coloring.",
        "This is time to make beautiful lens. Come back now!",
        "That's an impressive feat! Keep pushing forward.",
        "We know that you're master of eye coloring in EyeColorMix. Let's color new lens!",
        "Don't miss today's eye coloring session, Let's keep that streak going!",
        "Create beautiful, eye-catching lenses with intricate designs and share your masterpiece!",
        "Create stunning new eye lenses with vibrant colors and cool effects! Try it now!",
        "Perfect your lenses by adding customizable pupils for that finishing touch!",
        "We noticed you haven't played in a while. Come back and play to keep your mind calm.",
        "New patterns have arrived! Experiment with them and level up your creativity!",
        "Explore endless possibilities by combining various colors, effects, and pupils for your ideal design!"
    };

    private List<int> _messageIndex;
    private const string ChannelName = "Notification Channel";

    #endregion

    #region General Methods

    private void Start()
    {
        _messageIndex = GenerateRandomNumberList(0, titles.Count, titles.Count);

#if UNITY_IOS
        StartCoroutine(AuthorizeNotificationIOS());
        SendIOSNotification();
#elif UNITY_ANDROID
        AndroidNotificationCenter.CancelAllNotifications();
        AndroidNotificationCenter.RegisterNotificationChannel(CreateNotificationChannel());
        SendNewNotification();
#endif
    }

    private static List<int> GenerateRandomNumberList(int min, int max, int count)
    {
        var list = new List<int>();

        for (var i = 0; i < count; i++)
        {
            int randomNumber;
            do
            {
                randomNumber = Random.Range(min, max);
            } while (list.Contains(randomNumber));

            list.Add(randomNumber);
        }

        return list;
    }

    #endregion

    #region Android

#if UNITY_ANDROID
    private void SendNewNotification()
    {
        for (var i = 0; i < NotificationCount; i++)
        {
            var notification = CreateNewNotification(i);
            var identifier = AndroidNotificationCenter.SendNotification(notification, ChannelName);
            if (AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier) !=
                NotificationStatus.Scheduled) return;
            AndroidNotificationCenter.CancelNotification(identifier);
            AndroidNotificationCenter.SendNotification(notification, ChannelName);
        }
    }

    private AndroidNotification CreateNewNotification(int notificationIndex)
    {
        return new AndroidNotification
        {
            Title = titles[_messageIndex[notificationIndex % messages.Count]],
            Text = messages[_messageIndex[notificationIndex % messages.Count]],
            SmallIcon = "icon_1",
            LargeIcon = "icon_2",
            ShowTimestamp = true,
            FireTime = DateTime.Now.AddDays(notificationIndex + dayInterval + StarDayDaley),
            ShouldAutoCancel = true
        };
    }

    private static AndroidNotificationChannel CreateNotificationChannel()
    {
        return new AndroidNotificationChannel
        {
            Id = ChannelName,
            Name = "Notification",
            Importance = Importance.High,
            Description = "Notification"
        };
    }

#endif

    #endregion

    #region IOS

#if UNITY_IOS
    private IEnumerator AuthorizeNotificationIOS()
    {
        using var req = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, true);
        while (!req.IsFinished) yield return null;
    }

    private void SendIOSNotification()
    {
        iOSNotificationCenter.RemoveAllScheduledNotifications();
        
        for (var i = 0; i < NotificationCount; i++)
        {
            iOSNotificationCenter.ScheduleNotification(GetIOSNotification(i));
        }
    }

    private iOSNotification GetIOSNotification(int notificationIndex)
    {
        var firstFireTime = DateTime.Now.AddDays(notificationIndex+ dayInterval + StarDayDaley);

        // Set up the time trigger to repeat daily
        var timeTrigger = new iOSNotificationCalendarTrigger()
        {
            Year = firstFireTime.Year,
            Month = firstFireTime.Month,
            Day = firstFireTime.Day,
            Hour = firstFireTime.Hour,
            Minute = firstFireTime.Minute,
            Repeats = true, // Repeat daily
        };

        // Create the notification
        var notification = new iOSNotification()
        {
            Title = titles[_messageIndex[notificationIndex%messages.Count]],
            Body = messages[_messageIndex[notificationIndex%messages.Count]],
            ShowInForeground = false,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger
        };

        return notification;
    }
#endif

    #endregion
}