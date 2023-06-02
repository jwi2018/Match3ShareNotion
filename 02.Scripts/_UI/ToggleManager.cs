using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleManager : MonoBehaviour
{
    [SerializeField] protected List<ToggleInfo> Toggles;

    private void Start()
    {
        //if(PlayerData.GetInstance != null)
        //{
        //    if (PlayerData.GetInstance.IsPlaySFX == true)
        //    {
        //        Toggles[0]._toggle.isOn = true;
        //    }
        //    else
        //    {
        //        VolumChange(Toggles[0]._toggle);
        //    }
        //    if (PlayerData.GetInstance.IsPlayBGM == true)
        //    {
        //        Toggles[1]._toggle.isOn = true;
        //    }
        //    else
        //    {
        //        VolumChange(Toggles[1]._toggle);
        //    }

        //}
    }

    public void ToggleVlaueChange(Toggle toggle)
    {
        foreach (var nToggles in Toggles)
            if (nToggles._toggle == toggle)
                foreach (var nImage in nToggles._Images)
                    if (toggle.isOn)
                    {
                        toggle.transform.Find("IconToggle").GetComponent<Image>().sprite = nToggles._Images[0];
                        VolumChange(toggle);
                        break;
                    }
                    else
                    {
                        toggle.transform.Find("IconToggle").GetComponent<Image>().sprite = nToggles._Images[1];
                        VolumChange(toggle);
                        break;
                    }
    }

    public void VolumChange(Toggle toggle)
    {
        if (Toggles[0]._toggle == toggle)
        {
            if (toggle.isOn)
            {
                if (PlayerData.GetInstance != null) PlayerData.GetInstance.IsPlaySFX = true;
                foreach (var nCnt in SoundManager.GetInstance.SFXSource) nCnt.volume = 1;
            }
            else
            {
                if (PlayerData.GetInstance != null) PlayerData.GetInstance.IsPlaySFX = false;
                foreach (var nCnt in SoundManager.GetInstance.SFXSource) nCnt.volume = 0;
            }
        }
        else if (Toggles[1]._toggle == toggle)
        {
            if (toggle.isOn)
            {
                if (PlayerData.GetInstance != null) PlayerData.GetInstance.IsPlayBGM = true;
                SoundManager.GetInstance.BGMSource.volume = 1;
            }
            else
            {
                if (PlayerData.GetInstance != null) PlayerData.GetInstance.IsPlayBGM = false;
                SoundManager.GetInstance.BGMSource.volume = 0;
            }
        }
    }

    [Serializable]
    protected struct ToggleInfo
    {
        public Toggle _toggle;
        public List<Sprite> _Images;
    }
}