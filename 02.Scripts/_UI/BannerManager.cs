using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public class OtherGame
{
    public Sprite sprite;
    public EOtherGames gameID = EOtherGames.NONE;
    public string address_aos;
    public string address_ios;
    public bool isShow;
}

public class BannerManager : MonoBehaviour
{
    //[SerializeField] private List<Sprite> bannerSprites = new List<Sprite>();
    [SerializeField] private List<OtherGame> otherGames = new List<OtherGame>();

    [SerializeField] private Image bannerImage;
    [SerializeField] private EOtherGames myGame = EOtherGames.NONE;
    private EOtherGames bannerGames = EOtherGames.NONE;
    private OtherGame otherGame;

    private void Start()
    {
        var ranNum = 0;
        var count = 0;
        var isClear = false;
        while (!isClear)
        {
            ranNum = Random.Range(0, otherGames.Count);
            if (otherGames[ranNum].isShow)
            {
                otherGame = otherGames[ranNum];

                bannerImage.sprite = otherGame.sprite;
                isClear = true;
            }

            count++;
            if (count > 100) return;
        }

        switch (Application.systemLanguage)
        {
            case SystemLanguage.Indonesian:
            case SystemLanguage.Russian:
            case SystemLanguage.Turkish:
            case SystemLanguage.Ukrainian:
                {
                    otherGame = otherGames[otherGames.Count - 1];
                    bannerImage.sprite = otherGame.sprite;
                    break;
                }
        }
    }

    public void OnClickBanner()
    {
#if UNITY_ANDROID
        Application.OpenURL(otherGame.address_aos);
#elif UNITY_IOS
                Application.OpenURL(otherGame.address_ios);
#endif
        if (BaseSystem.GetInstance != null)
        {
            if (BaseSystem.GetInstance.GetSystemList("CircusSystem"))
            {
                if (FirebaseManager.GetInstance != null)
                    FirebaseManager.GetInstance.FirebaseLogEvent("GameCrossBanner");
            }
            else
            {
                if (FirebaseManager.GetInstance != null)
                    FirebaseManager.GetInstance.FirebaseLogEvent("GameCrossBanner");
            }
        }
        else
        {
            if (FirebaseManager.GetInstance != null)
                FirebaseManager.GetInstance.FirebaseLogEvent("GameCrossBanner");
        }
    }
}