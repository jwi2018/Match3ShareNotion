using System.IO;
using UnityEditor;

public class EditorAutoLog : Singleton<EditorAutoLog>
{
    public void TestJson()
    {
        string path = null;
#if UNITY_EDITOR
        path = "Assets/AutoModeData/";
#endif
        var title = "TestStage " + EditorAutoModeControll._isStartStage;
        path = path + title + ".json";
        string Json = null;
        string AddRange = null;

        foreach (var item in EditorAutoModeControll._isChallengeInfo)
        {
            AddRange += "Stage : " + item._isStage + "\nStageChallengeCount : " + item._isChallenge_Count
                        + "\nStageClearCount : " + item._isClearCount + "\n" + "RamainMoveCount : [";
            foreach (var _initem in item.RemainMoveCount) AddRange += _initem + ", ";
            AddRange += "]\nStageFailCount : " + (item._isChallenge_Count - item._isClearCount) + "\n\n";

            Json += AddRange;
        }
        /*string title = "Auto" + EditorAutoModeControll._isStartStage + "~" + StageManager.StageNumber;
        path = path + title + ".json";
        string Json = null;
        int Check = EditorAutoModeControll._isStartStage;

        while(Check <= StageManager.StageNumber)
        {
            string AddRange = null;
            foreach(var item in EditorAutoModeControll._isChallengeInfo)
            {
                if(item._isStage == Check)
                {
                    AddRange += "Stage : " + item._isStage + "\nStageChallengeCount : " + item._isChallenge_Count
                                                 + "\nStageClearCount : " + item._isClearCount + "\n" + "RamainMoveCount : [";
                    foreach(var _initem in item.RemainMoveCount)
                    {
                        AddRange += _initem + ", ";
                    }
                    AddRange += "]\nStageFailCount : " + (item._isChallenge_Count - item._isClearCount) + "\n\n";
                }
            }
            Json += AddRange;
            Check++;
        }*/

        using (var fs = new FileStream(path, FileMode.Create))
        {
            using (var writer = new StreamWriter(fs))
            {
                writer.Write(Json);
            }
        }
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }
}