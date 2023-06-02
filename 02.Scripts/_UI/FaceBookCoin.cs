using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceBookCoin : MonoBehaviour
{
    [SerializeField] private SetupPopup setupPopup = null;
    [SerializeField] private GameObject coinImage;
    [SerializeField] private GameObject popupManager;

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
            if (PlayerData.GetInstance.FaceBookYear == 0 && PlayerData.GetInstance.FaceBookMonth == 0 && PlayerData.GetInstance.FaceBookDay == 0)
            {
                r_isActive = true;
            }
            else
            {
                System.DateTime time = new System.DateTime(PlayerData.GetInstance.FaceBookYear, PlayerData.GetInstance.FaceBookMonth, PlayerData.GetInstance.FaceBookDay);
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
        coinImage.SetActiveSelf(IsCoinActive());
    }

    public void OnClickFaceBookButton()
    {
        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Setting_Facebook");
        popupManager.GetComponent<PopupManager>().FaceBookLike();
        if (coinImage.activeSelf) StartCoroutine(GetCoin());
    }

    private IEnumerator GetCoin()
    {
        yield return new WaitForSeconds(0.1f);
        PlayerData.GetInstance.FaceBookYear = System.DateTime.Now.Year;
        PlayerData.GetInstance.FaceBookMonth = System.DateTime.Now.Month;
        PlayerData.GetInstance.FaceBookDay = System.DateTime.Now.Day;
        DailyCoinCheck();
        PlayerData.GetInstance.Gold += 100;
        var obj = popupManager.GetComponent<PopupManager>().GetCoin();
        obj.GetComponent<Animator>().SetTrigger("Normal");
    }
}