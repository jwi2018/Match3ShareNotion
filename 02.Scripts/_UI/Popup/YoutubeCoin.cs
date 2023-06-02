using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YoutubeCoin : MonoBehaviour
{
    [SerializeField] private SetupPopup setupPopup = null;
    [SerializeField] private GameObject coinImage;
    [SerializeField] private GameObject popupManager;
    private bool isGetReward = false;

    private void Start()
    {
        popupManager = GameObject.Find("PopupManager");
        DailyCoinCheck();
    }

    public static bool IsCoinActive()
    {
        bool r_isActive = false;

        if (PlayerData.GetInstance != null)
        {
            if (PlayerData.GetInstance.YoutubeYear == 0 && PlayerData.GetInstance.YoutubeMonth == 0 && PlayerData.GetInstance.YoutubeDay == 0)
            {
                r_isActive = true;
            }
            else
            {
                System.DateTime time = new System.DateTime(PlayerData.GetInstance.YoutubeYear, PlayerData.GetInstance.YoutubeMonth, PlayerData.GetInstance.YoutubeDay);
                System.TimeSpan resultTime = time - System.DateTime.Now;
                if (resultTime.Days < 0)
                {
                    r_isActive = true;
                }
            }
        }

        return r_isActive;
    }

    public void DailyCoinCheck()
    {
        coinImage.SetActive(IsCoinActive());
    }

    public void OnClickYoutubeButton()
    {
        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Setting_YouTube");
        popupManager.GetComponent<PopupManager>().YoutubeFollow();
        if (coinImage.activeSelf) StartCoroutine(GetCoin());
    }

    private IEnumerator GetCoin()
    {
        yield return new WaitForSeconds(0.1f);
        PlayerData.GetInstance.YoutubeYear = System.DateTime.Now.Year;
        PlayerData.GetInstance.YoutubeMonth = System.DateTime.Now.Month;
        PlayerData.GetInstance.YoutubeDay = System.DateTime.Now.Day;
        DailyCoinCheck();
        PlayerData.GetInstance.Gold += 100;
        var obj = popupManager.GetComponent<PopupManager>().GetCoin();
        obj.GetComponent<Animator>().SetTrigger("Normal");
    }
}