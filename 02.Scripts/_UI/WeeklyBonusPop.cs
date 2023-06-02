using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct RewardItem
{
    public EUseItem useItem;
    public int intValue;
}

public class DailyMonthRewardItem
{
    public EDailyQuestRewardType rewardItem;
    public int intValue;
}

public class WeeklyBonusPop : PopupSetting
{
    [SerializeField] private GameObject FireParticle_1 = null;
    [SerializeField] private GameObject FireParticle_2 = null;
    
    [SerializeField] private List<Animator> DailyBonusItem_Cover;

    [SerializeField] private GameObject AdsButton;

    [SerializeField] private GameObject NoAdsButton;

    [SerializeField] private GameObject ConfirmButton;

    [SerializeField] private GameObject X_Button;

    [SerializeField] private GameObject CloseButton;

    [SerializeField] private List<Image> _DailySprite;

    [SerializeField] private List<RewardItem> _DailyBonusCount;

    [SerializeField] private List<Sprite> _RewardItemSpriteList;

    [SerializeField] private WeeklyGetItem getItem;

    [SerializeField] private List<WeeklyGetItem> LastGetItem;

    [SerializeField] private List<RewardItem> LastDayRewardItem;

    [SerializeField] private List<Image> LastDayRewardSprite;

    [SerializeField] private Slider slider_DailyMonthDay;
    [SerializeField] private List<GameObject> listLockState;
    [SerializeField] private List<GameObject> listActiveState;
    [SerializeField] private List<GameObject> listActiveAnim;

    [SerializeField] private GameObject particleCongratulation = null;

    [SerializeField] private List<GameObject> DailyBonusGetItemButton = new List<GameObject>();
    [SerializeField] private List<GameObject> CheckDailyBonusItemButton = new List<GameObject>();
    [SerializeField] private GameObject WeeklyCheckBackGround, GetDailyBonusItemButton;

    private Vector3 vecGoldTextPosition;

    private bool _backKeyable;
    private bool _close = false;
    private bool _isChecking;
    private bool _isGetCoin;
    private int _xDay;

    private int _itemRewardCheck;

    private bool isCheckFirstAnim;
    private int checkCountAnim=0;
    private void Awake()
    {
        ShowParticleCongratulation(false);
    }

    private void Start()
    {
        _itemRewardCheck = 1;
        
        if(FireParticle_1 != null) FireParticle_1.SetActive(false);
        if(FireParticle_2 != null) FireParticle_2.SetActive(false);
        
        if (SoundManager.GetInstance != null)
        {
            SoundManager.GetInstance.Play("Popup");
            PlayerData.GetInstance.SaveGameInt("IsCondtion", 1);
        }

        GameObject gobTarget = GameObject.Find("Text_Gold");
        vecGoldTextPosition = gobTarget.transform.position;

        for (int i = 0; i < listLockState.Count; i++)
        {
            listLockState[i].SetActiveSelf(true);
            listActiveState[i].SetActiveSelf(false);
            listActiveAnim[i].SetActiveSelf(false);
        }

        XDailyBonus();
        DailyMonthBoxStatus();
        DailyMonth_SetUI();
        CheckDailyMonthReward();
    }

    private void ShowParticleCongratulation(bool isShow)
    {
        if (particleCongratulation != null)
        {
            particleCongratulation.SetActiveSelf(isShow);
            if (isShow == true)
            {
                StartCoroutine(HideParticleCongratulation(4f));
            }
        }
    }

    private IEnumerator HideParticleCongratulation(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (particleCongratulation != null)
        {
            particleCongratulation.SetActiveSelf(false);
        }
    }

    private void DailyMonthBoxOpening()
    {
    }

