using System.Collections;
using UnityEngine;

public class LoadingPopup : PopupSetting
{
    [SerializeField] private RectTransform LoadingImage;

    private void Start()
    {
        OnPopupSetting();
    }

    public override void OnButtonClick()
    {
    }

    public override void PressedBackKey()
    {
    }

    public override void OnPopupSetting()
    {
        StartCoroutine(NowLoading());
    }

    public override void OffPopupSetting()
    {
        Destroy(gameObject);
    }

    private IEnumerator NowLoading()
    {
        var FailTime = 0.0f;
        while (gameObject.activeSelf)
        {
            LoadingImage.Rotate(0, 0, 45);
            if (LoadingImage.localRotation.z > 360) LoadingImage.Rotate(0, 0, -360);
            FailTime += Time.deltaTime;
            if (FailTime > 5.0f) OffPopupSetting();
            yield return new WaitForSeconds(0.2f);
            FailTime += 0.2f;
        }

        yield return new WaitForEndOfFrame();
    }
}