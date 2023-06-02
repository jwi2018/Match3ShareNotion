using CompleteProject;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemInfo
{
    public string PurchaserID;
    public string PurchaserPrice;
}

public class ShopButtonSetting : MonoBehaviour
{
    [SerializeField] public int ShopButtonSet;

    [SerializeField] private Text text_PriceInfo;

    private ShopItemInfo itemInfo;

    private void OnEnable()
    {
        Init();
    }

    public void Init(int num = 0)
    {
        itemInfo = Purchaser.GetInstance.GetItems(ShopButtonSet);
        if (itemInfo == null) return;
        text_PriceInfo.text = itemInfo.PurchaserPrice;
    }

    public void BuyPurchaser()
    {
        switch (ShopButtonSet)
        {
            case 0:
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("255000coin_purchase");
                break;
            case 1:
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("142000coin_purchase");
                break;
            case 2:
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("61000coin_purchase");
                break;
            case 3:
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("27500coin_purchase");
                break;
            case 4:
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("11100coin_purchase");
                break;
            case 5:
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("2000coin_purchase");
                break;
            case 6:
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("noad_purchase");
                break;
        }
        if (itemInfo == null) return;
        Purchaser.GetInstance.BuyProductID(itemInfo.PurchaserID);
    }
}