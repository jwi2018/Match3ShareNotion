using UnityEngine;

public class PopupSetting : MonoBehaviour
{
    public virtual void OnEnable()
    {
        if (FirebaseManager.GetInstance != null)
        FirebaseManager.GetInstance.DebugLog("IceCubeLog: " + gameObject.name + "Popup Open");
    }

    public virtual void OnDestroy()
    {
        if (gameObject != null && FirebaseManager.GetInstance != null)
            FirebaseManager.GetInstance.DebugLog("IceCubeLog: " + gameObject.name + "Popup Close");
    }

    public virtual void PressedBackKey()
    {
    }

    public virtual void OnPopupSetting()
    {
    }

    public virtual void OffPopupSetting()
    {
    }

    public virtual void OnButtonClick()
    {
    }
}