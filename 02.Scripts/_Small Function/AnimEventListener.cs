using EnhancedScrollerCustom.StageScroll;
using UnityEngine;

public class AnimEventListener : MonoBehaviour
{
    public void StartAnim()
    {
        //AnimationManager.AnimCount++;
    }

    public void EndAnim()
    {
        AnimationManager.AnimCount--;
    }

    public void PlayDoubleRainbowSound()
    {
        if (SoundManager.GetInstance != null)
            SoundManager.GetInstance.Play("BombDoubleRainbow");
    }

    public void BigBombPop()
    {
        AnimationManager.GetInstance.ShowDoubleRhombus(transform.position);
        BoardShaking(0.3f, 0.15f);
        //SetActiveFalse();
    }

    public void RocketPop()
    {
        BoardShaking(0.15f);
    }

    public void BigRainPop()
    {
        AnimationManager.GetInstance.ShowDoubleRainbowPop(transform.position);
        BoardShaking(0.4f, 0.2f);
    }

    public void BoardShaking(float Time = 0.2f, float Strong = 0.1f)
    {
        if (CameraShake.GetInstance != null) CameraShake.GetInstance.Shaking(Time, Strong);
    }

    public void SetActiveFalse()
    {
        if (DynamicObjectPool.GetInstance != null)
        {
            var isDynamicObject = DynamicObjectPool.GetInstance.PoolObject(gameObject, false);

            if (!isDynamicObject) gameObject.SetActive(false);
        }
    }

    public void ActiveFalse()
    {
        gameObject.SetActive(false);
    }

    public void ShineItem()
    {
        transform.GetChild(0).GetComponent<ShiningItem>().StartShine();
    }
}