using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ERewardType
{
    SHOP,
    MAIN,
    PLAY_ITEM,
    RESULT,
    ROULETTE,
    PLAY_MOVE,
    CONTINUE
}


public enum EGameRewardType
{
    NONE,
    MOVE,
    ITEM1
}

public class RewardAdsButton : MonoBehaviour
{
    [SerializeField] private int RewardCoin = 100;

    [SerializeField] private ERewardType RewardType = ERewardType.MAIN;

    [SerializeField] private GameObject[] RewardImageObj;

    [SerializeField] private Animator animator;

    [SerializeField] private int RewardAnimType;

    [SerializeField] private List<GameObject> rewardItemObj = new List<GameObject>();

    private Image _image;
    private bool isNetworkOk = true;

    private PopupManager popupManager;
    private int ranNum;

    public int SetRewardAnimType
    {
        set => RewardAnimType = value;
    }

    private void Start()
    {
        _image = GetComponent<Image>();
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            SetActiveMode(false);
            isNetworkOk = false;
        }

        if (RewardType == ERewardType.PLAY_ITEM) ItemChange();
        if (popupManager == null) popupManager = GameObject.Find("PopupManager").GetComponent<PopupManager>();
    }

    private void Update()
    {
        if (!isNetworkOk) SetActiveMode(false);
    }


    public void ItemChange()
    {
        if (RewardType == ERewardType.PLAY_ITEM)
        {
            ranNum = Random.Range(0, 4);

            for (var i = 0; i < rewardItemObj.Count; i++)
                if (i == ranNum) rewardItemObj[i].SetActive(true);
                else rewardItemObj[i].SetActive(false);
        }
    }

    public void SetActiveMode(bool value)
    {
        foreach (var item in RewardImageObj)
            if (item != null)
                item.gameObject.SetActive(value);
        if (_image != null) gameObject.SetActive(value);
        gameObject.SetActive(value);
    }

    private void ApplyActvie(bool value)
    {
        if (RewardImageObj.Length != 0)
            foreach (var item in RewardImageObj)
                if (item != null)
                    if (item.gameObject != null)
                        item.gameObject.SetActive(value);

        if (this != null)
            if (gameObject != null)
                gameObject.SetActive(value);
    }

    public void ShowAds()
    {
//#if UNITY_EDITOR
//        GetReward();
//        return;
//#endif
//        if (animator != null) animator.SetBool("AdsLoading", false);
//        int temp = -1;
//        if (popupManager == null)
//        {
//            popupManager = GameObject.Find("PopupManager").GetComponent<PopupManager>();
//        }
//        popupManager.ExceptionPopup();
//        switch (RewardType)
//        {
//            case ERewardType.MAIN:
//                temp = 0;
//                break;
//            case ERewardType.ROULETTE:
//                temp = 1;
//                break;
//            case ERewardType.SHOP:
//                temp = 2;
//                break;
//            case ERewardType.RESULT:
//                temp = 3;
//                break;
//            case ERewardType.PLAY_MOVE:
//                temp = 4;
//                break;
//            case ERewardType.PLAY_ITEM:
//                temp = 5;
//                break;
//            case ERewardType.CONTINUE:
//                temp = 6;
//                break;
//            default:
//                break;
//        }
//        if (AdsManager.GetInstance.IsNewVideoLoaded(RewardType))
//        {

//            AdsManager.GetInstance.ShowNewRewardVideo((isDone) =>
//            {
//                Debug.Log("Reward Show : " + isDone.ToString());
//                Dictionary<string, string> paramater = new Dictionary<string, string>();
//                switch (RewardType)
//                {
//                    case ERewardType.MAIN:
//                        paramater.Add("AdsResult", isDone.ToString());
//                        FirebaseManager.GetInstance.FirebaseLogEvent("WM_Coin_Reward", paramater);
//                        break;
//                    case ERewardType.ROULETTE:
//                        paramater.Add("AdsResult", isDone.ToString());
//                        FirebaseManager.GetInstance.FirebaseLogEvent("Roulette_Reward", paramater);
//                        break;
//                    case ERewardType.SHOP:
//                        paramater.Add("AdsResult", isDone.ToString());
//                        FirebaseManager.GetInstance.FirebaseLogEvent("SHOP_Reward", paramater);
//                        break;
//                    case ERewardType.RESULT:
//                        paramater.Add("AdsResult", isDone.ToString());
//                        FirebaseManager.GetInstance.FirebaseLogEvent("Result_Reward", paramater);
//                        break;
//                    case ERewardType.PLAY_MOVE:
//                        paramater.Add("AdsNextResult", isDone.ToString());
//                        FirebaseManager.GetInstance.FirebaseLogEvent("Move_Reward", paramater);
//                        break;
//                    case ERewardType.PLAY_ITEM:
//                        paramater.Add("AdsResult", isDone.ToString());
//                        FirebaseManager.GetInstance.FirebaseLogEvent("Item_Reward", paramater);
//                        break;
//                    case ERewardType.CONTINUE:
//                        paramater.Add("AdsResult", isDone.ToString());
//                        FirebaseManager.GetInstance.FirebaseLogEvent("Continue_Reward", paramater);
//                        break;
//                    default:
//                        break;
//                }
//                if (isDone)
//                {
//                    GetReward();
//                }
//                else
//                {
//                }
//            }, RewardType);
//        }
//        else if (Application.internetReachability == NetworkReachability.NotReachable)
//        {
//            popupManager.CallTextLog(5);
//            return;
//        }
//        else
//        {
//            AdsManager.GetInstance.LoadRewardAds();
//            popupManager.CallRewardLoadingPop(this.GetComponent<RewardAdsButton>());
//            StartCoroutine(LoadReardAds());
//        }
    }

    private void GetReward()
    {
        if (PlayerData.GetInstance != null)
        {
            if (PopupList.GetInstance != null)
                switch (RewardType)
                {
                    case ERewardType.SHOP:
                    case ERewardType.RESULT:
                    case ERewardType.MAIN:
                        var obj = GameObject.Find("PopupManager").GetComponent<PopupManager>().GetCoin();

                        switch (RewardAnimType)
                        {
                            case 0:
                                obj.GetComponent<Animator>().SetTrigger("Normal");
                                break;
                            case 1:
                                obj.GetComponent<Animator>().SetTrigger("Twice");
                                break;
                            case 2:
                                obj.GetComponent<Animator>().SetTrigger("PlayBuy");
                                break;
                            case 3:
                                obj.GetComponent<Animator>().SetTrigger("PlayShop");
                                break;
                            case 4:
                                obj.GetComponent<Animator>().SetTrigger("GameClear");
                                break;
                            default:
                                Destroy(obj);
                                break;
                        }

                        PlayerData.GetInstance.Gold += RewardCoin;
                        break;
                    case ERewardType.PLAY_MOVE:
                        if (animator != null) animator.SetBool("IsRewarded", true);
                        break;
                    case ERewardType.PLAY_ITEM:
                        BlockManager.GetInstance.AddStartItem(ranNum);
                        break;
                    case ERewardType.CONTINUE:
                        popupManager.RewardContinueStage();
                        break;
                }

            //AdsManager.GetInstance.IsShowRewardAD = true;
            SetActiveMode(false);
        }
    }

    public void CancelReward()
    {
        if (animator != null) animator.SetBool("AdsLoading", false);
        StopAllCoroutines();
    }

    public void CloseRewardPopup()
    {
        if (animator != null) animator.SetBool("AdsLoading", false);
    }

    private IEnumerator LoadReardAds()
    {
        if (animator != null) animator.SetBool("AdsLoading", true);
        var WaitTime = 0.0f;
        yield return new WaitForEndOfFrame();
        //while (WaitTime < 5.0f)
        //{
        //    WaitTime += Time.deltaTime;

        //    if (AdsManager.GetInstance.IsNewVideoLoaded(RewardType))
        //    {
        //        for (int i = 0; i < popupManager.transform.childCount; i++)
        //        {
        //            if (popupManager.transform.GetChild(i).GetComponent<NotRewardedAdsPopup>() != null)
        //            {
        //                popupManager.transform.GetChild(i).GetComponent<NotRewardedAdsPopup>().OffPopupSetting();
        //            }
        //        }
        //        break;
        //    }
        //    yield return new WaitForEndOfFrame();
        //}
        //yield return new WaitForEndOfFrame();
        //if (!AdsManager.GetInstance.IsNewVideoLoaded(RewardType))
        //{
        //    for (int i = 0; i < popupManager.transform.childCount; i++)
        //    {
        //        if (popupManager.transform.GetChild(i).GetComponent<NotRewardedAdsPopup>() != null)
        //        {
        //            popupManager.transform.GetChild(i).GetComponent<NotRewardedAdsPopup>().LoadFail();
        //        }
        //    }
        //}
        //else
        //{
        //    ShowAds();
        //}
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator StartGetReward()
    {
        yield return new WaitForSeconds(0.1f);
        GetReward();
    }
}