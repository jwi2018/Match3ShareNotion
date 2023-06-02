public class RewardItemPop : PopupSetting
{
    private void Start()
    {
        OnPopupSetting();
    }

    public override void OnPopupSetting()
    {
        if (PlayerData.GetInstance != null)
        {
            PlayerData.GetInstance.ItemHammer += 3;
            PlayerData.GetInstance.ItemColor += 3;
            PlayerData.GetInstance.ItemCross += 3;
            PlayerData.GetInstance.ItemBomb += 3;
        }
    }

    public override void OffPopupSetting()
    {
    }

    public override void OnButtonClick()
    {
    }

    public override void PressedBackKey()
    {
    }
}