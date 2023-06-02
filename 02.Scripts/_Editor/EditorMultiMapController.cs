using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorMultiMapController : MonoBehaviour
{
    [SerializeField] private List<EditorMultiMapStatus> _MultiMapStatuses;

    public void CameraMove(Vector2 MoveMatrix)
    {
        Debug.Log(MoveMatrix);
    }

    public void MapOrderSorting(Vector2 matrix, int _isOrder)
    {
        foreach (var item in _MultiMapStatuses)
            if (item.IsMatrix != matrix)
                if (item.IsOrder >= _isOrder)
                    item.IsOrder += 1;
    }

    public List<int> GetMultiMapList()
    {
        var returnValue = new List<int>();
        var OrderList = new List<int>();
        var MatrixList = new List<Vector2>();

        foreach (var item in _MultiMapStatuses)
            if (item.IsActive)
            {
                OrderList.Add(item.IsOrder);
                MatrixList.Add(item.IsMatrix);
            }

        var Roop = 0;
        while (Roop < OrderList.Count)
        {
            for (var i = 0; i < OrderList.Count; i++)
                if (i + 1 < OrderList.Count && OrderList[i] > OrderList[i + 1])
                {
                    var temp = OrderList[i + 1];
                    OrderList[i + 1] = OrderList[i];
                    OrderList[i] = temp;

                    var vtemp = MatrixList[i + 1];
                    MatrixList[i + 1] = MatrixList[i];
                    MatrixList[i] = vtemp;
                }

            Roop++;
        }

        foreach (var item in MatrixList) returnValue.Add((int) (item.x * 10 + item.y));

        return returnValue;
    }

    public int GetMultiMapOrder()
    {
        var ints = new List<int>();
        var returnValue = 1;
        foreach (var item in _MultiMapStatuses)
            if (item.IsActive)
                ints.Add(item.IsOrder);
        while (true)
            if (ints.Contains(returnValue))
                returnValue++;
            else
                return returnValue;
    }

    public void CheckSameOrder()
    {
        var CheckOrder = 0;
        var SameToggles = new List<Toggle>();
        foreach (var item in _MultiMapStatuses)
            if (item.IsActive)
                if (!SameToggles.Contains(item.GetToggle))
                {
                    CheckOrder = item.IsOrder;

                    foreach (var _item in _MultiMapStatuses)
                        if (_item.IsActive && _item.IsOrder == CheckOrder && _item != item)
                        {
                            if (!SameToggles.Contains(item.GetToggle)) SameToggles.Add(item.GetToggle);
                            if (!SameToggles.Contains(_item.GetToggle)) SameToggles.Add(_item.GetToggle);
                        }
                }

        foreach (var item in _MultiMapStatuses)
            if (SameToggles.Contains(item.GetToggle))
                item.GetToggle.targetGraphic.color = new Color(1, 0, 0, 1);
            else
                item.GetToggle.targetGraphic.color = new Color(1, 1, 1, 1);

        MapEditor.GetInstance.SetMapSize();
    }

    public void SetMultiMapSetting(Vector2 MapMatrix, int Order)
    {
        foreach (var item in _MultiMapStatuses)
            if (item.IsMatrix == MapMatrix)
                item.SetMultiMapStatus(Order);
    }

    public void SetMultiMapSetting(int intMatrix, int Order)
    {
        var stringvalue = intMatrix.ToString();
        if (stringvalue.Length > 2)
        {
            Debug.Log("Fail intValue");
            return;
        }

        var VMatrix = new Vector2Int();
        var intx = 0;
        var inty = 0;

        if (stringvalue.Length == 2)
        {
            int.TryParse(stringvalue, out intx);
            intx = (int) (intx * 0.1f);
            int.TryParse(stringvalue, out inty);
            inty = inty % 10;
            VMatrix.x = intx;
            VMatrix.y = inty;
        }
        else
        {
            intx = 0;
            int.TryParse(stringvalue, out inty);
            VMatrix.x = intx;
            VMatrix.y = inty;
        }

        foreach (var item in _MultiMapStatuses)
            if (item.IsMatrix == VMatrix)
                item.SetMultiMapStatus(Order);
    }
}