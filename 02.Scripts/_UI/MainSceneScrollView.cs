using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MainSceneScrollView : MonoBehaviour
{
    
    [SerializeField] private ScrollRect ScrollViewObj;
    [SerializeField] private RectTransform ContentObj;
    [SerializeField] private BotShopPopup _shopPopup;

    [SerializeField] private List<GameObject> Btn_list = new List<GameObject>();
    [SerializeField] private List<ScrollRect> Scrollblockobj1 = new List<ScrollRect>();
    private Vector3 Selected_Menu_Before_Move = new Vector3(0f, 20f, 0f);
    //private float Selected_Menu_Before_Move = 53f;
    //private float Selected_Menu_After_Move = 80f;
    private Vector3 Selected_Menu_Scale = new Vector3(1.2f, 1.2f, 1f);

    public bool isCenter = false;
    private int SelectMenuNum = 2;
    private int ChildCount;
    
    private float Referencepoint;
    private float Contentinterval;
    private float MouseMoveSpeed;
    private float BtnClickSpeed;
    private float MouseDownPos;
    private float MouseUpPos;
    
    //public bool IsClick;
    //private bool IsBtnClick;
   
    
    private void Start()
    {
        SelectedMenu(SelectMenuNum);
        //간격
        ChildCount = ContentObj.childCount;
        Contentinterval = (1f / (ChildCount-1));

        //넘기는 속도
        BtnClickSpeed = (Contentinterval * 2) / 10;
        MouseMoveSpeed = (Contentinterval * 2) / 100;

        //기준점
        Referencepoint = (Contentinterval * 2) / ChildCount;

        StartCoroutine(StartMainScene());
        ScrollViewObj.horizontalNormalizedPosition = 0.5f;
        
    }


    IEnumerator StartMainScene()
    {
        while (true)
        {
            yield return null;
            
            // if (Input.GetMouseButtonDown(0))
            // {
            //     MouseDownPos = Input.mousePosition.x;
            // }
            //
            // if (Input.GetMouseButtonUp(0))
            // {
            //     MouseUpPos = Input.mousePosition.x;
            //     IsClick = true;
            // }
            
            // if (IsClick)
            // {
            //     if (!IsBtnClick)
            //     {
            //         CheckCondition();
            //     }
            //     IsClick = false;
            //     IsBtnClick = false;
            // }

            Currentlocation();
        }
    }


    public void CheckCondition()
    {
        if (MouseDownPos - MouseUpPos > 0)
        {
            LeftForRight();
        }
        else
        {
            RightForLeft();
        }
    }
    

    public void LeftForRight()
    {

        if (ScrollViewObj.horizontalNormalizedPosition < Referencepoint)
        {
            StartCoroutine(SequentialMove(Contentinterval * 0, MouseMoveSpeed, 0));
        }
        else if (ScrollViewObj.horizontalNormalizedPosition < (Contentinterval + Referencepoint))
        {
            StartCoroutine(SequentialMove(Contentinterval * 1, MouseMoveSpeed,1));
        }
        else if (ScrollViewObj.horizontalNormalizedPosition < (Contentinterval * 2 + Referencepoint))
        {
            StartCoroutine(SequentialMove(Contentinterval * 2, MouseMoveSpeed,2));
        }
        else if (ScrollViewObj.horizontalNormalizedPosition < (Contentinterval * 3 + Referencepoint))
        {
            StartCoroutine(SequentialMove(Contentinterval * 3, MouseMoveSpeed,3));
        }
        else if (ScrollViewObj.horizontalNormalizedPosition > (Contentinterval * 3 + Referencepoint))
        {
            StartCoroutine(SequentialMove(Contentinterval * 4, MouseMoveSpeed,4));
        }
    }

    public void RightForLeft()
    {

        if (ScrollViewObj.horizontalNormalizedPosition < (Contentinterval - Referencepoint))
        {
            StartCoroutine(SequentialMove(Contentinterval * 0, MouseMoveSpeed,0));
        }
        else if (ScrollViewObj.horizontalNormalizedPosition < (Contentinterval * 2 - Referencepoint))
        {
            StartCoroutine(SequentialMove(Contentinterval * 1, MouseMoveSpeed,1));
        }
        else if (ScrollViewObj.horizontalNormalizedPosition < (Contentinterval * 3 - Referencepoint))
        {
            StartCoroutine(SequentialMove(Contentinterval * 2, MouseMoveSpeed,2));
        }
        else if (ScrollViewObj.horizontalNormalizedPosition < (Contentinterval * 4 - Referencepoint))
        {
            StartCoroutine(SequentialMove(Contentinterval * 3, MouseMoveSpeed,3));
        }
        else if (ScrollViewObj.horizontalNormalizedPosition > (Contentinterval * 4 - Referencepoint))
        {
            StartCoroutine(SequentialMove(Contentinterval * 4, MouseMoveSpeed,4));
        }
    }

    IEnumerator SequentialMove(float _contentinterval, float _speed, int _num)
    {
        if (_num != SelectMenuNum)
        {
            OriginallyMenu(SelectMenuNum);
            SelectedMenu(_num);
        }


        while (true)
        {
            yield return null;

            //if (IsBtnClick) break;
            
            if (ScrollViewObj.horizontalNormalizedPosition > _contentinterval)
            {
                ScrollViewObj.horizontalNormalizedPosition -= _speed;
                if (ScrollViewObj.horizontalNormalizedPosition <= _contentinterval)
                {
                    ScrollViewObj.horizontalNormalizedPosition = _contentinterval;
                    break;
                }
            }
            else
            {
                ScrollViewObj.horizontalNormalizedPosition += _speed;
                if (ScrollViewObj.horizontalNormalizedPosition >= _contentinterval)
                {
                    ScrollViewObj.horizontalNormalizedPosition = _contentinterval;
                    break;
                }
            }
        }

        //SoftMoveScrollRecet(_num);
        // if (_num < 2)
        // {
        //     StartCoroutine(Scrollblock(_num));
        // }
        // else if (_num > 2)
        // {
        //     StartCoroutine(Scrollblock(_num-1));
        // }

    }

    IEnumerator Scrollblock(int _num)
    {
        Scrollblockobj1[_num].enabled = false;
        yield return new WaitForSeconds(1f);
        Scrollblockobj1[_num].enabled = true;
    }
    
    public void ClickNotice()
    {
        //IsBtnClick = true;
        StartCoroutine(SequentialMove(Contentinterval * 0, BtnClickSpeed,0));
        
    }

    public void ClickStage()
    {
        FirebaseManager.GetInstance.FirebaseLogEvent("intro_stage_enter");
        //IsBtnClick = true;
        StartCoroutine(SequentialMove(Contentinterval * 1, BtnClickSpeed,1));
        
    }

    public void ClickMain()
    {
        FirebaseManager.GetInstance.FirebaseLogEvent("intro_home_button");
        //IsBtnClick = true;
        StartCoroutine(SequentialMove(Contentinterval * 2, BtnClickSpeed,2));
    }
    
    public void ClickDailyQuest()
    {
        FirebaseManager.GetInstance.FirebaseLogEvent("missionquest_enter");
        //IsBtnClick = true;
        StartCoroutine(SequentialMove(Contentinterval * 3, BtnClickSpeed,3));
    }

    public void ClickShop(int _num)
    {
        FirebaseManager.GetInstance.FirebaseLogEvent("intro_package_shop_enter");
        //IsBtnClick = true;
        if (_num.Equals(0))
        {
            _shopPopup.ChangeShopList(EShopKind.PACKAGE);
        }
        else
        {
            FirebaseManager.GetInstance.FirebaseLogEvent("intro_enter_coinshop");
            _shopPopup.ChangeShopList(EShopKind.GOLD);
        }
        
        StartCoroutine(SequentialMove(Contentinterval * 4, BtnClickSpeed,4));
    }

    void SelectedMenu(int _num)
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
        switch (_num)
        {
            case 0:
                Btn_list[_num].transform.localPosition += Selected_Menu_Before_Move;
                Btn_list[_num].transform.localScale = Selected_Menu_Scale;
                SelectMenuNum = _num;
                break;
            
            case 1:
                Btn_list[_num].transform.localPosition += Selected_Menu_Before_Move;
                Btn_list[_num].transform.localScale = Selected_Menu_Scale;
                SelectMenuNum = _num;
                break;
            
            case 2:
                Btn_list[_num].transform.localPosition += Selected_Menu_Before_Move;
                Btn_list[_num].transform.localScale = Selected_Menu_Scale;
                SelectMenuNum = _num;
                break;
            
            case 3:
                Btn_list[_num].transform.localPosition += Selected_Menu_Before_Move;
                Btn_list[_num].transform.localScale = Selected_Menu_Scale;
                SelectMenuNum = _num;
                break;
                
            case 4:
                Btn_list[_num].transform.localPosition += Selected_Menu_Before_Move;
                Btn_list[_num].transform.localScale = Selected_Menu_Scale;
                SelectMenuNum = _num;
                break;
        }
    }

    void OriginallyMenu(int _num)
    {
        Btn_list[_num].transform.localPosition -= Selected_Menu_Before_Move;
        Btn_list[_num].transform.localScale = Vector3.one;
    }

    void SoftMoveScrollRecet(int _num)
    {
        if (ScrollViewObj.horizontalNormalizedPosition.Equals(Contentinterval*0))
        {
            StartCoroutine(Scrollblock(_num));
        }
        else if (ScrollViewObj.horizontalNormalizedPosition.Equals(Contentinterval*1))
        {
            StartCoroutine(Scrollblock(_num));
        }
        else if (ScrollViewObj.horizontalNormalizedPosition.Equals(Contentinterval*3))
        {
            StartCoroutine(Scrollblock(_num-1));
        }
        else if (ScrollViewObj.horizontalNormalizedPosition.Equals(Contentinterval*4))
        {
            StartCoroutine(Scrollblock(_num-1));
        }
    }

    void Currentlocation()
    {
        if (ScrollViewObj.horizontalNormalizedPosition.Equals(0.5f))
        {
            isCenter = true;
        }
        else
        {
            isCenter = false;
        }
    }
}

