using UnityEngine;
using UnityEngine.UI;

public class CoinItemSetting : MonoBehaviour
{
    [SerializeField] private int price;
    [SerializeField] private int itemNumber;
    [SerializeField] private Text priceText;
    private PopupManager popupManager;

    public void Init()
    {
        //priceText.text = price.ToString();
        popupManager = GameObject.Find("PopupManager").GetComponent<PopupManager>();
    }

    public void OnClickBuyButton()
    {
        int Hammer = 0;
        int Bomb = 0;
        int Color = 0;
        if (BaseSystem.GetInstance != null)
        {
            if (BaseSystem.GetInstance.GetSystemList("CircusSystem") || BaseSystem.GetInstance.GetSystemList("Fantasy"))
            {
                Hammer = 500;
                Bomb = 800;
                Color = 1000;
            }
            else
            {
                Hammer = 300;
                Bomb = 500;
                Color = 700;
            }
        }
        
        switch (itemNumber)
        {
            case 1:
                if (PlayerData.GetInstance.Gold >= Hammer)
                {
                    if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Item1_shop_buy");
                    PlayerData.GetInstance.ItemHammer++;
                    PlayerData.GetInstance.Gold -= price;
                    popupManager.GoldRefresh();
                    popupManager.ShowGetItemPopup(true);
                }
                else
                {
                    popupManager.ShowGetItemPopup(false);
                }

                break;

            case 2:
                if (PlayerData.GetInstance.Gold >= 400)
                {
                    if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("intro_cashitem_02");
                    PlayerData.GetInstance.ItemCross++;
                    PlayerData.GetInstance.Gold -= price;
                    popupManager.GoldRefresh();
                    popupManager.ShowGetItemPopup(true);
                }
                else
                {
                    popupManager.ShowGetItemPopup(false);
                }

                break;

            case 3:
                if (PlayerData.GetInstance.Gold >= Bomb)
                {
                    if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Item2_shop_buy");
                    PlayerData.GetInstance.ItemBomb++;
                    PlayerData.GetInstance.Gold -= price;
                    popupManager.GoldRefresh();
                    popupManager.ShowGetItemPopup(true);
                }
                else
                {
                    popupManager.ShowGetItemPopup(false);
                }

                break;

            case 4:
                if (PlayerData.GetInstance.Gold >= Color)
                {
                    if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Item3_shop_buy");
                    PlayerData.GetInstance.ItemColor++;
                    PlayerData.GetInstance.Gold -= price;
                    popupManager.GoldRefresh();
                    popupManager.ShowGetItemPopup(true);
                }
                else
                {
                    popupManager.ShowGetItemPopup(false);
                }

                break;
        }
    }
}