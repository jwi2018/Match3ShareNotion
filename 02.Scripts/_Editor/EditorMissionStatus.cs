using UnityEngine;
using UnityEngine.UI;

public struct MissionInfo
{
    public EID eID;
    public EColor eColor;
    public int MissionCount;
}

public class EditorMissionStatus : MonoBehaviour
{
    [SerializeField] private Image MissionImage;

    [SerializeField] private InputField MissionCountingText;


    private MissionInfo missionInfo;

    public MissionInfo GetInfo => missionInfo;

    public void MissionSetting(Sprite sprite, EID iD, EColor color, int missionCount)
    {
        MissionImage.sprite = sprite;
        missionInfo.eID = iD;
        missionInfo.eColor = color;
        MissionCountingText.text = missionCount.ToString();
        MissionCounting(MissionCountingText.textComponent);
    }

    public void MissionCounting(Text text)
    {
        var strings = text.text;
        int.TryParse(strings, out missionInfo.MissionCount);
    }

    public void DestroyMission()
    {
        Destroy(gameObject);
    }

    public void SetMissionCount(int missionCount)
    {
        MissionCountingText.text = missionCount.ToString();
        missionInfo.MissionCount = missionCount;
    }
}