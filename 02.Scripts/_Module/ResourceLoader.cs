using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ResourceLoader<T>
{
    public static T LoadResource(string filename)
    {
        var loadText = Resources.Load<TextAsset>(filename);
        if (loadText == null) return default;
        var data = JsonUtility.FromJson<T>(loadText.text);
        return data;
    }

    public static void SaveMapInfo(string contents, int stageNum)
    {
        string path = null;
#if UNITY_EDITOR
        path = "Assets/Resources/MapData/";
#endif
        var title = string.Format("{0:D4}", stageNum);
        path = path + title + ".json";
        using (var fs = new FileStream(path, FileMode.Create))
        {
            if (fs == null)
            {
                MapEditor.GetInstance.SetSavePopup(true, "에러 발생");
                return;
            }

            using (var writer = new StreamWriter(fs))
            {
                writer.Write(contents);
                MapEditor.GetInstance.SetSavePopup(true, "저장 완료");
            }
        }
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    public static T LoadDataSA(string filename)
    {
        try
        {
            var sr = new StreamReader(Application.streamingAssetsPath + "/MapData/" + filename + ".json");
            var text = sr.ReadToEnd();
            sr.Close();

            var data = JsonUtility.FromJson<T>(text);
            return data;
        }
        catch (FileNotFoundException ex)
        {
            var sr = new StreamReader(Application.streamingAssetsPath + "/MapData/" + "Base_Mapdata" + ".json");
            var text = sr.ReadToEnd();
            sr.Close();

            var data = JsonUtility.FromJson<T>(text);
            return data;
        }
    }

    public static void SaveMapInfoSA(string contents, int stageNum)
    {
        var path = "/MapData/";
        var title = string.Format("{0:D4}", stageNum);
        path = path + title + ".json";

        var f = new FileStream(Application.streamingAssetsPath + path, FileMode.Create, FileAccess.Write);

        var writer = new StreamWriter(f, Encoding.Unicode);
        writer.WriteLine(contents);
        writer.Close();

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }
}