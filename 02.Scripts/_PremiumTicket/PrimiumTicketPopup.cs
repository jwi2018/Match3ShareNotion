using CompleteProject;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public class AnimItemValue
{
    public string item_name;
    public Sprite item_img;
}

public class PrimiumTicketPopup : PopupSetting
{
    private static int Scrollveiwstandard = 25;
    private int StarCount;
    private int MaxStarCount;
    public GameObject gobDummy;
    public Transform trSlotParent;

    [SerializeField] Text remainDaytxt;
    [SerializeField] Text starCounttxt;
    [SerializeField] Text Leveltxt;
    [SerializeField] Image GuageIMG;
    [SerializeField] ScrollRect contentPos;
    [SerializeField] GameObject primium_effect;

    [Header("구매하기 버튼")]
    [SerializeField] Button Buy_btn;
    [SerializeField] List<GameObject> buy_btn_OnOff;


    [Header("아이템 애니메이션")]
    [SerializeField] Image IMG;
    [SerializeField] Animator GetItemAnim;
    [SerializeField] AnimItemValue[] itemValue;
    [SerializeField] Animator UnlockAnim;

    private List<PrimiumTicketEntity> ticketEntityList = new List<PrimiumTicketEntity>();

    private void Start()
    {
        //if(PrimiumTicketSystem.GetInstance != null) IsBuyTicket = PrimiumTicketSystem.GetInstance.ISBUYPRIMIUMTICKET;
        gobDummy.SetActive(false);
        OnPopupSetting();
    }

    public override void OnPopupSetting()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("GetCoin");

        //레벨, 별획득 상태
        remainDaytxt.text = string.Format(I2.Loc.LocalizationManager.GetTermTranslation("After_Renewed_2"), PrimiumTicketSystem.GetInstance.RemainDay);

        if (PrimiumTicketSystem.GetInstance.LEVEL > PrimiumTicketSystem.MAXLEVEL)
        {
            Leveltxt.text = PrimiumTicketSystem.MAXLEVEL.ToString();
            MaxStarCount = PrimiumTicketSystem.MAXLEVEL;
        }
        else
        {
            Leveltxt.text = PrimiumTicketSystem.GetInstance.LEVEL.ToString();
            MaxStarCount = PrimiumTicketSystem.GetInstance.LEVEL;
        }
        StarCount = PrimiumTicketSystem.GetInstance.PRIMIUMTICKETSTAR;
        starCounttxt.text = $"{StarCount} / {MaxStarCount}";
        
        float amount = (float)StarCount / MaxStarCount;
        GuageIMG.fillAmount = amount;


        if (PlayerData.GetInstance.IsBuyPrimiumTicket)
        {
            Buy_btn.interactable = false;
            buy_btn_OnOff[0].SetActive(false);
            buy_btn_OnOff[1].SetActive(true);
        }
        else
        {
            Buy_btn.interactable = true;
            buy_btn_OnOff[0].SetActive(true);
            buy_btn_OnOff[1].SetActive(false);
        }

        int nowLevel = 0;
     
        for (int i = 0; i < PrimiumTicketSystem.GetInstance.ItemValue.Count / 2; i++)
        {
            var loadedDailyMission = GameObject.Instantiate(gobDummy, trSlotParent);
            loadedDailyMission.SetActive(true);
            loadedDailyMission.name = $"Level_{i + 1}";

            var PTEntity = loadedDailyMission.GetComponent<PrimiumTicketEntity>();
            bool isthisLevelForLock = false;
            bool isReceiveLevel_primium = false;
            bool isReceiveLevel_free = false;

            if (PrimiumTicketSystem.GetInstance.LEVEL == i+2)
            {
                nowLevel = i;
                isthisLevelForLock = true;

                if (PrimiumTicketSystem.GetInstance.LEVEL == 31) isthisLevelForLock = false;
            }

            if (PrimiumTicketSystem.GetInstance.RECEIVELEVEL_FREE.Equals(i)) isReceiveLevel_free = true;
            if (PrimiumTicketSystem.GetInstance.RECEIVELEVEL_PRIMIUM.Equals(i)) isReceiveLevel_primium = true;



            PTEntity.SetTicketEntity(i, isthisLevelForLock, isReceiveLevel_free, isReceiveLevel_primium);

            ticketEntityList.Add(PTEntity);
        }

        var levelpercent = (float)nowLevel / Scrollveiwstandard;
        contentPos.verticalNormalizedPosition = 1f - levelpercent;

