using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum EEditorMode
{
    NONE,
    GRAVITY,
    OBJECT,
    TILE
}

public class MapEditor : Singleton<MapEditor>
{
    [SerializeField] private EditImageTile[] editImageTiles;
    [SerializeField] private EditorUI editorUI;
    [SerializeField] private SpriteContainer spriteContainer;
    [SerializeField] private EditSpriteContainer editSpriteContainer;
    [SerializeField] private GameObject editFieldObj;
    [SerializeField] private GameObject[] editUseObjects;
    [SerializeField] private EditorToggleManager editorToggleManager;

    [SerializeField] private GameObject bigSideObject;
    [SerializeField] private GameObject bigSideColorObject;
    [SerializeField] private GameObject bigSideDirectionObject;
    [SerializeField] private GameObject tablet11Object;
    [SerializeField] private GameObject tablet12Object;
    [SerializeField] private GameObject tablet22Object;
    [SerializeField] private GameObject tablet23Object;
    [SerializeField] private GameObject tablet33Object;
    [SerializeField] private GameObject gearObject;

    [SerializeField] private GameObject barricadeObject;

    [SerializeField] private GameObject tunnelEnranceObj;
    [SerializeField] private GameObject tunnelExitObj;

    [SerializeField] private Sprite[] railSprites;
    [SerializeField] private GameObject editRailObj;
    [SerializeField] private TabletContainer tabletContainer;

    private readonly EDirection barricadeDirection = EDirection.UP;
    private readonly List<Vector2Int> barricadesHorizontal = new List<Vector2Int>();

    private readonly Dictionary<Vector2Int, GameObject> barricadesHorizontalObjs =
        new Dictionary<Vector2Int, GameObject>();

    private readonly List<Vector2Int> barricadesVertical = new List<Vector2Int>();

    private readonly Dictionary<Vector2Int, GameObject> barricadesVerticalObjs =
        new Dictionary<Vector2Int, GameObject>();

    private readonly Dictionary<EID, EDepth> bigObjDepth = new Dictionary<EID, EDepth>();
    private readonly Dictionary<Vector2Int, BlockInfo> bigObjMatrixDictionary = new Dictionary<Vector2Int, BlockInfo>();
    private readonly Dictionary<EditImageTile, GameObject> bigObjOnTile = new Dictionary<EditImageTile, GameObject>();
    private readonly Dictionary<EID, Vector2Int> bigSizeDictionary = new Dictionary<EID, Vector2Int>();
    private int etcValue;
    private int hp = 1;
    private StageMapData initMapdata = new StageMapData();
    private bool isMultiMap;
    private bool isTestStart;
    private Vector2Int mapSize = new Vector2Int(1, 1);

    private Vector2Int preMap = new Vector2Int(0, 0);

    private StageMapData preMapdata = new StageMapData();

    private RailProperty? preRailData;
    private readonly List<RailOnMatrix> railOnMatrices = new List<RailOnMatrix>();
    private readonly Dictionary<RailOnMatrix, GameObject> railOnMatrixObjs = new Dictionary<RailOnMatrix, GameObject>();
    private readonly List<EID> specialBlocks = new List<EID>();


    private StageInfo stageInfo;
    private readonly Dictionary<Vector2Int, EditTile> tileDictionary = new Dictionary<Vector2Int, EditTile>();

    private readonly Dictionary<Vector2Int, EditImageTile> tileImageDictionary =
        new Dictionary<Vector2Int, EditImageTile>();

    private readonly Dictionary<Vector2Int, GameObject> tunnelEntranceObjs = new Dictionary<Vector2Int, GameObject>();

    private readonly List<Tunnel> tunnelEntrances = new List<Tunnel>();
    private readonly Dictionary<Vector2Int, GameObject> tunnelExitObjs = new Dictionary<Vector2Int, GameObject>();
    private readonly List<Tunnel> tunnelExits = new List<Tunnel>();
    public EEditorMode Mode { get; private set; } = EEditorMode.NONE;

    public EDirection DropDirection { get; private set; } = EDirection.DOWN;

    public ETileKind TileKind { get; private set; } = ETileKind.NORMAL;

    public EID PreID { get; private set; } = EID.NONE;

    public EColor PreColor { get; private set; } = EColor.NONE;

    public int Hp { get; }
    public int EtcValue { get; }
    public BlockInfo BlockINFO { get; } = new BlockInfo(EID.NONE, EColor.NONE, 1, 0);

    public EDepth Depth { get; private set; } = EDepth.NORMAL;

    public bool IsBigMode { get; set; }
    public RailProperty? PreRailProperty => preRailData;

    public bool IsMouseDown { get; set; }

    public void Init()
    {
        stageInfo = new StageInfo();
        StageManager.GetInstance.IsEditorMode = true;

        if (editorUI != null)
            editorUI.Init();
        //EditSetting();

        int maxX = 27, maxY = 27;

        for (var y = 0; y < maxY; y++)
        for (var x = 0; x < maxX; x++)
        {
            var tile = new EditTile();
            tile.Init();
            tile.Setting(new Vector2Int(x, y), ETileKind.VOID, EDirection.DOWN);
            tileDictionary.Add(tile.Matrix, tile);
        }

        foreach (var imageTile in editImageTiles) imageTile.Init();

        bigSizeDictionary.Add(EID.BIG_SIDE, new Vector2Int(2, 2));
        bigSizeDictionary.Add(EID.BIG_SIDE_COLOR, new Vector2Int(2, 2));
        bigSizeDictionary.Add(EID.BIG_SIDE_DIRECTION, new Vector2Int(2, 2));
        bigSizeDictionary.Add(EID.GEAR, new Vector2Int(3, 3));

        bigObjDepth.Add(EID.BIG_SIDE, EDepth.NORMAL);
        bigObjDepth.Add(EID.BIG_SIDE_COLOR, EDepth.NORMAL);
        bigObjDepth.Add(EID.BIG_SIDE_DIRECTION, EDepth.NORMAL);
        bigObjDepth.Add(EID.GEAR, EDepth.FLOOR);
        bigObjDepth.Add(EID.TABLET, EDepth.TABLET);

        specialBlocks.Add(EID.JAM);
        specialBlocks.Add(EID.BIG_SIDE);
        specialBlocks.Add(EID.BIG_SIDE_COLOR);
        specialBlocks.Add(EID.GOLD);
        specialBlocks.Add(EID.RELIC_IN_INVISIBLE_BOX);
        specialBlocks.Add(EID.TABLET);
        specialBlocks.Add(EID.CLAM);
        specialBlocks.Add(EID.GLASS_COLOR);
        specialBlocks.Add(EID.ACTINIARIA);
        specialBlocks.Add(EID.TURN_BOX);
        specialBlocks.Add(EID.DOUBLE);
        specialBlocks.Add(EID.CLIMBER_NEST);

        MapSetting(false);
        SetUI();
    }

    public void EditSetting()
    {
        IsBigMode = false;
        IsMouseDown = false;
        isTestStart = false;
        Mode = EEditorMode.NONE;
        DropDirection = EDirection.DOWN;
        TileKind = ETileKind.NORMAL;
        PreID = EID.NONE;
        hp = 1;
        etcValue = 0;
        Depth = EDepth.NORMAL;
        SetUI();
    }

