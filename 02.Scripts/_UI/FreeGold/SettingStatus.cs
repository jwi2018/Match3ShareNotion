using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingStatus : MonoBehaviour
{
    [SerializeField] private GameObject gobAlram = null;

    private void Start()
    {
        gobAlram.SetActive(false);

        StartCoroutine(CoUpdate(0.73f));
    }

    private IEnumerator CoUpdate(float delayTime)
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
        bool facebookCoin = FaceBookCoin.IsCoinActive();
        bool youtubeCoin = YoutubeCoin.IsCoinActive();
        bool moreGameCoin = MoreGameCoin.IsCoinActive();
        if (facebookCoin || youtubeCoin || moreGameCoin)
        {
            gobAlram.SetActiveSelf(true);
        }
        else
        {
            gobAlram.SetActiveSelf(false);
        }
    }

    public void ShowPopup()
    {
    }

    private void CooltimeRefresh()
    {
    }
}