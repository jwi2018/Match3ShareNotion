using System.Collections.Generic;
using CompleteProject;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class PackageSetting : MonoBehaviour
{
    [SerializeField] protected ShopPopup Shop;

    [SerializeField] protected Text TotalGoldText;

    [SerializeField] protected Text text_PriceInto;

    [SerializeField] protected List<Image> PackageItemImages = new List<Image>();

    [SerializeField] protected List<Text> PackageItemValues = new List<Text>();

    [SerializeField] protected bool _goldIsSpecial;

    protected ShopItemInfo itemInfo;
    
    private int packageNum;

    public virtual void Init(int num)
    {
        packageNum = num;
        
        if (Purchaser.GetInstance != null)
        {
            if (num >= Purchaser.GetInstance.packageInfo.Count)
            {
                gameObject.SetActive(false);
                return;
            }

            var MyPackageInfo = Purchaser.GetInstance.packageInfo[num];
            var item = EUseItem.NONE;
            var Pass = 0;
            for (var i = 0; i < MyPackageInfo.buyItems.Count; i++)
            {
                item = MyPackageInfo.buyItems[i].item;

                if (_goldIsSpecial && item == EUseItem.NONE)
                {
                    TotalGoldText.text = string.Format("{0:#,##0}", MyPackageInfo.buyItems[i].intValue);
                }
                else
                {
                    if (MyPackageInfo.buyItems[i].intValue == 0)
                    {
                        PackageItemImages[Pass].transform.parent.gameObject.SetActiveSelf(false);
                    }
                    else
                    {
                        if (item != EUseItem.CROSS)
                        {
                            PackageItemImages[Pass].transform.parent.gameObject.SetActiveSelf(true);
                        }
                    }
                    switch (item)
                    {
                        case EUseItem.NONE:
                            PackageItemImages[Pass].sprite = Shop.GetSpriteOrNull((int)item);
                            break;

                        case EUseItem.HAMMER:
                            PackageItemImages[Pass].sprite = Shop.GetSpriteOrNull((int)item);
                            break;

                        case EUseItem.CROSS:
                            PackageItemImages[Pass].sprite = Shop.GetSpriteOrNull((int)item);
                            break;

                        case EUseItem.BOMB:
                            PackageItemImages[Pass].sprite = Shop.GetSpriteOrNull((int)item);
                            break;

                        case EUseItem.COLOR:
                            PackageItemImages[Pass].sprite = Shop.GetSpriteOrNull((int)item);
                            break;
                    }

                    PackageItemValues[Pass].text = "x" + MyPackageInfo.buyItems[i].textValue;
                    Pass++;
                }
            }
            /*for (int i = 0; i < PackageItemImages.Count;i++)
            {
                if (i >= Pass)
                {
                    PackageItemImages[i].transform.parent.gameObject.SetActive(false);
                }
            }*/
        }

        itemInfo = Purchaser.GetInstance.GetPackageItems(num);
        if (itemInfo == null) return;
        if (text_PriceInto != null) text_PriceInto.text = itemInfo.PurchaserPrice;
    }

    public void BuyPurchaser()
    {
        switch (packageNum)
        {
            case 0:
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("1package_purchase");
                break;
            case 1:
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("2package_purchase");
                break;
            case 2:
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("3package_purchase");
                break;
            case 3:
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("4package_purchase");
                break;
            case 4:
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("5package_purchase");
                break;
            case 5:
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("6package_purchase");
                break;
            case 6:
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("7package_purchase");
                break;
        }

        if (itemInfo == null) return;
        Purchaser.GetInstance.BuyProductID(itemInfo.PurchaserID);
    }

    public void BuyMainPackagePurchaser()
    {
        if (itemInfo == null) return;
        Purchaser.GetInstance.BuyProductID(itemInfo.PurchaserID, () =>
        {
            if (PlayerData.GetInstance.MainPackageCheck < 6) PlayerData.GetInstance.MainPackageCheck++;
            else PlayerData.GetInstance.MainPackageCheck = 0;
        });
    }
}