using Spine;
using Spine.Unity;
using UnityEngine;
using AnimationState = Spine.AnimationState;

public class SpineCharacterController : MonoBehaviour
{
    public Skeleton skeleton;

    public AnimationState spineAnimationState;

    private void Start()
    {
        charAnimation = GetComponent<SkeletonGraphic>();
        spineAnimationState = charAnimation.AnimationState;
        skeleton = charAnimation.Skeleton;
        //spineAnimationState.SetAnimation(0, idle1, false);
    }

    public void ShowAnim()
    {
        spineAnimationState.SetAnimation(0, active1, false);
    }

    #region Inspector

    [SpineAnimation] public string idle1;

    [SpineAnimation] public string active1;

    public SkeletonGraphic charAnimation;

    #endregion
}