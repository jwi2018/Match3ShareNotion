using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorMultiMapStatus : MonoBehaviour
{
    [SerializeField] private InputField _mField;

    [SerializeField] private List<GameObject> _mChangeActiveObj;

    [SerializeField] private EditorMultiMapController _MultiMapContoller;

    [SerializeField] private Vector2 _isMatrix;

    [SerializeField] private Toggle _isToggle;

    private int _isOrder;

    public Toggle GetToggle => _isToggle;

    public bool IsActive => _isToggle.isOn;

    public Vector2 IsMatrix => _isMatrix;

    public int IsOrder
    {
        get => _isOrder;
        set => MapOrderChange(value);
    }

    private void Start()
    {
        if (IsActive) int.TryParse(_mField.textComponent.text, out _isOrder);
    }

    public void ToggleChange()
    {
        if (_isToggle.isOn)
        {
            foreach (var item in _mChangeActiveObj) item.SetActive(true);
            GetComponent<Image>().enabled = true;
            if (_isOrder == 0)
            {
                MapOrderChange(_MultiMapContoller.GetMultiMapOrder());
                _MultiMapContoller.CheckSameOrder();
            }
        }
        else
        {
            foreach (var item in _mChangeActiveObj) item.SetActive(false);
            GetComponent<Image>().enabled = false;
            _isOrder = 0;
            var matrix = Vector2Int.zero;
            matrix.x = (int) _isMatrix.x;
            matrix.y = (int) _isMatrix.y;

            MapEditor.GetInstance.ClearMultiMap(matrix);
            _MultiMapContoller.CheckSameOrder();
        }
    }

    public void MapOrderChange(int num)
    {
        var intValue = num.ToString();
        _mField.text = intValue;
        int.TryParse(intValue, out _isOrder);
        _MultiMapContoller.CheckSameOrder();
    }

    public void FieldValueChange(Text text)
    {
        var intValue = text.text;
        int.TryParse(intValue, out _isOrder);

        var order = _isOrder;
        _MultiMapContoller.CheckSameOrder();
    }

    public void SetMultiMapStatus(int Order)
    {
        _isToggle.isOn = true;
        MapOrderChange(Order);
    }

    public void CameraMoveMultiMap()
    {
        var returnValue = (int) (_isMatrix.x * 10 + _isMatrix.y);
        Debug.Log(returnValue);
        MapEditor.GetInstance.ChangePreMap(returnValue);
    }
}