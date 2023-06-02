using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using System.Text;

[Serializable]
public class ObjectData
{
    public int id;
    public int depth;
    public int color;
    public int itemKind;
}

[Serializable]
public class ObjectDataList
{
    public ObjectData[] objectDatas;
}

[Serializable]
public class TutoData
{
    public int id;
    public int stage;
}

[Serializable]
public class TutoDataList
{
    public TutoData[] tutoDatas;
}

[Serializable]
public class PracticeTutorialData
{
    public int condition;
    public string explanation;
    public bool isDarkBackground;
    public bool isShowCharacter;
    public bool isShowHand;
    public int[] highlightPosition;
    public int swapDirection;
    public int swapPosition;
    public int charPosition;
    public string charAnimTrigger;
    public bool isTextboxDown;
}

[Serializable]
public class PracticeTutorialDataList
{
    public PracticeTutorialData[] pracTutoDatas;
}

[Serializable]
public class CashItemTutorialData
{
    public int id;
    public int stage;
    public int[] highlightCashItem;
    public string explanation;
}

[Serializable]
public class CashItemTutorialDataList
{
    public CashItemTutorialData[] cashItemTutoDatas;
}

[Serializable]
public class StageMapData
{
    public int stageNum;
    public int moveCount;
    public int mapSize;
    public int[] basicBlocks;
    public int[] multimapOrder;
    public int[] tiles;
    public int[] missions;
    public int[] object_XY;
    public int[] object_EtcID;
    public int[] object_ColorHp;
}

[Serializable]
public class StageMapDataList
{
    public StageMapData stageMapDatas;
}

[Serializable]
public class ChallengeStageMapData
{
    public int stageNum;
    public int moveCount;
    public int mapSize;
    public int[] basicBlocks;
    public int[] multimapOrder;
    public int[] tiles;
    public int[] missions;
    public int[] object_XY;
    public int[] object_EtcID;
    public int[] object_ColorHp;
}

[Serializable]
public class ChallengeStageMapDataList
{
    public ChallengeStageMapData challengeStageMapData;
}
/*
//챌린지 레벨
[Serializable]
public class ChallengeStageMapData
{
    public int stageNum;
    public int moveCount;
    public int mapSize;
    public int[] basicBlocks;
    public int[] multimapOrder;
    public int[] tiles;
    public int[] missions;
    public int[] object_XY;
    public int[] object_EtcID;
    public int[] object_ColorHp;
}

//챌린지 레벨
[Serializable]
public class ChallengeStageMapDataList
{
    public ChallengeStageMapData ChallengestageMapDatas;
}
*/

[Serializable]
public class ObstacleData
{
    public string id;
    public string depth;
    public bool swap;
    public bool drop;
    public bool colorPop;
    public bool bombPop;
    public bool sidePop;
    public bool sideColorPop;
    public bool sideBombPop;
    public bool isBreak;
    public bool AnotherDrop;
    public bool AnotherPop;
    public bool AnotherSwap;
    public bool AntiBomb;
}

[Serializable]
public class ObstacleDataList
{
    public ObstacleData[] obstacleDatas;
}

[Serializable]
public struct PackageProductInfo
{
    public EPackageType PackageType;
    public string AosProductID;
    public string IosProductId;
    public Sprite PackageImage;
    public int Sale;
    public int GoldValue;
    public int HammerValue;
    public int BombValue;
    public int RainbowValue;
}

[Serializable]
public struct GoldProductInfo
{
    public EGoldType GoldType;
    public string AosProductID;
    public string IosProductId;
    public Sprite GoldImage;
    public int GoldValue;
    public int BonusValue;
}
/*
public class ItemInfo
{
    public EItem item;
    public Vector2Int matrix;
    public int color;
    public int id;
}
*/
/*
[Serializable]
public class ChallengeStageReward
{
    public int id;
    public int Acorn;
    public int Coin;
    public int Hammer;
    public int Bomb;
    public int Rainbow;
}

[Serializable]
public class ChallengeStageRewardList
{
    public ChallengeStageReward[] challengeStageRewardDatas;
}
*/
 
