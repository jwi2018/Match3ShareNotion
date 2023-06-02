using UnityEngine;
using UnityEngine.UI;

public class EditorSpecialTileItemStatus : MonoBehaviour
{
    [SerializeField] private Image _itemImage;

    [SerializeField] private InputField _perSentField;

    [SerializeField] private EditorSpecialTileItemController _controller;

    private BlockInfo _blockInfo;

    public void SetItemStatus(Sprite sprite, BlockInfo blockInfo, int persent)
    {
        _blockInfo = blockInfo;
        _itemImage.sprite = sprite;
        var strings = persent.ToString();
        _perSentField.text = strings;
        _perSentField.textComponent.text = strings;
        CheckText();
    }

    public void ChangeValue(Text text)
    {
        var intValue = 0;
        int.TryParse(text.text, out intValue);
        _controller.ChangeValue(_blockInfo, intValue);
    }

    public void CheckText()
    {
        var intValue = 0;
        int.TryParse(_perSentField.textComponent.text, out intValue);
        _controller.ChangeValue(_blockInfo, intValue);
    }
}