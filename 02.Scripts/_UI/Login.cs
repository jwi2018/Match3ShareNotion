using UnityEngine;

public class Login : MonoBehaviour
{
    [SerializeField] private GameObject _googleObj;

    [SerializeField] private GameObject _appleObj;

    private void Start()
    {
#if UNITY_ANDROID
        _appleObj.SetActive(false);
#elif  UNITY_IOS
        _googleObj.SetActive(false);
#endif
    }
}