public class DataContainer : Singleton<DataContainer>
{
    [SerializeField] public List<PackageProductInfo> packageInfoList = new List<PackageProductInfo>();
    [SerializeField] public List<GoldProductInfo> goldInfoList = new List<GoldProductInfo>();
    private Dictionary<int, Dictionary<EChallengeRewardType, string>> ChallengeStageRewardList;
    private StageMapData _baseMapdata = new StageMapData();
    private StageMapData _baseChallengeMapData = new StageMapData();
    
    //챌린지 레벨
    //private ChallengeStageMapData _baseChallengeData = new ChallengeStageMapData();

    private StageMapData _editMapdata = new StageMapData();
    private StageMapData _editChallengeMapData = new StageMapData();
    
    private readonly StageMapData _firstMapdata = new StageMapData();
    private readonly Dictionary<int, StageMapData> _mapData = new Dictionary<int, StageMapData>();

    private readonly Dictionary<int, StageMapData> _challengeStageMapData = new Dictionary<int, StageMapData>();

    //챌린지 레벨
    //private readonly Dictionary<int, ChallengeStageMapData> _ChallengemapData = new Dictionary<int, ChallengeStageMapData>();
    private readonly Dictionary<int, ObjectData> _objectData = new Dictionary<int, ObjectData>();
    private readonly Dictionary<EID, BlockProperty> _obstacleData = new Dictionary<EID, BlockProperty>();

    private readonly Dictionary<int, PracticeTutorialDataList> _pracTutoData =
        new Dictionary<int, PracticeTutorialDataList>();

    private readonly Dictionary<int, TutoData> _tutoData = new Dictionary<int, TutoData>();

    private readonly Dictionary<int, CashItemTutorialData> _cashItemTutorialData =
        new Dictionary<int, CashItemTutorialData>();
    
    //private readonly Dictionary<int, ChallengeStageReward> _dicChallengeStageRewards
    //    = new Dictionary<int, ChallengeStageReward>();

    private int[] missionID;

    //챌린지 맵 개수 확인
    public int ChallengeMapCount;
    
    public void InitData()
    {
        LoadBaseMapData();
        LoadObstacleData();
        LoadMapData(StageManager.StageNumber);
        LoadTutoData();
        LoadDailyQuestData();
        SaveMissionID();
        LoadCashItemTutoData();
        LoadChallengeReward();
    }

    public void SetFirstMapData(StageMapData data)
    {
        _firstMapdata.basicBlocks = data.basicBlocks;
        _firstMapdata.mapSize = data.mapSize;
        _firstMapdata.missions = data.missions;
        _firstMapdata.moveCount = data.moveCount;
        _firstMapdata.multimapOrder = data.multimapOrder;
        _firstMapdata.object_ColorHp = data.object_ColorHp;
        _firstMapdata.object_XY = data.object_XY;
        _firstMapdata.object_EtcID = data.object_EtcID;
        _firstMapdata.stageNum = StageManager.StageNumber;
        _firstMapdata.tiles = data.tiles;
    }

    public StageMapData GetFirstMapData()
    {
        return _firstMapdata;
    }

    public void SaveMissionID()
    {
        missionID = new int[StaticGameSettings.TotalStage + 1];
        var count = 0;
        ChallengeMapCount = 0;

        for (var i = 1; i <= StaticGameSettings.TotalStage; i++)
        {
            LoadMapData(i);

            if (BaseSystem.GetInstance != null)
            {
                if (ChallengeSystem.GetInstance!=null)
                {
                    var isChallengeMap = i % 20;

                    if (i.Equals(50))
                    {
                        LoadChallengeMapData(i);
                    }
                    if (i >= 200 && isChallengeMap.Equals(0))
                    {
                        LoadChallengeMapData(i);
                        ChallengeMapCount++;
                    }
                }
            }
            if (_mapData[i].missions.Length == 0)
            {
                missionID[i - 1] = -1;
            }
            else if (_mapData[i].missions[0] == -1)
            {
                missionID[i - 1] = -1;
            }
            else
            {
                var missionValue = _mapData[i].missions[0];
                count = missionValue / 10000;
                missionID[i - 1] = (missionValue - count * 10000) / 10;
            }
        }
    }

