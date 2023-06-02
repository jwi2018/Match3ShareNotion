using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WCEventReceiver : MonoBehaviour
{
    public TMP_Text txtpro_timeScale = null;

    private void Start()
    {
        txtpro_timeScale.text = string.Format($"{Time.timeScale}x");
    }

    public void ChangeTimeScale()
    {
        if (WCGoogleSheetMapAnalyze.instance == null)
            return;

        WCGoogleSheetMapAnalyze.instance.timeScale++;
        if (WCGoogleSheetMapAnalyze.instance.timeScale > 4)
        {
            WCGoogleSheetMapAnalyze.instance.timeScale = 1;
        }
        
        Time.timeScale = WCGoogleSheetMapAnalyze.instance.timeScale;
        if (txtpro_timeScale != null)
        {
            txtpro_timeScale.text = string.Format($"{Time.timeScale}x");
        }
    }
}
