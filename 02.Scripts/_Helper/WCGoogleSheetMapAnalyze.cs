using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleSheetsForUnity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine.UI;

public class WCGoogleSheetMapAnalyze : MonoBehaviour
{
    [System.Serializable]
    public struct MapAnalyzeRow
    {
        public int level;
        public bool isSuccess;
        public int remainmoves;
        public int usemoves;
        public string useTimeSeconds;
        public string date;
    }

    public static WCGoogleSheetMapAnalyze instance = null;

    private string _tableName = "MapAnalyze2";
    public int timeScale = 1;

    private int testCurrentLevelCounter = 0;
    public int TestCurrentLevelCount = 20;

    public DateTime testStartDate;

    public InputField inputTestCount = null;

    public Toggle webLogCollect = null;

    public bool isWeblogCollectToggled = false;

    private void OnEnable()
    {
        // Suscribe for catching cloud responses.
        Drive.responseCallback += HandleDriveResponse;
        Refresh();
    }

    public void WebLogToggle_OnValueChanged(bool val)
    {
        webLogCollect.isOn = val;
        isWeblogCollectToggled = val;
        PlayerPrefs.SetString("Toggle_WebLogCollect", val.ToString());
    }

    private void OnDisable()
    {
        // Remove listeners.
        Drive.responseCallback -= HandleDriveResponse;
    }

    public void Refresh()
    {
        inputTestCount = GameObject.Find("InputField_TESTCount").GetComponent<InputField>();
        if (PlayerPrefs.HasKey("TestCurrentLevelCount") == true)
        {
            TestCurrentLevelCount = PlayerPrefs.GetInt("TestCurrentLevelCount");
            if (inputTestCount != null)
            {
                inputTestCount.SetTextWithoutNotify(TestCurrentLevelCount.ToString());
            }
        }
        else
        {
            var remainCount = TestCurrentLevelCount - testCurrentLevelCounter;
            inputTestCount.SetTextWithoutNotify(remainCount.ToString());
        }

        webLogCollect = GameObject.Find("Toggle_WebLogCollect").GetComponent<Toggle>();
        if (PlayerPrefs.HasKey("Toggle_WebLogCollect") == true)
        {
            isWeblogCollectToggled = Convert.ToBoolean(PlayerPrefs.GetString("Toggle_WebLogCollect"));
            if (webLogCollect != null)
            {
                webLogCollect.SetIsOnWithoutNotify(isWeblogCollectToggled);
                webLogCollect.onValueChanged.AddListener(WebLogToggle_OnValueChanged);
            }
        }
    }

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
    }

    public void CreateTableInitialize()
    {
        string[] fieldNames = new string[5];
        fieldNames[0] = "level";
        fieldNames[1] = "isSuccess";
        fieldNames[2] = "remainmoves";
        fieldNames[3] = "usemoves";
        fieldNames[4] = "useTimeSeconds";
        fieldNames[5] = "date";

        Drive.CreateTable(fieldNames, _tableName, true);
    }

    public void HandleDriveResponse(Drive.DataContainer dataContainer)
    {
        Debug.Log(dataContainer.msg);
    }

    private WaitForSeconds waitTime = new WaitForSeconds(0.3F);
    private WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();

    public IEnumerator ScreenShot()
    {
        yield return waitTime;
        yield return frameEnd;

        string folder = Application.streamingAssetsPath + "/MapScreenShot/";
        Directory.CreateDirectory(folder);
        string filename = StageManager.GetInstance.StageInfo.stageNumber.ToString("0000");
        int width = Screen.width;
        int height = Screen.height;

        Texture2D tt = new Texture2D((int)width, (int)height, TextureFormat.RGB24, false);
        tt.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tt.Apply();

        yield return frameEnd;
        // Encode texture into PNG
        byte[] bytes = tt.EncodeToPNG();

        // Save file to disk
        string filepath = Path.Combine(folder, filename + ".png");
        //FileUtil.WriteAllBytes(filepath, bytes);

        // Destroy the temporary Texture2D object
        UnityEngine.Object.Destroy(tt);
        tt = null;

        StartCoroutine(AutoRestart(1.0f));
    }

    public void Match3TestStart()
    {
        testStartDate = DateTime.Now;
    }

    public void Match3GameResult(bool isClear)
    {
        MapAnalyzeRow analyzeRow = new MapAnalyzeRow();
        foreach (var item in EditorAutoModeControll._isChallengeInfo)
        {
        }
        // analyzeRow.level = StageManager.GetInstance.;
        analyzeRow.level = StageManager.StageNumber;
        analyzeRow.isSuccess = isClear;
        analyzeRow.remainmoves = StageManager.GetInstance.PreMoveCount;
        analyzeRow.usemoves = StageManager.GetInstance.StageInfo.moveCount - StageManager.GetInstance.PreMoveCount;
        var useTime = DateTime.Now - testStartDate;
        analyzeRow.useTimeSeconds = (useTime.TotalSeconds * Time.timeScale).ToString();
        analyzeRow.date = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff", CultureInfo.InvariantCulture);
        Debug.LogWarningFormat("KKI {0}", isWeblogCollectToggled);
        if (isWeblogCollectToggled == true)
        {
            SaveAnalyzeData(analyzeRow);
        }
        if (EditorAutoModeControll._isAutoMode == true)
        {
            StartCoroutine(AutoRestart(2.0f));
        }
        Refresh();
    }

    public void StartScreenShot()
    {
        StartCoroutine(ScreenShot());
    }

    public void SetScreenShotStart(string _value)
    {
        int ioutValue = 0;
        int.TryParse(_value, out ioutValue);
        EditorAutoModeControll._iScreenShotStart = ioutValue;
    }

    public void SetTestCount(string _value)
    {
        TestCurrentLevelCount = int.Parse(_value);
        testCurrentLevelCounter = 0;
        PlayerPrefs.SetInt("TestCurrentLevelCount", TestCurrentLevelCount);
    }

    public void SetScreenShotEnd(string _value)
    {
        int ioutValue = 0;
        int.TryParse(_value, out ioutValue);
        EditorAutoModeControll._iScreenShotEnd = ioutValue;
    }

    private IEnumerator AutoRestart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        testCurrentLevelCounter++;
        if (testCurrentLevelCounter > TestCurrentLevelCount)
        {
            testCurrentLevelCounter = 0;
            MapEditor.GetInstance.NextStage();
            yield return new WaitForSeconds(2);
        }
        else
        {
            MapEditor.GetInstance.ClickGameTest();
        }

        yield return new WaitForSeconds(waitTime);

        Refresh();
        MapEditor.GetInstance.ClickGameTest();
    }

    public void SaveAnalyzeData(MapAnalyzeRow analyzeData)
    {
        string jsonPlayer = JsonUtility.ToJson(analyzeData);
        Drive.CreateObject(jsonPlayer, _tableName, true);
    }
}