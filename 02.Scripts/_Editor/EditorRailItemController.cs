using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorRailItemController : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;

    [SerializeField] private RectTransform _viewPort;

    [SerializeField] private InputField _powerInputField;

    [SerializeField] private InputField _orderInputField;

    [SerializeField] private Toggle _removeToggle;

    [SerializeField] private Transform _toggleCheckImage;

    [SerializeField] private GameObject _activeObj;

    [SerializeField] private EditorToggleManager _toggleMaster;

    [SerializeField] private Transform _orderCheckImage;

    [SerializeField] private List<Image> _RailImages;

    private bool _isRemove;
    private int _orderValue = 1;
    private int _powerValue = 1;
    private int _raileKind = 1;


    public void Start()
    {
        _scrollRect.content = _viewPort;
    }

    public void ChangeText(Text text)
    {
        var intvalue = 0;
        int.TryParse(text.text, out intvalue);

        if (_powerInputField.textComponent == text)
        {
            _powerValue = intvalue;
            _toggleMaster.LogMessage();
        }
        else if (_orderInputField.textComponent == text)
        {
            _orderValue = intvalue;
            _toggleMaster.LogMessage();
        }
    }

    public void ChangeToggle(Toggle toggle)
    {
        if (toggle == _removeToggle)
        {
            if (toggle.isOn)
                _isRemove = true;
            else
                _isRemove = false;
        }
        else
        {
            if (toggle.isOn)
            {
                _toggleCheckImage.SetParent(toggle.transform);
                _toggleCheckImage.localPosition = new Vector3(0, 0, 0);
                _raileKind = toggle.GetComponent<EditorRailItemStatus>().GetTileKind;
                _toggleMaster.LogMessage();
            }
        }
    }

    public void ChangeOrderButton(int num)
    {
        _orderValue = num;
        var color = new Color();
        switch (num)
        {
            case 1:
                color = new Color(1, 1, 1, 0.5f);
                break;
            case 2:
                color = new Color(1, 1, 0, 0.5f);
                break;
            case 3:
                color = new Color(0, 1, 0, 0.5f);
                break;
            case 4:
                color = new Color(0, 0, 1, 0.5f);
                break;
            case 5:
                color = new Color(0, 1, 1, 0.5f);
                break;
            case 6:
                color = new Color(1, 0, 0, 0.5f);
                break;
            case 7:
                color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                break;
            case 8:
                color = new Color(0, 0, 0, 0.5f);
                break;
            default:
                color = new Color(0, 0, 0, 0);
                break;
        }

        foreach (var item in _RailImages) item.color = color;
        _toggleMaster.LogMessage();
        MapEditor.GetInstance.ChangeRailOrder(num);
    }

    public void ChangeOrderButton(Transform obj)
    {
        _orderCheckImage.SetParent(obj);
        _orderCheckImage.localPosition = new Vector3(0, 0, 0);
    }

    public void CloseButton()
    {
        _activeObj.SetActive(false);
        _toggleMaster.SelectIDClear();
    }

    public void OpenRailList()
    {
        _activeObj.SetActive(true);
    }

    public RailProperty GetStatus()
    {
        var returnValue = new RailProperty();
        returnValue.isRemove = _isRemove;
        returnValue.order = _orderValue;
        returnValue.power = _powerValue;
        returnValue.railKind = _raileKind;

        return returnValue;
    }
}