    private void DailyMonthBoxStatus(int openingIndex = -1)
    {
        int iOpenedBox = -1;
        if (PlayerData.GetInstance.DailyMonthRewardContinueDay >= 28)
        {
            iOpenedBox = 3;
        }
        else if (PlayerData.GetInstance.DailyMonthRewardContinueDay >= 20)
        {
            iOpenedBox = 2;
        }
        else if (PlayerData.GetInstance.DailyMonthRewardContinueDay >= 15)
        {
            iOpenedBox = 1;
        }
        else if (PlayerData.GetInstance.DailyMonthRewardContinueDay >= 10)
        {
            iOpenedBox = 0;
        }

        switch (PlayerData.GetInstance.DailyMonthRewardGetDay)
        {
        }

        if (iOpenedBox != -1)
        {
            for (int i = 0; i < listLockState.Count; i++)
            {
                if (openingIndex != -1 && openingIndex == i)
                {
                    listActiveState[i].SetActiveSelf(false);
                    listLockState[i].SetActiveSelf(false);
                    listActiveAnim[i].SetActiveSelf(true);
                }
                else
                {
                    if (i <= iOpenedBox)
                    {
                        listLockState[i].SetActiveSelf(false);
                        listActiveState[i].SetActiveSelf(true);
                    }
                    else
                    {
                        listLockState[i].SetActiveSelf(true);
                        listActiveState[i].SetActiveSelf(false);
                    }
                }
            }
        }
    }

    private void DailyMonth_SetUI()
    {
        DateTime nowDate = DateTime.Now;
        nowDate = nowDate.AddDays(StaticGameSettings.TestPlusDay);

        DateTime savedDate;
        DateTime.TryParse(PlayerData.GetInstance.DailyMonthRewardDate, out savedDate);
        bool isEqualDay = savedDate.CompareDateTimes(nowDate);
        if (isEqualDay == false)
        { 
            DateTime nextDate = savedDate.AddDays(1);
            if (nextDate.CompareDateTimes(nowDate))
            {
                PlayerData.GetInstance.DailyMonthRewardContinueDay++;
            }
            else
            {
                PlayerData.GetInstance.DailyMonthRewardContinueDay = 1;
            }

            PlayerData.GetInstance.DailyMonthRewardDate = nowDate.ToString();
        }

        slider_DailyMonthDay.maxValue = 28;
        slider_DailyMonthDay.value = PlayerData.GetInstance.DailyMonthRewardContinueDay;
    }

    private void CheckDailyMonthReward()
    {
        int rewardIndex = -1;
        List<DailyMonthRewardItem> _rewardItems = new List<DailyMonthRewardItem>();
        switch (PlayerData.GetInstance.DailyMonthRewardContinueDay)
        {
            //case 1:
            //    rewardIndex = 0;
            //    _rewardItems.Add(new DailyMonthRewardItem() { rewardItem = EDailyQuestRewardType.HAMMER, intValue = 5 });
            //    break;

            case 10:
                rewardIndex = 0;
                _rewardItems.Add(new DailyMonthRewardItem() { rewardItem = EDailyQuestRewardType.HAMMER, intValue = 5 });
                break;

            case 15:
                rewardIndex = 1;
                _rewardItems.Add(new DailyMonthRewardItem() { rewardItem = EDailyQuestRewardType.BOMB, intValue = 3 });
                break;

            case 20:
                rewardIndex = 2;
                _rewardItems.Add(new DailyMonthRewardItem() { rewardItem = EDailyQuestRewardType.COLOR, intValue = 2 });
                break;

            case 28:
                rewardIndex = 3;
                _rewardItems.Add(new DailyMonthRewardItem() { rewardItem = EDailyQuestRewardType.COIN, intValue = 3000 });
                _rewardItems.Add(new DailyMonthRewardItem() { rewardItem = EDailyQuestRewardType.ALLITEM, intValue = 1 });
                break;
        }

        StartCoroutine(TakeDailyMonthReward(PlayerData.GetInstance.DailyMonthRewardContinueDay, rewardIndex, _rewardItems));
    }

