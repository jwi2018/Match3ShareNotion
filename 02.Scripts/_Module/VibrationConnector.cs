#if MOREMOUNTAINS_NICEVIBRATIONS

using MoreMountains.NiceVibrations;

#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationConnector : Singleton<VibrationConnector>
{
    public float f_Intensity = 0.75f;
    public float f_Sharpness = 0.05f;
    private bool isVibrationing = false;
    private float iCollapseVibrationLimitTime = 0.1f;

    protected virtual void Awake()
    {
#if MOREMOUNTAINS_NICEVIBRATIONS
        MMNViOSCoreHaptics.CreateEngine();
#endif
    }

    public void CallStartVibration()
    {
        if (isVibrationing == false)
        {
            isVibrationing = true;
            StartCoroutine(co_VibrationPlayer());
        }
    }

    private IEnumerator co_VibrationPlayer()
    {
#if MOREMOUNTAINS_NICEVIBRATIONS
        if (MMNVPlatform.iOS())
        {
            MMVibrationManager.ContinuousHaptic(0.75f, 1f, 0.01f, HapticTypes.LightImpact, alsoRumble: true, controllerID: -1, threaded: true);
        }
        else
        {
            MMVibrationManager.ContinuousHaptic(1f, 1f, 0.01f, HapticTypes.LightImpact, alsoRumble: true, controllerID: -1, threaded: true);
        }
        yield return new WaitForSeconds(iCollapseVibrationLimitTime);
        isVibrationing = false;
#endif
        yield return null;
    }

    public static void StartVibration()
    {
#if MOREMOUNTAINS_NICEVIBRATIONS
        if (PlayerData.GetInstance == null)
            return;
        if (PlayerData.GetInstance.IsPlayVib)
        {
            VibrationConnector.GetInstance.CallStartVibration();

            //MMVibrationManager.TransientHaptic(intensity, sharpness, alsoRumble);
            //MMVibrationManager.TransientHaptic(0.85f, 0.05f, alsoRumble);
        }
#endif
    }
}