    public int[] GetMissionID()
    {
        return missionID;
    }

    public void LoadBaseMapData()
    {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
        _baseMapdata = ResourceLoader<StageMapData>.LoadResource("MapData/" + "Base_Mapdata");
        _editMapdata = ResourceLoader<StageMapData>.LoadResource("MapData/" + "Base_Mapdata");
#else
        _baseMapdata = ResourceLoader<StageMapData>.LoadDataSA("Base_Mapdata");
        _editMapdata = ResourceLoader<StageMapData>.LoadDataSA("Base_Mapdata");
#endif
    }

    public void LoadObjectData()
    {
        var objectData = ResourceLoader<ObjectDataList>.LoadResource("ObjectData");
        for (var i = 0; i < objectData.objectDatas.Length; i++)
            if (!_objectData.ContainsKey(objectData.objectDatas[i].id))
                _objectData.Add(objectData.objectDatas[i].id, objectData.objectDatas[i]);
    }

    public void LoadMapData(int stageNumber)
    {
        var file = string.Format("{0:D4}", stageNumber);
        StageMapData mapData = null;
        //mapData = ResourceLoader<StageMapData>.LoadDataSA(file);
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
        mapData = ResourceLoader<StageMapData>.LoadResource("MapData/" + file);
#else
        mapData = ResourceLoader<StageMapData>.LoadDataSA(file);
#endif
        if (mapData != null)
        {
            if (!_mapData.ContainsKey(stageNumber)) _mapData.Add(stageNumber, mapData);
        }
        else
        {
            if (!_mapData.ContainsKey(stageNumber)) _mapData.Add(stageNumber, _baseMapdata);
        }
    }

    public void LoadChallengeMapData(int stageNumber)
    {
        var file = string.Format("{0:D4}", stageNumber);
        StageMapData challengeMapData = null;
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
        challengeMapData = ResourceLoader<StageMapData>.LoadResource("ChallengeMapData/" + file);
#else
        challengeMapData = ResourceLoader<StageMapData>.LoadDataSA(file);
#endif
        if (challengeMapData != null)
        {
            if(!_challengeStageMapData.ContainsKey(stageNumber)) _challengeStageMapData.Add(stageNumber, challengeMapData);
        }
        else
        {
            if(!_challengeStageMapData.ContainsKey(stageNumber)) _challengeStageMapData.Add(stageNumber, _baseChallengeMapData);
        }
    }
    
    //챌린지 레벨
    /*
    public void ChallengeLoadMapData(int stageNumber)
    {
        var file = string.Format("{0:D4}", stageNumber);
        ChallengeStageMapData challengeStageMapData = null;
        //mapData = ResourceLoader<StageMapData>.LoadDataSA(file);
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
        challengeStageMapData = ResourceLoader<ChallengeStageMapData>.LoadResource("ChallengeMapData/" + file);
#else
        mapData = ResourceLoader<StageMapData>.LoadDataSA(file);
#endif
        if (challengeStageMapData != null)
        {
            if (!_ChallengemapData.ContainsKey(stageNumber)) _ChallengemapData.Add(stageNumber, challengeStageMapData);
        }
        else
        {
            if (!_ChallengemapData.ContainsKey(stageNumber)) _ChallengemapData.Add(stageNumber, _baseChallengeData);
        }
    }
    */

