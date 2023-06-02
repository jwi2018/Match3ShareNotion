using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarChestStatus : Singleton<StarChestStatus>
{
    [SerializeField] private GameObject ChestImage;

    [SerializeField] private Image StarGauge;

    [SerializeField] private Text StarText;

    [SerializeField] private PopupManager popupManager;

    [SerializeField] private Button chestButton;

    [SerializeField] private GameObject boxParticle;

    [SerializeField] private Animator chestAnimator;

    [SerializeField] private Transform starPosition;

    [SerializeField] private GameObject baseChest = null;
    [SerializeField] private GameObject gobAlram = null;
    [SerializeField] private GameObject gobActive = null;
    
    [SerializeField] private Slider sliderTresureBox;
    [SerializeField] private Text textTresureBox;

    [SerializeField] private GameObject checkButton_Dis;

    private int _boxColumnCount;

    private int _myStar = 0;
    private int _needStar = 0;

    private int _totalStar;
    private int showRemainStarCount;

    private int showStarCount;

    public Transform GetStarPosition => starPosition;

    Coroutine coUpdate = null;
    private bool sliderCoroutine;

    // Use this for initialization

    private void Start()
    {
        ActivatedReward(false);
        _totalStar = PlayerData.GetInstance.GetTotalStarCount();
        _boxColumnCount = PlayerData.GetInstance.StarBoxOpenCount;
        SwitchingBoxButton();


        if (BaseSystem.GetInstance.GetSystemList("CircusSystem") || BaseSystem.GetInstance.GetSystemList("Fantasy"))
        {
            sliderCoroutine = true;
            StartCoroutine(SliderCoroutine());
        }

        coUpdate = StartCoroutine(DelayUpdate());
    }

    private void ActivatedReward(bool isActivated)
    {
        if (null != baseChest) baseChest.SetActiveSelf(!isActivated);
        if (null != gobAlram) gobAlram.SetActiveSelf(isActivated);
        if (null != gobActive) gobActive.SetActiveSelf(isActivated);
    }

    public void ReactUpdate()
    {
        if(null != coUpdate)
        {
            StopCoroutine(coUpdate);
        }
        coUpdate = StartCoroutine(DelayUpdate());
    }

    private IEnumerator DelayUpdate()
    {
        while (true)
        {
            if (PlayerData.GetInstance != null)
            {

                

                    if (_boxColumnCount != PlayerData.GetInstance.StarBoxOpenCount)
                {
                    _boxColumnCount = PlayerData.GetInstance.StarBoxOpenCount;
                    //_boxColumnCount = 10000;
                    SwitchingBoxButton();
                }
            }
            yield return new WaitForSeconds(0.4f);
        }
    }

    public void SliderTresureBoxValue()
    {
        _boxColumnCount = PlayerData.GetInstance.StarBoxOpenCount;
        //_boxColumnCount = 10000;
        SwitchingBoxButton();
    }

    public void OpenStarChest()
    {
        if (popupManager == null)
        {
            var obj = GameObject.Find("PopupManager");
            if (obj != null)
            {
                popupManager = obj.GetComponent<PopupManager>();
            }
        }

        var gameObject = transform.parent.GetComponent<PopupManager>().OnClickStarChestGameObject();
        StarChestPopup starChest = gameObject.GetComponent<StarChestPopup>();
        if (starChest != null)
        {
            starChest.SetOffAction(SliderTresureBoxValue);
        }


        //if (popupManager != null)
        //{
        //    popupManager.OnClickStarChest();
        //}
        //else
        //{
        //    var obj = GameObject.Find("PopupManager");
        //    if (obj != null)
        //    {
        //        popupManager = obj.GetComponent<PopupManager>();
        //        popupManager.OnClickStarChest();

        //        var gameObject = transform.parent.GetComponent<PopupManager>().OnClickStarChestGameObject();
        //        StarChestPopup starChest = gameObject.GetComponent<StarChestPopup>();
        //        if (starChest != null)
        //        {
        //            starChest.SetOffAction(SliderTresureBoxValue);
        //        }
        //    }
        //}
    }

    public void GetStar()
    {
        showStarCount++;
        if (StarText != null)
        {
            StarText.text = "" + showStarCount + " / " + showRemainStarCount;
        }
        Instantiate(boxParticle, starPosition);
        //animator.SetTrigger("InStar");
        if (showStarCount > 19)
        {
            if (chestAnimator != null)
            {
                chestAnimator.SetBool("isMove", true);
            }
        }
    }

    private void SwitchingBoxButton()
    {
        var NeedStar = 0;
        var RemainStar = 0;
        var moreStar = 0;
        for (var nCnt = 0; nCnt <= _boxColumnCount; nCnt++)
        {
            if (nCnt < 3)
            {
                NeedStar += 20;
                RemainStar = 20;
            }
            else if (nCnt < 6)
            {
                NeedStar += 20;
                RemainStar = 20;
            }
            else
            {
                NeedStar += 20;
                RemainStar = 20;
            }

            if (nCnt == 0)
            {
            }
            else if (nCnt < 4)
            {
                moreStar += 20;
            }
            else if (nCnt < 7)
            {
                moreStar += 20;
            }
            else
            {
                moreStar += 20;
            }
        }

        showRemainStarCount = RemainStar;

        var temp = 0;
        
        if (BaseSystem.GetInstance.GetSystemList("CircusSystem"))
        {
            RemainStar = GameVariable.GetNeedOpenStarCount();
            temp = GameVariable.GetRemainStarCount();
        }
        else
        {
            temp = _totalStar - moreStar;
        }

        if (temp >= RemainStar)
        {
            chestButton.enabled = true;
            float GaugeSize = RemainStar - (NeedStar - _totalStar);
            GaugeSize = (float)temp / RemainStar;
            var StarTemp = Mathf.Max(temp, 0);
            showStarCount = StarTemp;
            if (StarText != null)
            {
                StarText.text = "" + StarTemp + " / " + RemainStar;
            }

            if (StarGauge != null)
            {
                StarGauge.fillAmount = GaugeSize;
            }

            if (showStarCount >= RemainStar)
            {
                if (chestAnimator != null)
                {
                    chestAnimator.SetBool("isMove", true);
                }
                else
                {
                }

                ActivatedReward(true);
            }

            //ChestImage.GetComponent<Animator>().SetBool("isStatus", true);
        }
        else
        {
            chestButton.enabled = false;
            //chestButton.enabled = true;
            float GaugeSize = RemainStar - (NeedStar - _totalStar);
            GaugeSize = (float)temp / RemainStar;
            var StarTemp = Mathf.Max(temp, 0);
            showStarCount = StarTemp;
            if (StarText != null)
            {
                StarText.text = "" + StarTemp + " / " + RemainStar;
            }
            if (StarGauge != null)
            {
                StarGauge.fillAmount = GaugeSize;
            }
            if (showStarCount < RemainStar)
            {
                if (chestAnimator != null)
                {
                    chestAnimator.SetBool("isMove", false);
                }
                ActivatedReward(false);
            }
            //ChestImage.GetComponent<Animator>().SetBool("isStatus", false);
        }
    }
    
    public void SliderTresureBoxValueCircus()
    {
        if (sliderTresureBox != null)
        {
            sliderTresureBox.maxValue = GameVariable.GetNeedOpenStarCount();
            sliderTresureBox.value = GameVariable.GetRemainStarCount();
            textTresureBox.text = string.Format("{0} / {1}", GameVariable.GetRemainStarCount(), GameVariable.GetNeedOpenStarCount());
            if (checkButton_Dis != null)
            {
                if (GameVariable.GetRemainStarCount() < GameVariable.GetNeedOpenStarCount())
                {
                    checkButton_Dis.SetActive(true);
                }
                else
                {
                    checkButton_Dis.SetActive(false);
                }
            }
        }
    }
    
    private IEnumerator SliderCoroutine()
    {
        while(sliderCoroutine)
        {
            SliderTresureBoxValueCircus();
            yield return new WaitForSeconds(0.5f);
        }
    }

    public bool starChestCheck()
    {
        if (GameVariable.GetRemainStarCount() > GameVariable.GetNeedOpenStarCount()) return true;
        else return false;
    }
}