    public void MapSetting(bool isBaseData)
    {
        HardClear();

        var tunnelEntranceDatas = new List<Vector2Int>();
        var tunnelExitDatas = new List<Vector2Int>();

        var num = StageManager.StageNumber;

        StageMapData data = null;
        if (isBaseData) num = -2;
        data = DataContainer.GetInstance.GetMapDataOrNULL(num);

        if (BaseSystem.GetInstance != null)
        {
            if (ChallengeSystem.GetInstance!=null)
            {
                if (ChallengeSystem.GetInstance.IsChallengeStage)
                {
                    data = DataContainer.GetInstance.GetChallengeMapDataOrNULL(num);
                }
                else
                {
                    data = DataContainer.GetInstance.GetMapDataOrNULL(num);
                }
            }
        }

        if (data != null)
        {
            preMapdata = data;
            initMapdata = data;

            stageInfo.moveCount = data.moveCount;
            editorToggleManager.SetMoveCount(stageInfo.moveCount);

            int percent, id, color = 0;

            var basicColors = new List<EColor>();

            foreach (var item in data.basicBlocks)
            {
                percent = item / 10000;
                id = (item - percent * 10000) / 10;
                color = item - percent * 10000 - id * 10;

                stageInfo.AddBasicBlock(new BlockInfo((EID) id, (EColor) color, 1, 0), percent);
                editorToggleManager.SetStageBlock((EID) id, (EColor) color, 1, percent);
                if ((EID) id == EID.NORMAL)
                    if (!basicColors.Contains((EColor) color))
                        basicColors.Add((EColor) color);
            }

            editorToggleManager.SetBlockCount(basicColors.Count);

            int count, mID, mColor = 0;

            foreach (var item in data.missions)
            {
                if (item == -1) continue;

                count = item / 10000;
                mID = (item - count * 10000) / 10;
                mColor = item - count * 10000 - mID * 10;

                stageInfo.AddMissions(new MissionKind((EID) mID, (EColor) mColor, count));
                editorToggleManager.SetMission((EID) mID, (EColor) mColor, count);
            }

            //if(data.multimapOrder.Length == 1)
            {
                int direction, kind = 0;
                int mapSizeX = 1, mapSizeY = 1;
                mapSizeX = data.mapSize / 10;
                mapSizeY = data.mapSize - mapSizeX * 10;

                var tempList = new List<int>();

                for (var i = 0; i < data.multimapOrder.Length; i++)
                {
                    var x = data.multimapOrder[i] / 10;
                    var y = data.multimapOrder[i] - x * 10;
                    tempList.Add(x * 10 + y);
                }

                var order = 1;
                foreach (var MultimapPos in tempList)
                {
                    editorToggleManager.SetMultiMapSetting(MultimapPos, order);
                    order++;
                }

                var key = new Vector2Int();

                for (var y = 0; y < 9 * mapSizeY; y++)
                for (var x = 0; x < 9 * mapSizeX; x++)
                {
                    key.x = x;
                    key.y = y;
                    direction = data.tiles[x + y * 9 * mapSizeX] / 10;
                    kind = data.tiles[x + y * 9 * mapSizeX] - direction * 10;

                    if (tileDictionary.ContainsKey(key))
                        tileDictionary[key].Setting(key, (ETileKind) kind, (EDirection) direction);
                }

                preMap.x = data.multimapOrder[0] / 10;
                preMap.y = data.multimapOrder[0] - preMap.x * 10;
            }

            for (var i = 0; i < data.object_XY.Length; i++)
            {
                if (i >= data.object_EtcID.Length) continue;
                if (i >= data.object_ColorHp.Length) continue;

                var objXY = data.object_XY[i];
                var objEtcID = data.object_EtcID[i];
                var objColorHp = data.object_ColorHp[i];

                if (objXY == -1 || objEtcID == -1 || objColorHp == -1) break;

                var x = objXY / 100;
                var y = objXY - x * 100;
                var xy = new Vector2Int(x, y);

                var objEtc = objEtcID / 1000;
                var objID = objEtcID - objEtc * 1000;

                var objColor = objColorHp / 100;
                var objHp = objColorHp - objColor * 100;

                if ((EID) objID != EID.BARRICADE && (EID) objID != EID.TUNNEL_ENTRANCE
                                                 && (EID) objID != EID.TUNNEL_EXIT && (EID) objID != EID.RAIL)
                {
                    if (tileDictionary.ContainsKey(xy))
                    {
                        var property = DataContainer.GetInstance.GetObstacleDataOrNULL((EID) objID);

                        var depth = EDepth.NORMAL;
                        if (property != null) depth = property.depth;
                        var block = new BlockInfo((EID) objID, (EColor) objColor, objHp, objEtc);
                        if (!bigSizeDictionary.ContainsKey(block.ID) && (EID) objID != EID.TABLET)
                            tileDictionary[xy].InputBlock(depth, block);
                        else
                            CreateBigObjInData(xy, block);
                    }
                }
                else if ((EID) objID == EID.BARRICADE)
                {
                    AddBarricadeData(xy, objEtc);
                }
                else if ((EID) objID == EID.TUNNEL_ENTRANCE)
                {
                    if ((EColor) objColor == EColor.RED)
                        tunnelEntranceDatas.Add(xy);
                    else
                        tunnelExitDatas.Add(xy);
                }
                else if ((EID) objID == EID.TUNNEL_EXIT)
                {
                    tunnelExitDatas.Add(xy);
                }
                else if ((EID) objID == EID.RAIL)
                {
                    var railOnMatrix = new RailOnMatrix();
                    railOnMatrix.matrix = xy;
                    railOnMatrix.property.order = objEtc / 10;
                    var isRemoveValue = false;
                    if (objEtc - railOnMatrix.property.order * 10 == 1) isRemoveValue = true;
                    railOnMatrix.property.isRemove = isRemoveValue;

                    railOnMatrix.property.power = objColor;
                    railOnMatrix.property.railKind = objHp;

                    AddRailInfo(railOnMatrix);
                }
            }

            foreach (var item in tunnelEntranceDatas) AddTunnelInfo(item, true);
            foreach (var item in tunnelExitDatas) AddTunnelInfo(item, false);
            DataContainer.GetInstance.SetFirstMapData(data);
            SetPreMap();
        }
    }

    public void SetPreMap()
    {
        var matrix = new Vector2Int();
        for (var y = preMap.y * 9; y < preMap.y * 9 + 9; y++)
        for (var x = preMap.x * 9; x < preMap.x * 9 + 9; x++)
        {
            var imageTileNum = x + (y - preMap.y * 9) * 9 - preMap.x * 9;
            matrix.x = x;
            matrix.y = y;
            if (tileDictionary.ContainsKey(matrix))
            {
                var tileData = tileDictionary[matrix];
                editImageTiles[imageTileNum].Setting(tileData.Matrix, tileData.TileKind, tileData.DropDirection);
                tileImageDictionary.Add(new Vector2Int(x, y), editImageTiles[imageTileNum]);

                var dictionary = tileData.GetObjectDictionary();
                if (dictionary != null)
                    foreach (var item in dictionary)
                        editImageTiles[imageTileNum].InputObject(item.Key, item.Value);

                if (barricadesHorizontal.Contains(matrix))
                    AddBarricadeInPreMap(editImageTiles[imageTileNum], EDirection.UP);
                if (barricadesVertical.Contains(matrix))
                    AddBarricadeInPreMap(editImageTiles[imageTileNum], EDirection.LEFT);
                foreach (var tunnel in tunnelEntrances)
                    if (tunnel.Matrix == matrix)
                        AddTunnelOnImageTile(editImageTiles[imageTileNum], true);
                foreach (var tunnel in tunnelExits)
                    if (tunnel.Matrix == matrix)
                        AddTunnelOnImageTile(editImageTiles[imageTileNum], false);
                foreach (var item in railOnMatrices)
                    if (item.matrix == matrix)
                        AddRailOnTile(editImageTiles[imageTileNum], item);
            }
        }


        foreach (var item in bigObjMatrixDictionary)
            if (tileImageDictionary.ContainsKey(item.Key))
                CreateBigObjOnPreTile(tileImageDictionary[item.Key], item.Value);
    }

    public void SetUI()
    {
        if (editorUI == null) return;

        editorUI.SetGravityRotation(DropDirection);
        editorUI.SetStageNumber(StageManager.StageNumber);
    }

