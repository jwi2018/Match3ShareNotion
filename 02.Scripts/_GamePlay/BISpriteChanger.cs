using UnityEngine;
using UnityEngine.UI;

public class BISpriteChanger : MonoBehaviour
{
    [SerializeField] private Image _biImage;

    [SerializeField] private Sprite _japaneseBI;

    [SerializeField] private Sprite _chineseBI_SW;

    [SerializeField] private Sprite _chineseBI_TW;

    private void Start()
    {
        if (PlayerData.GetInstance != null)
        {
            if (PlayerData.GetInstance.NumLanguage == 0)
            {
                if (Application.systemLanguage == SystemLanguage.Japanese)
                    _biImage.sprite = _japaneseBI;
                else if (Application.systemLanguage == SystemLanguage.ChineseSimplified)
                    _biImage.sprite = _chineseBI_SW;
                else if (Application.systemLanguage == SystemLanguage.ChineseTraditional)
                    _biImage.sprite = _chineseBI_TW;
            }
            else
            {
                if (PlayerData.GetInstance.NumLanguage == 10)
                    _biImage.sprite = _japaneseBI;
                else if (PlayerData.GetInstance.NumLanguage == 11)
                    _biImage.sprite = _chineseBI_SW;
                else if (PlayerData.GetInstance.NumLanguage == 12) _biImage.sprite = _chineseBI_TW;
            }
        }
    }
}