using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Coffee.UIEffects;

[System.Serializable]
public class SavingCoinEntity
{
    public UIEffect imgCoin;
    public Text txCoin;

    public bool SetUI(int _targetCoin, int _savedCoin, string _colorDisable, string _colorActive)
    {
        bool r_isOver = false;
        Color color;
        if (_targetCoin <= _savedCoin)
        {
            imgCoin.effectFactor = 0;
            ColorUtility.TryParseHtmlString(_colorActive, out color);
            r_isOver = true;
        }
        else
        {
            imgCoin.effectFactor = 1;
            ColorUtility.TryParseHtmlString(_colorDisable, out color);
            r_isOver = false;
        }

        txCoin.text = string.Format("{0:#,0}", _targetCoin);
        txCoin.color = color;

        return r_isOver;
    }
}

public class SavingPopup : PopupSetting
{
    private ShopItemInfo itemInfo;
    [SerializeField] private Slider sliderSavingGauge = null;
    [SerializeField] private Button btnPurchaser = null;
    [SerializeField] private GameObject purchaser_Disable = null;
    [SerializeField] private Text txPrice = null;
    [SerializeField] private Text txRewardExplain = null;

    [SerializeField] private UIEffect imgPig;
    [SerializeField] private GameObject purchaseAbleArea;

    [SerializeField] private List<SavingCoinEntity> listSavingCoinEntity = new List<SavingCoinEntity>();

    [SerializeField] private Sprite spCoin = null;

    [SerializeField] private Text txTestInfo = null;

    [SerializeField] private string TextColorDisable = "#7B7B7B";
    [SerializeField] private string TextColorActive = "#FFEB61";

    private int savingRewardCoin = 0;

    [ContextMenu("SetNextTestValue")]
    public void SetNextTestValue()
    {
        PlayerData.GetInstance.SavingCoin += 1000;
        OnPopupSetting();
    }

    private void Start()
    {
        OnPopupSetting();
    }

    public void Restore()
    {
        CompleteProject.Purchaser.GetInstance.RestorePurchases();
    }

    public override void OnPopupSetting()
    {
        FirebaseManager.GetInstance.FirebaseLogEvent("Intro_piggybank_enter");

        CheckSavingCoin();
        
        itemInfo = CompleteProject.Purchaser.GetInstance.GetPigCoinPrice();
       
        if (itemInfo == null) return;
        
        txPrice.text = itemInfo.PurchaserPrice;

        txTestInfo.text = string.Format($"{itemInfo.PurchaserID}");

        

        if (SoundManager.GetInstance != null)
        {
            SoundManager.GetInstance.Play("GetCoin");
        }
    }

    public override void OffPopupSetting()
    {
        this.transform.parent.GetComponent<PopupManager>().SetNotchHighlight(false);
        if (SoundManager.GetInstance != null)
        {
            SoundManager.GetInstance.Play("PopupShop");
        }
        this.GetComponent<Animator>().SetTrigger("Off");
    }

    public override void PressedBackKey()
    {
        OffPopupSetting();
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null)
        {
            SoundManager.GetInstance.Play("ButtonPush");
        }
    }

    private void PurchaseStateChanger(bool _isAblePurchase)
    {
        if (_isAblePurchase == true)
        {
            btnPurchaser.gameObject.SetActive(true);
            purchaser_Disable.gameObject.SetActive(false);
            imgPig.effectFactor = 0;
            purchaseAbleArea.SetActive(true);
        }
        else
        {
            btnPurchaser.gameObject.SetActive(false);
            purchaser_Disable.gameObject.SetActive(true);
            imgPig.effectFactor = 1;
            purchaseAbleArea.SetActive(false);
        }
    }

    public void CheckSavingCoin()
    {
        bool isAblePurchase = false;
        for (int i = 0; i < listSavingCoinEntity.Count; i++)
        {
            bool isAble = listSavingCoinEntity[i].SetUI(SavingInfomation.GetSavingCoin(i), PlayerData.GetInstance.SavingCoin, TextColorDisable, TextColorActive);
            if (isAble == true)
            {
                savingRewardCoin = SavingInfomation.GetSavingCoin(i);
                isAblePurchase = isAble;
            }
        }

        sliderSavingGauge.maxValue = SavingInfomation.isTotalSavingCoin;
        sliderSavingGauge.value = PlayerData.GetInstance.SavingCoin;

        PurchaseStateChanger(isAblePurchase);
        if (isAblePurchase == true)
        {
            string locData = I2.Loc.LocalizationManager.GetTermTranslation("BankRewardExplain");
            if(itemInfo != null) txRewardExplain.text = string.Format(locData, savingRewardCoin.ToString("#,##0"), itemInfo.PurchaserPrice);
        }
        else
        {
            txRewardExplain.text = I2.Loc.LocalizationManager.GetTermTranslation("SavingBox_Empty");
        }
    }

    public void BuyPurchaser()
    {
        if (itemInfo == null) return;
        PurchaseStateChanger(false);
        CompleteProject.Purchaser.GetInstance.BuyProductID(itemInfo.PurchaserID, ReceivePurchase);
        
        StartCoroutine(ReCheckSavingCoin());
    }
    private IEnumerator ReCheckSavingCoin()
    {
        yield return new WaitForSeconds(1.0f);
        CheckSavingCoin();
    }

    public void ReceivePurchase()
    {
        Debug.LogWarningFormat("KKI ReceivePurchase");
        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent(string.Format($"piggybank_{savingRewardCoin}coin_purchase"));
        GameObject gobTarget = GameObject.Find("Text_Gold");
        Vector3 vecTargetPosition = gobTarget.transform.position;

        GameObject gobPopup = PopupManager.instance.LoadPopup(PopupList.GetInstance.Pop_GetReward);
        Pop_GetReward popReward = gobPopup.GetComponent<Pop_GetReward>();
        popReward.ShowReward(spCoin, savingRewardCoin / 1000, savingRewardCoin, "GetCoin", vecTargetPosition);

        PlayerData.GetInstance.SavingCoin = PlayerData.GetInstance.SavingCoin - savingRewardCoin;
        PlayerData.GetInstance.Gold += savingRewardCoin;
        var obj = GameObject.Find("PopupManager");
        var coin = obj.GetComponent<PopupManager>().GetCoin();
        coin.GetComponent<Animator>().SetTrigger("Normal");

        //FindObjectOfType<SavingPopup>()
        //OffPopupSetting();
        //OnPopupSetting();
        CheckSavingCoin();
    }
}