    public void RemoveAtMatrix(Vector2Int matrix)
    {
        Tunnel removeTunnel = null;
        foreach (var item in tunnelEntrances)
            if (item.Matrix == matrix)
                removeTunnel = item;
        if (removeTunnel != null)
        {
            RemoveTunnelOnImageTile(matrix, true);
            RemoveTunnelInfo(matrix, true);
        }

        removeTunnel = null;

        foreach (var item in tunnelExits)
            if (item.Matrix == matrix)
                removeTunnel = item;
        if (removeTunnel != null)
        {
            RemoveTunnelOnImageTile(matrix, false);
            RemoveTunnelInfo(matrix, false);
        }

        var removeList = new List<RailOnMatrix>();

        foreach (var rail in railOnMatrices)
            if (rail.matrix == matrix)
                removeList.Add(rail);

        foreach (var item in removeList)
        {
            RemoveRailObj(item);
            RemoveRailInfo(item);
        }
    }


    public void ClickGameTest()
    {
        // send present data to datacontainer
        // hide ui like editor object
        //

        if (!isTestStart)
        {
            isTestStart = true;
            editorToggleManager.isTest(isTestStart);
            foreach (var item in editUseObjects) item.SetActive(false);
            ApplyData();
            LogicManager.GetInstance.Init();
            
            if (WCGoogleSheetMapAnalyze.instance != null)
            {
                WCGoogleSheetMapAnalyze.instance.Match3TestStart();
            }
        }
        else
        {
            isTestStart = false;
            editorToggleManager.isTest(isTestStart);
            StageManager.GetInstance.Clear();
            BlockManager.GetInstance.Clear();
            TileManager.GetInstance.Clear();
            LogicManager.GetInstance.Clear();
            AnimationManager.GetInstance.Clear();

            editorUI.GameEnd();
            editorToggleManager.SelectIDClear();

            foreach (var item in editUseObjects) item.SetActive(true);
            if (EditorAutoModeControll._isAutoMode)
                if (EditorAutoModeControll._isAutoFull)
                    AutoModeTestController();
        }
    }

    public void LoadBaseMap()
    {
        var datas = editorToggleManager.GetMissions();

        foreach (var info in datas) editorToggleManager.SetMission(info.eID, info.eColor, 0);

        MapSetting(true);
    }

    private void ApplyBasicBlock()
    {
        var datas = editorToggleManager.GetStageBlock_Debug();

        stageInfo.ClearBasicBlock();
        preMapdata.basicBlocks = new int[datas.Count];

        foreach (var item in datas) stageInfo.AddBasicBlock(item.blockInfo, item._isPersent);
    }

    private void ApplyMission()
    {
        var datas = editorToggleManager.GetMissions();
        /*
        foreach (var item in data.missions)
        {
            count = item / 10000;
            mID = ((item - count * 10000) / 10);
            mColor = item - count * 10000 - mID * 10;
        }
        */

        preMapdata.missions = new int[datas.Count];
        var basicMissions = stageInfo.GetMissions();
        if (basicMissions != null) basicMissions.Clear();

        foreach (var data in datas) basicMissions.Add(new MissionKind(data.eID, data.eColor, data.MissionCount));
    }

    private void ApplyData()
    {
        ApplyBasicBlock();
        ApplyMission();
        preMapdata.stageNum = StageManager.StageNumber;
        preMapdata.moveCount = editorToggleManager.GetMoveCount();

        var sizeX = preMapdata.mapSize / 10;
        var sizeY = preMapdata.mapSize - sizeX * 10;
        preMapdata.tiles = new int[sizeX * 9 * sizeY * 9];

        if (!isMultiMap)
        {
            preMapdata.multimapOrder[0] = 0;

            for (var i = 0; i < preMapdata.tiles.Length; i++)
            {
                var kind = (int) editImageTiles[i].TileKind;
                var drop = (int) editImageTiles[i].DropDirection;

                preMapdata.tiles[i] = kind + drop * 10;
            }
        }
        else
        {
            for (var y = 0; y < 9 * sizeY; y++)
            for (var x = 0; x < 9 * sizeX; x++)
            {
                var tile = tileDictionary[new Vector2Int(x, y)];
                var kind = (int) tile.TileKind;
                var drop = (int) tile.DropDirection;

                preMapdata.tiles[x + y * 9 * sizeX] = kind + drop * 10;
            }
        }

        for (var i = 0; i < stageInfo.GetMissions().Count; i++)
        {
            var missionCount = stageInfo.GetMissions()[i].count;
            var missionID = (int) stageInfo.GetMissions()[i].ID;
            var missionColor = (int) stageInfo.GetMissions()[i].color;

            preMapdata.missions[i] = missionColor + missionID * 10 + missionCount * 10000;
        }

        var iter = 0;
        foreach (var item in stageInfo.GetBlocks())
        {
            var blockPercent = (int) item.Value;
            var blockID = (int) item.Key.ID;
            var blockColor = (int) item.Key.color;

            preMapdata.basicBlocks[iter] = blockPercent * 10000 + blockID * 10 + blockColor;
            iter++;
        }

        var objectCount = 0;
        foreach (var item in tileDictionary) objectCount += item.Value.GetObjectDictionary().Count;

        objectCount += bigObjMatrixDictionary.Count;
        objectCount += barricadesVertical.Count;
        objectCount += barricadesHorizontal.Count;

        objectCount += tunnelExits.Count * 2;
        objectCount += railOnMatrices.Count;

        preMapdata.object_XY = new int[objectCount];
        preMapdata.object_EtcID = new int[objectCount];
        preMapdata.object_ColorHp = new int[objectCount];

        var count = 0;
        foreach (var item in tileDictionary)
        foreach (var obj in item.Value.GetObjectDictionary())
        {
            preMapdata.object_XY[count] = item.Key.x * 100 + item.Key.y;
            preMapdata.object_EtcID[count] = obj.Value.ETC * 1000 + (int) obj.Value.ID;
            preMapdata.object_ColorHp[count] = (int) obj.Value.color * 100 + obj.Value.HP;

            count++;
        }

        foreach (var item in bigObjMatrixDictionary)
        {
            preMapdata.object_XY[count] = item.Key.x * 100 + item.Key.y;
            preMapdata.object_EtcID[count] = item.Value.ETC * 1000 + (int) item.Value.ID;
            preMapdata.object_ColorHp[count] = (int) item.Value.color * 100 + item.Value.HP;

            count++;
        }

        foreach (var item in barricadesHorizontal)
        {
            preMapdata.object_XY[count] = item.x * 100 + item.y;
            preMapdata.object_EtcID[count] = 0 * 1000 + 33;
            preMapdata.object_ColorHp[count] = (int) EColor.NONE * 100 + 1;

            count++;
        }

        foreach (var item in barricadesVertical)
        {
            preMapdata.object_XY[count] = item.x * 100 + item.y;
            preMapdata.object_EtcID[count] = 1 * 1000 + 33;
            preMapdata.object_ColorHp[count] = (int) EColor.NONE * 100 + 1;

            count++;
        }

        foreach (var item in tunnelExits)
        {
            var opTunnel = item.OppenentTunnel;

            preMapdata.object_XY[count] = item.Matrix.x * 100 + item.Matrix.y;
            preMapdata.object_EtcID[count] = (opTunnel.Matrix.x * 100 + opTunnel.Matrix.y) * 1000 + 35;
            preMapdata.object_ColorHp[count] = (int) EColor.NONE * 100 + 1;

            count++;

            preMapdata.object_XY[count] = opTunnel.Matrix.x * 100 + opTunnel.Matrix.y;
            preMapdata.object_EtcID[count] = (item.Matrix.x * 100 + item.Matrix.y) * 1000 + 34;
            preMapdata.object_ColorHp[count] = (int) EColor.RED * 100 + 1;

            count++;
        }

        foreach (var item in railOnMatrices)
        {
            var isRemoveValue = 0;
            if (item.property.isRemove) isRemoveValue = 1;

            preMapdata.object_XY[count] = item.matrix.x * 100 + item.matrix.y;
            preMapdata.object_EtcID[count] = (item.property.order * 10 + isRemoveValue) * 1000 + 37;
            preMapdata.object_ColorHp[count] = item.property.power * 100 + item.property.railKind;

            count++;
        }


        DataContainer.GetInstance.LoadTestMapData(preMapdata);
    }

