using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorStageBlockController : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;

    [SerializeField] private RectTransform _viewPort;

    [SerializeField] private GameObject _stageBlockInfo;

    private void Start()
    {
        _scrollRect.content = _viewPort;
    }

    public void CreateStageBlock(Sprite sprite, EID iD, EColor color = EColor.NONE, int HP = 1, int persent = 0,
        int etc = 0)
    {
        var stageBlockInfos = new List<StageBlockInfo>();
        stageBlockInfos.AddRange(GetStageBlocks());
        foreach (var item in stageBlockInfos)
            if (item.blockInfo.ID == iD && item.blockInfo.HP == HP)
                return;
        var obj = Instantiate(_stageBlockInfo, _viewPort.transform, true);
        obj.GetComponent<EditorStageBlockStatus>().CreateStageBlock(sprite, iD, color, HP, persent, etc);
        obj.SetActive(true);
        CheckPersentValue();
    }

    public void CheckPersentValue()
    {
        var persent = 100;
        var StageBlockInfos = new List<StageBlockInfo>();
        StageBlockInfos.AddRange(GetStageBlocks());
        foreach (var item in StageBlockInfos)
        {
            if (item.blockInfo.ID == EID.NORMAL) continue;
            persent -= item._isPersent;
        }

        if (persent < 0)
        {
            for (var i = 0; i < _viewPort.childCount; i++)
                if (_viewPort.GetChild(i).GetComponent<EditorStageBlockStatus>() != null)
                    _viewPort.GetChild(i).GetComponent<EditorStageBlockStatus>().GetInputField.targetGraphic.color =
                        new Color(1, 0, 0, 1);
        }
        else
        {
            for (var i = 0; i < _viewPort.childCount; i++)
                if (_viewPort.GetChild(i).GetComponent<EditorStageBlockStatus>() != null)
                {
                    _viewPort.GetChild(i).GetComponent<EditorStageBlockStatus>().GetInputField.targetGraphic.color =
                        new Color(1, 1, 1, 1);
                    if (_viewPort.GetChild(i).GetComponent<EditorStageBlockStatus>().GetStageBlockInfo().blockInfo.ID ==
                        EID.NORMAL)
                        _viewPort.GetChild(i).GetComponent<EditorStageBlockStatus>().BlockPersentChange(persent);
                }
        }
    }

    public List<StageBlockInfo> GetStageBlocks()
    {
        var returnValue = new List<StageBlockInfo>();

        for (var i = 0; i < _viewPort.childCount; i++)
            if (_viewPort.GetChild(i).GetComponent<EditorStageBlockStatus>() != null)
                returnValue.Add(_viewPort.GetChild(i).GetComponent<EditorStageBlockStatus>().GetStageBlockInfo());
        return returnValue;
    }
}