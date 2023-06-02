using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PopupLoadingTutorial : PopupSetting
{
    [SerializeField] private List<Sprite> LoadingImageList;

    [SerializeField] private List<Sprite> LoadingBackgroundImageList;

    [SerializeField] private Image TutorialImage;

    [SerializeField] private Image BgImage;

    [SerializeField] private List<GameObject> ActiveControlObj;

    [SerializeField] private Image _mImage;

    [SerializeField] private GameObject _missionGuid;
    private bool _isSceneChange;

    private void Start()
    {
        if (BlockManager.GetInstance != null) BlockManager.GetInstance.IsSwapAble = false;
        BgImage.sprite = LoadingBackgroundImageList[StageManager.LoadingBackground];
    }

    public override void OnPopupSetting()
    {
    }

    public override void OffPopupSetting()
    {
        Destroy(gameObject);
    }

    public override void PressedBackKey()
    {
    }

    public override void OnButtonClick()
    {
    }

    public void SceneChange()
    {
        _isSceneChange = true;
    }

    public void FadeIn(float Time, int ImageNum, string SceneName)
    {
        if (LoadingImageList.Count > ImageNum)
        {
            if (BaseSystem.GetInstance.GetSystemList("Fantasy"))
                TutorialImage.sprite = LoadingImageList[ImageNum];
            StageManager.LoadingBackground = Random.Range(0, LoadingBackgroundImageList.Count);
            BgImage.sprite = LoadingBackgroundImageList[StageManager.LoadingBackground];
            GetComponent<Animator>().SetTrigger("ImageIn");
            //AdsManager.GetInstance.IsShowRewardAD = false;
        }
        else
        {
            GetComponent<Animator>().SetTrigger("BlackIn");
            //AdsManager.GetInstance.IsShowRewardAD = false;
        }

        StartCoroutine(FadeInAnim(Time, SceneName));
    }

    public void FadeOut(float Time, int ImageNum)
    {
        if (LoadingImageList.Count > ImageNum)
        {
            if (BaseSystem.GetInstance.GetSystemList("Fantasy"))
                TutorialImage.sprite = LoadingImageList[ImageNum];
            _missionGuid = transform.parent.GetComponent<PopupManager>().OnClickMissionGuid();
            _missionGuid.SetActive(false);
            if (SoundManager.GetInstance != null) SoundManager.GetInstance.PlayBGM("Game");

            GetComponent<Animator>().SetTrigger("ImageOut");
            if (AnimationManager.GetInstance != null) AnimationManager.AnimCount++;
        }
        else
        {
            GetComponent<Animator>().SetTrigger("BlackOut");
        }

        StartCoroutine(FadeOutAnim(Time));
    }

    private IEnumerator FadeInAnim(float Timer, string SceneName)
    {
        yield return new WaitWhile(() => _isSceneChange == false);
        LoadingImageManager.CallLoadingPopup = true;
        Debug.LogWarningFormat("KKI FadeInAnim", SceneName);
        var operation = SceneManager.LoadSceneAsync(SceneName);
    }

    private IEnumerator FadeOutAnim(float Timer)
    {
        LoadingImageManager.CallLoadingPopup = false;

        if (BlockManager.GetInstance == null) ADManager.GetInstance.HideBanner(EBannerKind.BANNER);

        while (true)
        {
            if (BlockManager.GetInstance != null) BlockManager.GetInstance.IsSwapAble = false;
            if (_isSceneChange) break;
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitWhile(() => _isSceneChange == false);

        if (BlockManager.GetInstance != null)
        {
            BlockManager.GetInstance.IsSwapAble = true;
            ADManager.GetInstance.ShowBanner(EBannerKind.BANNER);
        }

        if (_missionGuid != null) _missionGuid.SetActive(true);
        if (AnimationManager.GetInstance != null) AnimationManager.AnimCount--;
        Destroy(gameObject);
    }
}