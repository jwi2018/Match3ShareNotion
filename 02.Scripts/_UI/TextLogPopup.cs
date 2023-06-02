using System;
using I2.Loc;
using UnityEngine;

public class TextLogPopup : PopupSetting
{
    [SerializeField] private Localize _localization;
    private bool IsLoadSuccess;
    
    [SerializeField] private GameObject _stageScroll;

    public override void OnPopupSetting()
    {
       
    }

    public override void OffPopupSetting()
    {
        //스테이지 버튼에 레벨 적혀있는곳
        if (IsLoadSuccess)
        {
            var Start_btn = GameObject.Find("MyStageButton_Text").GetComponent<TextContentConnector>();
            
            if (Start_btn != null)
            {
                Start_btn.LoadGameData();
            }
            
            //위치캐슬 전용
            _stageScroll = GameObject.Find("Stage Scroll");
            if (_stageScroll != null)
            {
                if (_stageScroll.activeSelf.Equals(true))
                {
                    GameObject.Find("PopupManager").GetComponent<PopupManager>().CallLoadingTutorialPop("MainScene", 100);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
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

    public void SetString(int num)
    {
        IsLoadSuccess = false;
        if (num == 0)
        {
            _localization.Term = "SuccessSave";
        }
        else if (num == 1)
        {
            _localization.Term = "SuccessLoad";
            var obj = transform.parent;
            for (var i = 0; i < obj.childCount; i++)
                if (obj.GetChild(i).GetComponent<LoadingPopup>() != null)
                {
                    obj.GetChild(i).GetComponent<LoadingPopup>().OffPopupSetting();
                    break;
                }

            obj.GetComponent<PopupManager>().GoldFix();
            
            //var list = obj.GetComponent<PopupManager>().GetStageScrollInfo;
            //list.ResettingButton();
            IsLoadSuccess = true;
            LanguageManager.GetInstance.Init();
        }
        else if (num == 2)
        {
            _localization.Term = "Failed Save";
        }
        else if (num == 3)
        {
            _localization.Term = "Failed Load";
        }
        else if (num == 4)
        {
            _localization.Term = "LoadDataSelect";
        }
        else if (num == 5)
        {
            _localization.Term = "NotConnectNetwork";
        }
        else if (num == 6)
        {
            _localization.Term = "CheckInternet";
        }
    }
}