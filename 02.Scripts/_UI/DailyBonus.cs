using System;
using System.Collections;
using UnityEngine;

public class DailyBonus : MonoBehaviour
{
    private int resultTime;

    private void Start()
    {
        resultTime = TimeStamp();
    }

    private int TimeStamp()
    {
        var now = DateTime.Now.ToLocalTime();
        var span = now - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
        var timestamp = (int) span.TotalSeconds;
        return timestamp;
    }

    private IEnumerator Present(int Time)
    {
        yield return new WaitForEndOfFrame();
        var tempTime = TimeStamp();

        if ((tempTime - Time) / 86400 > 1) Debug.Log("OK");
    }
}