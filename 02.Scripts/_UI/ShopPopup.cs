using System;
using System.Collections.Generic;
using CompleteProject;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct PackageInfo
{
    public List<BuyItemInfo> buyItems;
}

[Serializable]
public struct BuyItemInfo
{
    public EUseItem item;
    public int intValue;
    public string textValue;
}

public class ShopPopup : PopupSetting
{
    public bool isCoinShop;

    [SerializeField] private GameObject Restory;

    [SerializeField] private UserGold Gold;

    [SerializeField] private List<Text> TotalGoldValue;

    [SerializeField] private List<Text> BonusGoldValue;

    [SerializeField] private RewardAdsButton rewardAdsButton;

    [SerializeField] private GameObject adsFreeGrayButton;

    [SerializeField] private GameObject adsFreeNormalButton;

    [SerializeField] private GameObject viewMoreButtonFrame;

    [SerializeField] private ScrollRect shopScrollRect;

    [SerializeField] private List<Sprite> EUseItemSprites;

    [SerializeField] private List<PackageSetting> packages = new List<PackageSetting>();

    [SerializeField] private List<ShopButtonSetting> PurchaserItems = new List<ShopButtonSetting>();

    [SerializeField] private List<GameObject> viewMoreObject = new List<GameObject>();

    [SerializeField] private Button packageListButton;

    [SerializeField] private Button coinListButton;

    [SerializeField] private Sprite[] listButtonSprites;

    private readonly int PackageListCount = 0;

    private void Start()
    {
        OnPopupSetting();
    }

    private void Update()
    {
        AdsFree();
    }

    public Sprite GetSpriteOrNull(int num)
    {
        Sprite returnValue = null;
        if (EUseItemSprites.Count > num) returnValue = EUseItemSprites[num];
        return returnValue;
    }

    public override void OnPopupSetting()
    {
        //this.transform.parent.GetComponent<PopupManager>().SetNotchHighlight(true);

        for (var i = 0; i < packages.Count; i++)
        {
#if UNITY_ANDROID
            packages[i].Init(i);
#elif UNITY_IOS
            if(i != 5 || i != 6)
            {
                packages[i].Init(i);
            }
#endif
        }

        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("GetCoin");
        if (Gold != null)
        {
            var obj = GameObject.Find("PopupManager");
            obj.GetComponent<PopupManager>().GoldAdd(Gold);
        }

        for (var i = 0; i < PurchaserItems.Count; i++)
        {
            PurchaserItems[i].Init(i);
            PurchaserItems[i].transform.SetSiblingIndex(PackageListCount + i);
            PurchaserItems[i].gameObject.SetActive(isCoinShop);
        }

#if UNITY_ANDROID
        Restory.SetActive(false);
#endif
        if (rewardAdsButton != null)
            rewardAdsButton.SetActiveMode(true);

        if (packageListButton != null && coinListButton != null)
        {
            packageListButton.onClick.AddListener(() => ChangeShopList(EShopKind.PACKAGE));

            coinListButton.onClick.AddListener(() => ChangeShopList(EShopKind.COIN));
        }

#if UNITY_IOS
        if (packages.Count > 5)
        {
            packages[5].gameObject.SetActive(false);
            packages[6].gameObject.SetActive(false);
        }
#endif
        foreach (var item in packages) item.gameObject.SetActive(true);
    }

    public override void OffPopupSetting()
    {
        //this.transform.parent.GetComponent<PopupManager>().SetNotchHighlight(false);
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("PopupShop");
        GetComponent<Animator>().SetTrigger("Off");
        if (Gold != null)
        {
            var obj = GameObject.Find("PopupManager");
            obj.GetComponent<PopupManager>().GoldRemove(Gold);
        }
    }

    public override void PressedBackKey()
    {
        OffPopupSetting();
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ButtonPush");
    }

    public void AdsFree()
    {
        if (PlayerData.GetInstance == null)
            return;
        if (PlayerData.GetInstance.IsAdsFree)
        {
            adsFreeNormalButton.SetActive(false);
            adsFreeGrayButton.SetActive(true);
        }
    }

    public void ViewMore(bool isbool = true)
    {
        if (isbool)
        {
            foreach (var item in viewMoreObject) item.SetActive(true);
            if (shopScrollRect != null) shopScrollRect.vertical = true;
            viewMoreButtonFrame.SetActive(false);
        }
        else
        {
            foreach (var item in viewMoreObject) item.SetActive(false);
            if (shopScrollRect != null) shopScrollRect.vertical = false;
            viewMoreButtonFrame.SetActive(true);
        }
    }

    public void RestorePurchases()
    {
        Purchaser.GetInstance.RestorePurchases();
    }

    public void ChangeShopList(EShopKind kind)
    {
        switch (kind)
        {
            case EShopKind.PACKAGE:
                if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("PopupShop");
                foreach (var item in packages) item.gameObject.SetActive(true);
                foreach (var item in PurchaserItems) item.gameObject.SetActive(false);
#if UNITY_IOS
                packages[6].gameObject.SetActive(false);
                packages[5].gameObject.SetActive(false);
#endif
                packageListButton.image.sprite = listButtonSprites[0];
                coinListButton.image.sprite = listButtonSprites[1];
                break;

            case EShopKind.COIN:
                if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("PopupShop");
                foreach (var item in packages) item.gameObject.SetActive(false);
                foreach (var item in PurchaserItems)
                {
                    item.gameObject.SetActive(true);
                    item.Init(item.ShopButtonSet);
                }

                packageListButton.image.sprite = listButtonSprites[1];
                coinListButton.image.sprite = listButtonSprites[0];
                break;
        }
    }
}