using UnityEngine;

public class GearCore : ObstacleBlock
{
    [SerializeField] private Gear gear;
    [SerializeField] private Animator gearCoreAnimator;

    protected override void OnMouseDown()
    {
        if (gear != null)
        {
            if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("GearPush");

            if (gear.isClockwise) gearCoreAnimator.SetTrigger("R_L");
            else gearCoreAnimator.SetTrigger("L_R");

            gear.ChangeDirection();
        }
    }

    public void SettingGear(Gear _gear)
    {
        gear = _gear;
    }
}