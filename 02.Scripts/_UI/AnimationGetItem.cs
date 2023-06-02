using UnityEngine;
using UnityEngine.UI;

public class AnimationGetItem : MonoBehaviour
{
    [SerializeField] private Image mImage;

    [SerializeField] private Text mText;

    [SerializeField] private GameObject ParentObject;

    public void ChangeItem(Sprite sprite, int intValue, RectTransform rect = null)
    {
        mImage.sprite = sprite;
        mText.text = $"x {intValue.ToString()}";
        if (rect != null) mImage.rectTransform.sizeDelta = rect.sizeDelta;
        gameObject.SetActive(true);
    }

    public void AnimationControlEnd()
    {
        if (ParentObject.GetComponent<RoulettePopup>() != null)
        {
            ParentObject.GetComponent<RoulettePopup>().GetItemAnimationEnd();
        }
        else if (ParentObject.GetComponent<RoulettePopup_Dotween>() != null)
        {
            ParentObject.GetComponent<RoulettePopup_Dotween>().GetItemAnimationEnd();
        }
    }

    public void AnimationControlActive()
    {
        if (ParentObject.GetComponent<RoulettePopup>() != null)
        {
            ParentObject.GetComponent<RoulettePopup>().GetItemAnimaion_Off_End();
        }
        else if (ParentObject.GetComponent<RoulettePopup_Dotween>() != null)
        {
            ParentObject.GetComponent<RoulettePopup_Dotween>().GetItemAnimaion_Off_End();
        }
    }
}