    private IEnumerator TakeDailyMonthReward(int continueDay, int openingIndex, List<DailyMonthRewardItem> rewardItems)
    {
        if (PlayerData.GetInstance.DailyMonthRewardGetDay != continueDay)
        {
            PlayerData.GetInstance.DailyMonthRewardGetDay = continueDay;
            if (rewardItems.Count > 0)
            {
                yield return new WaitForSeconds(3.0f);
            }
            DailyMonthBoxStatus(openingIndex);

            foreach (DailyMonthRewardItem rewardType in rewardItems)
            {
                yield return new WaitForSeconds(1.0f);
                GameObject gobPopup = PopupManager.instance.LoadPopup(PopupList.GetInstance.Pop_GetReward);
                Pop_GetReward popReward = gobPopup.GetComponent<Pop_GetReward>();
                int showItemCount = rewardType.intValue;
                if (rewardType.rewardItem == EDailyQuestRewardType.COIN)
                {
                    showItemCount = 5;
                }
                popReward.GiveReward(rewardType.rewardItem, showItemCount);
                popReward.ShowReward(rewardType.rewardItem, showItemCount, rewardType.intValue, "GetCoin", vecGoldTextPosition);
            }
            yield return new WaitForSeconds(1.3f);
        }

        
        // 테스트용으로 잠깐 끔
        // 서커스일 경우 비활성화 , 그 외의 프로젝트는 활성화로 해야함.
        // 자동 팝업 off인데 서커스는 아이템 받고 off돼야 함.
        //StartCoroutine(PopupClose());
        if (DailyBonusGetItemButton.Count == 0 || DailyBonusGetItemButton == null)
        {
            StartCoroutine(PopupClose());
        }
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ButtonPush");
    }

    public override void PressedBackKey()
    {
        OffPopupSetting();
    }

    public void XDailyBonus()
    {
        if (PlayerData.GetInstance == null) return;
        StartCoroutine(DailyBonusCheckDelay(0.1f));
        var XDay = PlayerData.GetInstance.WeeklyXDay;
        _xDay = XDay;
        XDay++;
        if (XDay >= DailyBonusItem_Cover.Count + 1) XDay = 1;  // 7일 이후 하루가 체크되지 않음.
        PlayerData.GetInstance.WeeklyXDay = XDay;
    }

    IEnumerator DailyBonusCheckDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        var XDay = PlayerData.GetInstance.WeeklyXDay;
        
