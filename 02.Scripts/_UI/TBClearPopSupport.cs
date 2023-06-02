using UnityEngine;

public class TBClearPopSupport : MonoBehaviour
{
    [SerializeField] private MissionClearPopup clearPopup;

    public void EndStartAnimation()
    {
        clearPopup.StarAnimationEnd();
    }

    public void EndBoxAnimation()
    {
        clearPopup.RewardAnimationEnd();
    }

    public void EndButtonAnimation()
    {
        clearPopup.ButtonAnimationEnd();
    }
}