using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public class EditorMapShuffleController : MonoBehaviour
{
    public InputField inputMapIndexMin = null;
    public InputField inputMapIndexMax = null;

    private int iShuffleTargetMin = 0;
    private int iShuffleTargetMax = 0;

    public class MapMoveData
    {
        public bool isChanged = false;
        public int level = 0;
        public string fileName = "";
        public string movedPath = "";
    }

    private Dictionary<string, MapMoveData> dicPaths = new Dictionary<string, MapMoveData>();

    [ContextMenu("ShuffleMapData")]
    public void ShuffleMapData()
    {
        bool isMinSuccess = int.TryParse(inputMapIndexMin.text, out iShuffleTargetMin);
        bool isMaxSuccess = int.TryParse(inputMapIndexMax.text, out iShuffleTargetMax);
        dicPaths.Clear();
        for (var stageNumber = iShuffleTargetMin; stageNumber <= iShuffleTargetMax; stageNumber++)
        {
            var filename = string.Format("{0:D4}", stageNumber);
            string path = string.Format($"{Application.streamingAssetsPath}/MapData/{filename}.json");

            if (File.Exists(path))
            {
                string destPath = string.Format($"{Application.streamingAssetsPath}/MapData/{filename}_move.json");
                File.Move(path, destPath);

                MapMoveData indata = new MapMoveData();
                indata.fileName = filename;
                indata.movedPath = destPath;
                indata.level = stageNumber;
                dicPaths.Add(filename, indata);
            }
        }

        List<string> tempKeys = dicPaths.Keys.ToList();
        while (tempKeys.Count > 0)
        {
            string currentKey = tempKeys[0];
            tempKeys.Remove(currentKey);

            if (tempKeys.Count == 0)
            {
                ExchangeMapData(dicPaths[currentKey], dicPaths[currentKey]);
                break;
            }
            string targetKey = tempKeys[UnityEngine.Random.Range(0, tempKeys.Count)];
            tempKeys.Remove(targetKey);

            ExchangeMapData(dicPaths[currentKey], dicPaths[targetKey]);
            ExchangeMapData(dicPaths[targetKey], dicPaths[currentKey]);
        }

        MapEditor.GetInstance.SetSavePopup(true, $"{iShuffleTargetMin}~{iShuffleTargetMax} : {dicPaths.Keys.Count}개 섞기 완료");
    }

    private void ExchangeMapData(MapMoveData source, MapMoveData dest)
    {
        var sr = new StreamReader(source.movedPath);
        var text = sr.ReadToEnd();
        sr.Close();
        var data = JsonUtility.FromJson<StageMapData>(text);
        data.stageNum = dest.level;

        string saveJson = JsonUtility.ToJson(data);
        var f = new FileStream(source.movedPath, FileMode.Create, FileAccess.Write);
        var writer = new StreamWriter(f, Encoding.Unicode);
        writer.WriteLine(saveJson);
        writer.Close();

        string firstItemPath = string.Format($"{Application.streamingAssetsPath}/MapData/{dest.fileName}.json");
        File.Move(source.movedPath, firstItemPath);
    }
}