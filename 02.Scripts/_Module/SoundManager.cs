using System;
using UnityEngine;
using DarkTonic;
using DarkTonic.MasterAudio;

public enum SoundName
{
    DAILY_BONUS = 0,
    TOUCH_BUTTON = 1,
    TOUCH_BUTTON_
}

[Serializable]
public struct NamedSound
{
    public SoundName Name;
    public AudioClip[] SoundClip;
}

public class SoundManager : Singleton<SoundManager>
{
    public bool IsAdsTime;

    public AudioSource BGMSource;
    public AudioClip[] sceneBGM;

    public AudioSource[] SFXSource;
    public AudioClip[] soundClip;

    public AudioClip IntroSFX;
    public AudioClip DailyBonus;
    public AudioClip GetCoin;
    public AudioClip StarBoxComing;
    public AudioClip StarBoxClose;
    public AudioClip StarBoxGetItem;
    public AudioClip Popup;
    public AudioClip PopupShop;
    public AudioClip ButtonPush;
    public AudioClip PlayButtonPush = null;
    public AudioClip MissionPopupOpen;
    public AudioClip MissionPopupClose;
    public AudioClip BlockTouch;
    public AudioClip BlockSwap;
    public AudioClip BlockDrop;
    public AudioClip BlockDestroy;
    public AudioClip BlockCreateItem;
    public AudioClip BombDirect;
    public AudioClip BombX;
    public AudioClip BombCircle;
    public AudioClip BombCross;
    public AudioClip BombBigCross;
    public AudioClip BombBigX;
    public AudioClip BombDoubleCircle;
    public AudioClip BombRainbow;
    public AudioClip BombDoubleRainbow;
    public AudioClip ComboMatch;
    public AudioClip ComboMatchMessage;

    public AudioClip ComboMatch_Nice;
    public AudioClip ComboMatch_Perfect;
    public AudioClip ComboMatch_Fantastic;
    public AudioClip Hint;
    public AudioClip GetStarInPlay;
    public AudioClip UseHammer;
    public AudioClip UseShuffle;
    public AudioClip ClearFirework;
    public AudioClip RemainMoveFireworkShoot;
    public AudioClip ClearBravo;
    public AudioClip ClearStar;
    public AudioClip ClearPopup;
    public AudioClip FailPopup;
    public AudioClip ShowFiveCount;
    public AudioClip GetFiveCount;
    public AudioClip CollectRelic;
    public AudioClip CheckMissionClear;
    public AudioClip Oak;
    public AudioClip Box;
    public AudioClip Sand;
    public AudioClip Floor;
    public AudioClip StoneFloor;
    public AudioClip ColorCup;
    public AudioClip Bubble;
    public AudioClip Prison;
    public AudioClip Ice;
    public AudioClip Lamp;
    public AudioClip Nelumbo;
    public AudioClip GiftBox;
    public AudioClip Band;
    public AudioClip BigGoldBox;
    public AudioClip ChangeChameleon;
    public AudioClip ColorBox;
    public AudioClip Lava;
    public AudioClip TimeBombNoTime;
    public AudioClip TimeBomb;
    public AudioClip SuperBomb;
    public AudioClip GoldMine;
    public AudioClip Pick;
    public AudioClip BigEntrails;
    public AudioClip Gear;
    public AudioClip WaterWave;
    public AudioClip RelicDrop;
    public AudioClip RouletteSpin;
    public AudioClip RouletteItem;
    public AudioClip Lemon;
    public AudioClip clamOpen;
    public AudioClip clamBroken;
    public AudioClip poolHit;
    public AudioClip MetalDestroy;
    public AudioClip TurnSwitch;
    public AudioClip TurnDestroy;
    public AudioClip GearActive;
    public AudioClip GearPush;
    public AudioClip GiftBoxOpen;
    public AudioClip CashBlockSelect;
    public AudioClip RailActive;
    public AudioClip Bravo;
    public AudioClip Jam;
    public AudioClip LavaCreate;
    public AudioClip BandageCreate;
    public AudioClip savingCoin = null;
    public AudioClip SeasonPass_GetItem;
    public AudioClip SeasonPass_NewItemOpen;
    public AudioClip SeasonPass_UnlockOpen;

    public void PlayBGM(int id)
    {
        if (id < 0 || id >= sceneBGM.Length)
        {
            Debug.Log("this");
            return;
        }

        BGMSource.Stop();
        if (sceneBGM[id] != null)
        {
            BGMSource.clip = sceneBGM[id];
            BGMSource.Play();
        }
    }

    public static void SFXOnOff(bool isOn)
    {
        if (isOn == true)
        {
            MasterAudio.SetBusVolumeByName("SFX", 1f);
            MasterAudio.SetBusVolumeByName("Loop", 1f);
        }
        else
        {
            MasterAudio.SetBusVolumeByName("SFX", 0f);
            MasterAudio.SetBusVolumeByName("Loop", 0f);
        }
    }

    public static void BGMOnOff(bool isOn)
    {
        if (isOn == true)
        {
            MasterAudio.PlaylistMasterVolume = 1f;
        }
        else
        {
            MasterAudio.PlaylistMasterVolume = 0f;
        }
    }

    public void PlayBGM(string bgm)
    {
        Debug.LogWarningFormat("KKI{0}", bgm);
        MasterAudio.StartPlaylistOnClip("BGM", bgm);
    }

    public void PauseBGM()
    {
        MasterAudio.StopAllPlaylists();
        //BGMSource.Pause();
    }

    public void ResumeRGM()
    {
        //if (!BGMSource.isPlaying) BGMSource.Play();
    }

    public void Play(int id)
    {
        if (id < 0 || id >= soundClip.Length) return;
        for (var i = 0; i < SFXSource.Length; i++)
            if (!SFXSource[i].isPlaying)
            {
                SFXSource[i].PlayOneShot(soundClip[id]);
                return;
            }

        SFXSource[0].PlayOneShot(soundClip[id]);
    }

    public void Play(string strAudio)
    {
        MasterAudio.PlaySound(strAudio);
    }

    public void Play(AudioClip audioClip)
    {
        if (audioClip == null) return;
        for (var i = 0; i < SFXSource.Length; i++)
            if (!SFXSource[i].isPlaying)
            {
                SFXSource[i].PlayOneShot(audioClip);
                return;
            }

        //SFXSource[0].Stop();
        SFXSource[0].PlayOneShot(audioClip);
    }
}