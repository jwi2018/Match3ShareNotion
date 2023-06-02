using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScenePackageShop : PopupSetting
{
    [SerializeField] private Text titleText;
    [SerializeField] private List<GameObject> packageItem = new List<GameObject>();
    [SerializeField] private List<Text> packageTitleText = new List<Text>();
    [SerializeField] private List<PackageSetting> packages = new List<PackageSetting>();

    private void Start()
    {
        OnPopupSetting();
    }

    public override void OnPopupSetting()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("GetCoin");
        for (var i = 0; i < packages.Count; i++)
        {
#if UNITY_ANDROID
            packages[i].Init(i);
#elif UNITY_IOS
            if(i != 5 || i != 6) packages[i].Init(i);
#endif
        }

        foreach (GameObject temp in packageItem)
        {
            temp.SetActive(false);
        }
        packageItem[PlayerData.GetInstance.MainPackageCheck].SetActive(true);
        titleText.text = packageTitleText[PlayerData.GetInstance.MainPackageCheck].text;
    }

    public override void OffPopupSetting()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("PopupShop");
        GetComponent<Animator>().SetTrigger("Off");
    }

    public void ChagePackageItem()
    {
        foreach (GameObject temp in packageItem)
        {
            temp.SetActive(false);
        }
        packageItem[PlayerData.GetInstance.MainPackageCheck].SetActive(true);
        titleText.text = packageTitleText[PlayerData.GetInstance.MainPackageCheck].text;
    }
}