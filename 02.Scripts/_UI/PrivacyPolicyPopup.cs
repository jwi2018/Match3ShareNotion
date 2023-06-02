using UnityEngine;
using UnityEngine.UI;

public class PrivacyPolicyPopup : PopupSetting
{
    [SerializeField] private Toggle[] _toggleList;

    private void Start()
    {
        OnPopupSetting();
    }

    public override void OnPopupSetting()
    {
        //if(PlayerData.GetInstance != null)
        //{
        //    if(PlayerData.GetInstance.IsTermsOfUse_EU)
        //    {
        //        foreach (Toggle nToggle in _toggleList)
        //        {
        //            nToggle.isOn = true;
        //        }
        //    }
        //}
        //else
        //{
        //    if (PlayerData.GetInstance.IsTermsOfUse_EU)
        //    {
        //        foreach (Toggle nToggle in _toggleList)
        //        {
        //            nToggle.isOn = false;
        //        }
        //    }
        //}
    }


    public override void OffPopupSetting()
    {
        Destroy(transform.parent.gameObject);
    }

    public override void PressedBackKey()
    {
        PrivacyCheck();
    }

    public override void OnButtonClick()
    {
    }

    public void PrivacyCheck()
    {
        OnButtonClick();
        var IsPass = true;

        foreach (var nToggle in _toggleList)
            if (nToggle.isOn == false)
                IsPass = false;

        if (IsPass)
        {
            OffPopupSetting();
            if (PlayerData.GetInstance.NumLanguage == 9)
            {
                if (PlayerData.GetInstance != null) PlayerData.GetInstance.IsTermsOfUse_KR = true;
            }
            else
            {
                if (PlayerData.GetInstance != null) PlayerData.GetInstance.IsTermsOfUse_EU = true;
            }
        }
        else
        {
        }
    }

    public void PrivacyPolicyOpenRUL()
    {
        OnButtonClick();
        Application.OpenURL("http://.com/privacypolicy.php");
    }
}