using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MissionFailPopup : PopupSetting, ICoroutineAnimationController
{
    [SerializeField] private Image[] _failMissionImages;

    [SerializeField] private Text[] _failMissionTexts;

    [SerializeField] private GameObject[] _failMissionCheckMark;

    [SerializeField] private UserGold Gold;
    [SerializeField] private int RewardCoinValue;

    private bool GetButton = false;
    private readonly int nextSceneValue = 0;

    public Image[] FailMissionImage => _failMissionImages;

    public Text[] FailMissionTexts => _failMissionTexts;

    private void Start()
    {
        if (FirebaseManager.GetInstance != null)
            FirebaseManager.GetInstance.FirebaseLogEvent(string.Format($"Stage_{StageManager.StageNumber}_Fail"));
        OnPopupSetting();
        ADManager.GetInstance.SetInterstitialTimer(false);
        ADManager.GetInstance.ShowBanner(EBannerKind.BANNER);
    }

    private void OnDisable()
    {
        if (null != ADManager.GetInstance)
        {
            ADManager.GetInstance.SetInterstitialTimer(true);
            ADManager.GetInstance.HideBanner(EBannerKind.BANNER);
        }
    }

    public bool IsNext { set; get; }

    public override void OnPopupSetting()
    {
        if (SoundManager.GetInstance != null)
        {
            SoundManager.GetInstance.PauseBGM();
            SoundManager.GetInstance.Play("FailPopup");
        }

        var missions = StageManager.GetInstance.GetMissions();
        var obj = GameObject.Find("SpriteContainer");
        var container = obj.GetComponent<SpriteContainer>();

        for (var i = 0; i < missions.Count; i++)
        {
            var sprite = container.GetMissionImage(missions[i].ID);
            if (missions[i].ID == EID.NORMAL) sprite = container.GetSpriteOrNull(missions[i].ID, missions[i].color, 1);
            var intValue = missions[i].count;
            if (intValue == 0)
            {
                _failMissionImages[i].sprite = sprite;
                _failMissionTexts[i].gameObject.SetActive(false);
                _failMissionCheckMark[i].SetActive(true);
            }
            else
            {
                _failMissionImages[i].sprite = sprite;
                _failMissionTexts[i].text = "x " + intValue;
                _failMissionCheckMark[i].SetActive(false);
            }
        }

        for (var i = 0; i < _failMissionImages.Length; i++)
            if (i >= missions.Count)
                _failMissionImages[i].gameObject.SetActive(false);
        BlockManager.GetInstance.IsSwapAble = false;
        if (EditorAutoModeControll._isAutoMode)
            FirebaseManager.GetInstance.FirebaseLogEvent("Auto_Stage_Fail", "StageNumber", StageManager.StageNumber.ToString());
        else FirebaseManager.GetInstance.FirebaseLogEvent("Stage_Fail", "StageNumber", StageManager.StageNumber.ToString());
    }

    public override void OffPopupSetting()
    {
        OnClickGoMain();
    }

    public override void PressedBackKey()
    {
        OnClickGoMain();
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ButtonPush");
    }

    public void OnClickGoMain()
    {
        var popupManager = transform.parent.GetComponent<PopupManager>();

        ADManager.GetInstance.noAdsPopup = true;
        ADManager.GetInstance.ShowInterstitial(EInterstitialKind.INTERSTITIAL);
        popupManager.CallLoadingTutorialPop("MainScene", 100);
        //if (!GetButton)
        //{
        //    if (Gold != null)
        //    {
        //        GameObject popmanager = GameObject.Find("PopupManager");
        //        popmanager.GetComponent<PopupManager>().GoldRemove(Gold);
        //    }
        //    GetButton = true;
        //    nextSceneValue = 0;
        //    StartCoroutine(MissionClearCoroutine());
        //}
    }

    public void OnClickRePlay()
    {
        var popupManager = transform.parent.GetComponent<PopupManager>();

        ADManager.GetInstance.ShowInterstitial(EInterstitialKind.INTERSTITIAL);
        popupManager.CallLoadingTutorialPop("GameScene");
        //if (!GetButton)
        //{
        //    if (Gold != null)
        //    {
        //        GameObject popmanager = GameObject.Find("PopupManager");
        //        popmanager.GetComponent<PopupManager>().GoldRemove(Gold);
        //    }
        //    GetButton = true;
        //    nextSceneValue = 1;
        //    StartCoroutine(MissionClearCoroutine());
        //}
    }

    private IEnumerator MissionClearCoroutine()
    {
        var popupManager = transform.parent.GetComponent<PopupManager>();

        if (popupManager == null) yield return null;

        IsNext = false;
        popupManager.CallLimitedShop();

        yield return new WaitWhile(() => IsNext == false);

        switch (nextSceneValue)
        {
            case 0:
                popupManager.CallLoadingTutorialPop("MainScene", 100);
                break;

            case 1:
                popupManager.CallLoadingTutorialPop("GameScene");
                break;

            case 2:
                popupManager.CallLoadingTutorialPop("GameScene");
                break;
        }

        yield return new WaitForEndOfFrame();
    }

    public void ShowAd()
    {
        ADManager.GetInstance.ShowReward(ERewardedKind.REWARD, () =>
        {
            var popupManager = transform.parent.GetComponent<PopupManager>();
            PlayerData.GetInstance.Gold += RewardCoinValue;
            var coin = popupManager.GetCoin(RewardCoinValue);
            coin.GetComponent<Animator>().SetTrigger("Normal");
        });
        /*AD.ShowAd(ERewardedKind.GETITEM, () =>
        {
            var popupManager = transform.parent.GetComponent<PopupManager>();
            PlayerData.GetInstance.Gold += RewardCoinValue;
            var coin = popupManager.GetCoin();
            coin.GetComponent<Animator>().SetTrigger("Normal");
            AD.gameObject.SetActive(false);
        });*/
    }
}