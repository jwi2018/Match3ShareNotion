using System.Collections.Generic;

public class StageChallengeInfo
{
    public int _isChallenge_Count;
    public int _isClearCount;
    public int _isStage;
    public List<int> RemainMoveCount;
}

public static class EditorAutoModeControll
{
    public static bool _isAutoFull = false;
    public static int TestCount = 9;
    public static int NowCount = 0;
    public static bool _isAutoMode = false;
    public static List<StageChallengeInfo> _isChallengeInfo = new List<StageChallengeInfo>();
    public static int _isStartStage = 0;
    public static List<int> TestStageList = new List<int>();

    public static int _iScreenShotStart = 0;
    public static int _iScreenShotEnd = 0;
    public static bool _isAutoScreenshot = false;
}