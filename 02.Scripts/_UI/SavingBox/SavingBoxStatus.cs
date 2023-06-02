using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavingBoxStatus : MonoBehaviour
{
    [SerializeField] private Image imgAlram = null;
    [SerializeField] private Text savingCoin = null;
    [SerializeField] private GameObject gobCoin = null;

    private void Start()
    {
        imgAlram.gameObject.SetActive(false);
        gobCoin.SetActive(false);
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
        StartCoroutine(CoInit(0.3f));
    }

    private IEnumerator CoInit(float delayTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(delayTime);
            SetUI();
        }
    }

    private void SetUI()
    {
        if (PlayerData.GetInstance == null) return;
        savingCoin.text = string.Format("{0:+#,0;-#,0;0}", PlayerData.GetInstance.SavingCoin);

        if (BaseSystem.GetInstance != null)
        {
            if (BaseSystem.GetInstance.GetSystemList("Fantasy") != null || !BaseSystem.GetInstance.GetSystemList("Fantasy"))
            {
                if (PlayerData.GetInstance.SavingCoin > 0)
                {
                    gobCoin.SetActiveSelf(true);
                }
                else
                {
                    gobCoin.SetActiveSelf(true);
                }
            }
        }
        
        if (PlayerData.GetInstance.SavingCoin >= SavingInfomation.isSavingCoinLevel1)
        {
            imgAlram.gameObject.SetActiveSelf(true);
        }
        else
        {
            imgAlram.gameObject.SetActiveSelf(false);
        }
    }

    public void Init()
    {
    }

    public void AddSavingCoin(int value)
    {
    }

    public void SavingAnimation(int value)
    {
    }

    public void ShowParticle()
    {
    }

    private int testValue = 0;

    public void ShowSavingPopup()
    {
        if (BaseSystem.GetInstance != null)
        {
            if (BaseSystem.GetInstance.GetSystemList("CircusSystem"))
            {
                FirebaseManager.GetInstance.FirebaseLogEvent("intro_enter_piggy");
            }
        }
        else
        {
            FirebaseManager.GetInstance.FirebaseLogEvent("intro_pigshop_enter");
        }
        
        var popupManager = GameObject.Find("PopupManager").GetComponent<PopupManager>();
        if (popupManager != null)
        {
            popupManager.OnClickSavingBox();
        }
    }
}