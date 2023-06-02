using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScenePackageState : MonoBehaviour
{
    [SerializeField] private Text sceneTitle;
    [SerializeField] private List<Sprite> packageImage;
    [SerializeField] private Image targetImage;

    void Start()
    {
        //PlayerData.GetInstance.MainPackageChangeCheckTime = DateTime.Now;
        StartCoroutine(ChageTitle());
    }

    private IEnumerator ChageTitle()
    {
        while (true)
        {
            PlayerData.GetInstance.MainPackageTime = DateTime.Now;
            if (PlayerData.GetInstance.MainPackageTime.Day != PlayerData.GetInstance.MainPackageChangeCheckTime.Day)
            {
                PlayerData.GetInstance.MainPackageCheck = 0;
                PlayerData.GetInstance.MainPackageChangeCheckTime = DateTime.Now;
            }
            switch (PlayerData.GetInstance.MainPackageCheck)
            {
                case 0:
                    sceneTitle.text = I2.Loc.LocalizationManager.GetTermTranslation("Package_Beginner");
                    break;
                case 1:
                    sceneTitle.text = I2.Loc.LocalizationManager.GetTermTranslation("Package_Super");
                    break;
                case 2:
                    sceneTitle.text = I2.Loc.LocalizationManager.GetTermTranslation("Package_Mega");
                    break;
                case 3:
                    sceneTitle.text = I2.Loc.LocalizationManager.GetTermTranslation("Package_Mystery");
                    break;
                case 4:
                    sceneTitle.text = I2.Loc.LocalizationManager.GetTermTranslation("Package_Giant");
                    break;
                case 5:
                    sceneTitle.text = I2.Loc.LocalizationManager.GetTermTranslation("Package_Messive");
                    break;
                case 6:
                    sceneTitle.text = I2.Loc.LocalizationManager.GetTermTranslation("Package_Champion");
                    break;
            }
            targetImage.sprite = packageImage[PlayerData.GetInstance.MainPackageCheck];
            yield return new WaitForSeconds(0.2f);
        }
    }
}