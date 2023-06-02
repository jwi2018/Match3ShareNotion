using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;
using UnityEngine.UI;

public enum RouletteState
{
    Idle,
    Start,
    Ending,
    End
}

public class RoulettePopup_Dotween : PopupSetting
{
    [SerializeField] private GameObject MoreAdsButton;

    [SerializeField] private GameObject MoreNoAdsButton;

    [SerializeField] private GameObject SpinButton;
    [SerializeField] private GameObject SpinStopButton;

    [SerializeField] private GameObject SpinCloseButton;

    [SerializeField] private GameObject GetItemButton;

    [SerializeField] private GameObject RouletteBase;

    [SerializeField] private GameObject X_Button;

    [SerializeField] private List<float> ItemList;

    [SerializeField] private List<RouletteItemStatus> RouletteItems_Info;

    [SerializeField] private AnimationCurve SpinSpeedController;

    [SerializeField] private AnimationGetItem animationGetItem;

    [SerializeField] private AnimationListener _listener;

    [SerializeField] private GameObject getItemParticle;

    [SerializeField] private GameObject gobPINPivot;

    [SerializeField] private Animator animatorRoulette;

    [SerializeField] private GameObject animateTarget;

    [SerializeField] private List<GameObject> disabledObjects = new List<GameObject>();

    [SerializeField] private List<Image> effectDeco = new List<Image>();

    [SerializeField] private Ease easeRotationEnd = Ease.InOutSine;

    private bool _ShowAdsSpin;

    private float deg;
    private bool isSpining;

    private Tweener tween = null;
    private bool isStop = false;
    private int getItemType = 1;

    private Coroutine corDeco = null;

    private void StopButtonState(bool isSpin)
    {
        SpinButton.SetActiveSelf(!isSpin);
        SpinStopButton.SetActiveSelf(isSpin);
    }

    private void SpinEffectStartCoroutine(float speed)
    {
        if (null != corDeco)
        {
            StopCoroutine(corDeco);
        }
        if (speed > 0)
        {
            corDeco = StartCoroutine(RotateSpinEffect(speed));
        }
    }

    public void SpinDotween()
    {
        //SpinEffectStartCoroutine(0.5f);

        //DOTween.Kill(tween);
        //isStop = false;
        //Debug.LogWarningFormat("KKI  SpinDotween");
        StopButtonState(false);
        //tween = animateTarget.transform.DORotate(new Vector3(0, 0, -90), 75.0f, RotateMode.FastBeyond360).SetSpeedBased(true).SetEase(Ease.InSine)
        //    .OnComplete(() =>
        //    {
        //        StopButtonState(false);
        //        RotateIdling();
        //    });
    }

    private IEnumerator RotateSpinEffect(float speed)
    {
        while (true)
        {
            yield return new WaitForSeconds(speed);
            for (int i = 0; i < effectDeco.Count; i++)
            {
                effectDeco[i].enabled = !effectDeco[i].enabled;
            }
        }
    }

