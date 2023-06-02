using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBarController : MonoBehaviour
{
    [SerializeField] private Image _mImage;

    public int _Persent { get; set; }

    public bool _Checking { get; set; }

    private void Awake()
    {
        StartCoroutine(LoadingBarCheck());
    }

    private IEnumerator LoadingBarCheck()
    {
        while (true)
        {
            if (_mImage.fillAmount < (float) _Persent * 0.01) _mImage.fillAmount += 0.05f;
            if (_mImage.fillAmount >= (float) _Persent * 0.01) _Checking = true;
            yield return new WaitForEndOfFrame();
        }
    }
}