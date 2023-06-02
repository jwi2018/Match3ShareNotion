using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticePopup : PopupSetting
{
    private const int RewardAcorns = 100;
    private float HaveAcorns = 0;
    
    [SerializeField] private List<GameObject> targetDescribe;
    [SerializeField] private Toggle donotShowWeekly = null;
    [SerializeField] private Text txtNoticeAcornDescribe = null;
    [SerializeField] private Text txtNoticeEventLevelDescribe = null;
    [SerializeField] private Text txtChallengeDescribe = null;
    [SerializeField] private GameObject contestBanners = null;
    [SerializeField] private GameObject GO_btn = null;
    [SerializeField] private Text savingCoin = null;
    [SerializeField] private Text AcornsNum = null;
    [SerializeField] private GameObject Event_GO_btn = null;
    [SerializeField] private GameObject Chest_GO_btn = null;
    
    [SerializeField] private Slider Bar_Acorns;
    [SerializeField] private Text Number_Of_Acorns_InBar;

    [Header("어드벤쳐 언어마다 이미지 다르게")] 
    [SerializeField]
    private List<Image> LanguageImg = new List<Image>();
    [SerializeField] private List<Image> LanguageImg_Clicked = new List<Image>();
    [SerializeField] private List<Sprite> EnglishIMG = new List<Sprite>();
    [SerializeField] private List<Sprite> KoreanIMG = new List<Sprite>();
    [SerializeField] private List<Sprite> JapanIMG = new List<Sprite>();
    private string Language;
    
    private PopupManager popupManager;
    

    private void Start()
    {
        Language = LanguageManager.GetInstance.GetLanguages();
        StartCoroutine(NoticeGoBtn());
        OnPopupSetting();
        SetToggleState_WithoutNotify();
        Debug.Log("PlayerData.GetInstance.LastVipGetDay   :   " + PlayerData.GetInstance.LastVipGetDay);
    }

    

    public static bool IsOpenAble()
    {
        bool rIsOpening = false;

        if (PlayerPrefs.HasKey("NoticeDaily"))
        {
            string strDate = PlayerPrefs.GetString("NoticeDaily");
            DateTime saveddate = DateTime.MinValue;
            if (DateTime.TryParse(strDate, out saveddate))
            {
                //Sucess
                DateTime dateToday = DateTime.Today;
                TimeSpan span = saveddate.Subtract(dateToday);
                if (span.Days < 1)
                {
                    rIsOpening = true;
                }
                else
                {
                }
            }
        }
        else
        {
            rIsOpening = true;
        }

        return rIsOpening;
    }

    public void SetToggleState_WithoutNotify()
    {
        if (donotShowWeekly == null)
        {
            return;
        }

        if (true == IsOpenAble())
        {
            donotShowWeekly.SetIsOnWithoutNotify(false);
        }
        else
        {
            donotShowWeekly.SetIsOnWithoutNotify(true);
        }
    }

    public override void OnPopupSetting()
    {
        if(LanguageImg.Count != 0) ChangeSprite();

        if (BaseSystem.GetInstance != null)
        {
            if (BaseSystem.GetInstance.GetSystemList("CircusSystem"))
            {
                StartCoroutine(AcornsSilder());
            }
        }

        popupManager = GameObject.Find("PopupManager").GetComponent<PopupManager>();
        FirebaseManager.GetInstance.FirebaseLogEvent(string.Format("notice_enter"));
        //
        //string date = dateToday.ToString();
        if (txtNoticeAcornDescribe != null) txtNoticeAcornDescribe.text = I2.Loc.LocalizationManager.GetTermTranslation("NoticeAcornDescribe");

        string i2NoticeEventLevel = I2.Loc.LocalizationManager.GetTermTranslation("NoticeEventLevelDescribe");
        if (!string.IsNullOrEmpty(i2NoticeEventLevel))
        {
            string strNoticeEventLevel = string.Format(i2NoticeEventLevel, StaticGameSettings.iLimitStageEventLevel);
            if (txtNoticeEventLevelDescribe != null) txtNoticeEventLevelDescribe.text = strNoticeEventLevel;
        }

        string i2NoticeChallenge = I2.Loc.LocalizationManager.GetTermTranslation("NoticeChallengeDescribe");
        if (!string.IsNullOrEmpty(i2NoticeEventLevel))
        {
            string strNoticeChallenge = string.Format(i2NoticeChallenge, 200);
            if (txtChallengeDescribe != null) txtChallengeDescribe.text = strNoticeChallenge;
        }

        //서커스
        if (savingCoin != null) savingCoin.text = string.Format("{0:+#,0;-#,0;0}", PlayerData.GetInstance.SavingCoin);
        if (AcornsNum != null) AcornsNum.text = $"{PlayerData.GetInstance.Acorn} / {AcornSystem.GetInstance.RewardedStandardValue}";

        if (Event_GO_btn != null)
        {
            if (PlayerData.GetInstance.PresentLevel < StaticGameSettings.iLimitStageEventLevel)
            {
                Event_GO_btn.SetActive(false);
            }
        }
        
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
    }
    
    private IEnumerator NoticeGoBtn()
    {
        while (true)
        {
            if (Chest_GO_btn != null)
            {
                if (GameVariable.GetRemainStarCount() < GameVariable.GetNeedOpenStarCount())
                {
                    Chest_GO_btn.SetActive(false);
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    public override void PressedBackKey()
    {
        OffPopupSetting();
    }
    
    public override void OffPopupSetting()
    {
        //Destroy(gameObject);
        if (BaseSystem.GetInstance != null)
        {
            if (BaseSystem.GetInstance.GetSystemList("CircusSystem"))
            {
                StopCoroutine(AcornsSilder());
            }
        }
        StopCoroutine(NoticeGoBtn());
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
        GetComponent<Animator>().SetTrigger("Off");
    }

    public void SelectedIndex(int idx)
    {
        Debug.LogWarningFormat("KKI {0}", idx);
        switch (idx)
        {
            case 0:
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent(string.Format("Notice_01"));
                break;

            case 1:
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent(string.Format("Notice_02"));

                break;

            case 2:
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent(string.Format("Notice_03"));
                break;
        }

        if (contestBanners != null)
        {
            contestBanners.SetActive(false);
        }
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ButtonPush");
        for (int i = 0; i < targetDescribe.Count; i++)
        {
            if (i == idx)
            {
                targetDescribe[i].SetActiveSelf(true);
            }
            else
            {
                targetDescribe[i].SetActiveSelf(false);
            }
        }

        if (idx.Equals(1))
        {
            if (GO_btn != null)
            {
                if (PlayerData.GetInstance.PresentLevel < StaticGameSettings.iLimitStageEventLevel)
                {
                    GO_btn.SetActive(false);
                }
            }
        }
    }

    public void SelectIndex(int idx)
    {
        switch (idx)
        {
            case 0:
                //골드
                FirebaseManager.GetInstance.FirebaseLogEvent("intro_piggy_enter");
                popupManager.OnClickSavingBox();
                break;

            case 1:
                //도장
                FirebaseManager.GetInstance.FirebaseLogEvent("intro_bonus_hat_enter");
                popupManager.OnClickAcorns();
                break;

            case 2:
                //이벤트레벨
                FirebaseManager.GetInstance.FirebaseLogEvent("intro_eventgame_enter");
                popupManager.OnClickEventLevelButton();
                break;

            case 3:
                //챌린지 레벨 (설명)
                FirebaseManager.GetInstance.FirebaseLogEvent("intro_hardmode_enter");
                if (contestBanners != null)
                {
                    contestBanners.SetActive(false);
                }

                targetDescribe[2].SetActiveSelf(true);
                break;

            case 4:
                // 룰렛
                FirebaseManager.GetInstance.FirebaseLogEvent("intro_roullet_enter");
                popupManager.OnClickRoulette(PlayerData.GetInstance.IsDailyRoulette);
                break;

            case 5:
                // 체스트 (가진 별이 20개 이상일때)
                FirebaseManager.GetInstance.FirebaseLogEvent("intro_treasurebox_enter");
                if (BaseSystem.GetInstance.GetSystemList("CircusSystem"))
                {
                    if (GameVariable.GetRemainStarCount() >= GameVariable.GetNeedOpenStarCount())
                    {
                        popupManager.OnClickStarChest();
                    }
                }
                else
                {
                    popupManager.OnClickStarChestGameObject();
                }
                break;

            case 6:
                // VIP

                break;
        }
    }

    public void DisableDescribe()
    {
        if (contestBanners != null)
        {
            contestBanners.SetActive(true);
        }
        for (int i = 0; i < targetDescribe.Count; i++)
        {
            targetDescribe[i].SetActiveSelf(false);
        }
    }

    public void ToggleDonotShowDaily(bool isOn)
    {
        Debug.LogWarningFormat("KKI {0}", isOn);
        if (isOn)
        {
            DateTime dateToday = DateTime.Today;
            //int daysUntilMonday = ((int)DayOfWeek.Monday - (int)dateToday.DayOfWeek + 7) % 7;
            //DateTime nextMonday = dateToday.AddDays(daysUntilMonday);
            DateTime nextDay = dateToday.AddDays(1);
            PlayerPrefs.SetString("NoticeDaily", nextDay.ToString());
        }
        else
        {
            DateTime dateInit = DateTime.MinValue;
            PlayerPrefs.SetString("NoticeDaily", dateInit.ToString());
        }
    }

    public void GoToEventLevel()
    {
        EventLevelStatus.StaticShowPopup();
    }

    public void GoToAcorn()
    {
        PopupManager.instance.OnClickAcorns();
    }

    private IEnumerator AcornsSilder()
    {
        while (true)
        {
            HaveAcorns = PlayerData.GetInstance.Acorn;
            
            Bar_Acorns.value = HaveAcorns / RewardAcorns;
            Number_Of_Acorns_InBar.text = $"{HaveAcorns} / {RewardAcorns}";
            
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    public void ChangeSprite()
    {
        for (int i = 0; i < LanguageImg.Count; i++)
        {
            if (Language.Equals("English"))
            {
                LanguageImg[i].sprite = EnglishIMG[i];
                LanguageImg_Clicked[i].sprite = EnglishIMG[i];
            }
            else if (Language.Equals("Korean"))
            {
                LanguageImg[i].sprite = KoreanIMG[i];
                LanguageImg_Clicked[i].sprite = KoreanIMG[i];
            }
            else if ((Language.Equals("Japanese")))
            {
                LanguageImg[i].sprite = JapanIMG[i];
                LanguageImg_Clicked[i].sprite = JapanIMG[i];
            }
        }
    }
}