    public EditFieldObject GetEditFieldObjectOrNull(BlockInfo info)
    {
        var obj = Instantiate(editFieldObj);
        if (obj != null)
        {
            var script = obj.GetComponent<EditFieldObject>();
            if (script != null)
            {
                //TODO
                var sprite = spriteContainer.GetSpriteOrNull(info.ID, info.color, info.HP);
                script.SetSprite(sprite);
                
                if (info.ID == EID.TARGET_RELIC) script.SetSize(new Vector2(50, 44));
                if (info.ID == EID.INVISIBLE_BOX)
                    obj.GetComponent<SpriteRenderer>().color = Color.yellow;
                else
                    obj.GetComponent<SpriteRenderer>().color = Color.white;
                return script;
            }
        }

        return null;
    }

    public void SaveData()
    {
        ApplyData();
        if (stageInfo.GetMissions().Count == 0)
            Debug.LogError("Mission Count Zero");
        //return;
        DataContainer.GetInstance.SaveMapData(preMapdata);
    }

    public void NextStage()
    {
        StageManager.StageNumber++;
        SceneManager.LoadScene("PC_EditorScene");
    }

    public void PreStage()
    {
        StageManager.StageNumber--;
        SceneManager.LoadScene("PC_EditorScene");
    }

    public void ReturnStage()
    {
        SceneManager.LoadScene("PC_EditorScene");
    }

    public void GoMapSelectStage()
    {
        SceneManager.LoadScene("EditorStageSelectScene");
    }

    public void SetObjectMode(EID _id, EColor _color, int _hp, int _etc)
    {
        preRailData = null;

        editorUI.ClearGreenRedTile();

        Mode = EEditorMode.OBJECT;
        PreID = _id;
        PreColor = _color;
        hp = _hp;
        etcValue = _etc;

        BlockINFO.color = PreColor;
        BlockINFO.ID = PreID;
        BlockINFO.HP = hp;
        BlockINFO.ETC = etcValue;

        if (bigSizeDictionary.ContainsKey(_id) || _id == EID.TABLET)
            IsBigMode = true;
        else
            IsBigMode = false;

        var data = DataContainer.GetInstance.GetObstacleDataOrNULL(_id);

        if (data != null) Depth = data.depth;
        else Depth = EDepth.NORMAL;
    }

    public void SetGravityMode(EDirection direction)
    {
        IsBigMode = false;
        Mode = EEditorMode.GRAVITY;
        DropDirection = direction;
        SetUI();
    }

    public void ChangeTileKind(ETileKind kind)
    {
        IsBigMode = false;
        Mode = EEditorMode.TILE;
        TileKind = kind;
        SetUI();
    }

    public void SetTileInfo(Vector2Int matrix, EditTile tile)
    {
        if (tileDictionary.ContainsKey(matrix)) tileDictionary[matrix].Clone(tile);
    }

    public Sprite GetSpriteOrNull(EEditImage image)
    {
        if (editSpriteContainer != null) return editSpriteContainer.GetSpriteOrNull(image);
        return null;
    }

    public void TileMouseEnter(EditImageTile tile)
    {
        if (!bigSizeDictionary.ContainsKey(PreID) && PreID != EID.TABLET) return;
        if (!IsBigMode) return;

        var size = new Vector2Int();
        var tiles = new List<EditImageTile>();
        var matrix = new Vector2Int();

        if (PreID == EID.TABLET)
        {
            size = tabletContainer.GetSize(hp);
            if ((EDirection) etcValue == EDirection.LEFT || (EDirection) etcValue == EDirection.RIGHT)
            {
                var temp = size.x;
                size.x = size.y;
                size.y = temp;
            }
        }
        else
        {
            size = bigSizeDictionary[PreID];
        }

        for (var x = 0; x < size.x; x++)
        for (var y = 0; y < size.y; y++)
        {
            matrix.x = tile.Matrix.x + x;
            matrix.y = tile.Matrix.y + y;

            if (!tileImageDictionary.ContainsKey(matrix)) return;
            tiles.Add(tileImageDictionary[matrix]);
        }

        if (CheckBigAble(tile, PreID))
            editorUI.ShowGreenRedTile(tiles, true);
        else
            editorUI.ShowGreenRedTile(tiles, false);
    }

    public void TileMouseDown(EditImageTile tile)
    {
        if (!bigSizeDictionary.ContainsKey(PreID) && PreID != EID.TABLET) return;
        if (!IsBigMode) return;

        if (!CheckBigAble(tile, PreID))
        {
            editorUI.ClearGreenRedTile();

            var isDelete = false;

            var size = Vector2Int.zero;
            var size2 = Vector2Int.zero;
            Vector2Int? bigObjKey = null;

            if (PreID == EID.TABLET)
            {
                size = tabletContainer.GetSize(hp);
                if ((EDirection) etcValue == EDirection.LEFT || (EDirection) etcValue == EDirection.RIGHT)
                {
                    var temp = size.x;
                    size.x = size.y;
                    size.y = temp;
                }
            }
            else
            {
                size = bigSizeDictionary[PreID];
            }

            foreach (var item in bigObjMatrixDictionary)
            {
                if (item.Value.ID != EID.TABLET)
                {
                    size2 = bigSizeDictionary[item.Value.ID];
                }
                else
                {
                    size2 = tabletContainer.GetSize(item.Value.HP);
                    if ((EDirection) item.Value.ETC == EDirection.LEFT ||
                        (EDirection) item.Value.ETC == EDirection.RIGHT)
                    {
                        var temp = size2.x;
                        size2.x = size2.y;
                        size2.y = temp;
                    }
                }

                bool isX = false, isY = false;
                for (var x = tile.Matrix.x; x < tile.Matrix.x + size.x; x++)
                    if (x >= item.Key.x && x < item.Key.x + size2.x)
                        isX = true;
                for (var y = tile.Matrix.y; y < tile.Matrix.y + size.y; y++)
                    if (y >= item.Key.y && y < item.Key.y + size2.y)
                        isY = true;

                if (isX && isY)
                {
                    isDelete = true;
                    bigObjKey = item.Key;
                }
            }

            if (isDelete)
            {
                Destroy(
                    bigObjOnTile[tileImageDictionary[(Vector2Int) bigObjKey]]);

                bigObjOnTile.Remove(tileImageDictionary[(Vector2Int) bigObjKey]);
                bigObjMatrixDictionary.Remove((Vector2Int) bigObjKey);
            }
        }
        else
        {
            var bi = new BlockInfo(PreID, PreColor, hp, etcValue);

            CreateBigObjInData(tile.Matrix, bi);
            CreateBigObjOnPreTile(tile, bi);
        }
    }

    public void CreateBigObjInData(Vector2Int matrix, BlockInfo info)
    {
        bigObjMatrixDictionary.Add(matrix, info);
    }

