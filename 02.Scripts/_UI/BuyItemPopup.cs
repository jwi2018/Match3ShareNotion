using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class BuyItemPopup : PopupSetting
{
    [SerializeField] private GameObject[] _itemList;

    [SerializeField] private GameObject[] _itemShadow;

    [SerializeField] private int[] _itemPrice;

    [SerializeField] private Localize _itemName;

    [SerializeField] private Localize _itemInfomation;

    [SerializeField] private Text _itemPayGold;

    [SerializeField] private UserGold Gold;

    private int _itemValues;

    [SerializeField] private Text _userCoin;

    private void Start()
    {
        OnPopupSetting();
    }

    public override void OnPopupSetting()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
        if (Gold != null)
        {
            var obj = GameObject.Find("PopupManager");
            obj.GetComponent<PopupManager>().GoldAdd(Gold);
        }

        if (_userCoin != null)
        {
            StartCoroutine(UserCoin());
        }
        
        BlockManager.GetInstance.IsSwapAble = false;
    }

    public override void OffPopupSetting()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
        if (Gold != null)
        {
            var obj = GameObject.Find("PopupManager");
            obj.GetComponent<PopupManager>().GoldRemove(Gold);
        }
        
        if (_userCoin != null)
        {
            StopCoroutine(UserCoin());
        }
        
        BlockManager.GetInstance.IsSwapAble = true;
        Destroy(gameObject);
    }

    public override void PressedBackKey()
    {
        OffPopupSetting();
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ButtonPush");
    }

    public void ItemSetting(int WhatItem)
    {
        _itemValues = WhatItem;
        switch (WhatItem)
        {
            case 0:
                _itemName.SetTerm("Hammer");
                _itemInfomation.SetTerm("Tuto07_item01");
                _itemPayGold.text = _itemPrice[WhatItem].ToString();
                foreach (var nObj in _itemShadow)
                    if (nObj == _itemShadow[WhatItem]) nObj.SetActive(true);
                    else nObj.SetActive(false);
                foreach (var nObj in _itemList)
                    if (nObj == _itemList[WhatItem]) nObj.SetActive(true);
                    else nObj.SetActive(false);
                break;
            case 1:
                _itemName.SetTerm("Cross Bomb");
                _itemInfomation.SetTerm("CrossBombInfo");
                _itemPayGold.text = _itemPrice[WhatItem].ToString();
                foreach (var nObj in _itemShadow)
                    if (nObj == _itemShadow[WhatItem]) nObj.SetActive(true);
                    else nObj.SetActive(false);
                foreach (var nObj in _itemList)
                    if (nObj == _itemList[WhatItem]) nObj.SetActive(true);
                    else nObj.SetActive(false);
                break;
            case 2:
                _itemName.SetTerm("Circular bomb");
                _itemInfomation.SetTerm("Tuto07_item02");
                _itemPayGold.text = _itemPrice[WhatItem].ToString();
                foreach (var nObj in _itemShadow)
                    if (nObj == _itemShadow[WhatItem]) nObj.SetActive(true);
                    else nObj.SetActive(false);
                foreach (var nObj in _itemList)
                    if (nObj == _itemList[WhatItem]) nObj.SetActive(true);
                    else nObj.SetActive(false);
                break;
            case 3:
                _itemName.SetTerm("Rainbow Bomb");
                _itemInfomation.SetTerm("RainbowBombInfo");
                _itemPayGold.text = _itemPrice[WhatItem].ToString();
                foreach (var nObj in _itemShadow)
                    if (nObj == _itemShadow[WhatItem]) nObj.SetActive(true);
                    else nObj.SetActive(false);
                foreach (var nObj in _itemList)
                    if (nObj == _itemList[WhatItem]) nObj.SetActive(true);
                    else nObj.SetActive(false);
                break;
        }
    }

    public void BuyItem()
    {
        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Play_item_button_shop");

        if (PlayerData.GetInstance != null)
        {
            if (PlayerData.GetInstance.Gold >= _itemPrice[_itemValues])
            {
                PlayerData.GetInstance.Gold -= _itemPrice[_itemValues];

                int itemCount = 0;

                if (BaseSystem.GetInstance != null)
                {
                    if (BaseSystem.GetInstance.GetSystemList("Fantasy"))
                    {
                        itemCount = 3;
                    }
                    else
                    {
                        itemCount = 4;
                    }
                }
                else
                {
                    itemCount = 4;
                }
                
                switch (_itemValues)
                {
                    case 0:
                        PlayerData.GetInstance.ItemHammer += itemCount;
                        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Item1_buy");
                        break;
                    case 1:
                        PlayerData.GetInstance.ItemCross += itemCount;
                       // if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Item2_Buy");
                        break;
                    case 2:
                        PlayerData.GetInstance.ItemBomb += itemCount;
                        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Item2_buy");
                        break;
                    case 3:
                        PlayerData.GetInstance.ItemColor += itemCount;
                        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Item3_buy");
                        break;
                }

                OffPopupSetting();
            }
            else
            {
                var paramater = new Dictionary<string, string>();
                paramater.Add("ShopKinds", "BuyItem_Fail");
                FirebaseManager.GetInstance.FirebaseLogEvent("Shop", paramater);

                OnClickShopButton();
            }
        }

        StageManager.GetInstance.SetUI();
    }

    public void OnClickShopButton()
    {
        var paramater = new Dictionary<string, string>();
        paramater.Add("ShopKinds", "BuyItem_Shop");
        FirebaseManager.GetInstance.FirebaseLogEvent("Shop", paramater);

        transform.parent.GetComponent<PopupManager>().OnClickShopCoinButton();
    }

    public void ShowAd()
    {
        SoundManager.GetInstance?.Play("ButtonPush");
        ADManager.GetInstance.ShowReward(ERewardedKind.REWARD,()=>
        {
            switch (_itemValues)
            {
                case 0:
                    PlayerData.GetInstance.ItemHammer += 1;
                    break;
                case 1:
                    PlayerData.GetInstance.ItemCross += 1;
                    break;
                case 2:
                    PlayerData.GetInstance.ItemBomb += 1;
                    break;
                case 3:
                    PlayerData.GetInstance.ItemColor += 1;
                    break;
            }

            StageManager.GetInstance.SetUI();
            OffPopupSetting();
        });
        /*AD.popupManager = transform.parent.GetComponent<PopupManager>();
        AD.ShowAd(ERewardedKind.GETCASHITEM, () =>
        {
            switch (_itemValues)
            {
                case 0:
                    PlayerData.GetInstance.ItemHammer += 1;
                    break;
                case 1:
                    PlayerData.GetInstance.ItemCross += 1;
                    break;
                case 2:
                    PlayerData.GetInstance.ItemBomb += 1;
                    break;
                case 3:
                    PlayerData.GetInstance.ItemColor += 1;
                    break;
            }

            StageManager.GetInstance.SetUI();
            OffPopupSetting();
        });*/
    }

    IEnumerator UserCoin()
    {
        while (true)
        {
            _userCoin.text = PlayerData.GetInstance.Gold.ToString("#,##0");
            yield return new WaitForSeconds(0.8f);
        }
    }
}