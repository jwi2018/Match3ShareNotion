using System;
using System.Collections;
using UnityEngine;

public class MoreGameCoin : MonoBehaviour
{
    [SerializeField] private GameObject PopupManager;

    [SerializeField] private GameObject CoinImage;

    private void Start()
    {
        PopupManager = GameObject.Find("PopupManager");
        DailyCoinCheck();
    }

    public static bool IsCoinActive()
    {
        bool r_isActive = false;

        if (PlayerData.GetInstance != null)
        {
            if (PlayerData.GetInstance.MoreGameYear == 0 && PlayerData.GetInstance.MoreGameMonth == 0 &&
                PlayerData.GetInstance.MoreGameDay == 0)
            {
                r_isActive = true;
            }
            else
            {
                var time = new DateTime(PlayerData.GetInstance.MoreGameYear, PlayerData.GetInstance.MoreGameMonth,
                    PlayerData.GetInstance.MoreGameDay);
                var resultTime = time - DateTime.Now;
                if (resultTime.Days < 0) r_isActive = true;
            }
        }

        return r_isActive;
    }

    public void DailyCoinCheck()
    {
        CoinImage.SetActive(IsCoinActive());
    }

    public void OnClickMoreGameButton()
    {
        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Setting_MoreGames");
        PopupManager.GetComponent<PopupManager>().OnClickMoreGame();
        if (CoinImage.activeSelf) StartCoroutine(GetCoin());
    }

    private IEnumerator GetCoin()
    {
        yield return new WaitForSeconds(0.1f);
        PlayerData.GetInstance.MoreGameYear = DateTime.Now.Year;
        PlayerData.GetInstance.MoreGameMonth = DateTime.Now.Month;
        PlayerData.GetInstance.MoreGameDay = DateTime.Now.Day;
        DailyCoinCheck();
        PlayerData.GetInstance.Gold += 100;
        var obj = PopupManager.GetComponent<PopupManager>().GetCoin();
        obj.GetComponent<Animator>().SetTrigger("Normal");
        //FirebaseManager.GetInstance.FirebaseLogEvent("MoreGame");
    }
}