    public void LoadObstacleData()
    {
        var obstacleDataList = ResourceLoader<ObstacleDataList>.LoadResource("BlockData");
        for (var i = 0; i < obstacleDataList.obstacleDatas.Length; i++)
        {
            var idString = obstacleDataList.obstacleDatas[i].id;
            var id = (EID)Enum.Parse(typeof(EID), idString);
            var depthString = obstacleDataList.obstacleDatas[i].depth;
            var depth = (EDepth)Enum.Parse(typeof(EDepth), depthString);

            var blockProperty = new BlockProperty();
            blockProperty.moveProperty = new BlockMoveProperty();
            blockProperty.popProperty = new BlockPopProperty();

            blockProperty.depth = depth;
            blockProperty.moveProperty.Drop = obstacleDataList.obstacleDatas[i].drop;
            blockProperty.moveProperty.Swap = obstacleDataList.obstacleDatas[i].swap;
            blockProperty.moveProperty.AnotherDrop = obstacleDataList.obstacleDatas[i].AnotherDrop;
            blockProperty.moveProperty.AnotherSwap = obstacleDataList.obstacleDatas[i].AnotherSwap;

            blockProperty.popProperty.ColorPop = obstacleDataList.obstacleDatas[i].colorPop;
            blockProperty.popProperty.BombPop = obstacleDataList.obstacleDatas[i].bombPop;
            blockProperty.popProperty.SidePop = obstacleDataList.obstacleDatas[i].sidePop;
            blockProperty.popProperty.SideColorPop = obstacleDataList.obstacleDatas[i].sideColorPop;
            blockProperty.popProperty.SideBombPop = obstacleDataList.obstacleDatas[i].sideBombPop;
            blockProperty.popProperty.Break = obstacleDataList.obstacleDatas[i].isBreak;
            blockProperty.popProperty.AntiBomb = obstacleDataList.obstacleDatas[i].AntiBomb;
            blockProperty.popProperty.AnotherPop = obstacleDataList.obstacleDatas[i].AnotherPop;

            if (!_obstacleData.ContainsKey(id)) _obstacleData.Add(id, blockProperty);
        }
    }

    public void LoadDailyQuestData()
    {
        var dailyQuestData = ResourceLoader<DailyQuestData>.LoadResource("DailyQuestData");
    }

    public void LoadTutoData()
    {
        var tutoDataList = ResourceLoader<TutoDataList>.LoadResource("PracticeTutorialData");
        for (var i = 0; i < tutoDataList.tutoDatas.Length; i++)
            if (!_tutoData.ContainsKey(tutoDataList.tutoDatas[i].stage))
                _tutoData.Add(tutoDataList.tutoDatas[i].stage, tutoDataList.tutoDatas[i]);

        foreach (var item in _tutoData)
        {
            var id = item.Value.id;
            var file = string.Format("{0:D4}", id);
            var pracDataList =
                ResourceLoader<PracticeTutorialDataList>.LoadResource("PTutoData/" + "TUTO_" + file);

            if (pracDataList != null)
                if (!_pracTutoData.ContainsKey(id))
                    _pracTutoData.Add(id, pracDataList);
        }
    }

    public void LoadCashItemTutoData()
    {
        var cashItemTutoList = ResourceLoader<CashItemTutorialDataList>.LoadResource("CashItemTutorialData");
        for (var i = 0; i < cashItemTutoList.cashItemTutoDatas.Length; i++)
        {
            if (!_cashItemTutorialData.ContainsKey(cashItemTutoList.cashItemTutoDatas[i].id))
            {
                _cashItemTutorialData.Add(cashItemTutoList.cashItemTutoDatas[i].id, cashItemTutoList.cashItemTutoDatas[i]);
            }
        }
    }
    
