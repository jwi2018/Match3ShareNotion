using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class AcornsStatus : MonoBehaviour
{
    [SerializeField] private PopupManager PopupManager;
    [SerializeField] private GameObject CanReceiveItemAlram;
    [SerializeField] private Animator ReceiveItemAnimation;
    [SerializeField] private ShiningItem _shiningItem;
    [SerializeField] private GameObject inactiveImg;
    [SerializeField] private Text Number_Of_Acorns_InBar;
    
    private const int RewardAcorns = 100;
    private float HaveAcorns = 0;
    private void Start()
    {
        StartCoroutine(CheckAcornsCount());
    }

    public void OnClickButton()
    {
        if (BaseSystem.GetInstance.GetSystemList("AdventuerSystem"))
        {
            if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Stamp_bonus_enter"); 
        }
        else
        {
            if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Acorn_bonus_enter");
        }

        
        PopupManager.OnClickAcorns();
    }

    private IEnumerator CheckAcornsCount()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            if (Number_Of_Acorns_InBar != null)
            {
                HaveAcorns = PlayerData.GetInstance.Acorn;
                Number_Of_Acorns_InBar.text = $"{HaveAcorns} / {RewardAcorns}";
            }

            if (AcornSystem.GetInstance.IsReceiveItem())
            {
                CanReceiveItemAlram.SetActive(true);
                ReceiveItemAnimation.enabled = true;
                _shiningItem.enabled = true;
                if (inactiveImg != null)
                {
                    inactiveImg.SetActive(false);
                }
            }
            else
            {
                CanReceiveItemAlram.SetActive(false);
                ReceiveItemAnimation.enabled = false;
                _shiningItem.enabled = false;
                if (inactiveImg != null)
                {
                    inactiveImg.SetActive(true);
                }
            }
        }
    }
}