        if (BaseSystem.GetInstance != null)
        {
            if (BaseSystem.GetInstance.GetSystemList("CircusSystem") || BaseSystem.GetInstance.GetSystemList("Fantasy"))
            {
                for (var i = 0; i < DailyBonusItem_Cover.Count; i++)
                    if (i == XDay-1)
                    {
                        DailyBonusItem_Cover[i].SetTrigger("On");
                    }
                    else if(i < XDay)
                    {
                        DailyBonusItem_Cover[i].SetTrigger("OnAfterDay");
                    }
                    else if(i > XDay)
                    {
                        DailyBonusItem_Cover[i].SetTrigger("Off"); 
            
                    }
            }
            else
            {
                for (var i = 0; i < DailyBonusItem_Cover.Count; i++)
                    if (i < XDay)
                    {
                        DailyBonusItem_Cover[i].SetTrigger("On");
                    }
                    else
                    {
                        DailyBonusItem_Cover[i].SetTrigger("Off");
        
                    }
            }
        }
        
    }

    public void OnRewardClick()
    {
        //AdsManager.GetInstance.ShowNewRewardVideo((isDone) =>
        //{
        //    if (isDone)
        //    {
        //        GetReward();
        //    }
        //}, ERewardType.SHOP);
    }

    public void GetDailyBonus()
    {
        // 애니메이션에 붙어 있음
        Debug.Log("데일리 보너스 겟 스타트");
        // 테스트용 주석
        if (DailyBonusGetItemButton.Count == 0 || DailyBonusGetItemButton == null)
        {
            StartCoroutine(DailyBonusAnimation(true));
        }
        //StartCoroutine(DailyBonusAnimation());
    }

    // 테스트용으로 잠깐 만들어둠
    public void onClickGetItemButton()
    {
        Debug.Log("1");
        if (CheckDailyBonusItemButton != null && DailyBonusGetItemButton.Count != 0)
        {
            _itemRewardCheck = 1;
            StartCoroutine(DailyBonusAnimation(true));
            WeeklyCheckBackGround.SetActive(false);
            foreach (var button in DailyBonusGetItemButton)
            {
                button.SetActive(false);
            }
        }
    }

    public void onClickGetMoreItemButton()
    {
        if (CheckDailyBonusItemButton != null && DailyBonusGetItemButton.Count != 0)
        {
            _itemRewardCheck = 2;
            StartCoroutine(DailyBonusAnimation(true));
            WeeklyCheckBackGround.SetActive(false);
            GetDailyBonusItemButton.SetActive(false);
        }
    }

    public void onClickShowMoreItemButton()
    {
        // 두배로 주는 리워드 버튼 광고
        if (CheckDailyBonusItemButton != null && DailyBonusGetItemButton.Count != 0)
        {
            ADManager.GetInstance.ShowReward(ERewardedKind.REWARD, () =>
            {
                _itemRewardCheck = 2;
                foreach (var button in CheckDailyBonusItemButton)
                {
                    button.SetActive(false);
                }
                foreach (var button in DailyBonusGetItemButton)
                {
                    button.SetActive(false);
                }
                WeeklyCheckBackGround.SetActive(true);
                GetDailyBonusItemButton.SetActive(true);
                StartCoroutine(DailyBonusAnimation(false));
            });
        }
    }

    public void onClickCheckWeeklyItem()
    {
        if (CheckDailyBonusItemButton != null && DailyBonusGetItemButton.Count != 0)
        {
            foreach (var button in CheckDailyBonusItemButton)
            {
                button.SetActive(false);
            }
            WeeklyCheckBackGround.SetActive(true);
            foreach (var button in DailyBonusGetItemButton)
            {
                button.SetActive(true);
            }
            StartCoroutine(DailyBonusAnimation(false));
            //StartCoroutine(DailyBonusShowItemAnimation());
        }
    }

    public void GetReward()
    {
        _backKeyable = true;
        AdsButton.SetActive(false);
        ConfirmButton.SetActive(true);
    }

    public void AnimationEnd()
    {
        Debug.Log("5");
        _backKeyable = true;
        GetCoinButton();
    }

    public void GetCoinButton()
    {
        Debug.Log("6");
        if (!_backKeyable)
        {
            _backKeyable = true;
            var image = DailyBonusItem_Cover[_xDay].GetComponent<Image>();
            var color = new Color(0, 0, 0, 0);
            image.color = color;
            getItem.StopAllCoroutines();
            StopAllCoroutines();
        }

        if (!_isGetCoin)
        {
            _isGetCoin = true;

            if (_xDay + 1 != DailyBonusItem_Cover.Count)
            {
                if (PlayerData.GetInstance != null)
                    switch (_DailyBonusCount[_xDay].useItem)
                    {
                        case EUseItem.NONE:
                            getItem.gameObject.SetActive(false);
                            PlayerData.GetInstance.Gold += _DailyBonusCount[_xDay].intValue * _itemRewardCheck;
                            var obj = transform.parent.GetComponent<PopupManager>().GetCoin();
                            obj.GetComponent<Animator>().SetTrigger("Normal");
                            break;

                        case EUseItem.HAMMER:
                            getItem.ItemClose(0.7f);
                            PlayerData.GetInstance.ItemHammer += _DailyBonusCount[_xDay].intValue * _itemRewardCheck;
                            break;

                        case EUseItem.CROSS:
                            getItem.ItemClose(0.7f);
                            PlayerData.GetInstance.ItemCross += _DailyBonusCount[_xDay].intValue * _itemRewardCheck;
                            break;

                        case EUseItem.BOMB:
                            getItem.ItemClose(0.7f);
                            PlayerData.GetInstance.ItemBomb += _DailyBonusCount[_xDay].intValue * _itemRewardCheck;
                            break;

                        case EUseItem.COLOR:
                            getItem.ItemClose(0.7f);
                            PlayerData.GetInstance.ItemColor += _DailyBonusCount[_xDay].intValue * _itemRewardCheck;
                            break;
                    }
            }
            else
            {
                if (PlayerData.GetInstance != null)
                {
                    foreach (var item in LastDayRewardItem)
                        switch (item.useItem)
                        {
                            case EUseItem.NONE:
                                getItem.gameObject.SetActive(false);
                                PlayerData.GetInstance.Gold += item.intValue * _itemRewardCheck;
                                var obj = transform.parent.GetComponent<PopupManager>().GetCoin();
                                obj.GetComponent<Animator>().SetTrigger("Normal");
                                break;

                            case EUseItem.HAMMER:
                                PlayerData.GetInstance.ItemHammer += item.intValue * _itemRewardCheck;
                                break;

                            case EUseItem.CROSS:
                                PlayerData.GetInstance.ItemCross += item.intValue * _itemRewardCheck;
                                break;

                            case EUseItem.BOMB:
                                PlayerData.GetInstance.ItemBomb += item.intValue * _itemRewardCheck;
                                break;

                            case EUseItem.COLOR:
                                PlayerData.GetInstance.ItemColor += item.intValue * _itemRewardCheck;
                                break;
                        }

                    foreach (var obj in LastGetItem) obj.ItemClose(0.7f);
                }
            }

            ConfirmButton.SetActive(false);
        }
        
        Debug.Log("여기가 종료?");
        StartCoroutine(PopupClose());
    }

    public void OnClickAdsButton()
    {
        //AdsManager.GetInstance.ShowNewRewardVideo((isDone) =>
        //{
        //    if (isDone)
        //    {
        //        AdsButton.SetActive(false);
        //        GetCoinButton();
        //        GameObject obj = this.transform.parent.GetComponent<PopupManager>().GetCoin();
        //        obj.GetComponent<Animator>().SetTrigger("Normal");
        //        if (PlayerData.GetInstance != null)
        //        {
        //            switch (_DailyBonusCount[_xDay].useItem)
        //            {
        //                case EUseItem.NONE:
        //                    PlayerData.GetInstance.Gold += _DailyBonusCount[_xDay].intValue;
        //                    break;
        //                case EUseItem.HAMMER:
        //                    PlayerData.GetInstance.ItemHammer += _DailyBonusCount[_xDay].intValue;
        //                    break;
        //                case EUseItem.CROSS:
        //                    PlayerData.GetInstance.ItemCross += _DailyBonusCount[_xDay].intValue;
        //                    break;
        //                case EUseItem.BOMB:
        //                    PlayerData.GetInstance.ItemBomb += _DailyBonusCount[_xDay].intValue;
        //                    break;
        //                case EUseItem.COLOR:
        //                    PlayerData.GetInstance.ItemColor += _DailyBonusCount[_xDay].intValue;
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }
        //    }
        //}, ERewardType.SHOP);
    }

    public void CheckAnimationEnd()
    {
        _isChecking = true;
    }

    /*
    private IEnumerator DailyBonusShowItemAnimation()
    {
        var animator = DailyBonusItem_Cover[_xDay];
        animator.SetTrigger("DDay");
        yield return new WaitWhile(() => _isChecking == false);
        if (_xDay + 1 != DailyBonusItem_Cover.Count)
        {
            getItem.gameObject.SetActive(true);
            ShowParticleCongratulation(true);
            if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("DailyBonus");
            getItem.StartMove(_RewardItemSpriteList[(int)_DailyBonusCount[_xDay].useItem],
                _DailyBonusCount[_xDay].intValue * _itemRewardCheck, _DailySprite[_xDay].rectTransform.localPosition, 1.0f, false);
        }
        else
        {
            for (var i = 0; i < LastGetItem.Count; i++)
            {
                LastGetItem[i].gameObject.SetActive(true);
                if (SoundManager.GetInstance != null)
                    SoundManager.GetInstance.Play("DailyBonus");
                if (LastDayRewardItem[i].useItem == EUseItem.COLOR)
                    LastGetItem[i].StartMove(_RewardItemSpriteList[(int)LastDayRewardItem[i].useItem],
                        LastDayRewardItem[i].intValue * _itemRewardCheck, LastDayRewardSprite[i].rectTransform.localPosition, 1.1f, false);
                else
                    LastGetItem[i].StartMove(_RewardItemSpriteList[(int)LastDayRewardItem[i].useItem],
                        LastDayRewardItem[i].intValue * _itemRewardCheck, LastDayRewardSprite[i].rectTransform.localPosition, 1.0f,false);
            }
        }
    }

    private IEnumerator DailyBonusGetItemAnimation()
    {
        var animator = DailyBonusItem_Cover[_xDay];
        animator.SetTrigger("DDay");
        yield return new WaitWhile(() => _isChecking == false);

        if (_xDay + 1 != DailyBonusItem_Cover.Count)
        {
            getItem.StartMove(_RewardItemSpriteList[(int)_DailyBonusCount[_xDay].useItem],
                _DailyBonusCount[_xDay].intValue * _itemRewardCheck, _DailySprite[_xDay].rectTransform.localPosition);
        }
        else
        {
            for (var i = 0; i < LastGetItem.Count; i++)
            {
                if (LastDayRewardItem[i].useItem == EUseItem.COLOR)
                        LastGetItem[i].StartMove(_RewardItemSpriteList[(int)LastDayRewardItem[i].useItem],
                            LastDayRewardItem[i].intValue * _itemRewardCheck, LastDayRewardSprite[i].rectTransform.localPosition, 1.1f);
                    else
                        LastGetItem[i].StartMove(_RewardItemSpriteList[(int)LastDayRewardItem[i].useItem],
                            LastDayRewardItem[i].intValue * _itemRewardCheck, LastDayRewardSprite[i].rectTransform.localPosition);
            }
        }
        
        yield return new WaitWhile(() => _backKeyable == false);

        yield return new WaitForEndOfFrame();
    }
    */
    
    private IEnumerator DailyBonusAnimation(bool StartMoveCoroutine)
    {
        Debug.Log("2");
        if (_xDay.Equals(7)) _xDay = 0;
        var animator = DailyBonusItem_Cover[_xDay];
        
        if (isCheckFirstAnim) animator.SetTrigger("ReceiveClick");
        else animator.SetTrigger("DDay");
        
        yield return new WaitWhile(() => _isChecking == false);

        if (_xDay + 1 != DailyBonusItem_Cover.Count)
        {
            getItem.gameObject.SetActive(true);
            ShowParticleCongratulation(true);
            if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("DailyBonus");
            getItem.StartMove(_RewardItemSpriteList[(int)_DailyBonusCount[_xDay].useItem],
                _DailyBonusCount[_xDay].intValue * _itemRewardCheck, _DailySprite[_xDay].rectTransform.localPosition, 1.0f, StartMoveCoroutine);
        }
        else
        {
            for (var i = 0; i < LastGetItem.Count; i++)
            {
                LastGetItem[i].gameObject.SetActive(true);
                if (SoundManager.GetInstance != null)
                    SoundManager.GetInstance.Play("DailyBonus");
                if (LastDayRewardItem[i].useItem == EUseItem.COLOR)
                    LastGetItem[i].StartMove(_RewardItemSpriteList[(int)LastDayRewardItem[i].useItem],
                        LastDayRewardItem[i].intValue * _itemRewardCheck, LastDayRewardSprite[i].rectTransform.localPosition, 1.1f, StartMoveCoroutine);
                else
                    LastGetItem[i].StartMove(_RewardItemSpriteList[(int)LastDayRewardItem[i].useItem],
                        LastDayRewardItem[i].intValue * _itemRewardCheck, LastDayRewardSprite[i].rectTransform.localPosition, 1.0f, StartMoveCoroutine);
            }
        }

        yield return new WaitWhile(() => _backKeyable == false);

        yield return new WaitForEndOfFrame();
    }

    private IEnumerator PopupClose()
    {
        yield return new WaitForSeconds(2.8f);
        //GetComponent<Animator>().SetTrigger("Off");
        OffPopupSetting();
        yield return new WaitForEndOfFrame();
    }

    public override void OffPopupSetting()
    {
        if(FireParticle_1 != null) FireParticle_1.SetActive(true);
        if(FireParticle_2 != null) FireParticle_2.SetActive(true);
        
        if (_backKeyable)
        {
            if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
            StartCoroutine(PopupLimitedOpen());
            GetComponent<Animator>().SetTrigger("Off");
            if (!_isGetCoin)
            {
                _isGetCoin = true;

                if (PlayerData.GetInstance != null)
                    switch (_DailyBonusCount[_xDay].useItem)
                    {
                        case EUseItem.NONE:
                            getItem.gameObject.SetActive(false);
                            var obj = transform.parent.GetComponent<PopupManager>().GetCoin();
                            obj.GetComponent<Animator>().SetTrigger("Normal");
                            PlayerData.GetInstance.Gold += _DailyBonusCount[_xDay].intValue * _itemRewardCheck;
                            break;

                        case EUseItem.HAMMER:
                            getItem.ItemClose(0.7f);
                            PlayerData.GetInstance.ItemHammer += _DailyBonusCount[_xDay].intValue * _itemRewardCheck;
                            break;

                        case EUseItem.CROSS:
                            getItem.ItemClose(0.7f);
                            PlayerData.GetInstance.ItemCross += _DailyBonusCount[_xDay].intValue * _itemRewardCheck;
                            break;

                        case EUseItem.BOMB:
                            getItem.ItemClose(0.7f);
                            PlayerData.GetInstance.ItemBomb += _DailyBonusCount[_xDay].intValue * _itemRewardCheck;
                            break;

                        case EUseItem.COLOR:
                            getItem.ItemClose(0.7f);
                            PlayerData.GetInstance.ItemColor += _DailyBonusCount[_xDay].intValue * _itemRewardCheck;
                            break;
                    }
            }

            isNextLimited = true;
            if (Application.internetReachability != NetworkReachability.NotReachable)
                transform.parent.GetComponent<PopupManager>().CallLimitedShop();
        }
    }

    bool isNextLimited = false;
    
    /*
    public override void OnDestroy()
    {
        if (isNextLimited == true)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                transform.parent.GetComponent<PopupManager>().CallLimitedShop();
                
                if (LimitedShopController.GetInstance != null)
                {
                    PlayerData.GetInstance.IsWeeklyPopupEnd = true;
                    var myDt = DateTime.Now;
                    if (myDt.Hour >= LimitedShopController.GetInstance.Time)
                    {
                        PlayerData.GetInstance.IsLimitedPopupCheck = true;
                    }
                }
                
            }
        }
    }*/
  
    public void ActiveFalse()
    {
    }
    private IEnumerator PopupLimitedOpen()
    {
        yield return new WaitForSeconds(0.2f);
    }

    public void CheckFirstAnimEnd()
    {
        checkCountAnim++;
        if (checkCountAnim == 1)
        {
            isCheckFirstAnim = true;
            checkCountAnim = 0;
        }
        
    }
}