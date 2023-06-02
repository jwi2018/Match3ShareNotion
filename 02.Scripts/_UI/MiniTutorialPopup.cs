using UnityEngine;

public class MiniTutorialPopup : PopupSetting
{
    [SerializeField] private Animator _tutorialAnimator;

    private void Start()
    {
        OnPopupSetting();
    }

    public override void OnPopupSetting()
    {
        //_tutorialAnimator.GetComponent<TutoAnimController>().OnTutorialPop();
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
        if (BlockManager.GetInstance != null) BlockManager.GetInstance.IsSwapAble = false;
        if (EditorAutoModeControll._isAutoMode) OffPopupSetting();
    }

    public override void OffPopupSetting()
    {
        GetComponent<Animator>().SetTrigger("Off");
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
        if (BlockManager.GetInstance != null) BlockManager.GetInstance.IsSwapAble = true;
    }

    public override void PressedBackKey()
    {
        OffPopupSetting();
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ButtonPush");
    }

    public void OnTutorial(int num)
    {
        _tutorialAnimator.GetComponent<TutoAnimController>().OnWhatTutorial(num);
    }

    public void OnClickMoreTutorial()
    {
        OffPopupSetting();
        transform.parent.GetComponent<PopupManager>().OnClickTutorial();
    }
}