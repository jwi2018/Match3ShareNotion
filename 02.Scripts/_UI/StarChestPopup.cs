using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public class ItemAndSprite
{
    public EUseItem eUseitem;
    public Sprite eUseSprite;
}

public class StarChestPopup : PopupSetting
{
    [SerializeField] private GameObject closeButton;

    [SerializeField] private GameObject openButton;

    [SerializeField] private List<ItemAndSprite> itemAndSprites = new List<ItemAndSprite>();

    [SerializeField] private List<int> getGoldList = new List<int>();

    [SerializeField] private List<Image> rewardImages = new List<Image>();

    [SerializeField] private List<Text> rewardTexts = new List<Text>();

    [SerializeField] private Text rewardGoldText;

    [SerializeField] private Image showItemSprite_0;

    [SerializeField] private Image showItemSprite_1;

    [SerializeField] private Text showItemValue_0;

    [SerializeField] private Text showItemValue_1;

    [SerializeField] private UserGold userGold;

    [SerializeField] private List<StarChestMoveObject> moveObjects = new List<StarChestMoveObject>();

    [SerializeField] private List<GameObject> showParticles = new List<GameObject>();

    [SerializeField] private StarChestStatus starChestStatus;

    [SerializeField] private Slider sliderTresureBox;
    [SerializeField] private Text textTresureBox;

    private Animator animator;
    private int goldValue;

    private int moveAnimations;

    private bool sliderCoroutine;

    private PopupManager popupManager;

    private EUseItem rewarditem_1 = EUseItem.NONE;
    private EUseItem rewarditem_2 = EUseItem.NONE;

    private Action eventOpenTresureBoxCount;

    public void Start()
    {
        OnPopupSetting();
    }

    public void SetOffAction(Action e)
    {
        this.eventOpenTresureBoxCount = e;
    }

