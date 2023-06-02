using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorMissionListController : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;

    [SerializeField] private RectTransform _viewPort;

    [SerializeField] private GameObject MissionInfo;

    private void Start()
    {
        _scrollRect.content = _viewPort;
    }

    public List<MissionInfo> GetMissions()
    {
        var returnValue = new List<MissionInfo>();

        for (var i = 0; i < _viewPort.childCount; i++)
            if (_viewPort.GetChild(i).GetComponent<EditorMissionStatus>() != null)
                returnValue.Add(_viewPort.GetChild(i).GetComponent<EditorMissionStatus>().GetInfo);
        return returnValue;
    }

    public void CreateMissionList(Sprite sprite, EID iD, EColor color = EColor.NONE, int missionCount = 1)
    {
        var missionInfos = new List<MissionInfo>();
        missionInfos.AddRange(GetMissions());

        if (iD != EID.NORMAL) color = EColor.NONE;

        foreach (var item in missionInfos)
            if (item.eID == iD && item.eColor == color)
            {
                //Debug.LogError("This mission is already there");
                SetMissionCount(iD, missionCount);
                return;
            }

        if (missionCount != 0)
        {
            var obj = Instantiate(MissionInfo, _viewPort.transform, true);
            obj.GetComponent<EditorMissionStatus>().MissionSetting(sprite, iD, color, missionCount);
            obj.SetActive(true);
        }
    }

    public void SetMissionCount(EID id, int missionCount)
    {
        for (var i = 0; i < _viewPort.childCount; i++)
        {
            var data = _viewPort.GetChild(i).GetComponent<EditorMissionStatus>();
            if (data != null)
                if (data.GetInfo.eID == id)
                {
                    if (missionCount != 0)
                        data.SetMissionCount(missionCount);
                    else
                        data.DestroyMission();
                }
        }
    }
}