using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorSpecialTileItemController : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;

    [SerializeField] private RectTransform _viewPort;

    [SerializeField] private GameObject _tileItemObj;

    [SerializeField] private EditorToggleManager ToggleMaster;

    private int _etcValue;

    private void Start()
    {
        _scrollRect.content = _viewPort;
    }

    public void ChangeItem(int EtcValue)
    {
        _etcValue = EtcValue;

        var itemStatuses = new List<SpecialItemStatus>();
        if (SpecialTileItemList.GetInstance.GetSpecialItems(EtcValue) != null)
            itemStatuses = SpecialTileItemList.GetInstance.GetSpecialItems(EtcValue);
        if (itemStatuses.Count > _viewPort.childCount)
            while (itemStatuses.Count > _viewPort.childCount)
                CreateItem();
        for (var i = 0; i < _viewPort.childCount; i++)
            if (i < itemStatuses.Count)
            {
                _viewPort.GetChild(i).gameObject.SetActive(true);
                var blocksprite = ToggleMaster.GetSptieContainer(itemStatuses[i].blockInfo);
                _viewPort.GetChild(i).GetComponent<EditorSpecialTileItemStatus>()
                    .SetItemStatus(blocksprite, itemStatuses[i].blockInfo, itemStatuses[i].persent);
            }
            else
            {
                _viewPort.GetChild(i).gameObject.SetActive(false);
            }
    }

    public void CreateItem()
    {
        var obj = Instantiate(_tileItemObj, _viewPort, true);
    }

    public void ChangeValue(BlockInfo blockInfo, int persent)
    {
        foreach (var item in SpecialTileItemList.GetInstance.GetSpecialItems(_etcValue))
            if (item.blockInfo == blockInfo)
            {
                item.persent = persent;
                break;
            }
    }
}