    public void CreateBigObjOnPreTile(EditImageTile tile, BlockInfo info)
    {
        GameObject bigObj = null;
        if (info.ID == EID.BIG_SIDE) bigObj = Instantiate(bigSideObject);
        if (info.ID == EID.BIG_SIDE_COLOR)
        {
            bigObj = Instantiate(bigSideColorObject);
            var r = bigObj.GetComponent<SpriteRenderer>();

            switch (info.color)
            {
                case EColor.RED:
                    r.color = Color.red;
                    break;
                case EColor.YELLOW:
                    r.color = Color.yellow;
                    break;
                case EColor.GREEN:
                    r.color = Color.green;
                    break;
                case EColor.BLUE:
                    r.color = Color.blue;
                    break;
                case EColor.PURPLE:
                    r.color = new Color(223, 0, 255, 255);
                    break;
                case EColor.NONE:
                    break;
            }
        }

        if (info.ID == EID.BIG_SIDE_DIRECTION) bigObj = Instantiate(bigSideDirectionObject);
        if (info.ID == EID.GEAR) bigObj = Instantiate(gearObject, tile.transform);
        if (info.ID == EID.TABLET)
            switch (info.HP)
            {
                case 0:
                    bigObj = Instantiate(tablet11Object, tile.transform);
                    break;
                case 1:
                    bigObj = Instantiate(tablet12Object, tile.transform);
                    break;
                case 2:
                    bigObj = Instantiate(tablet22Object, tile.transform);
                    break;
                case 3:
                    bigObj = Instantiate(tablet23Object, tile.transform);
                    break;
                case 4:
                    bigObj = Instantiate(tablet33Object, tile.transform);
                    break;
            }

        if (bigObj == null) return;
        bigObj.transform.SetParent(tile.transform);
        bigObj.transform.localScale = new Vector3(100, 100, 0);
        bigObj.transform.localPosition = new Vector3(40, -40);

        if (info.ID == EID.GEAR) bigObj.transform.localPosition = new Vector3(80, -80);
        if (info.ID == EID.TABLET)
        {
            var size = tabletContainer.GetSize(info.HP);
            if ((EDirection) info.ETC == EDirection.LEFT || (EDirection) info.ETC == EDirection.RIGHT)
            {
                var temp = size.x;
                size.x = size.y;
                size.y = temp;
            }

            bigObj.transform.localPosition = new Vector3(-40, 40) + new Vector3(40 * size.x, -40 * size.y);
            bigObj.transform.SetParent(tile.transform);
            switch ((EDirection) info.ETC)
            {
                case EDirection.DOWN:
                    bigObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    break;
                case EDirection.UP:
                    bigObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                    break;
                case EDirection.LEFT:
                    bigObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                    break;
                case EDirection.RIGHT:
                    bigObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
                    break;
            }
        }

        bigObjOnTile.Add(tile, bigObj);
    }

    public bool CheckBigAble(EditImageTile tile, EID id)
    {
        if (!bigSizeDictionary.ContainsKey(id) && id != EID.TABLET) return false;

        var size = new Vector2Int();
        var bigDepth = bigObjDepth[id];

        var matrix = new Vector2Int();
        var tiles = new List<EditImageTile>();

        if (PreID == EID.TABLET)
        {
            size = tabletContainer.GetSize(hp);
            if ((EDirection) etcValue == EDirection.LEFT || (EDirection) etcValue == EDirection.RIGHT)
            {
                var temp = size.x;
                size.x = size.y;
                size.y = temp;
            }
        }
        else
        {
            size = bigSizeDictionary[id];
        }

        for (var x = 0; x < size.x; x++)
        for (var y = 0; y < size.y; y++)
        {
            matrix.x = tile.Matrix.x + x;
            matrix.y = tile.Matrix.y + y;

            if (matrix.x > 9 * (preMap.x + 1) || matrix.y > 9 * (preMap.y + 1)) return false;
            if (!tileDictionary.ContainsKey(matrix)) return false;
            if (!tileImageDictionary.ContainsKey(matrix)) return false;
            if (tileDictionary[matrix].IsExistBlock(bigDepth)) return false;
            //if (tileDictionary[matrix].IsExistBlock(EDepth.TOP)) return false;

            tiles.Add(tileImageDictionary[matrix]);
        }

        if (IsBigObject(tiles, bigDepth)) return false;

        return true;
    }

    public bool IsBigObject(List<EditImageTile> tiles, EDepth depth)
    {
        foreach (var item in bigObjMatrixDictionary)
        {
            if (!bigSizeDictionary.ContainsKey(item.Value.ID) && item.Value.ID != EID.TABLET) continue;
            var size = Vector2Int.zero;
            if (item.Value.ID == EID.TABLET)
            {
                size = tabletContainer.GetSize(item.Value.HP);
                if ((EDirection) item.Value.ETC == EDirection.LEFT || (EDirection) item.Value.ETC == EDirection.RIGHT)
                {
                    var temp = size.x;
                    size.x = size.y;
                    size.y = temp;
                }
            }
            else
            {
                size = bigSizeDictionary[item.Value.ID];
            }

            var matrix = new Vector2Int();

            for (var x = 0; x < size.x; x++)
            for (var y = 0; y < size.y; y++)
            {
                matrix.x = x;
                matrix.y = y;
                foreach (var tile in tiles)
                    if (tile.Matrix == item.Key + matrix
                        && depth == bigObjDepth[item.Value.ID])
                        return true;
            }
        }

        return false;
    }

    public bool IsBigObject(Vector2 _matrix, EDepth depth)
    {
        foreach (var item in bigObjMatrixDictionary)
        {
            var data = DataContainer.GetInstance.GetObstacleDataOrNULL(item.Value.ID);

            var size = Vector2Int.zero;
            if (item.Value.ID == EID.TABLET)
            {
                size = tabletContainer.GetSize(item.Value.HP);
                if ((EDirection) item.Value.ETC == EDirection.LEFT || (EDirection) item.Value.ETC == EDirection.RIGHT)
                {
                    var temp = size.x;
                    size.x = size.y;
                    size.y = temp;
                }
            }
            else
            {
                size = bigSizeDictionary[item.Value.ID];
            }

            var matrix = new Vector2Int();

            for (var x = 0; x < size.x; x++)
            for (var y = 0; y < size.y; y++)
            {
                matrix.x = x;
                matrix.y = y;
                if (item.Key + matrix == _matrix)
                    if (depth == data.depth)
                        return true;
            }
        }

        return false;
    }

    public bool IsSpecialBlock(EID id)
    {
        if (specialBlocks.Contains(id)) return true;
        return false;
    }

    public void ModifySpecialMission()
    {
        foreach (var specialID in specialBlocks)
        {
            var result = 0;
            if (specialID == EID.GOLD)
            {
                result = GetCountMissionObj(specialID);
                editorToggleManager.SetMission(specialID, EColor.NONE, result);
            }

            if (specialID == EID.RELIC_IN_INVISIBLE_BOX)
            {
                result = GetCountMissionObj(specialID);
                editorToggleManager.SetMission(specialID, EColor.NONE, result);
            }

            if (specialID == EID.JAM)
            {
                if (GetCountMissionObj(specialID) == 0)
                {
                    editorToggleManager.SetMission(specialID, EColor.NONE, result);
                    continue;
                }

                result += GetTileKindCount(ETileKind.NORMAL);
                result += GetTileKindCount(ETileKind.START);
                result += GetTileKindCount(ETileKind.END);

                result -= GetCountMissionObj(specialID);
                editorToggleManager.SetMission(specialID, EColor.NONE, result);
            }

            if (specialID == EID.TABLET)
            {
                foreach (var item in bigObjMatrixDictionary)
                    if (item.Value.ID == EID.TABLET)
                        result++;
                editorToggleManager.SetMission(specialID, EColor.NONE, result);
            }

            if (specialID == EID.CLAM)
            {
                result = GetCountMissionObj(specialID);
                editorToggleManager.SetMission(specialID, EColor.NONE, result);
            }

            if (specialID == EID.GLASS_COLOR)
            {
                result = GetCountMissionObj(specialID);
                editorToggleManager.SetMission(specialID, EColor.NONE, result);
            }

            if (specialID == EID.ACTINIARIA)
            {
                result = GetCountMissionObj(specialID);
                if (result == 0) editorToggleManager.SetMission(specialID, EColor.NONE, result);
            }

            if (specialID == EID.BIG_SIDE)
            {
                result = GetCountMissionObj(specialID);
                editorToggleManager.SetMission(specialID, EColor.NONE, result);
            }

            if (specialID == EID.BIG_SIDE_COLOR)
            {
                result = GetCountMissionObj(specialID) * 5;
                editorToggleManager.SetMission(specialID, EColor.NONE, result);
            }

            if (specialID == EID.BIG_SIDE_DIRECTION)
            {
                //result = GetCountMissionObj(specialID);
                //editorToggleManager.SetMission(specialID, EColor.NONE, result);
            }

            if (specialID == EID.TURN_BOX)
            {
                result = GetCountMissionObj(specialID);
                editorToggleManager.SetMission(specialID, EColor.NONE, result);
            }

            if (specialID == EID.DOUBLE)
            {
                result = GetCountMissionObj(specialID);
                editorToggleManager.SetMission(specialID, EColor.NONE, result);
            }
        }
    }

