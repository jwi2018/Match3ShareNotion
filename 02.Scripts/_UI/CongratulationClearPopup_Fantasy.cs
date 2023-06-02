using UnityEngine;

public class CongratulationClearPopup_Fantasy : PopupSetting
{
    private float _fillamount;

    private bool _isFirst;
    private int _stars = 1;

    private Animator GameClearAnimator;
    private Animator IsLandAnimator;

    private void Start()
    {
        OnPopupSetting();
    }

    public override void OnPopupSetting()
    {
        StageManager.GetInstance.IsSkipAble = false;
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("WaterWave");
        transform.parent.GetComponent<PopupManager>().IsStageClear = true;
        var obj = Instantiate(PopupList.GetInstance.Popup_MissionClear, transform.parent, false);
        obj.transform.SetAsFirstSibling();
        GameClearAnimator = obj.GetComponent<Animator>();
    }

    public override void OffPopupSetting()
    {
        GameClearAnimator.SetTrigger("ClearAnim");
        IsLandAnimator.SetTrigger("ClearAnim");
        IsLandAnimator.GetComponent<AddAnimation>().SkyAnimationStart();
        GetComponent<Animator>().SetTrigger("End");
    }

    public override void PressedBackKey()
    {
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ButtonPush");
    }

    public void OnSfxFirePlay()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ClearFirework");
    }

    public void OnBraboPlay()
    {
        /*if (SoundManager.GetInstance != null)
        {
            SoundManager.GetInstance.Play(SoundManager.GetInstance.ClearBravo);
            SoundManager.GetInstance.Play(SoundManager.GetInstance.ClearStar);
        }*/
    }

    public void Setting(int Star, float Fillamount, bool First)
    {
        _stars = Star;
        _fillamount = Fillamount;
        _isFirst = First;
    }

    public void CallGameClearPopup()
    {
        GameClearAnimator.gameObject.SetActive(true);
        GameClearAnimator.GetComponent<MissionClearPopup>().StageAndScore(0, 0); //(Stage, Score);
        GameClearAnimator.GetComponent<MissionClearPopup>().GetStars(_stars, _isFirst);
        var UIobj = GameObject.Find("UI Top");
        UIobj.SetActive(false);
        var _island = GameObject.Find("Top BG");
        IsLandAnimator = _island.GetComponent<Animator>();
        var obj = GameObject.Find("Main Camera");
        if (obj != null) obj.SetActive(false);
    }
}