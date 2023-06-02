using UnityEngine;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;

#endif

public class IOSATT : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        //AdmobManager.GetInstance.Init();
#if UNITY_IOS
        if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() ==
        ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {

            ATTrackingStatusBinding.RequestAuthorizationTracking();

        }

#endif
    }

    // Update is called once per frame
    private void Update()
    {
    }
}