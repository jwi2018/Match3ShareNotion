using UnityEngine;

public class CashItemUseing : MonoBehaviour
{
    [SerializeField] private PopupManager Manager;

    public void StartItemAnimation()
    {
        if (Manager != null) Manager.SetisItemAnimation = true;
    }

    public void EndItemAnimation()
    {
        if (Manager != null) Manager.SetisItemAnimation = false;
    }
}