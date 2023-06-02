using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorConnectController : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;

    [SerializeField] private RectTransform _viewPort;

    [SerializeField] private InputField _hpInputField;

    [SerializeField] private Transform _toggleCheckImage;

    [SerializeField] private GameObject _activeObj;

    [SerializeField] private Toggle _removeToggle;

    [SerializeField] private EditorToggleManager _toggleMaster;

    [SerializeField] private Transform _orderCheckImage;

    [SerializeField] private List<Image> _ConnectImages;

    private EOneWay _direction = EOneWay.NONE;

    private int _hpValue = 1;
    private int _orderValue = 1;

    public void Start()
    {
        _scrollRect.content = _viewPort;
    }

    public void ChangeText(Text text)
    {
        var intvalue = 0;
        int.TryParse(text.text, out intvalue);

        if (_hpInputField.textComponent == text)
        {
            _hpValue = intvalue;
            var etcValue = _orderValue * 100 + (int) _direction;
            _toggleMaster.SetObjectID = EID.CONNECTION;
            _toggleMaster.SetEtcValue = etcValue;
            _toggleMaster.SetHP = _hpValue;
            _toggleMaster.LogMessage();
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

        foreach (var item in _ConnectImages) item.color = color;
        var etcValue = _orderValue * 100 + (int) _direction;
        _toggleMaster.SetObjectID = EID.CONNECTION;
        _toggleMaster.SetEtcValue = etcValue;
        _toggleMaster.SetHP = _hpValue;
        _toggleMaster.LogMessage();
    }

    public void ChangeToggle(Toggle toggle)
    {
        if (toggle.isOn)
        {
            _toggleCheckImage.SetParent(toggle.transform);
            _toggleCheckImage.localPosition = new Vector3(0, 0, 0);
            _direction = toggle.GetComponent<EditorConnectStatus>().GetDirection;

            var etcValue = _orderValue * 100 + (int) _direction;
            _toggleMaster.SetEtcValue = etcValue;
            _toggleMaster.SetHP = _hpValue;
            _toggleMaster.LogMessage();
        }
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

    public void OpenConnectionList()
    {
        _activeObj.SetActive(true);
    }

    public ConnectionProperty GetStatus()
    {
        var returnValue = new ConnectionProperty();

        returnValue.hp = _hpValue;
        returnValue.direction = _direction;
        returnValue.order = _orderValue;

        return returnValue;
    }
}