    public void LoadChallengeReward()
    {
        if (BaseSystem.GetInstance != null)
        {
            if (ChallengeSystem.GetInstance!=null)
            {
                var loadText = Resources.Load<TextAsset>("ChallengeStageRewardItemData");
                ChallengeStageRewardList = JsonConvert.DeserializeObject<Dictionary<int,Dictionary<EChallengeRewardType,string>>>(loadText.text);
            }
        }
    }

    public Dictionary<EChallengeRewardType, string> GetChallengeRewardList(int stageNum)
    {
        return ChallengeStageRewardList[stageNum];
    }

    public void LoadTestMapData(StageMapData editData)
    {
        _editMapdata = editData;
    }

    public void SaveMapData(StageMapData mapData)
    {
        var jsonData = JsonUtility.ToJson(mapData);
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
        ResourceLoader<StageMapDataList>.SaveMapInfo(jsonData, mapData.stageNum);
#else
        ResourceLoader<StageMapDataList>.SaveMapInfoSA(jsonData, mapData.stageNum);
#endif
    }

    public StageMapData GetMapDataOrNULL(int stageNum)
    {
        /*
        if (!StageInfo.IsEditMode)
        {
            if (_mapData.ContainsKey(stageNum))
            {
                return _mapData[stageNum];
            }
            else
            {
                LoadMapData(stageNum);
                return _mapData[stageNum];
            }
        }
        else
        {
            if (stageNum == -1)
            {
                return _baseMapdata;
            }
            else
            {
                return _editMapdata;
            }
        }
        */

        if (_mapData.ContainsKey(stageNum))
            return _mapData[stageNum];
        if (stageNum == -1)
            return _editMapdata;
        return _baseMapdata;
    }

    public StageMapData GetChallengeMapDataOrNULL(int stageNum)
    {
        if (_challengeStageMapData.ContainsKey(stageNum))
        {
            return _challengeStageMapData[stageNum];
        }

        if (stageNum == -1)
        {
            return _editChallengeMapData;
        }
        return _baseChallengeMapData;
    }

    public PracticeTutorialDataList GetTutorialDataListOrNull(int stageNum)
    {
        if (!_tutoData.ContainsKey(stageNum)) return null;

        var id = _tutoData[stageNum].id;

        if (!_pracTutoData.ContainsKey(id)) return null;

        return _pracTutoData[id];
    }

    
    public CashItemTutorialData GetCashItemTutorialDataOrNull(int stageNum)
    {
        if (!_cashItemTutorialData.ContainsKey(stageNum)) return null;

        return _cashItemTutorialData[stageNum];
    }

    /*
    public ChallengeStageReward GetChallengeStageRewardDataOrNull(int stageNum)
    {
        if (!_dicChallengeStageRewards.ContainsKey(stageNum)) return null;

        return _dicChallengeStageRewards[stageNum];
    }
    */
    
    public StageMapData GetBaseData()
    {
        return _baseMapdata;
    }

    public ObjectData GetObjectDataOrNULL(int id)
    {
        if (_objectData.ContainsKey(id))
            return _objectData[id];
        return null;
    }

    public BlockProperty GetObstacleDataOrNULL(EID id)
    {
        if (_obstacleData.ContainsKey(id))
            return _obstacleData[id];
        return null;
    }

    public int GetTutorialNumber(int stageNum)
    {
        if (_tutoData.ContainsKey(stageNum))
        {
            return _tutoData[stageNum].id;
        }

        if (_cashItemTutorialData.ContainsKey(stageNum))
        {
            return _cashItemTutorialData[stageNum].id;
        }
        return -1;
    }

    public bool GetCashItemTutorialStart(int stageNum)
    {
        if (_cashItemTutorialData.ContainsKey(stageNum))
        {
            return true;
        }

        return false;
    }

    public void Clear()
    {
        _mapData.Clear();
        _objectData.Clear();
        _obstacleData.Clear();
        _tutoData.Clear();
        if (ChallengeSystem.GetInstance!=null)
        {
            _challengeStageMapData.Clear();
        }
    }
}