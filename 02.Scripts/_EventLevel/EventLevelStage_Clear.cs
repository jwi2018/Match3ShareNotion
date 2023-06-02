using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventLevelStage_Clear : PopupSetting
{
    private PopupManager popupManager;
    [SerializeField] private Text title;
    [SerializeField] private Text Stagescript;
    [SerializeField] private GameObject Home_BTN;
    [SerializeField] private GameObject NextStage_BTN;
    [SerializeField] private GameObject Receive_BTN;
    [SerializeField] private GameObject ADIMG;
    [SerializeField] private GameObject ItemIMG;
    [SerializeField] private GameObject X_BTN;

    [SerializeField] private Text txtClearRewardGold;
    [SerializeField] private Text txtClearRewardHammer;
    [SerializeField] private Text txtClearRewardBomb;
    [SerializeField] private Text txtClearRewardRainbow;
    [SerializeField] private Text txtClearRewardAcorn;

    [SerializeField] private int IClearRewardGold = 1000;
    [SerializeField] private int IClearRewardHammer = 2;
    [SerializeField] private int IClearRewardBomb = 2;
    [SerializeField] private int IClearRewardRainbow = 2;
    [SerializeField] private int IClearRewardAcorn = 5;

    [SerializeField] private GameObject objParticle = null;

    private void Start()
    {
        popupManager = transform.parent.GetComponent<PopupManager>();
        StaticScript.SetActiveCheckNULL(objParticle, false);
        OnPopupSetting();
    }

    public override void OnPopupSetting()
    {
        if (SoundManager.GetInstance != null)
        {
            SoundManager.GetInstance.PauseBGM();
            SoundManager.GetInstance.Play("ClearBravo");
        }

        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent(string.Format($"{EventLevelSystem.GetInstance.EventLevelNum}_event_game_clear"));

        if (txtClearRewardGold != null)
        {
            txtClearRewardGold.text = string.Format("x {0:#,##0}", IClearRewardGold);
            txtClearRewardHammer.text = string.Format("x {0:#,##0}", IClearRewardHammer);
            txtClearRewardBomb.text = string.Format("x {0:#,##0}", IClearRewardBomb);
            txtClearRewardRainbow.text = string.Format("x {0:#,##0}", IClearRewardRainbow);
            txtClearRewardAcorn.text = string.Format("x {0:#,##0}", IClearRewardAcorn);
        }
        StartCoroutine(DelayStartCongratulation());
        if (StageManager.GetInstance == null)
        {
            return;
        }

        StageManager.GetInstance.SetSkipText(false);

        if (EventLevelSystem.GetInstance != null)
        {
            ChangePopup(6);
            Stagescript.text = I2.Loc.LocalizationManager.GetTermTranslation("EventLevelClear5");
            PlayerData.GetInstance.SaveIsEventMapAllClear(true);
        }
    }

    private IEnumerator DelayStartCongratulation()
    {
        yield return new WaitForSeconds(1.5f);
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ClearFirework");
        StaticScript.SetActiveCheckNULL(objParticle, true);
    }

    private void ChangePopup(int _stage)
    {
        if (_stage < 5)
        {
            //ItemIMG.SetActive(false);
            StaticScript.SetActiveCheckNULL(X_BTN, true);

            StaticScript.SetActiveCheckNULL(Home_BTN, true);
            StaticScript.SetActiveCheckNULL(NextStage_BTN, true);
            StaticScript.SetActiveCheckNULL(Receive_BTN, false);
        }
        else
        {
            //ItemIMG.SetActive(true);
            StaticScript.SetActiveCheckNULL(X_BTN, false);

            StaticScript.SetActiveCheckNULL(Home_BTN, false);
            StaticScript.SetActiveCheckNULL(NextStage_BTN, false);
            StaticScript.SetActiveCheckNULL(Receive_BTN, true);
        }
    }

    public override void OffPopupSetting()
    {
        GetComponent<Animator>().SetTrigger("Off");
    }

    public override void PressedBackKey()
    {
        OffPopupSetting();
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ButtonPush");
    }

    public void OnClickGoMain()
    {
        if (EventLevelSystem.GetInstance != null)
        {
            EventLevelSystem.GetInstance.IsEventLevel = false;
            PopupManager.instance.ShowEventLevelRetryPopup();
        }
    }

    public void OnClickNextStage()
    {
        if (EventLevelSystem.GetInstance != null)
        {
            EventLevelSystem.GetInstance.IsEventLevel = true;
            EventLevelSystem.GetInstance.EventLevelNum = PlayerData.GetInstance.GetEventStageNum();
            popupManager.CallLoadingTutorialPop("GameScene");
        }
    }

    public void ReceviceItem()
    {
        if (EventLevelSystem.GetInstance != null)
        {
            FirebaseManager.GetInstance.FirebaseLogEvent(string.Format("eventgame_reward_get"));

            StaticScript.SetActiveCheckNULL(Receive_BTN, false);
            StartCoroutine(DelayGodoMainScene());
            //ClearReceviceItem();
            EventLevelSystem.GetInstance.IsEventLevel = false;

            StartCoroutine(DelayGiveReward());
            //StartCoroutine(DelayGodoMainScene());
            ClearReceviceItem();
        }
    }

    private IEnumerator DelayGiveReward()
    {
        GameObject gobTarget = GameObject.Find("Move");
        if (null != gobTarget)
        {
            Vector3 vecTargetPosition = gobTarget.transform.position;

            RewardSpawn(txtClearRewardGold.transform.position, vecTargetPosition, EDailyQuestRewardType.COIN, IClearRewardGold / 200, IClearRewardGold);
            yield return new WaitForSeconds(0.3f);
            RewardSpawn(txtClearRewardHammer.transform.position, vecTargetPosition, EDailyQuestRewardType.HAMMER, 1, IClearRewardHammer);
            yield return new WaitForSeconds(0.3f);
            RewardSpawn(txtClearRewardBomb.transform.position, vecTargetPosition, EDailyQuestRewardType.BOMB, 1, IClearRewardBomb);
            yield return new WaitForSeconds(0.3f);
            RewardSpawn(txtClearRewardRainbow.transform.position, vecTargetPosition, EDailyQuestRewardType.COLOR, 1, IClearRewardRainbow);
            yield return new WaitForSeconds(0.3f);
            RewardSpawn(txtClearRewardAcorn.transform.position, vecTargetPosition, EDailyQuestRewardType.ACORN, IClearRewardAcorn, IClearRewardAcorn);
        }
    }

    private void RewardSpawn(Vector3 vecSpawnPos, Vector3 vecTargetPos, EDailyQuestRewardType rewardType, int spawnAmount, int iValue)
    {
        GameObject gobPopup = PopupManager.instance.LoadPopup(PopupList.GetInstance.Pop_GetReward);
        if (gobPopup != null)
        {
            gobPopup.transform.position = vecSpawnPos;
            Pop_GetReward popReward = gobPopup.GetComponent<Pop_GetReward>();
            popReward.ShowReward(rewardType, spawnAmount, spawnAmount, "GetCoin", vecTargetPos);
        }
    }

    private IEnumerator DelayGodoMainScene()
    {
        yield return new WaitForSeconds(3.0f);
        popupManager.CallLoadingTutorialPop("MainScene", 100);
    }

    public void ClearReceviceItem()
    {
        if (PlayerData.GetInstance.GetIsEventMapAllClear())
        {
            // 보상 정해주기.
            PlayerData.GetInstance.Gold += IClearRewardGold;
            PlayerData.GetInstance.ItemHammer += IClearRewardHammer;
            PlayerData.GetInstance.ItemBomb += IClearRewardBomb;
            PlayerData.GetInstance.ItemColor += IClearRewardRainbow;
            PlayerData.GetInstance.Acorn += IClearRewardAcorn;
        }
    }
}