using CompleteProject;
using UnityEngine;
using UnityEngine.UI;

public class LimitedPackageSetting : PackageSetting
{
    [SerializeField] private int purchaserOldID;
    [SerializeField] private int purchaserID;
    public Text oldPrice;

    private void Start()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable) return;
        if (oldPrice != null)
            if (Purchaser.GetInstance != null)
            {
                ShopItemInfo shopinfo = Purchaser.GetInstance.GetItems(purchaserOldID);
                if (null != shopinfo)
                {
                    oldPrice.text = shopinfo.PurchaserPrice; //GetPackageItems(4).PurchaserPrice;
                }
            }

        Init(purchaserID);
    }

    public override void Init(int num)
    {
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
                    //TotalGoldText.text = MyPackageInfo.buyItems[i].textValue;
                    TotalGoldText.text = string.Format("{0:#,##0}", MyPackageInfo.buyItems[i].intValue);
                }
                else
                {
                    PackageItemValues[Pass].text = "x" + MyPackageInfo.buyItems[i].textValue;
                    Pass++;
                }
            }

            itemInfo = Purchaser.GetInstance.GetPackageItems(num);
        }

        if (itemInfo == null) return;
        if (text_PriceInto != null) text_PriceInto.text = itemInfo.PurchaserPrice;
    }
}