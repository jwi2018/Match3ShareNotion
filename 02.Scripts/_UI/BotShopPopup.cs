using System;
using System.Collections.Generic;
using CompleteProject;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct CoinItem
{
    public GameObject gameObject;
    public bool isPurchase;
}

[Serializable]
public struct MoreList
{
    public List<GameObject> btn_list;
}

public class BotShopPopup : PopupSetting
{
    [SerializeField] private GameObject Restore;

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

    [SerializeField] private List<CoinItem> coinItems = new List<CoinItem>();

    [SerializeField] private List<GameObject> viewMoreObject = new List<GameObject>();

    [SerializeField] private Button packageListButton;

    [SerializeField] private Button coinListButton;

    [SerializeField] private Button goldListButton;

    [SerializeField] private GameObject packageListContents;
    [SerializeField] private GameObject coinListContents;
    [SerializeField] private GameObject goldListContents;

    [SerializeField] private Sprite[] listButtonSprites;

    [SerializeField] private GameObject noAd;

    [SerializeField] private List<MoreList> more_btn_list = new List<MoreList>();
    [SerializeField] private List<GameObject> more_btn = new List<GameObject>();

    

    public Text coinInfo;
    private readonly int PackageListCount = 0;
    Action e;

    public void Init(Action e =null)
    {
        this.e = e;
    }
    
    private void Start()
    {
        Debug.Log("1111111111111");
        foreach (var item in coinItems)
            if (!item.isPurchase)
                item.gameObject.GetComponent<CoinItemSetting>().Init();
        noAd.GetComponent<ShopButtonSetting>().Init(6);
        OnPopupSetting();
    }

    private void Update()
    {
        AdsFree();
        coinInfo.text = PlayerData.GetInstance.Gold.ToString("#,##0");
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
        FirebaseManager.GetInstance.FirebaseLogEvent("item_shop");
        ChangeShopList(EShopKind.PACKAGE);
        for (var i = 0; i < packages.Count; i++)
        {
#if UNITY_ANDROID
            packages[i].Init(i);
#elif UNITY_IOS
            if (i != 5 || i != 6)
            {
                packages[i].Init(i);
            }
#endif
        }
        Debug.Log("2222222222222");
        noAd.GetComponent<ShopButtonSetting>().Init(6);

        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("GetCoin");
        if (Gold != null)
        {
            var obj = GameObject.Find("PopupManager");
            obj.GetComponent<PopupManager>().GoldAdd(Gold);
        }

        for (var i = 0; i < coinItems.Count; i++)
        {
            coinItems[i].gameObject.transform.SetSiblingIndex(PackageListCount + i);
            //coinItems[i].gameObject.SetActive(false);
        }

#if UNITY_ANDROID
        Restore.SetActive(false);
#endif
        if (rewardAdsButton != null)
            rewardAdsButton.SetActiveMode(true);

        if (packageListButton != null && coinListButton != null)
        {
            packageListButton.onClick.AddListener(() =>
            {
                FirebaseManager.GetInstance.FirebaseLogEvent("intro_package_shop_enter");
                ChangeShopList(EShopKind.PACKAGE);
            });

            coinListButton.onClick.AddListener(() =>
            {
                FirebaseManager.GetInstance.FirebaseLogEvent("intro_itemshop_enter");
                ChangeShopList(EShopKind.COIN);
            });
        }

        if (goldListButton != null)
        {
            goldListButton.onClick.AddListener(() =>
            {
                FirebaseManager.GetInstance.FirebaseLogEvent("intro_coinshop");
                ChangeShopList(EShopKind.GOLD);
            });

            //ChangeShopList(EShopKind.PACKAGE);
        }

#if UNITY_IOS
        if (packages.Count > 5)
        {
            packages[5].gameObject.SetActive(false);
            packages[6].gameObject.SetActive(false);
        }
#endif
    }

    private void TapClicked(EShopKind tapKind)
    {
        ChangeShopList(tapKind);
    }

