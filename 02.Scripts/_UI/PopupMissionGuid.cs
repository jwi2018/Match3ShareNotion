using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupMissionGuid : PopupSetting
{
    [SerializeField] private List<Image> _mImages;

    [SerializeField] private List<Text> _mTexts;

    [SerializeField] private float normalDelayTimeLimit = 1.4f;

    [SerializeField] private GameObject adsButton;
    [SerializeField] private Animator animator;

    private bool _isTouchable;
    private float delayTime;
    private float normalDelayTime;

    private bool isClickAds;

    public void Start()
    {
        _isTouchable = true;
        StartCoroutine(TouchableCheck());
        //if (StageManager.GetInstance.TutorialCheck())
        //    StartCoroutine(TutorialWaitTime());
        //if (!transform.parent.GetComponent<PopupManager>().SerchTutorialStage(StageManager.StageNumber))
        //    adsButton.SetActive(false);

        LogicManager.GetInstance.ChangeLogicState(new LogicStates.WaitUserInputLogic());
        AnimationManager.AutoTargetBombCount = 0;
        OnPopupSetting();
    }

    public override void OnPopupSetting()
    {
        var missions = StageManager.GetInstance.GetMissions();
        var obj = GameObject.Find("SpriteContainer");
        if (obj != null)
        {
            var container = obj.GetComponent<SpriteContainer>();
            if (container != null)
                for (var i = 0; i < missions.Count; i++)
                {
                    var sprite = container.GetMissionImage(missions[i].ID);
                    if (sprite == null) continue;

                    if (missions[i].ID == EID.NORMAL)
                        sprite = container.GetSpriteOrNull(missions[i].ID, missions[i].color, 1);
                    var intValue = missions[i].count;
                    _mImages[i].sprite = sprite;
                    _mTexts[i].text = "X " + intValue;
                }
        }

        for (var i = 0; i < _mImages.Count; i++)
            if (i >= missions.Count)
                _mImages[i].gameObject.SetActive(false);
        if (BlockManager.GetInstance != null) BlockManager.GetInstance.IsSwapAble = false;
        if (EditorAutoModeControll._isAutoMode)
            FirebaseManager.GetInstance.FirebaseLogEvent("Auto_Stage_Start", "StageNumber", StageManager.StageNumber.ToString());
        else
            FirebaseManager.GetInstance.FirebaseLogEvent("Stage_Start", "StageNumber", StageManager.StageNumber.ToString());

        if (EditorAutoModeControll._isAutoMode) OffPopupSetting();
    }

    public override void OffPopupSetting()
    {
        GetComponent<Animator>().SetTrigger("Off");
        if (BlockManager.GetInstance != null) BlockManager.GetInstance.IsSwapAble = true;
    }

    private void OnDisable()
    {
        if (BlockManager.GetInstance != null) BlockManager.GetInstance.IsSwapAble = true;
    }

    public override void OnButtonClick()
    {
    }

    public override void PressedBackKey()
    {
        OnClickScreen();
    }

    public void OnClickScreen()
    {
        if (_isTouchable) return;
        _isTouchable = true;
        OffPopupSetting();
    }

    public void SerchTutorial()
    {
        if (GetComponent<Animator>() != null)
            if (GetComponent<Animator>().GetBool("IsRewarded"))
                StageManager.GetInstance.AddStartMoveCount();

        if (!isClickAds)
        {
            StageManager.GetInstance.TutorialStart();
            if (BlockManager.GetInstance != null && !StageManager.GetInstance.TutorialCheck())
            {
                BlockManager.GetInstance.IsSwapAble = true;
                //StageManager.GetInstance.ShowReward_PlayAdButton();
            }
        }
    }

    public void StartAnimation()
    {
        if (AnimationManager.GetInstance != null) AnimationManager.AnimCount++;
    }

    public void EndAnimation()
    {
        if (AnimationManager.GetInstance != null) AnimationManager.AnimCount--;
    }

    private IEnumerator TutorialWaitTime()
    {
        while (normalDelayTime < normalDelayTimeLimit)
        {
            normalDelayTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        OffPopupSetting();
    }

    private IEnumerator TouchableCheck()
    {
        while (delayTime < 0.5f)
        {
            delayTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        _isTouchable = false;
        yield return new WaitForEndOfFrame();
    }

    public void ShowAD()
    {
        //if (animator != null) animator.SetBool("AdsLoading", false);
        ADManager.GetInstance.ShowReward(ERewardedKind.REWARD, () =>
        {
            if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent(string.Format("Move_reward"));
            animator.SetBool("IsRewarded", true);
            adsButton.SetActive(false);
            isClickAds = true;
        });

        /*AD.ShowAd(ERewardedKind.QUESTCOMPLETE, () =>
        {
            animator.SetBool("IsRewarded", true);
            adsButton.SetActive(false);
        });*/
    }
}