        if (PrimiumTicketSystem.GetInstance.IsLevelup)
        {
            StartCoroutine(Levelup());
        }
    }

    public override void OffPopupSetting()
    {
        FindObjectOfType<PrimiumTicket>().Settext();
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("PopupShop");
        this.GetComponent<Animator>().SetTrigger("Off");

    }

    public override void PressedBackKey()
    {
        OffPopupSetting();
    }


    public void GetItem(bool _Isprimium)
    {
        
        if (_Isprimium)
        {
            int EntityTarget = PrimiumTicketSystem.GetInstance.RECEIVELEVEL_PRIMIUM;

            var level = EntityTarget + PrimiumTicketSystem.MAXLEVEL;
            GetLevelItemValue(level);
            
            //두번 되어야함
            ticketEntityList[EntityTarget].ChangeEntity(false, true);
            PrimiumTicketSystem.GetInstance.RECEIVELEVEL_PRIMIUM++;
            EntityTarget++;
            if(!EntityTarget.Equals(30)) ticketEntityList[EntityTarget].ChangeEntity(true, true);
        }
        else
        {
            int EntityTarget = PrimiumTicketSystem.GetInstance.RECEIVELEVEL_FREE;

            GetLevelItemValue(EntityTarget);
            
            ticketEntityList[EntityTarget].ChangeEntity(false);
            PrimiumTicketSystem.GetInstance.RECEIVELEVEL_FREE++;
            EntityTarget++;
            if(!EntityTarget.Equals(30)) ticketEntityList[EntityTarget].ChangeEntity(true);
        }

        //초기화 카운트

    }

    public void GetLevelItemValue(int _level)
    {
        if (PrimiumTicketSystem.GetInstance.ItemValue[_level].itemCount > 1)
        {
            for (int i = 0; i < PrimiumTicketSystem.GetInstance.ItemValue[_level].itemCount; i++)
            {
                string item_name = PrimiumTicketSystem.GetInstance.ItemValue[_level].Item;
                int item_count = 0;
                bool isCoin = false;

                switch (i)
                {
                    case 0:
                        item_name = PrimiumTicketSystem.GetInstance.ItemValue[_level].Item;
                        item_count = PrimiumTicketSystem.GetInstance.ItemValue[_level].Count;
                        break;

                    case 1:
                        item_name = PrimiumTicketSystem.GetInstance.ItemValue[_level].Item_1;
                        item_count = PrimiumTicketSystem.GetInstance.ItemValue[_level].Count_1;
                        break;
                }

                switch (item_name)
                {
                    case "None":
                        PlayerData.GetInstance.Gold += item_count;
                        isCoin = true;
                        break;

                    case "Hammer":
                        PlayerData.GetInstance.ItemHammer += item_count;
                        break;

                    case "Bomb":
                        PlayerData.GetInstance.ItemBomb += item_count;
                        break;

                    case "Color":
                        PlayerData.GetInstance.ItemColor += item_count;
                        break;

                    case "Hammer+Bomb":
                        PlayerData.GetInstance.ItemHammer += item_count;
                        PlayerData.GetInstance.ItemBomb += item_count;
                        break;

                    case "Hammer+Color":
                        PlayerData.GetInstance.ItemHammer += item_count;
                        PlayerData.GetInstance.ItemColor += item_count;
                        break;

                    case "Bomb+Color":
                        PlayerData.GetInstance.ItemBomb += item_count;
                        PlayerData.GetInstance.ItemColor += item_count;
                        break;
                }

                if(isCoin)
                {
                    var coin = PopupManager.instance.GetCoin(item_count);
                    coin.GetComponent<Animator>().SetTrigger("Normal");
                }
                else
                {
                    foreach (var img in itemValue)
                    {
                        if (img.item_name.Equals(item_name)) IMG.sprite = img.item_img;
                    }

                    GetItemAnim.SetTrigger("GetItemrAnim_2");
                }
            }
            
        }
        else
        {
            string item_name = PrimiumTicketSystem.GetInstance.ItemValue[_level].Item;
            int item_count = PrimiumTicketSystem.GetInstance.ItemValue[_level].Count;
            bool isCoin = false;

            switch (item_name)
            {
                case "None":
                    PlayerData.GetInstance.Gold += item_count;
                    isCoin = true;
                    break;

                case "Hammer":
                    PlayerData.GetInstance.ItemHammer += item_count;
                    break;

                case "Bomb":
                    PlayerData.GetInstance.ItemBomb += item_count;
                    break;

                case "Color":
                    PlayerData.GetInstance.ItemColor += item_count;
                    break;

                case "Hammer+Bomb":
                    PlayerData.GetInstance.ItemHammer += item_count;
                    PlayerData.GetInstance.ItemBomb += item_count;
                    break;

                case "Hammer+Color":
                    PlayerData.GetInstance.ItemHammer += item_count;
                    PlayerData.GetInstance.ItemColor += item_count;
                    break;

                case "Bomb+Color":
                    PlayerData.GetInstance.ItemBomb += item_count;
                    PlayerData.GetInstance.ItemColor += item_count;
                    break;
            }

          

            if (isCoin)
            {
                var coin = PopupManager.instance.GetCoin(item_count);
                coin.GetComponent<Animator>().SetTrigger("Normal");
            }
            else
            {
                foreach (var img in itemValue)
                {
                    if (img.item_name.Equals(item_name)) IMG.sprite = img.item_img;
                }

                GetItemAnim.SetTrigger("GetItemrAnim_1");
            }

        }
    }

    IEnumerator Levelup()
    {
        yield return new WaitForSeconds(0.3f);

        UnlockAnim.SetTrigger("IsLevelUp");
        PrimiumTicketSystem.GetInstance.IsLevelup = false;
    }

    public void UnlockLevelup()
    {

    }

    public void UnlockBuyTicket()
    {
        for (int i = 0; i < PrimiumTicketSystem.MAXLEVEL; i++)
        {
            if (i == 0) contentPos.verticalNormalizedPosition = 1f;

            ticketEntityList[i].BuyTicket(i);
        }
    }


    public void BuyTicket()
    {
        Purchaser.GetInstance.BuyProductID(Purchaser.GetInstance.GetPackageItems(10).PurchaserID, () =>
        {
            UnlockAnim.SetTrigger("IsBuyTicket");
            PlayerData.GetInstance.IsBuyPrimiumTicket = true;

            Buy_btn.interactable = false;
            buy_btn_OnOff[0].SetActive(false);
            buy_btn_OnOff[1].SetActive(true);
        });
    }

    public void OpenShop()
    {
        PopupManager.instance.OnClickShopCoinButton();
    }
}