    public override void OffPopupSetting()
    {
        if (e != null)
        {
            e?.Invoke();
        }
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

    public void OnClickVIPShop()
    {
        PopupManager.instance.ShowVipPopup();
    }

    public void AdsFree()
    {
        if (PlayerData.GetInstance.IsAdsFree)
        {
            adsFreeNormalButton.SetActive(false);
            adsFreeGrayButton.SetActive(true);
        }
        else
        {
            adsFreeNormalButton.SetActive(true);
            adsFreeGrayButton.SetActive(false);
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

    public void MoreBtnClick(int idx)
    {
        // switch (idx)
        // {
        //     //package
        //     case 0 :
        //         if (more_btn[idx] != null)
        //         {
        //             more_btn[idx].SetActive(false);
        //         }
        //         
        //         for (int i = 0; i < more_btn_list[idx].btn_list.Count; i++)
        //         {
        //             if(more_btn_list[idx].btn_list[i] != null) more_btn_list[idx].btn_list[i].SetActive(true);
        //         }
        //
        //         break;
        //     
        //     //package
        //     case 1 :
        //         
        //     
        //         if (more_btn[idx] != null)
        //         {
        //             more_btn[idx].SetActive(false);
        //         }
        //
        //         for (int i = 0; i < more_btn_list[idx].btn_list.Count; i++)
        //         {
        //             if(more_btn_list[idx].btn_list[i] != null) more_btn_list[idx].btn_list[i].SetActive(true);
        //         }
        //
        //         break;
        // }
        
        
    }

    public void ChangeShopList(EShopKind kind)
    {
        switch (kind)
        {
            case EShopKind.PACKAGE:
                if (packageListButton != null)
                {
                    if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("PopupShop");

                    //morebtn 있을때
                    // if (more_btn.Count != 0)
                    // {
                    //     for(int i =0; i< 2; i++)
                    //     {
                    //         packages[i].gameObject.SetActive(true);
                    //     }
                    //     
                    //     for(int i =2; i< packages.Count; i++) packages[i].gameObject.SetActive(false);
                    //     more_btn[0].SetActive(true);
                    // }
                    // else
                    
                    foreach (var item in packages) item.gameObject.SetActive(true);
                    foreach (var item in coinItems) item.gameObject.SetActive(false);

                    if (goldListContents != null)
                    {
                        packageListContents.SetActiveSelf(true);
                        coinListContents.SetActiveSelf(false);
                        goldListContents.SetActiveSelf(false);
                        goldListButton.image.sprite = listButtonSprites[1];
                    }

#if UNITY_IOS
                packages[6].gameObject.SetActive(false);
                packages[5].gameObject.SetActive(false);
#endif
                    packageListButton.image.sprite = listButtonSprites[0];
                    coinListButton.image.sprite = listButtonSprites[1];

                    //packageListButton.transform.GetChild(0).gameObject.SetActive(false);
                    //coinListButton.transform.GetChild(0).gameObject.SetActive(true);
                }

                break;

            case EShopKind.COIN:
                if (coinListButton != null)
                {
                    
                    if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("PopupShop");
                    foreach (var item in packages) item.gameObject.SetActive(false);
                    foreach (var item in coinItems) item.gameObject.SetActive(true);
                    noAd.GetComponent<ShopButtonSetting>().Init(6);
                    if (goldListContents != null)
                    {
                        packageListContents.SetActiveSelf(false);
                        coinListContents.SetActiveSelf(true);
                        goldListContents.SetActiveSelf(false);
                        goldListButton.image.sprite = listButtonSprites[1];
                    }

                    packageListButton.image.sprite = listButtonSprites[1];
                    coinListButton.image.sprite = listButtonSprites[0];

                    //packageListButton.transform.GetChild(0).gameObject.SetActive(true);
                    //coinListButton.transform.GetChild(0).gameObject.SetActive(false);
                }

                break;

            case EShopKind.GOLD: // NEW Pakage, Gold, Item Combine
                if (goldListButton != null)
                {
                    if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("PopupShop");

                    foreach (var item in packages) item.gameObject.SetActive(false);
                    foreach (var item in coinItems) item.gameObject.SetActive(false);
                    
                    //morebtn 있을때
                    // if (more_btn.Count != 0)
                    // {
                    //     for(int i =0; i< 3; i++)
                    //     {
                    //         more_btn_list[0].btn_list[i].SetActive(false);
                    //         
                    //     }
                    //     
                    //     more_btn[0].SetActive(true);
                    // }

                    if (goldListContents != null)
                    {
                        packageListContents.SetActiveSelf(false);
                        coinListContents.SetActiveSelf(false);
                        goldListContents.SetActiveSelf(true);
                    }

                    packageListButton.image.sprite = listButtonSprites[1];
                    coinListButton.image.sprite = listButtonSprites[1];
                    goldListButton.image.sprite = listButtonSprites[0];
                }

                break;
        }
    }
}