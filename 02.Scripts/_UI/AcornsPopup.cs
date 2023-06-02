using System;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AcornsPopup : PopupSetting
{
    private const int RewardAcorns = 100;
    private float HaveAcorns = 0;

    [SerializeField] private Text Number_Of_Acorns;

    [SerializeField] private Slider Bar_Acorns;
    [SerializeField] private Text Number_Of_Acorns_InBar;

    [SerializeField] private GameObject Receive_btn;
    [SerializeField] private GameObject Receive_btn_Over100 = null;
    [SerializeField] private GameObject Home_Btn;

    [SerializeField] private GameObject Pop_Acorns_Full_Reward;

    [SerializeField] private GameObject acornImageLevelFolder;

    [Range(0f, 100f)] [SerializeField] private float speed;

    private List<GameObject> acornLevels = new List<GameObject>();

    private int preLevel = 0;

    private Action eventOpenAcornCount;

    private void Start()
    {
        HaveAcorns = PlayerData.GetInstance.Acorn;
        //HaveAcorns = 100;

        if (!BaseSystem.GetInstance.GetSystemList("CircusSystem"))
        {
            var acornLevelImagesValue = acornImageLevelFolder.transform.childCount;

            for (int i = 0; i < acornLevelImagesValue; i++)
            {
                acornLevels.Add(acornImageLevelFolder.transform.GetChild(i).gameObject);
            }
        }

        OnPopupSetting();
    }

    public void SetOffAction(Action e)
    {
        this.eventOpenAcornCount = e;
        
    }

    public override void OnPopupSetting()
    {
        if (BaseSystem.GetInstance != null)
        {
            if (BaseSystem.GetInstance.GetSystemList("CircusSystem"))
            {
                FirebaseManager.GetInstance.FirebaseLogEvent("intro_enter_bonus_hat");
            }
        }
        else
        {
            FirebaseManager.GetInstance.FirebaseLogEvent("acorn_enter");
        }
        
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
        //이미지, 게이지, 개수 확인
        CheckAcornsObj();
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ButtonPush");
    }

    public override void PressedBackKey()
    {
        OffPopupSetting();
    }
    public override void OffPopupSetting()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
        GetComponent<Animator>().SetTrigger("Off");
    }

    private void CheckAcornsObj()
    {
        Color color = new Color(255, 255, 255, 255);

        Home_Btn.SetActive(false);
        Receive_btn.SetActive(false);
        Pop_Acorns_Full_Reward.SetActive(false);

        preLevel = PlayerData.GetInstance.BeforeAcornGettingLevel;

        var level = AccGettingLevel();

        for (int i = 0; i < (preLevel > level ? level : preLevel); i++)
        {
            acornLevels[i].SetActive(true);
            acornLevels[i].GetComponent<Image>().color = color;
        }

        if (level > preLevel)
        {
            StartCoroutine(NewLevelCoroutine(preLevel, level));
        }

        //텍스트
        if (HaveAcorns < RewardAcorns)
        {
            Number_Of_Acorns.text = string.Format(I2.Loc.LocalizationManager.GetTermTranslation("AcornNotEnough"), RewardAcorns - HaveAcorns);
            //Number_Of_Acorns.text = $"도토리 '{RewardAcorns - HaveAcorns}'개가 필요합니다. \n 남은 도토리를 더 획득해주세요!";
            Receive_btn.SetActive(false);
            Home_Btn.SetActive(true);
            if (Receive_btn_Over100 != null) Receive_btn_Over100.SetActive(false);
        }
        else
        {
            Number_Of_Acorns.text = I2.Loc.LocalizationManager.GetTermTranslation("AcornFull");
            //Number_Of_Acorns.text = "축하합니다. 유리병이 꽉 찼습니다! \n 선물을 획득하세요.";
            Receive_btn.SetActive(true);
            Home_Btn.SetActive(false);
            if (Receive_btn_Over100 != null) Receive_btn_Over100.SetActive(true);
        }

        //게이지
        Bar_Acorns.value = HaveAcorns / RewardAcorns;
        Number_Of_Acorns_InBar.text = $"{HaveAcorns} / {RewardAcorns}";
    }

    public void PlayReceiveAnim()
    {
        if (Receive_btn_Over100 != null)
        {
            Receive_btn_Over100.SetActive(false);
        }

        var a = GetComponent<Animator>().parameters;
        foreach (var item in a)
        {
            if (item.name == "Receive")
            {
                GetComponent<Animator>().SetTrigger("Receive");
            }
        }
    }

    public void ReceiveItem()
    {
        // var a = GetComponent<Animator>().parameters;
        // foreach (var item in a)
        // {
        //     if (item.name == "Receive")
        //     {
        //         GetComponent<Animator>().SetTrigger("Receive");
        //     }
        // }

        if (HaveAcorns < RewardAcorns)
        {
            Debug.LogWarningFormat(string.Format("KKI 도토리 버그 확인용 {0}/{1}", HaveAcorns, RewardAcorns));
            return;
        }

        Receive_btn.SetActive(false);

        Pop_Acorns_Full_Reward.SetActive(true);

        PlayerData.GetInstance.Acorn = (int)HaveAcorns - RewardAcorns;

        PlayerData.GetInstance.Gold += 1000;
        PlayerData.GetInstance.ItemHammer += 1;
        PlayerData.GetInstance.ItemBomb += 1;
        PlayerData.GetInstance.ItemColor += 1;

        var popupManager = GameObject.Find("PopupManager").GetComponent<PopupManager>();
        if (popupManager != null)
        {
            popupManager.GoldRefresh();
        }
        
        if (BaseSystem.GetInstance != null)
        {
            if (BaseSystem.GetInstance.GetSystemList("CircusSystem"))
            {
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Acorn_bonus_get");
            }
            else if(BaseSystem.GetInstance.GetSystemList("AdventuerSystem"))
            {
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Stamp_bonus_get");
            }
            else
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Acorn_bonus_get");
        }
        else
        {
            if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Acorn_bonus_get");
        }
        

        if (eventOpenAcornCount != null)
        {
            eventOpenAcornCount?.Invoke();
        }

        if (Receive_btn_Over100 != null)
        {
            GetComponent<Animator>().SetTrigger("Off");
        }
    }

    private IEnumerator NewLevelCoroutine(int preLevel, int level)
    {
        for (int i = preLevel; i < level; i++)
        {
            var animator = acornLevels[i].GetComponent<Animator>();

            if (animator == null)
            {
                Color color = Color.white;
                color.a = 0;

                var thisLevel = acornLevels[i].GetComponent<Image>();
                thisLevel.color = color;

                acornLevels[i].SetActive(true);

                while (color.a < 1)
                {
                    thisLevel.color = color;
                    color.a += color.a + ((1 / 255f) * speed) > 1 ? 1 : ((1 / 255f) * speed);

                    yield return new WaitForSeconds(1 / 255f);
                }
            }
            else
            {
                acornLevels[i].SetActive(true);
                animator.SetInteger("Level", (int)((i + 1) * 100 / (float)acornLevels.Count));
            }
        }
    }

    public int AccGettingLevel()
    {
        var haveValue = PlayerData.GetInstance.Acorn < 100 ? PlayerData.GetInstance.Acorn : 100;

        var level = haveValue / (100 / (float)acornLevels.Count);

        PlayerData.GetInstance.BeforeAcornGettingLevel = (int)level;

        return PlayerData.GetInstance.BeforeAcornGettingLevel;
    }
    
}