using System.Collections.Generic;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPopup : PopupSetting
{
    [SerializeField] private List<Toggle> Tutorials = new List<Toggle>();

    [SerializeField] private Localize _tutorialInfo;

    private void Start()
    {
        OnPopupSetting();
    }

    public override void OnPopupSetting()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
        if (BlockManager.GetInstance != null) BlockManager.GetInstance.IsSwapAble = false;
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

    public void SetTutorial(int num)
    {
        if (num < Tutorials.Count) Tutorials[num].isOn = true;
    }

    public void ChangeText(string TermName)
    {
        _tutorialInfo.SetTerm(TermName);
    }
}