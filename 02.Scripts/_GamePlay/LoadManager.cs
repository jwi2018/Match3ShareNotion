using System.Collections;
using CompleteProject;
using DarkTonic.MasterAudio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    [SerializeField] private float SceneTransTime = 1f;

    [SerializeField] private LoadingBarController LoadingGauge;

    private void Start()
    {
        StartCoroutine(GoToNextSceneCoroutine());
        WeeklyRetentionIndicator.SetStartDate();
        WeeklyRetentionIndicator.CheckToday();
    }

    private IEnumerator GoToNextSceneCoroutine()
    {
        yield return new WaitForSeconds(0.05f);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        Debug.LogWarningFormat("KKI GoToNextSceneCoroutine");
        var operation = SceneManager.LoadSceneAsync("MainScene");
        operation.allowSceneActivation = false;
        LoadingGauge._Checking = false;

        yield return new WaitForEndOfFrame();
        LoadingGauge._Persent = 2;
        Purchaser.GetInstance.Init();
        LoadingGauge._Persent = 5;
        var value = false;
        var delay = 0.0f;
        int limitCount = 0;
#if UNITY_ANDROID
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            ShopItemInfo limitedPackage = null;
            while (true)
            {
                yield return new WaitForSeconds(0.5f);
                if (Application.platform == RuntimePlatform.Android)
                {
                    limitedPackage = Purchaser.GetInstance.GetShopItemInfoByKey(Purchaser.GoogleproductLimitedPackage);
                }
                else
                {
                    limitedPackage = Purchaser.GetInstance.GetShopItemInfoByKey(Purchaser.AppleProductLimitedPackage);
                }
                if(null != limitedPackage)
                {
                    break;
                }
                limitCount++;
                Debug.LogWarningFormat("KKI  {0}", limitCount);
                if (limitCount > 20)
                {
                    break;
                }
            }
        }
#endif
        //while (!value)
        //{
        //    yield return new WaitForEndOfFrame();
        //    delay += Time.deltaTime;
        //    value = true;
        //    for (var i = 0; i < 7; i++)
        //    {
        //        var temp = Purchaser.GetInstance.GetItems(i);
        //        if (temp == null) value = false;
        //        if (!value) break;
        //    }

        //    for (var i = 0; i < 8; i++)
        //    {
        //        var temp = Purchaser.GetInstance.GetPackageItems(i);

        //        if (temp == null) value = false;
        //        if (!value) break;
        //    }

        //    if (delay > 5) break;
        //}

        LoadingGauge._Persent = 10;
        yield return new WaitWhile(() => LoadingGauge._Checking == false);
        LoadingGauge._Checking = false;
        LoadingImageManager._isGameStart = true;
        LoadingImageManager.LoadingImageNumber = 100;
        //AdsManager.GetInstance.InitAds();
        LoadingGauge._Persent = 30;
        yield return new WaitWhile(() => LoadingGauge._Checking == false);
        LoadingGauge._Checking = false;
        yield return new WaitWhile(() => LoadingGauge._Checking == false);
        LoadingGauge._Checking = false;
        //FirebaseManager.GetInstance.Init();
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        PlayerData.GetInstance.LoadData();

        if (BaseSystem.GetInstance.GetSystemList("CircusSystem"))
        {
            DoubleClickSystem.GetInstance.addRainbow(true);
        }

        if (!PlayerData.GetInstance.IsPlayBGM)
        {
            MasterAudio.PlaylistMasterVolume = 0f;
        }
        else
        {
            MasterAudio.PlaylistMasterVolume = 1f;
        }

        if (!PlayerData.GetInstance.IsPlaySFX)
        {
            MasterAudio.SetBusVolumeByName("SFX", 0f);
            MasterAudio.SetBusVolumeByName("Loop", 0f);
        }
        else
        {
            MasterAudio.SetBusVolumeByName("SFX", 1f);
            MasterAudio.SetBusVolumeByName("Loop", 1f);
        }

        LoadingGauge._Persent = 60;
        yield return new WaitWhile(() => LoadingGauge._Checking == false);
        LoadingGauge._Checking = false;

        if (GpgsManager.GetInstance != null)
            GpgsManager.GetInstance.InitGPGS();
        if (FlurryManager.GetInstance != null)
            FlurryManager.GetInstance.Init();

        DataContainer.GetInstance.InitData();
        LoadingGauge._Persent = 70;
        yield return new WaitWhile(() => LoadingGauge._Checking == false);
        LoadingGauge._Checking = false;
        LanguageManager.GetInstance.Init();
        LoadingGauge._Persent = 80;
        yield return new WaitWhile(() => LoadingGauge._Checking == false);
        LoadingGauge._Checking = false;

        if (PrimiumTicketSystem.GetInstance != null) PrimiumTicketSystem.GetInstance.Init();
        //SingularSDK.InitializeSingularSDK();

#if !UNITY_EDITOR
        //yield return new WaitUntil(() => AdsManager.GetInstance.IsLoaded());
#endif
        //GameObject obj = GameObject.Find("PopupManager");
        //obj.GetComponent<PopupManager>().CallLoadingTutorialPop("MainScene");
        LoadingGauge._Persent = 90;
        yield return new WaitWhile(() => LoadingGauge._Checking == false);
        LoadingGauge._Checking = false;
        operation.allowSceneActivation = true;
    }
}