    private void RotateIdling()
    {
        if (isStop == false)
        {
            Debug.LogWarningFormat("KKI OnComplete");
            tween = animateTarget.transform.DORotate(new Vector3(0, 0, -360), 110.0f, RotateMode.FastBeyond360).SetSpeedBased(true).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    RotateIdling();
                });
        }
    }

    private int seletedItemIndex()
    {
        float MaxinumPersent = 0;

        foreach (var item in ItemList) MaxinumPersent += item;
        var mydiceCount = Random.Range(0, MaxinumPersent);

        var myItem = 0;

        for (var i = 0; i < ItemList.Count; i++)
        {
            mydiceCount -= ItemList[i];
            if (mydiceCount < 0)
            {
                myItem = i;
                break;
            }
        }

        return myItem;
    }

    public void SpinStart()
    {
        isStop = true;

        X_Button.SetActive(false);
        StopButtonState(true);

        if (null != tween)
            tween.Kill();

        int iSelectedItem = seletedItemIndex();
        float anglePart = Random.Range(-10f, 10f);
        float angleToItem = (iSelectedItem * 45f) + (360 * 5) + 10f;

        Debug.LogWarningFormat("KKI STart {0} :: {1}", iSelectedItem, angleToItem);

        var sprite = RouletteItems_Info[iSelectedItem].GetImage.sprite;
        var intValue = RouletteItems_Info[iSelectedItem].GetIntValue;
        var rect = RouletteItems_Info[iSelectedItem].GetImage.rectTransform;

        if (PlayerData.GetInstance != null)
        {
            DailyQuestManager.CollectMission(EDailyQuestType.USEROULETTE, 1);
            StaticScript.GiveRewardFromName(sprite.name, intValue);
        }
        SpinEffectStartCoroutine(0.2f);

        SoundManager.GetInstance.Play("RouletteSpin");
        SpinButton.SetActiveSelf(false);
        SpinStopButton.SetActiveSelf(false);
        tween = animateTarget.transform.DORotate(new Vector3(0, 0, -angleToItem), 300.0f, RotateMode.FastBeyond360).SetSpeedBased(true).SetEase(Ease.OutQuart)
            .OnComplete(() =>
            {
                tween = animateTarget.transform.DORotate(new Vector3(0, 0, animateTarget.transform.eulerAngles.z + 10), 2.0f, RotateMode.FastBeyond360).SetEase(easeRotationEnd)
                .OnComplete(() =>
                {
                    SpinEffectStartCoroutine(0f);

                    var sprite = RouletteItems_Info[iSelectedItem].GetImage.sprite;
                    var intValue = RouletteItems_Info[iSelectedItem].GetIntValue;
                    var rect = RouletteItems_Info[iSelectedItem].GetImage.rectTransform;
                    animationGetItem.ChangeItem(sprite, intValue, rect);
                    _listener.Play(65);
                    SpinButton.SetActiveSelf(false);
                    SpinStopButton.SetActiveSelf(false);

                    //if (!_ShowAdsSpin)
                    //{
                    //    if (Application.internetReachability == NetworkReachability.NotReachable)
                    //    {
                    //        OffPopupSetting();
                    //    }
                    //    else
                    //    {
                    //        GetItemButton.SetActive(false);
                    //        MoreAdsButton.SetActive(true);
                    //    }
                    //}
                    //else
                    //{
                    //    StartCoroutine(DelayQuit(2.0f));
                    //}
                });
            });
    }

    private IEnumerator DelayQuit(float fDelay)
    {
        yield return new WaitForSeconds(fDelay);
        OffPopupSetting();
    }

    private void Start()
    {
        OnPopupSetting();
        StartCoroutine(CoInit(1.0f));
    }

    private IEnumerator CoInit(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        for (int i = 0; i < disabledObjects.Count; i++)
        {
            disabledObjects[i].SetActive(true);
        }
    }

    public override void OnPopupSetting()
    {
        FirebaseManager.GetInstance.FirebaseLogEvent("Intro_roulette_enter");
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
        deg = 360 / ItemList.Count;
    }

    public override void OffPopupSetting()
    {
        if (isSpining) return;
        GetComponent<Animator>().SetTrigger("Off");
    }

    public override void PressedBackKey()
    {
        OffPopupSetting();
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
    }

    public void CheckProgress()
    {
        if (PlayerData.GetInstance.IsDailyRoulette)
        {
            SpinButton.SetActive(false);

            if (Application.internetReachability == NetworkReachability.NotReachable)
                MoreNoAdsButton.SetActive(true);
            else
                MoreAdsButton.SetActive(true);
        }
        else
        {
            SpinDotween();
        }
    }

    public void OnClickButtonSpin()
    {
        if (isSpining) return;
        SpinButton.SetActive(false);
        //FirebaseManager.GetInstance.FirebaseLogEvent("Roulette_Daily");
        //animatorRoulette.SetTrigger("Green");

        SpinStart();
        FirebaseManager.GetInstance.FirebaseLogEvent("Roulette_spin_button");
        //StartCoroutine(SpiningRoulette());
        if (PlayerData.GetInstance != null)
        {
            PlayerData.GetInstance.IsDailyRoulette = true;
            PlayerData.GetInstance.RouletteYear = DateTime.Now.Year;
            PlayerData.GetInstance.RouletteMonth = DateTime.Now.Month;
            PlayerData.GetInstance.RouletteDay = DateTime.Now.Day;
            PlayerData.GetInstance.RouletteHour = DateTime.Now.Hour;
            PlayerData.GetInstance.RouletteMinute = DateTime.Now.Minute;
            PlayerData.GetInstance.RouletteSecond = DateTime.Now.Second;
        }
    }

    public void OnClickAdsOneMore()
    {
        ADManager.GetInstance.ShowReward(ERewardedKind.REWARD, () =>
        {
            getItemType = 2;
            var paramater = new Dictionary<string, string>();
            paramater.Add("AdsResult", true.ToString());

            StopButtonState(false);

            MoreAdsButton.SetActiveSelf(false);
            SpinDotween();

            FirebaseManager.GetInstance.FirebaseLogEvent("Roulette_ad_reward_spin_button");
            if (PlayerData.GetInstance != null)
            {
                PlayerData.GetInstance.IsAdsRoulette = true;

                PlayerData.GetInstance.RouletteYear = DateTime.Now.Year;
                PlayerData.GetInstance.RouletteMonth = DateTime.Now.Month;
                PlayerData.GetInstance.RouletteDay = DateTime.Now.Day;
                PlayerData.GetInstance.RouletteHour = DateTime.Now.Hour;
                PlayerData.GetInstance.RouletteMinute = DateTime.Now.Minute;
                PlayerData.GetInstance.RouletteSecond = DateTime.Now.Second;
            }

            _ShowAdsSpin = true;
            MoreAdsButton.SetActive(false);
            if (PlayerData.GetInstance != null) PlayerData.GetInstance.IsAdsRoulette = true;


            SpinButton.SetActive(false);
            SpinStart();
        });
    }

    public void GetItemAnimationEnd()
    {
        GetItemButton.SetActive(true);
        SpinButton.SetActiveSelf(false);
        SpinStopButton.SetActiveSelf(false);
    }

    public void OnClickGetItem()
    {
        FirebaseManager.GetInstance.FirebaseLogEvent(string.Format($"Roulette_obtainitem{getItemType}_button"));
        
        animationGetItem.gameObject.GetComponent<Animator>().SetTrigger("Off");
        GetItemButton.SetActive(false);
        X_Button.SetActive(true);
        if (null != transform.parent.GetComponent<PopupManager>())
        {
            transform.parent.GetComponent<PopupManager>().GoldRefresh();
        }
        getItemParticle.SetActive(true);
        GetComponent<AnimationListener>().Play(1);

        if (!_ShowAdsSpin)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                OffPopupSetting();
            }
            else
            {
                GetItemButton.SetActive(false);
                MoreAdsButton.SetActive(true);
            }
        }
        else
        {
            StartCoroutine(DelayQuit(1.0f));
        }
    }

    public void GetItemAnimaion_Off_End()
    {
        isSpining = false;
        animationGetItem.gameObject.SetActive(false);
    }

    public void CancelReward()
    {
        StopAllCoroutines();
    }
}