    public override void OnPopupSetting()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
        sliderCoroutine = true;
        StartCoroutine(SliderCoroutine());
        FirebaseManager.GetInstance.FirebaseLogEvent("treasurebox_enter");
        animator = GetComponent<Animator>();
        moveAnimations = moveObjects.Count;
        //popupManager = transform.parent.GetComponent<PopupManager>();
        popupManager = GameObject.Find("PopupManager").GetComponent<PopupManager>();
        if (userGold != null) popupManager.GetComponent<PopupManager>().GoldAdd(userGold);
        //ItemSetting();
    }

    public void ItemSetting()
    {
        var Inttemp = 0;
        var RandomValue = 0;
        moveAnimations = moveObjects.Count;
        RandomValue = Random.Range(0, getGoldList.Count);
        if (RandomValue < getGoldList.Count)
        {
            Inttemp = getGoldList[RandomValue];
            PlayerData.GetInstance.Gold += Inttemp;
            rewardGoldText.text = "+" + Inttemp;
        }
        else
        {
            Inttemp = getGoldList[0];
            PlayerData.GetInstance.Gold += Inttemp;
            rewardGoldText.text = "+" + Inttemp;
        }

        goldValue = Inttemp;

        List<ItemAndSprite> remainList = new List<ItemAndSprite>(itemAndSprites);
        int selected = Random.Range(1, remainList.Count);
        rewarditem_1 = remainList[selected].eUseitem;
        GetReward(remainList[selected], showItemValue_0, rewardImages[1], showItemSprite_0);
        remainList.RemoveAt(selected);

        int selected2 = Random.Range(1, remainList.Count);
        rewarditem_2 = remainList[selected2].eUseitem;
        GetReward(remainList[selected2], showItemValue_1, rewardImages[2], showItemSprite_1);
        remainList.RemoveAt(selected2);

        //RandomValue = Random.Range(0, (int)EUseItem.COLOR);
        //RandomValue++;
        //Inttemp = RandomValue;
        //if (Inttemp > (int)EUseItem.COLOR)
        //{
        //    RandomValue = Random.Range(0, (int)EUseItem.COLOR);
        //    RandomValue++;
        //}
        //else
        //{
        //    while (true)
        //    {
        //        RandomValue = Random.Range(0, (int)EUseItem.COLOR);
        //        RandomValue++;
        //        if (RandomValue != Inttemp) break;
        //    }
        //}

        //var Min = Mathf.Min(Inttemp, RandomValue);
        //var Max = Mathf.Max(Inttemp, RandomValue);

        //foreach (var item in itemAndSprites)
        //    if ((int)item.eUseitem == Min)
        //    {
        //        rewardImages[1].sprite = item.eUseSprite;
        //        showItemSprite_0.sprite = item.eUseSprite;
        //        rewarditem_1 = (EUseItem)Min;
        //        switch (Min)
        //        {
        //            case (int)EUseItem.HAMMER:
        //                showItemValue_0.text = PlayerData.GetInstance.ItemHammer.ToString();
        //                PlayerData.GetInstance.ItemHammer++;
        //                break;

        //            case (int)EUseItem.CROSS:
        //                showItemValue_0.text = PlayerData.GetInstance.ItemCross.ToString();
        //                PlayerData.GetInstance.ItemCross++;
        //                break;

        //            case (int)EUseItem.BOMB:
        //                showItemValue_0.text = PlayerData.GetInstance.ItemBomb.ToString();
        //                PlayerData.GetInstance.ItemBomb++;
        //                break;

        //            case (int)EUseItem.COLOR:
        //                showItemValue_0.text = PlayerData.GetInstance.ItemColor.ToString();
        //                PlayerData.GetInstance.ItemColor++;
        //                break;
        //        }
        //    }
        //    else if ((int)item.eUseitem == Max)
        //    {
        //        rewardImages[2].sprite = item.eUseSprite;
        //        rewarditem_2 = (EUseItem)Max;
        //        showItemSprite_1.sprite = item.eUseSprite;
        //        switch (Max)
        //        {
        //            case (int)EUseItem.HAMMER:
        //                showItemValue_1.text = PlayerData.GetInstance.ItemHammer.ToString();
        //                PlayerData.GetInstance.ItemHammer++;
        //                break;

        //            case (int)EUseItem.CROSS:
        //                showItemValue_1.text = PlayerData.GetInstance.ItemCross.ToString();
        //                PlayerData.GetInstance.ItemCross++;
        //                break;

        //            case (int)EUseItem.BOMB:
        //                showItemValue_1.text = PlayerData.GetInstance.ItemBomb.ToString();
        //                PlayerData.GetInstance.ItemBomb++;
        //                break;

        //            case (int)EUseItem.COLOR:
        //                showItemValue_1.text = PlayerData.GetInstance.ItemColor.ToString();
        //                PlayerData.GetInstance.ItemColor++;
        //                break;
        //        }
        //    }

        foreach (var item in rewardImages) item.gameObject.SetActive(false);
        foreach (var item in rewardTexts) item.gameObject.SetActive(false);
        foreach (var item in showParticles) item.SetActive(false);
        closeButton.SetActive(false);

        DailyQuestManager.CollectMission(EDailyQuestType.OPENTREASURE, 1);
        DailyQuestManager.Save();
        PlayerData.GetInstance.StarBoxOpenCount = PlayerData.GetInstance.StarBoxOpenCount + 1;
        //FirebaseManager.GetInstance.FirebaseLogEvent("Star_Chest");
    }

    private void GetReward(ItemAndSprite getItem, Text showItem, Image getImg1, Image getImg2)
    {
        getImg1.sprite = getItem.eUseSprite;
        getImg2.sprite = getItem.eUseSprite;
        switch (getItem.eUseitem)
        {
            case EUseItem.HAMMER:
                showItem.text = PlayerData.GetInstance.ItemHammer.ToString();
                PlayerData.GetInstance.ItemHammer++;
                break;

            case EUseItem.BOMB:
                showItem.text = PlayerData.GetInstance.ItemBomb.ToString();
                PlayerData.GetInstance.ItemBomb++;
                break;

            case EUseItem.COLOR:
                showItem.text = PlayerData.GetInstance.ItemColor.ToString();
                PlayerData.GetInstance.ItemColor++;
                break;
        }
        FirebaseManager.GetInstance.FirebaseLogEvent("treasurebox_open");
    }

    public override void OffPopupSetting()
    {
        sliderCoroutine = false;
        StopCoroutine(SliderCoroutine());
        //GetComponent<Animator>().SetTrigger("Off");
        animator.SetTrigger("Close");
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
    }

    public void DestoryPopup()
    {
        if (eventOpenTresureBoxCount != null)
        {
            eventOpenTresureBoxCount?.Invoke();
        }
        Destroy(gameObject);
    }

    public override void PressedBackKey()
    {
        if (closeButton.activeSelf) OffPopupSetting();
    }

    public override void OnButtonClick()
    {
        SoundManager.GetInstance.Play("ButtonPush");
    }

    public void OneMoreOpen()
    {
        FirebaseManager.GetInstance.FirebaseLogEvent("treasurebox_open_more");


        if (eventOpenTresureBoxCount != null)
        {
            eventOpenTresureBoxCount?.Invoke();
        }

        animator.SetTrigger("Open");
        ItemSetting();
    }

    public void OneMoreClose()
    {
        animator.SetTrigger("Close");
    }

    public void AnimationStart()
    {
        if (eventOpenTresureBoxCount != null)
        {
            eventOpenTresureBoxCount?.Invoke();
        }
        animator.SetTrigger("Open");
        userGold.GetRewardGold(goldValue);
        ItemSetting();
    }

    public void rewardItemActive()
    {
        foreach (var item in rewardImages)
            if (!item.gameObject.activeSelf)
            {
                item.gameObject.SetActive(true);
                break;
            }

        foreach (var item in rewardTexts)
            if (!item.gameObject.activeSelf)
            {
                item.gameObject.SetActive(true);
                break;
            }
    }

    public void ChestRolling()
    {
        SoundManager.GetInstance.Play("StarBoxComing");
    }

    public void ButtonActive()
    {
        closeButton.SetActive(true);
        openButton.SetActive(false);
    }

    public void EndMoveAnimation() // 박스 오픈 -> 받기 -> 버튼 누름.
    {
        moveAnimations--;
        if (moveAnimations == 0)
        {
            switch (rewarditem_2)
            {
                case EUseItem.HAMMER:
                    showItemValue_1.text = PlayerData.GetInstance.ItemHammer.ToString();
                    break;

                case EUseItem.CROSS:
                    showItemValue_1.text = PlayerData.GetInstance.ItemCross.ToString();
                    break;

                case EUseItem.BOMB:
                    showItemValue_1.text = PlayerData.GetInstance.ItemBomb.ToString();
                    break;

                case EUseItem.COLOR:
                    showItemValue_1.text = PlayerData.GetInstance.ItemColor.ToString();
                    break;
            }

            // 여기서 더 받기 or 종료 // moveAnimations이 놈이 뭔지 확인이  필요할듯
            //if (GameVariable.GetRemainStarCount() >= 20)
            if (GameVariable.GetRemainStarCount() >= GameVariable.GetNeedOpenStarCount())
                animator.SetTrigger("OneMore");
            else
                animator.SetTrigger("Close");
            /*
            if (starChestStatus.GetComponent<Button>().enabled)
                animator.SetTrigger("OneMore");
            else
                animator.SetTrigger("Close");
            */

            showParticles[2].SetActive(true);
        }
        else if (moveAnimations == 1)
        {
            switch (rewarditem_1)
            {
                case EUseItem.HAMMER:
                    showItemValue_0.text = PlayerData.GetInstance.ItemHammer.ToString();
                    break;

                case EUseItem.CROSS:
                    showItemValue_0.text = PlayerData.GetInstance.ItemCross.ToString();
                    break;

                case EUseItem.BOMB:
                    showItemValue_0.text = PlayerData.GetInstance.ItemBomb.ToString();
                    break;

                case EUseItem.COLOR:
                    showItemValue_0.text = PlayerData.GetInstance.ItemColor.ToString();
                    break;
            }

            showParticles[1].SetActive(true);
        }
        else if (moveAnimations == 2)
        {
            popupManager.GoldRefresh();
            showParticles[0].SetActive(true);
        }

        if (eventOpenTresureBoxCount != null)
        {
            eventOpenTresureBoxCount?.Invoke();
        }
    }

    public void ObjectsMove()
    {
        animator.SetTrigger("Clame");
        StartCoroutine(StartMove());
    }

    private IEnumerator StartMove()
    {
        yield return new WaitForEndOfFrame();

        for (var i = 0; i < moveObjects.Count; i++)
        {
            moveObjects[i].StartMove(rewardImages[i].sprite);
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void SliderTresureBoxValue()
    {
        if (sliderTresureBox != null)
        {
            sliderTresureBox.maxValue = GameVariable.GetNeedOpenStarCount();
            sliderTresureBox.value = GameVariable.GetRemainStarCount();
            textTresureBox.text = string.Format("{0} / {1}", GameVariable.GetRemainStarCount(), GameVariable.GetNeedOpenStarCount());
        }
    }

    private IEnumerator SliderCoroutine()
    {
        while (sliderCoroutine)
        {
            SliderTresureBoxValue();
            yield return new WaitForSeconds(0.2f);
        }
    }
}