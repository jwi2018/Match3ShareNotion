using DarkTonic.MasterAudio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private string strSoundName = "";
    [SerializeField] private bool isLoop = false;
    [SerializeField] private float minDelay = 1f;
    [SerializeField] private float maxDelay = 3f;

    void Start()
    {
        SoundManager.GetInstance.Play(strSoundName);
    }

    void OnDisable()
    {
        MasterAudio.StopBus("SFX");
    }
}