    private int GetTileKindCount(ETileKind kind)
    {
        var result = 0;
        var matrix = new Vector2Int();

        for (var y = 0; y < mapSize.y * 9; y++)
        for (var x = 0; x < mapSize.x * 9; x++)
        {
            matrix.x = x;
            matrix.y = y;

            if (tileDictionary[matrix].TileKind == kind) result++;
        }

        return result;
    }

    private int GetCountMissionObj(EID id)
    {
        var count = 0;

        foreach (var item in tileDictionary)
        {
            var dic = item.Value.GetObjectDictionary();

            if (dic == null) continue;
            if (dic.Count == 0) continue;

            foreach (var obj in dic)
                if (obj.Value.ID == id)
                    count++;
        }

        foreach (var item in bigObjMatrixDictionary)
            if (item.Value.ID == id)
                count++;

        return count;
    }

    public void AddBarricadeData(Vector2Int matrix, int etcValue)
    {
        if (etcValue == 0)
            barricadesHorizontal.Add(matrix);
        else
            barricadesVertical.Add(matrix);
    }

    public void AddBarricadeInPreMap(EditImageTile tile, EDirection direction)
    {
        var matrix = tile.Matrix;
        var rotation = new Vector3();
        var position = new Vector3();

        var d = direction;
        if (d == EDirection.NONE) d = barricadeDirection;

        switch (d)
        {
            case EDirection.UP:
                position.y = 45;
                rotation.z = 90;
                break;
            case EDirection.DOWN:
                position.y = -45;
                rotation.z = 90;
                matrix.y++;
                break;
            case EDirection.LEFT:
                position.x = -40;
                break;
            case EDirection.RIGHT:
                position.x = 40;
                matrix.x++;
                break;
        }

        var bObj = Instantiate(barricadeObject, tile.gameObject.transform);
        bObj.transform.localPosition = position;
        bObj.transform.rotation = Quaternion.Euler(rotation);

        if (d == EDirection.UP || d == EDirection.DOWN) barricadesHorizontalObjs.Add(matrix, bObj);
        if (d == EDirection.LEFT || d == EDirection.RIGHT) barricadesVerticalObjs.Add(matrix, bObj);
    }

    public void BarricadeActiveImage(EditImageTile tile, EDirection direction = EDirection.NONE)
    {
        var matrix = tile.Matrix;
        var rotation = new Vector3();
        var position = new Vector3();

        var d = direction;
        if (d == EDirection.NONE) d = (EDirection) editorToggleManager.SetColor;

        //TODO
        //if (d == EDirection.LEFT) d = EDirection.RIGHT;
        //else if (d == EDirection.RIGHT) d = EDirection.LEFT;

        switch (d)
        {
            case EDirection.UP:
                position.y = 40;
                rotation.z = 90;
                break;
            case EDirection.DOWN:
                position.y = -40;
                rotation.z = 90;
                matrix.y++;
                break;
            case EDirection.LEFT:
                position.x = -40;
                break;
            case EDirection.RIGHT:
                position.x = 40;
                matrix.x++;
                break;
        }

        if (CheckBarricadeAble(tile))
        {
            var bObj = Instantiate(barricadeObject, tile.gameObject.transform);
            bObj.transform.localPosition = position;
            bObj.transform.rotation = Quaternion.Euler(rotation);

            if (d == EDirection.UP || d == EDirection.DOWN)
            {
                barricadesHorizontal.Add(matrix);
                barricadesHorizontalObjs.Add(matrix, bObj);
            }

            if (d == EDirection.LEFT || d == EDirection.RIGHT)
            {
                barricadesVertical.Add(matrix);
                barricadesVerticalObjs.Add(matrix, bObj);
            }
        }
        else
        {
            if (d == EDirection.UP || d == EDirection.DOWN)
            {
                var obj = barricadesHorizontalObjs[matrix];
                Destroy(obj);
                barricadesHorizontalObjs.Remove(matrix);
                barricadesHorizontal.Remove(matrix);
            }

            if (d == EDirection.LEFT || d == EDirection.RIGHT)
            {
                var obj = barricadesVerticalObjs[matrix];
                Destroy(obj);
                barricadesVerticalObjs.Remove(matrix);
                barricadesVertical.Remove(matrix);
            }
        }
    }

    public bool CheckBarricadeAble(EditImageTile tile)
    {
        var matrix = tile.Matrix;
        var d = EDirection.NONE;

        if (d == EDirection.NONE) d = (EDirection) editorToggleManager.SetColor;
        //TODO
        //if (d == EDirection.LEFT) d = EDirection.RIGHT;
        //else if (d == EDirection.RIGHT) d = EDirection.LEFT;
        switch (d)
        {
            case EDirection.UP:
                break;
            case EDirection.DOWN:
                matrix.y++;
                break;
            case EDirection.LEFT:
                break;
            case EDirection.RIGHT:
                matrix.x++;
                break;
        }

        if (d == EDirection.UP || d == EDirection.DOWN)
        {
            if (barricadesHorizontal.Contains(matrix))
                return false;
            return true;
        }

        if (barricadesVertical.Contains(matrix))
            return false;
        return true;
    }

    public void AddTunnelOnImageTile(EditImageTile tile, bool isEntrance)
    {
        GameObject obj = null;
        if (isEntrance)
        {
            obj = DynamicObjectPool.GetInstance.GetObjectForType("EditorTunnelEntrance", false);

            obj.transform.SetParent(tile.gameObject.transform);
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.transform.localPosition = new Vector3(0, -15, 0);

            tunnelEntranceObjs.Add(tile.Matrix, obj);
            SetTunnelCountText();
        }
        else
        {
            obj = DynamicObjectPool.GetInstance.GetObjectForType("EditorTunnelExit", false);

            obj.transform.SetParent(tile.gameObject.transform);
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.transform.localPosition = new Vector3(0, 15, 0);

            tunnelExitObjs.Add(tile.Matrix, obj);
            SetTunnelCountText();
        }
    }

    public void SetTunnelCountText()
    {
        foreach (var tunnel in tunnelEntranceObjs)
        {
            var num = 0;
            foreach (var item in tunnelEntrances)
            {
                if (item.Matrix.Equals(tunnel.Key))
                {
                    var count = tunnel.Value.GetComponent<EditorTunnelCount>();
                    count.SetCount(num);
                }

                num++;
            }
        }

        foreach (var tunnel in tunnelExitObjs)
        {
            var num = 0;
            foreach (var item in tunnelExits)
            {
                if (item.Matrix.Equals(tunnel.Key))
                {
                    var count = tunnel.Value.GetComponent<EditorTunnelCount>();
                    count.SetCount(num);
                }

                num++;
            }
        }
    }

