using UnityEngine;

public class ButtonEventListenerCustom : MonoBehaviour
{
    public void GoToShop_Coin()
    {
        if (FirebaseManager.GetInstance != null)  FirebaseManager.GetInstance.FirebaseLogEvent("Play_item_shop");
        PopupManager.instance.OnClickShopCoinButton();
    }
}