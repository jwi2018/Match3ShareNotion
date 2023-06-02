using System;
using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CashItemDictionary : SerializableDictionary<int, UIItemButton> { }

/*
[Serializable]
public class CashItemTutorialShowObject
{
    public GameObject skipButtton;
    public GameObject gameObject;
    public GameObject cashTutoBackImage;
}

[Serializable]
public class CashItemTutorialShowObjectList
{
    public List<CashItemTutorialShowObject> cashItemTutorialShowObjectList;
}
*/
public class CashItemTutorial : MonoBehaviour
{
    [SerializeField] private Image cashTutoBackImage;
    [SerializeField] private GameObject SkipButton;
    [SerializeField] private CashItemDictionary cashItemList;
    [SerializeField] private RectTransform textboxRectTransform;
    [SerializeField] private RectTransform HandRectTransform;
    [SerializeField] private GameObject TempCashItemHighLigth;
    [SerializeField] private Localize tutoComponent;

    private string cashItemTutoText;
    //[SerializeField] private CashItemTutorialShowObjectList _itemTutorialShowObjectList;
    /// <summary>
    /// DataContainer에서 Json파일의 데이터를 받아와 초기 값 설정.
    /// </summary>
    private int tutorialClick;
    public void Init()
    {
        tutorialClick = 0;
        var pData = DataContainer.GetInstance.GetCashItemTutorialDataOrNull(StageManager.StageNumber);
        if (pData == null) return;
        
        cashItemTutoText = pData.explanation;
        SetCashItemTutorialShowObject(false);
        SetCashItemHighLigth(false);
    }

    /// <summary>
    /// 튜토리얼 시작 시
    /// </summary>
    public void StartTutorial()
    {
        SetCashItemTutorialShowObject(true);
        tutoComponent.SetTerm(cashItemTutoText);
        SetCashItemHighLigth(true, StageManager.StageNumber);
    }

    /// <summary>
    /// 튜토리얼 종료 시
    /// </summary>
    public void EndTutorial()
    {
        /*
        if (ChallengeSystem.GetInstance != null)
        {
            tutorialClick++;
            if (tutorialClick.Equals(1))
            {
                //HandRectTransform.gameObject.SetActive(false);
                textboxRectTransform.gameObject.SetActive(false);
                SkipButton.SetActive(false);
            }
            else if (tutorialClick.Equals(2))
            {
                SetCashItemTutorialShowObject(false);
            }
        }
        else
        {
            SetCashItemTutorialShowObject(false);
        }
        */
        
        SetCashItemTutorialShowObject(false);
        
        
        /*
        tutorialClick++;
        if (tutorialClick.Equals(1))
        {
            //HandRectTransform.gameObject.SetActive(false);
            textboxRectTransform.gameObject.SetActive(false);
            SkipButton.SetActive(false);
        }
        else if (tutorialClick.Equals(2))
        {
            SetCashItemTutorialShowObject(false);
        }
        */
    }

    public void SkipButtonOnClick()
    {
        SetCashItemTutorialShowObject(false);
    }

    /// <summary>
    /// Tutorial 활성화에 따른 Object 활성화
    /// </summary>
    /// <param name="value"></param>
    public void SetCashItemTutorialShowObject(bool value)
    {
        //textboxRectTransform = cashItemList[0].gameObject.;
        HandRectTransform.gameObject.SetActive(value);
        textboxRectTransform.gameObject.SetActive(value);
        cashTutoBackImage.gameObject.SetActive(value);
        SkipButton.SetActive(value);
        gameObject.SetActive(value);
        //CashItemGroup.SetActive(!value);
        TempCashItemHighLigth.SetActive(value);
    }

    /// <summary>
    /// 스테이지에 따른 Item의 HighLigth 설정
    /// </summary>
    /// <param name="value"></param>
    /// <param name="stageNum"></param>
    public void SetCashItemHighLigth(bool value, int stageNum)
    {
        switch (stageNum)
        {
            case 9:
                cashItemList[0].gameObject.SetActive(value);
                break;
            case 16:
                cashItemList[1].gameObject.SetActive(value);
                break;
            case 24:
                cashItemList[2].gameObject.SetActive(value);
                break;
        }
    }
    
    public void SetCashItemHighLigth(bool value)
    {
        for (int i = 0; i < cashItemList.Count; i++)
        {
            cashItemList[i].gameObject.SetActive(value);
        }
    }
}