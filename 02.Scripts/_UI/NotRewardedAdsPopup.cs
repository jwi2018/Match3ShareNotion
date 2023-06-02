using I2.Loc;
using UnityEngine;

public class NotRewardedAdsPopup : PopupSetting
{
    [SerializeField] private Localize localize_0 = new Localize();
    [SerializeField] private Localize localize_1 = new Localize();
    [SerializeField] private GameObject CloseButton;
    private bool isLoaded;

    private RewardAdsButton requestAdsButton;
    private RoulettePopup requestRouletteButton;

    public void Start()
    {
        CloseButton.SetActive(false);
        OnPopupSetting();
    }

    public override void OnPopupSetting()
    {
        if (BlockManager.GetInstance != null) BlockManager.GetInstance.IsSwapAble = false;
    }

    public override void OffPopupSetting()
    {
        if (BlockManager.GetInstance != null) BlockManager.GetInstance.IsSwapAble = true;
        if (requestAdsButton != null) requestAdsButton.CloseRewardPopup();
        Destroy(gameObject);
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ButtonPush");
    }

    public void SetRequestAdsButton(RewardAdsButton adsButton = null)
    {
        if (adsButton != null) requestAdsButton = adsButton;
    }

    public void SetRequestAdsButton(RoulettePopup adsButton = null)
    {
        if (adsButton != null) requestRouletteButton = adsButton;
    }

    public override void PressedBackKey()
    {
        if (!isLoaded) return;

        OffPopupSetting();
    }

    public void CancelShowReward()
    {
        if (requestAdsButton != null) requestAdsButton.CancelReward();
        if (requestRouletteButton != null) requestRouletteButton.CancelReward();
        //이거 뭐임..?
        //if (common != null) common.CancelReward();
        OffPopupSetting();
    }

    public void LoadFail()
    {
        isLoaded = true;
        if (CloseButton != null) CloseButton.SetActive(true);
        if (localize_0 != null) localize_0.SetTerm("NoReward_0");
        if (localize_1 != null) localize_1.SetTerm("NoReward_1");
    }
}