    public bool AddTunnelInfo(Vector2Int matrix, bool isEntrance)
    {
        if (isEntrance)
        {
            var tunnel = new Tunnel();
            tunnel.Matrix = matrix;
            tunnel.OppenentTunnel = null;

            tunnelEntrances.Add(tunnel);
        }
        else
        {
            if (tunnelEntrances.Count <= tunnelExits.Count) return false;
            var tunnelExit = new Tunnel();
            tunnelExit.Matrix = matrix;

            foreach (var tunnel in tunnelEntrances)
                if (tunnel.OppenentTunnel == null)
                {
                    tunnel.OppenentTunnel = tunnelExit;
                    tunnelExit.OppenentTunnel = tunnel;
                    break;
                }

            tunnelExits.Add(tunnelExit);
            SetTunnelCountText();
        }

        return true;
    }

    public void RemoveTunnelInfo(Vector2Int matrix, bool isEntrance)
    {
        Tunnel myTunnel = null;
        Tunnel oppenentTunnel = null;

        if (isEntrance)
        {
            foreach (var tunnelEntrance in tunnelEntrances)
                if (tunnelEntrance.Matrix == matrix)
                {
                    myTunnel = tunnelEntrance;
                    break;
                }

            tunnelEntrances.Remove(myTunnel);
            SetTunnelCountText();

            if (myTunnel.OppenentTunnel != null)
            {
                foreach (var tunnelExit in tunnelExits)
                    if (tunnelExit.Matrix == myTunnel.OppenentTunnel.Matrix)
                        oppenentTunnel = tunnelExit;

                tunnelExits.Remove(oppenentTunnel);
                SetTunnelCountText();
            }
        }
        else
        {
            foreach (var tunnelExit in tunnelExits)
                if (tunnelExit.Matrix == matrix)
                {
                    myTunnel = tunnelExit;
                    break;
                }

            tunnelExits.Remove(myTunnel);
            SetTunnelCountText();

            if (myTunnel.OppenentTunnel != null)
            {
                foreach (var tunnelEntrance in tunnelEntrances)
                    if (tunnelEntrance.Matrix == myTunnel.OppenentTunnel.Matrix)
                        oppenentTunnel = tunnelEntrance;

                tunnelEntrances.Remove(oppenentTunnel);
                SetTunnelCountText();
            }
        }
    }

    public void RemoveTunnelOnImageTile(Vector2Int matrix, bool isEntrance)
    {
        // first remove image next remove info

        Tunnel oppenentTunnel = null;
        if (isEntrance)
        {
            foreach (var item in tunnelEntranceObjs)
                if (item.Key == matrix)
                    DynamicObjectPool.GetInstance.PoolObject(item.Value, false);

            foreach (var item in tunnelEntrances)
                if (item.Matrix == matrix)
                    if (item.OppenentTunnel != null)
                        oppenentTunnel = item.OppenentTunnel;

            if (oppenentTunnel != null)
            {
                foreach (var item in tunnelExitObjs)
                    if (item.Key == oppenentTunnel.Matrix)
                        DynamicObjectPool.GetInstance.PoolObject(item.Value, false);

                tunnelExitObjs.Remove(oppenentTunnel.Matrix);
                SetTunnelCountText();
            }

            tunnelEntranceObjs.Remove(matrix);
            SetTunnelCountText();
        }
        else
        {
            foreach (var item in tunnelExitObjs)
                if (item.Key == matrix)
                    DynamicObjectPool.GetInstance.PoolObject(item.Value, false);

            foreach (var item in tunnelExits)
                if (item.Matrix == matrix)
                    if (item.OppenentTunnel != null)
                        oppenentTunnel = item.OppenentTunnel;

            if (oppenentTunnel != null)
            {
                foreach (var item in tunnelEntranceObjs)
                    if (item.Key == oppenentTunnel.Matrix)
                        DynamicObjectPool.GetInstance.PoolObject(item.Value, false);

                tunnelEntranceObjs.Remove(oppenentTunnel.Matrix);
                SetTunnelCountText();
            }

            tunnelExitObjs.Remove(matrix);
            SetTunnelCountText();
        }
    }

    public void TouchTunnel(EditImageTile tile, bool isEntrance)
    {
        var isTunnelExist = CheckTunnelExist(tile.Matrix, isEntrance);
        if (isTunnelExist)
        {
            //Remove Tunnel
            RemoveTunnelOnImageTile(tile.Matrix, isEntrance);
            RemoveTunnelInfo(tile.Matrix, isEntrance);
        }
        else
        {
            //Add Tunnel
            if (AddTunnelInfo(tile.Matrix, isEntrance)) AddTunnelOnImageTile(tile, isEntrance);
        }
    }

    public bool CheckTunnelExist(Vector2Int matrix, bool isEntrance)
    {
        if (isEntrance)
        {
            foreach (var tunnel in tunnelEntrances)
                if (tunnel.Matrix == matrix)
                    return true;
        }
        else
        {
            foreach (var tunnel in tunnelExits)
                if (tunnel.Matrix == matrix)
                    return true;
        }

        return false;
    }

    public bool CheckRailExist(Vector2Int matrix, int order = -1)
    {
        foreach (var rail in railOnMatrices)
            if (rail.matrix == matrix && (rail.property.order == order || order == -1))
                return true;
        return false;
    }

    public void AddRailInfo(RailOnMatrix railOnMatrix)
    {
        if (railOnMatrices.Contains(railOnMatrix)) return;

        railOnMatrices.Add(railOnMatrix);
    }

    public void AddRailOnTile(EditImageTile editImage, RailOnMatrix railOnMatrix)
    {
        var railObj = Instantiate(editRailObj, editImage.gameObject.transform);
        railOnMatrixObjs.Add(railOnMatrix, railObj);
        var rail = railObj.GetComponent<EditorRail>();
        rail.SetMainSprite(railSprites[railOnMatrix.property.railKind - 1]);
        railObj.transform.localPosition = Vector3.zero;
        rail.SetOrderSprite(railOnMatrix.property.order);
        var preDirection = EDirection.NONE;
        var nextDirection = EDirection.NONE;

        switch (railOnMatrix.property.railKind)
        {
            case 1:
                preDirection = EDirection.RIGHT;
                nextDirection = EDirection.LEFT;
                break;
            case 2:
                preDirection = EDirection.LEFT;
                nextDirection = EDirection.RIGHT;
                break;
            case 3:
                preDirection = EDirection.DOWN;
                nextDirection = EDirection.UP;
                break;
            case 4:
                preDirection = EDirection.UP;
                nextDirection = EDirection.DOWN;
                break;
            case 5:
                preDirection = EDirection.RIGHT;
                nextDirection = EDirection.DOWN;
                break;
            case 6:
                preDirection = EDirection.DOWN;
                nextDirection = EDirection.LEFT;
                break;
            case 7:
                preDirection = EDirection.DOWN;
                nextDirection = EDirection.RIGHT;
                break;
            case 8:
                preDirection = EDirection.LEFT;
                nextDirection = EDirection.DOWN;
                break;
            case 9:
                preDirection = EDirection.UP;
                nextDirection = EDirection.RIGHT;
                break;
            case 10:
                preDirection = EDirection.LEFT;
                nextDirection = EDirection.UP;
                break;
            case 11:
                preDirection = EDirection.RIGHT;
                nextDirection = EDirection.UP;
                break;
            case 12:
                preDirection = EDirection.UP;
                nextDirection = EDirection.LEFT;
                break;
        }

        rail.Setting(nextDirection, preDirection);
    }

    public void RemoveRailInfo(RailOnMatrix railOnMatrix)
    {
        foreach (var item in railOnMatrices)
            if (item.matrix == railOnMatrix.matrix
                && item.property.order == railOnMatrix.property.order)
            {
                railOnMatrices.Remove(item);
                break;
            }
    }

