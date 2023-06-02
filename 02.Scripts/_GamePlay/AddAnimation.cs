using UnityEngine;

public class AddAnimation : MonoBehaviour
{
    [SerializeField] private Animator SkyAnimator;

    public void SkyAnimationStart()
    {
        SkyAnimator.SetTrigger("ClearAnim");
    }
}