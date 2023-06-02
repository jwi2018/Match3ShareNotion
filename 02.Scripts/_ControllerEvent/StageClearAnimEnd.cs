using UnityEngine;

public class StageClearAnimEnd : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //    
    //}
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetBool("StarAnimEnd"))
        {
            var PopupManager = GameObject.Find("PopupParent");
            //PopupManager.GetComponent<PopupManager>().UnLockTouch();
            for (var i = 0; i < PopupManager.transform.childCount; i++)
                if (PopupManager.transform.GetChild(i).GetComponent<MissionClearPopup>() != null)
                    PopupManager.transform.GetChild(i).GetComponent<MissionClearPopup>()
                        .StarInGaugeStart(animator.GetInteger("CntStar"));
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}