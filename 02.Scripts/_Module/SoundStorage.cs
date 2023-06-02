using System;
using UnityEngine;

[Serializable]
public class DictionaryOfIntAudiClipFair : SerializableDictionary<int, AudioClip>
{
}

public class SoundStorage : MonoBehaviour
{
    public DictionaryOfIntAudiClipFair sounds;

    public void PlaySound(int key)
    {
        if (!sounds.ContainsKey(key)) return;
        if (SoundManager.GetInstance == null) return;

        //SoundManager.GetInstance.Play(sounds[key]);

        SoundManager.GetInstance.Play(sounds[key].name);
    }
}