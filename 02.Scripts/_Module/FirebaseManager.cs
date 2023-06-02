using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Firebase;

//using Firebase.Analytics;
//using Firebase.Crashlytics;
using Firebase.Messaging;
using UnityEngine;
using UnityEngine.Analytics;

public class FirebaseManager : Singleton<FirebaseManager>
{
    public void Start()
    {
        StartCoroutine(CheckPlayTime(30));
        StartCoroutine(CheckPlayTime(40));
        StartCoroutine(CheckPlayTime(50));
        StartCoroutine(CheckPlayTime(60));
        StartCoroutine(CheckPlayTime(70));
    }

    public IEnumerator CheckPlayTime(float min)
    {
        yield return new WaitForSeconds(min * 60);
        FirebaseLogEvent("Playing_" + min + "min");
        SingularSDK.Event("Playing_" + min + "min");
    }

    // Start is called before the first frame update
    public void Init()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                var app = FirebaseApp.DefaultInstance;

                // Create and hold a reference to your FirebaseApp, i.e.
                //   app = Firebase.FirebaseApp.DefaultInstance;
                // where app is a Firebase.FirebaseApp property of your application class.

                //FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
                FirebaseMessaging.TokenReceived += OnTokenReceived;
                FirebaseMessaging.MessageReceived += OnMessageReceived;
                // Set a flag here indicating that Firebase is ready to use by your
                // application.
            }
            else
            {
                Debug.LogError(string.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });

        //GpgsManager.GetInstance.InitGPGS();
    }

    //public void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    //{
    //    Debug.Log("Received Registration Token: " + token.Token);
    //}

    //public void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    //{
    //    Debug.Log("Received a new message from: " + e.Message.From);

    //    var notification = e.Message.Notification;
    //    if (notification != null)
    //    {
    //        Debug.Log("title: " + notification.Title);
    //        Debug.Log("body: " + notification.Body);
    //    }

    //    if (e.Message.From.Length > 0)
    //        Debug.Log("from: " + e.Message.From);
    //    if (e.Message.Data.Count > 0)
    //    {
    //        Debug.Log("data:");
    //        foreach (var iter in e.Message.Data) Debug.Log("  " + iter.Key + ": " + iter.Value);
    //    }
    //}

    public void FirebaseLogEvent(string EventName)
    {
#if UNITY_ANDROID
        Firebase.Analytics.FirebaseAnalytics.LogEvent(EventName);
        //Analytics.CustomEvent(EventName);
#elif UNITY_IOS
        Firebase.Analytics.FirebaseAnalytics.LogEvent(EventName);
#endif
    }

    public void FirebaseLogEvent(string EventName, string parameterName, string parameterValue)
    {
    }

    public void FirebaseLogEvent(string EventName, Dictionary<string, string> dParameter)
    {
        List<Firebase.Analytics.Parameter> parameters = new List<Firebase.Analytics.Parameter>();
        foreach (var item in dParameter)
        {
            parameters.Add(new Firebase.Analytics.Parameter(item.Key, item.Value));
        }

        Firebase.Analytics.FirebaseAnalytics.LogEvent(EventName, parameters.ToArray());
#if UNITY_IOS

#endif
    }

    public void DebugLog(string message)
    {
#if UNITY_IOS
        Firebase.Crashlytics.Crashlytics.Log(message);
#endif
    }

    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);

        var notification = e.Message.Notification;
        if (notification != null)
        {
            Debug.Log("title: " + notification.Title);
            Debug.Log("body: " + notification.Body);
        }
        if (e.Message.From.Length > 0)
            Debug.Log("from: " + e.Message.From);
        if (e.Message.Data.Count > 0)
        {
            Debug.Log("data:");
            foreach (System.Collections.Generic.KeyValuePair<string, string> iter in e.Message.Data)
            {
                Debug.Log("  " + iter.Key + ": " + iter.Value);
            }
        }
    }
}