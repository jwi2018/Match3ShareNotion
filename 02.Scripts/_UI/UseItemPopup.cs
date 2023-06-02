using I2.Loc;
using UnityEngine;

public class UseItemPopup : PopupSetting
{
    [SerializeField] private GameObject[] _itemList;

    [SerializeField] private GameObject[] _itemShadow;

    [SerializeField] private Localize _itemName;

    [SerializeField] private Localize _itemInfomation;

    private int _itemValues;

    public override void OnPopupSetting()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
    }

    public override void OffPopupSetting()
    {
        StageManager.GetInstance.CancelItem();
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

    public void OnClickAdMob()
    {
        GetComponent<RewardAdsButton>().ShowAds();
    }

    public void ItemSetting(int WhatItem)
    {
        _itemValues = WhatItem;
        switch (WhatItem)
        {
            case 0:
                _itemName.SetTerm("Hammer");
                _itemInfomation.SetTerm("HammerInfo");
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

                foreach (var nObj in _itemShadow)
                    if (nObj == _itemShadow[WhatItem]) nObj.SetActive(true);
                    else nObj.SetActive(false);
                foreach (var nObj in _itemList)
                    if (nObj == _itemList[WhatItem]) nObj.SetActive(true);
                    else nObj.SetActive(false);
                break;
            case 2:
                _itemName.SetTerm("Circular bomb");
                _itemInfomation.SetTerm("CircleBombInfo");

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

                foreach (var nObj in _itemShadow)
                    if (nObj == _itemShadow[WhatItem]) nObj.SetActive(true);
                    else nObj.SetActive(false);
                foreach (var nObj in _itemList)
                    if (nObj == _itemList[WhatItem]) nObj.SetActive(true);
                    else nObj.SetActive(false);
                break;
        }
    }
}