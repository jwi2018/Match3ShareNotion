using UnityEngine;
using UnityEngine.UI;

public class RouletteItemStatus : MonoBehaviour
{
    [SerializeField] private Image mImage;

    [SerializeField] private Text mText;

    [SerializeField] private int value;

    public Image GetImage => mImage;

    public int GetIntValue
    {
        get
        {
            var returnValue = 0;
            if (mText.gameObject.activeSelf)
            {
                int.TryParse(mText.text, out returnValue);
            }
            else
            {
                returnValue = value;
            }
            return returnValue;
        }
    }

    public void ChangeItem(Sprite sprite, string strings)
    {
        mImage.sprite = sprite;
        mText.text = strings;
    }
}