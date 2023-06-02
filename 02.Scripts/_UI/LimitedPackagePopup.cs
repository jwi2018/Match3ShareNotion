using System;
using System.Collections;
using System.Collections.Generic;
using CompleteProject;
using UnityEngine;
using UnityEngine.UI;

public class LimitedPackagePopup : PackageSetting
{
    // Start is called before the first frame update
    private Action buyEvent;

    public void Init(Action buyEvent)
    {
        this.buyEvent = buyEvent;

        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("GetCoin");

        TotalGoldText.text = Purchaser.GetInstance.packageInfo[7].buyItems[0].textValue;

        for (int i = 0; i < PackageItemValues.Count; i++)
        {
            PackageItemValues[i].text = "x" + Purchaser.GetInstance.packageInfo[7].buyItems[i + 1].textValue;
        }
        
        itemInfo = Purchaser.GetInstance.GetPackageItems(7);
        if (itemInfo == null) return;
        if (text_PriceInto != null) text_PriceInto.text = itemInfo.PurchaserPrice;
    }

    public void BuyLimitedPackage()
    {
        if (itemInfo == null) return;
        Purchaser.GetInstance.BuyProductID(itemInfo.PurchaserID, ()=>
        {
            buyEvent?.Invoke();
            Destroy(gameObject);
        });
    }

    public void Exit()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("PopupShop");
        Destroy(gameObject);
    }
}
