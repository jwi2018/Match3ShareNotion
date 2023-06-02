using UnityEngine;

public class WeeklyBonusChecker : MonoBehaviour
{
    [SerializeField] private WeeklyBonusPop weeklyBonusPop;

    public void CheckAnimationEnd()
    {
        if (weeklyBonusPop != null) weeklyBonusPop.CheckAnimationEnd();
    }

    public void CheckFirstAnimEnd()
    {
        if (weeklyBonusPop != null) weeklyBonusPop.CheckFirstAnimEnd();
    }
}