    public void RemoveRailObj(RailOnMatrix railOnMatrix)
    {
        foreach (var item in railOnMatrixObjs)
            if (item.Key.matrix == railOnMatrix.matrix
                && item.Key.property.order == railOnMatrix.property.order)
            {
                var obj = item.Value;
                Destroy(obj);
                railOnMatrixObjs.Remove(item.Key);
                break;
            }
    }

    public void RemoveRailOnTile(EditImageTile tile)
    {
        var removeList = new List<RailOnMatrix>();
        foreach (var item in railOnMatrices)
            if (item.matrix == tile.Matrix)
                removeList.Add(item);
        foreach (var item in removeList) railOnMatrices.Remove(item);

        removeList.Clear();

        foreach (var item in railOnMatrixObjs)
            if (item.Key.matrix == tile.Matrix)
                removeList.Add(item.Key);
        foreach (var item in removeList)
        {
            Destroy(railOnMatrixObjs[item]);
            railOnMatrixObjs.Remove(item);
        }
    }

    public void TouchRail(EditImageTile tile)
    {
        if (preRailData.Value.order == -1) return;
        if (preRailData.Value.railKind == 0)
        {
            RemoveRailOnTile(tile);
            return;
        }

        if (!tile.IsBlockInputAble(EDepth.FLOOR)) return;

        var railOnMatrix = new RailOnMatrix();
        railOnMatrix.matrix = tile.Matrix;
        railOnMatrix.property.railKind = preRailData.Value.railKind;
        railOnMatrix.property.order = preRailData.Value.order;
        railOnMatrix.property.power = preRailData.Value.power;
        railOnMatrix.property.isRemove = preRailData.Value.isRemove;

        if (CheckRailExist(tile.Matrix, preRailData.Value.order))
        {
            RemoveRailInfo(railOnMatrix);
            RemoveRailObj(railOnMatrix);
        }
        else
        {
            AddRailInfo(railOnMatrix);
            AddRailOnTile(tile, railOnMatrix);
        }
    }

    public void SetMapSize()
    {
        var list = editorToggleManager.GetMultiMap();
        preMapdata.multimapOrder = new int[list.Count];

        int maxX = 0, maxY = 0;

        for (var i = 0; i < list.Count; i++)
        {
            preMapdata.multimapOrder[i] = list[i];

            var tempX = list[i] / 10;
            var tempY = list[i] - tempX * 10;

            maxX = Mathf.Max(maxX, tempX);
            maxY = Mathf.Max(maxY, tempY);
        }

        preMapdata.mapSize = (maxX + 1) * 10 + maxY + 1;

        mapSize.x = maxX + 1;
        mapSize.y = maxY + 1;

        if (preMapdata.mapSize != 11) isMultiMap = true;
        else isMultiMap = false;
    }

    public void ClearMultiMap(Vector2Int matrix)
    {
        var settingMatrix = Vector2Int.zero;
        for (var y = 9 * matrix.y; y < 9 * matrix.y + 9; y++)
        for (var x = 9 * matrix.x; x < 9 * matrix.x + 9; x++)
        {
            settingMatrix.x = x;
            settingMatrix.y = y;
            tileDictionary[settingMatrix].Clear();
            tileDictionary[settingMatrix].Setting(settingMatrix, ETileKind.VOID, EDirection.DOWN);
        }
    }

    public void ChangePreMap(int num)
    {
        ApplyData();
        preMap.x = num / 10;
        preMap.y = num - preMap.x * 10;
        Clear();
        SetPreMap();
    }

    public void ChangePreX(Text text)
    {
        var t = text.text;
        preMap.x = int.Parse(t);
        ApplyData();
        Clear();
        SetPreMap();
    }

    public void ChangePreY(Text text)
    {
        var t = text.text;
        preMap.y = int.Parse(t);
        ApplyData();
        Clear();
        SetPreMap();
    }

    public void ShowVictoryPopup()
    {
        if (WCGoogleSheetMapAnalyze.instance != null)
        {
            WCGoogleSheetMapAnalyze.instance.Match3GameResult(true);
        }
        
        if (editorUI != null) editorUI.ShowVictoryPopup();
    }

    public void ShowFailPopup()
    {
        if (WCGoogleSheetMapAnalyze.instance != null)
        {
            WCGoogleSheetMapAnalyze.instance.Match3GameResult(false);
        }
        
        if (editorUI != null) editorUI.ShowFailPopUp();
    }

    public void Clear()
    {
        foreach (var t in tileImageDictionary) t.Value.Clear();
        tileImageDictionary.Clear();

        foreach (var item in bigObjOnTile) Destroy(item.Value);
        bigObjOnTile.Clear();

        foreach (var item in railOnMatrixObjs)
            Destroy(
                item.Value);
        railOnMatrixObjs.Clear();

        foreach (var item in tunnelExitObjs)
            Destroy(
                item.Value);
        tunnelExitObjs.Clear();

        foreach (var item in tunnelEntranceObjs)
            Destroy(
                item.Value);
        tunnelEntranceObjs.Clear();

        foreach (var item in barricadesVerticalObjs)
            Destroy(
                item.Value);
        barricadesVerticalObjs.Clear();

        foreach (var item in barricadesHorizontalObjs)
            Destroy(
                item.Value);
        barricadesHorizontalObjs.Clear();
    }

    public void HardClear()
    {
        Clear();
        foreach (var item in tileDictionary) item.Value.Clear();
        bigObjMatrixDictionary.Clear();
        barricadesVertical.Clear();
        barricadesHorizontal.Clear();
        tunnelEntrances.Clear();
        tunnelExits.Clear();
        railOnMatrices.Clear();
        foreach (var item in railOnMatrixObjs)
            Destroy(
                item.Value);
        railOnMatrixObjs.Clear();
    }

    public void AutoModeTestController()
    {
        StartCoroutine(AutoModeTestChange());
    }

    private IEnumerator AutoModeTestChange()
    {
        yield return new WaitForSeconds(1);
        foreach (var item in EditorAutoModeControll._isChallengeInfo)
            if (item._isStage == StageManager.StageNumber)
                if (item._isChallenge_Count == EditorAutoModeControll.TestCount)
                {
                    StageManager.StageNumber++;
                    SceneManager.LoadScene("PC_EditorScene");
                }

        yield return new WaitForSeconds(1);
        ClickGameTest();
    }

    public void SetRailProperty(RailProperty rail)
    {
        Debug.Log("Order: " + rail.order);
        preRailData = rail;
    }

    public void GravityRenderSetting(bool value)
    {
        foreach (var item in tileImageDictionary) item.Value.SetGravityTopMode(value);
    }

    public void ChangeRailOrder(int order)
    {
        foreach (var item in railOnMatrixObjs)
        {
            var railObj = item.Value;
            var rail = railObj.GetComponent<EditorRail>();
            if (item.Key.property.order == order)
                rail.alpha = 1.0f;
            else
                rail.alpha = 0.5f;
            rail.SetOrderSprite(item.Key.property.order);
        }
    }

    public void SetTabletTop(bool value)
    {
        foreach (var item in bigObjMatrixDictionary)
            if (item.Value.ID == EID.TABLET)
                foreach (var obj in bigObjOnTile)
                    if (obj.Key.Matrix == item.Key)
                    {
                        var tablet = obj.Value.GetComponent<SpriteRenderer>();
                        if (value)
                            tablet.sortingOrder = 100;
                        else
                            tablet.sortingOrder = (int) EDepth.TABLET;
                    }
    }

    public void SetFloorTop(bool value)
    {
    }

    public void SetSavePopup(bool value, string text)
    {
        editorUI.SetSaveText(text);
        editorUI.SetSavePopup(value);
    }
}