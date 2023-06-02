using System.Collections;
using UnityEngine;

public class MovePlusPopup : PopupSetting
{
    [SerializeField] private AudioClip getMoveSound;
    public override void OnPopupSetting()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play(SoundManager.GetInstance.GetFiveCount);
        //ADManager.GetInstance.ShowBanner(EBannerKind.BANNER);
    }

    public override void OffPopupSetting()
    {
        if (BlockManager.GetInstance != null) BlockManager.GetInstance.IsSwapAble = true;
    }

    public override void PressedBackKey()
    {
    }

    public override void OnButtonClick()
    {
    }


    public void AnimEnd()
    {
        StageManager.GetInstance.TutorialStart();
        if (BlockManager.GetInstance != null && !StageManager.GetInstance.TutorialCheck())
        {
            BlockManager.GetInstance.IsSwapAble = true;
            //StageManager.GetInstance.ShowReward_PlayAdButton();
        }
    }

    public void StageMovePlus()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("BlockCreateItem");
        StageManager.GetInstance.AddPreMoveCount();
    }
    

   
}