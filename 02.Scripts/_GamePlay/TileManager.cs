using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LogicStates;
using UnityEngine;
using UnityEngine.UI;

public class TileManager : Singleton<TileManager>
{
    [SerializeField] private Vector2 tileStartPosition = new Vector2(-3.2f, 3.2f);
    [SerializeField] private Vector2 tileSpriteSize = new Vector2(80.0f, 80.0f);
    [SerializeField] private Vector2Int map99Matrix = new Vector2Int(1, 1);
    [SerializeField] private CameraWork mainCamera;
    [SerializeField] private Sprite[] tileSprites;
    [SerializeField] private TabletContainer tabletContainer;

    private readonly Dictionary<Vector2Int, GameObject> barricadesHorizontalObjs =
        new Dictionary<Vector2Int, GameObject>();

    private readonly Dictionary<Vector2Int, GameObject> barricadesVerticalObjs =
        new Dictionary<Vector2Int, GameObject>();

    private readonly Dictionary<BigObject, List<GameBlock>> bigObjectDictionary =
        new Dictionary<BigObject, List<GameBlock>>();

    private readonly List<ObConnection> connections = new List<ObConnection>();
    private readonly List<ObConnectionUnion> connectionUnions = new List<ObConnectionUnion>();
    private readonly List<GameTile> endTiles = new List<GameTile>();

    private readonly List<List<GameTile>> endToStartTiles = new List<List<GameTile>>();
    private readonly List<GameObject> etcObjects = new List<GameObject>();
    private readonly List<Gear> gears = new List<Gear>();

    private bool isAnotherDirection;
    private int[] multimapOrder;
    private int preMapNumber;
    private Vector2Int preViewMap_leftTopTile_matrix = new Vector2Int(0, 0);
    private readonly List<Rail> rails = new List<Rail>();
    private readonly List<RailUnion> railUnions = new List<RailUnion>();
    private readonly List<Vector2Int> settingBlockList = new List<Vector2Int>();
    private readonly List<GameTile> startTiles = new List<GameTile>();
    private readonly List<Tablet> tablets = new List<Tablet>();

    private readonly Dictionary<Vector2Int, GameTile> tileDictionary = new Dictionary<Vector2Int, GameTile>();

    private Vector2 tileSize = new Vector2(0.8f, 0.8f);
    private readonly List<GameObject> tunnels = new List<GameObject>();

    public Vector2 TileSize => tileSize;
    public bool ShuffleOn { get; private set; }
    public bool IsMultiCameraWorkEnd { get; private set; } = true;

    public bool IsRailEnd { get; set; }

    public bool IsClimberEnd { get; set; }
    public bool IsClimberDamaged { get; set; }

    public int GetDropRelicCount { get; set; }

    private int iDelayClimberCounter = 3;

    public void SetClimberDamaged()
    {
        IsClimberDamaged = true;
    }

    public void GameStart()
    {
        ShuffleOn = false;
        IsMultiCameraWorkEnd = true;
        IsRailEnd = true;
        IsClimberEnd = true;
        preMapNumber = 0;
    }

    public void SettingTile()
    {
        tileStartPosition.x = -3.2f;
        tileStartPosition.y = 2.2f;
        tileSize.x = tileSpriteSize.x * 0.01f;
        tileSize.y = tileSpriteSize.y * 0.01f;

        var number = StageManager.StageNumber;
        if (StageManager.GetInstance.IsEditorMode) number = -1;

        var mapData = DataContainer.GetInstance.GetMapDataOrNULL(number);

        if (BaseSystem.GetInstance != null)
        {
            if (ChallengeSystem.GetInstance != null)
            {
                if (ChallengeSystem.GetInstance.IsChallengeStage)
                {
                    mapData = DataContainer.GetInstance.GetChallengeMapDataOrNULL(number);
                }
                else
                {
                    mapData = DataContainer.GetInstance.GetMapDataOrNULL(number);
                }
            }
            
            if (EventLevelSystem.GetInstance != null)
            {
                if (EventLevelSystem.GetInstance.IsEventLevel)
                {
                    mapData = DataContainer.GetInstance.GetChallengeMapDataOrNULL(number);
                }
            }
        }

        if (mapData != null)
        {
            multimapOrder = new int[mapData.multimapOrder.Length];
            for (var i = 0; i < mapData.multimapOrder.Length; i++) multimapOrder[i] = mapData.multimapOrder[i];
            if (multimapOrder.Length == 1)
            {
                var value = GetTileBalence();
                tileStartPosition.x += value.x * (tileSize.x * 0.5f);
                tileStartPosition.y += value.y * (tileSize.y * 0.5f);
            }

            var firstX = multimapOrder[0] / 10;
            var firstY = multimapOrder[0] - firstX * 10;
            preViewMap_leftTopTile_matrix.x = firstX * 9;
            preViewMap_leftTopTile_matrix.y = firstY * 9;

            map99Matrix.x = mapData.mapSize / 10;
            map99Matrix.y = mapData.mapSize - map99Matrix.x * 10;

            for (var y = 0; y < 9 * map99Matrix.y; y++)
                for (var x = 0; x < 9 * map99Matrix.x; x++)
                {
                    var tilePosition = tileStartPosition + new Vector2(x * tileSize.x,
                        -y * tileSize.y);
                    var tileObj = DynamicObjectPool.GetInstance.GetObjectForType("GameTile", true);
                    //tileObj.transform.SetParent(tileParentObject.transform);
                    tileObj.transform.localPosition = tilePosition;

                    var tile = tileObj.GetComponent<GameTile>();
                    if (tile != null)
                    {
                        var direction = 0;
                        if (mapData.tiles[x + y * 9 * map99Matrix.x] >= 10)
                            direction = mapData.tiles[x + y * 9 * map99Matrix.x] / 10;
                        var tileKind = mapData.tiles[x + y * 9 * map99Matrix.x] - direction * 10;

                        tile.Setting(new Vector2Int(x, y),
                            (ETileKind)tileKind, (EDirection)direction);
                        tileDictionary.Add(tile.Matrix, tile);

                        if ((EDirection)direction != EDirection.DOWN) isAnotherDirection = true;

                        if (tileKind == (int)ETileKind.START) startTiles.Add(tile);
                        if (tileKind == (int)ETileKind.END) endTiles.Add(tile);
                    }
                }
        }

        ConnectTile();
        SettingBlock();
        ConnectEndToStartTile();

        var count = 0;
        while (BlockManager.GetInstance.PopCheck(true))
        {
            count++;
            if (count > 2700)
            {
                DebugX.LogError("No Just Match");
                break;
            }

            BlockManager.GetInstance.ChangeColorToMapSetting(settingBlockList);
            BlockManager.GetInstance.ClearPopCondition();
        }

        var hintInfo = BlockManager.GetInstance.HintCheck();
        if (hintInfo == null)
        {
            var tiles = new List<GameTile>();
            var blocks = new List<GameBlock>();

            var blockInitPosition = new Dictionary<GameBlock, GameTile>();
            var destinationPosition = new Dictionary<GameBlock, GameTile>();

            var randNum = 0;
            var loop = true;

            foreach (var item in tileDictionary)
            {
                if (!IsPreViewTile(item.Value)) continue;
                if (!item.Value.IsSwapAble()) continue;

                var block = item.Value.NormalBlock;
                if (block != null)
                    if (block.ID == EID.NORMAL)
                        blockInitPosition.Add(block, item.Value);
            }

            var maxCount = 0;

            while (loop)
            {
                tiles.Clear();
                blocks.Clear();
                destinationPosition.Clear();

                foreach (var item in tileDictionary)
                {
                    if (!IsPreViewTile(item.Value)) continue;
                    if (!item.Value.IsSwapAble()) continue;
                    if (!item.Value.IsPopAble()) continue;

                    var block = item.Value.NormalBlock;
                    if (block != null)
                        if (block.ID == EID.NORMAL)
                        {
                            blocks.Add(block);
                            tiles.Add(item.Value);
                        }
                }

                foreach (var block in blocks)
                {
                    randNum = Random.Range(0, tiles.Count);
                    block.SetTile(tiles[randNum]);
                    destinationPosition.Add(block, tiles[randNum]);
                    tiles.RemoveAt(randNum);
                }

                if (IsAbleToMatchWhenSwap() && !BlockManager.GetInstance.PopCheck()) loop = false;
                BlockManager.GetInstance.ClearPopCondition();

                if (maxCount > 1000)
                    //gameOver
                    loop = false;
                maxCount++;
            }

            foreach (var item in destinationPosition)
            {
                item.Key.SetTile(item.Value);
                item.Key.SetPosition();
            }
        }

        BlockManager.GetInstance.ClearPopCondition();
        settingBlockList.Clear();
        if (isAnotherDirection) ShowGravity();
    }

    public void TileBombEnd()
    {
        foreach (var item in tileDictionary) item.Value.IsDamagedThisTurn = false;

        foreach (var item in bigObjectDictionary) item.Key.EndBomb();
    }

    private Vector2Int GetTileBalence()
    {
        var mapData = DataContainer.GetInstance.GetMapDataOrNULL(StageManager.StageNumber);

        if (BaseSystem.GetInstance != null)
        {
            if (ChallengeSystem.GetInstance!=null)
            {
                if (ChallengeSystem.GetInstance.IsChallengeStage)
                {
                    mapData = DataContainer.GetInstance.GetChallengeMapDataOrNULL(StageManager.StageNumber);
                }
            }
            
            if (EventLevelSystem.GetInstance != null)
            {
                if (EventLevelSystem.GetInstance.IsEventLevel)
                {
                    mapData = DataContainer.GetInstance.GetChallengeMapDataOrNULL(StageManager.StageNumber);
                }
            }
        }

        if (mapData == null) return Vector2Int.zero;

        var tileSettingValue = Vector2Int.zero;

        var uptime = true;
        var downtime = true;
        for (var y = 0; y < 4; y++)
            if (uptime)
            {
                var isVoid = true;
                for (var x = 0; x < 9; x++)
                {
                    var direction = mapData.tiles[x + y * 9] / 10;

                    var tileKind = (ETileKind)(mapData.tiles[x + y * 9] - direction * 10);
                    if (tileKind != ETileKind.VOID) isVoid = false;
                }

                if (isVoid)
                    tileSettingValue.y++;
                else
                    uptime = false;
            }

        for (var y = 8; y > 4; y--)
            if (downtime)
            {
                var isVoid = true;
                for (var x = 0; x < 9; x++)
                {
                    var direction = mapData.tiles[x + y * 9] / 10;

                    var tileKind = (ETileKind)(mapData.tiles[x + y * 9] - direction * 10);
                    if (tileKind != ETileKind.VOID) isVoid = false;
                }

                if (isVoid)
                    tileSettingValue.y--;
                else
                    downtime = false;
            }

        var leftTime = true;
        var rightTime = true;
        for (var x = 0; x < 4; x++)
            if (leftTime)
            {
                var isVoid = true;
                for (var y = 0; y < 9; y++)
                {
                    var direction = mapData.tiles[x + y * 9] / 10;

                    var tileKind = (ETileKind)(mapData.tiles[x + y * 9] - direction * 10);
                    if (tileKind != ETileKind.VOID) isVoid = false;
                }

                if (isVoid)
                    tileSettingValue.x--;
                else
                    leftTime = false;
            }

        for (var x = 8; x > 4; x--)
            if (rightTime)
            {
                var isVoid = true;
                for (var y = 0; y < 9; y++)
                {
                    var direction = mapData.tiles[x + y * 9] / 10;

                    var tileKind = (ETileKind)(mapData.tiles[x + y * 9] - direction * 10);
                    if (tileKind != ETileKind.VOID) isVoid = false;
                }

                if (isVoid)
                    tileSettingValue.x++;
                else
                    rightTime = false;
            }

        return tileSettingValue;
    }

    private void SettingBlock()
    {
        var blankList = new List<Vector2Int>();

        var number = StageManager.StageNumber;
        if (StageManager.GetInstance.IsEditorMode) number = -1;

        var mapData = DataContainer.GetInstance.GetMapDataOrNULL(number);

        if (BaseSystem.GetInstance != null)
        {
            if (ChallengeSystem.GetInstance!=null)
            {
                if (ChallengeSystem.GetInstance.IsChallengeStage)
                {
                    mapData = DataContainer.GetInstance.GetChallengeMapDataOrNULL(number);
                }
                else
                {
                    mapData = DataContainer.GetInstance.GetMapDataOrNULL(number);
                }
            }
            
            if (EventLevelSystem.GetInstance != null)
            {
                if (EventLevelSystem.GetInstance.IsEventLevel)
                {
                    mapData = DataContainer.GetInstance.GetChallengeMapDataOrNULL(number);
                }
            }
        }

        if (mapData != null)
        {
            for (var i = 0; i < mapData.object_XY.Length; i++)
            {
                if (i >= mapData.object_EtcID.Length) continue;
                if (i >= mapData.object_ColorHp.Length) continue;

                var objXY = mapData.object_XY[i];
                var objEtcID = mapData.object_EtcID[i];
                var objColorHp = mapData.object_ColorHp[i];

                if (objXY == -1 || objEtcID == -1 || objColorHp == -1) break;

                var x = objXY / 100;
                var y = objXY - x * 100;
                var xy = new Vector2Int(x, y);

                var objEtc = objEtcID / 1000;
                var objID = objEtcID - objEtc * 1000;

                var objColor = objColorHp / 100;
                var objHp = objColorHp - objColor * 100;

                if (objID == (int)EID.BLANK)
                {
                    blankList.Add(xy);
                }
                else
                {
                    if (BlockManager.GetInstance.IsBigBlock((EID)objID) || (EID)objID == EID.TABLET)
                    {
                        var obj = BlockManager.GetInstance.GetBigObject((EID)objID);
                        obj.transform.SetParent(tileDictionary[xy].transform);
                        obj.transform.localPosition = new Vector3(0.4f, -0.4f);

                        var bigObject = obj.GetComponent<BigObject>();
                        var size = BlockManager.GetInstance.GetBigObjSize((EID)objID);
                        if ((EID)objID == EID.TABLET)
                        {
                            size = tabletContainer.GetSize(objHp);
                            if ((EDirection)objEtc == EDirection.LEFT || (EDirection)objEtc == EDirection.RIGHT)
                            {
                                var temp = size.x;
                                size.x = size.y;
                                size.y = temp;
                            }
                        }

                        bigObject.Setting((EColor)objColor, xy, size);
                        var bigBlocks = new List<GameBlock>();

                        for (var bigX = 0; bigX < size.x; bigX++)
                            for (var bigY = 0; bigY < size.y; bigY++)
                            {
                                var bigBlock = BlockManager.GetInstance.GetGameBlockOrNull((EID)objID);
                                if (bigBlock == null) continue;
                                bigBlock.Init();
                                bigBlock.Setting((EColor)objColor, (EID)objID, objHp, objEtc);
                                bigBlock.SetProperty(DataContainer.GetInstance.GetObstacleDataOrNULL((EID)objID));
                                bigBlock.ApplySprite();
                                bigBlock.SetTile(tileDictionary[xy + new Vector2Int(bigX, bigY)]);
                                bigBlock.SetPosition();

                                bigBlocks.Add(bigBlock);
                            }

                        bigObjectDictionary.Add(bigObject, bigBlocks);

                        if ((EID)objID == EID.TABLET)
                        {
                            //TODO
                            var tablet = obj.GetComponent<Tablet>();

                            //tablet.Setting((EColor)objColor, xy, size);
                            tablet.SetSprite(tabletContainer.GetSprite(objHp));

                            switch ((EDirection)objEtc)
                            {
                                case EDirection.DOWN:
                                    obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                                    break;

                                case EDirection.UP:
                                    obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                                    break;

                                case EDirection.LEFT:
                                    obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                                    break;

                                case EDirection.RIGHT:
                                    obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
                                    break;
                            }

                            tablet.gameObject.transform.localPosition = new Vector2(-0.4f, 0.4f) +
                                                                        new Vector2(0.4f * size.x, -0.4f * size.y);
                            tablets.Add(tablet);
                        }

                        if ((EID)objID == EID.GEAR)
                        {
                            var tiles = new List<GameTile>();
                            tiles.Add(tileDictionary[xy]);
                            tiles.Add(tileDictionary[xy + new Vector2Int(1, 0)]);
                            tiles.Add(tileDictionary[xy + new Vector2Int(2, 0)]);
                            tiles.Add(tileDictionary[xy + new Vector2Int(2, 1)]);
                            tiles.Add(tileDictionary[xy + new Vector2Int(2, 2)]);
                            tiles.Add(tileDictionary[xy + new Vector2Int(1, 2)]);
                            tiles.Add(tileDictionary[xy + new Vector2Int(0, 2)]);
                            tiles.Add(tileDictionary[xy + new Vector2Int(0, 1)]);

                            obj.gameObject.transform.localPosition = new Vector2(0.8f, -0.8f);

                            var gear = obj.GetComponent<Gear>();
                            gear.AddTiles(tiles);

                            var coreBlock = BlockManager.GetInstance.GetGameBlockOrNull(EID.GEAR_CORE);
                            if (coreBlock == null) continue;
                            coreBlock.Init();
                            coreBlock.Setting(EColor.NONE, EID.GEAR_CORE);
                            coreBlock.SetProperty(DataContainer.GetInstance.GetObstacleDataOrNULL(EID.GEAR_CORE));
                            coreBlock.ApplySprite();
                            coreBlock.SetTile(tileDictionary[xy + new Vector2Int(1, 1)]);
                            coreBlock.SetPosition();

                            var core = coreBlock.gameObject.GetComponent<GearCore>();

                            var isClockwise = true;
                            if (objEtc == 1) isClockwise = false;

                            gear.Setting(core, isClockwise);

                            gears.Add(gear);
                        }
                    }
                    else
                    {
                        if ((EID)objID != EID.SPECIAL_TILE && (EID)objID != EID.TARGET_RELIC &&
                            (EID)objID != EID.BARRICADE &&
                            (EID)objID != EID.TUNNEL_EXIT &&
                            (EID)objID != EID.TUNNEL_ENTRANCE &&
                            (EID)objID != EID.RAIL && (EID)objID != EID.CONNECTION)
                        {
                            var block = BlockManager.GetInstance.GetGameBlockOrNull((EID)objID);
                            if (block != null)
                            {
                                block.Init();
                                block.SetTile(tileDictionary[xy]);
                                if ((EID)objID == EID.NORMAL || (EID)objID == EID.HORIZONTAL ||
                                    (EID)objID == EID.VERTICAL || (EID)objID == EID.X || (EID)objID == EID.RHOMBUS
                                    || (EID)objID == EID.CHAMELEON || (EID)objID == EID.TIMEBOMB_ICE ||
                                    (EID)objID == EID.TIMEBOMB_LAVA
                                    || (EID)objID == EID.GIFTBOX)
                                {
                                    if ((EColor)objColor != EColor.NONE)
                                        settingBlockList.Add(block.Tile.Matrix);
                                    else
                                        objColor = Random.Range(0, StageManager.GetInstance.GetBlockColorCount());
                                }

                                if ((EID)objID == EID.RELIC_IN_INVISIBLE_BOX) blankList.Add(xy);
                                if (DoubleClickSystem.GetInstance != null)
                                {
                                    foreach (var spBlock in DoubleClickSystem.GetInstance.GetBlockList())
                                    {
                                        if ((EID) objID == spBlock)
                                        {
                                            block.Setting(EColor.NONE, (EID) objID, objHp, objEtc);
                                            block.SetMadeColor((EColor) objColor);
                                            break;
                                        }

                                        block.Setting((EColor) objColor, (EID) objID, objHp, objEtc);
                                    }
                                }
                                else
                                {
                                    block.Setting((EColor) objColor, (EID) objID, objHp, objEtc);
                                }

                                block.SetProperty(DataContainer.GetInstance.GetObstacleDataOrNULL((EID)objID));
                                if ((EID)objID == EID.CLIMBER_NEST)
                                {
                                    GameTile gameTile = tileDictionary[xy];
                                    var bObj = DynamicObjectPool.GetInstance.GetObjectForType("ClimberNest", false);
                                    bObj.transform.SetParent(gameTile.gameObject.transform);
                                    bObj.transform.localPosition = Vector3.zero;
                                    switch (gameTile.DropDirection)
                                    {
                                        case EDirection.RIGHT:
                                            bObj.transform.rotation = Quaternion.Euler(0, 0, 90);
                                            break;

                                        case EDirection.LEFT:
                                            bObj.transform.rotation = Quaternion.Euler(0, 0, 270);
                                            break;

                                        case EDirection.UP:
                                            bObj.transform.rotation = Quaternion.Euler(0, 0, 180);
                                            break;

                                        case EDirection.DOWN:
                                            bObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                                            break;
                                    }
                                }
                                else
                                {
                                    block.ApplySprite();
                                }

                                block.SetPosition();

                                if (block.ID == EID.COLOR_BOMB)
                                {
                                    var DummyObj =
                                        DynamicObjectPool.GetInstance.GetObjectForType("RainbowDummy", false);
                                    DummyObj.transform.SetParent(block.transform);
                                    DummyObj.transform.position = block.transform.position;
                                    block.GetComponent<SpriteRenderer>().sprite = null;
                                }
                            }
                        }
                        else
                        {
                            if ((EID)objID == EID.BARRICADE)
                            {
                                var bObj = DynamicObjectPool.GetInstance.GetObjectForType("BarricadeObj", false);
                                bObj.transform.SetParent(tileDictionary[xy].gameObject.transform);
                                var position = new Vector3();
                                var rotation = new Vector3();

                                if (objEtc == 0)
                                {
                                    position.y = 0.4f;
                                    rotation.z = 90;
                                    barricadesHorizontalObjs.Add(xy, bObj);
                                }
                                else
                                {
                                    position.x = -0.4f;
                                    barricadesVerticalObjs.Add(xy, bObj);
                                }

                                bObj.transform.localPosition = position;
                                bObj.transform.rotation = Quaternion.Euler(rotation);
                            }

                            if ((EID)objID == EID.SPECIAL_TILE)
                            {
                                tileDictionary[xy].IsSpecialTile = true;
                                tileDictionary[xy].SpecialTileValue = objEtc;
                                var specialTileObj =
                                    DynamicObjectPool.GetInstance.GetObjectForType("SpecialTile", false);

                                if (specialTileObj != null)
                                {
                                    var tileRenderer = specialTileObj.GetComponent<SpriteRenderer>();
                                    tileRenderer.sprite =
                                        BlockManager.GetInstance.GetBlockSprite(EID.SPECIAL_TILE, EColor.NONE, objHp);

                                    var targetMatrix = GetTilePosition(xy);
                                    switch (tileDictionary[xy].DropDirection)
                                    {
                                        case EDirection.DOWN:
                                            targetMatrix.y += TileSize.y;
                                            targetMatrix.y -= 0.1f;
                                            break;

                                        case EDirection.UP:
                                            targetMatrix.y -= TileSize.y;
                                            targetMatrix.y += 0.1f;
                                            specialTileObj.transform.rotation =
                                                Quaternion.Euler(new Vector3(0, 0, 180));
                                            break;

                                        case EDirection.LEFT:
                                            targetMatrix.x += TileSize.x;
                                            targetMatrix.x -= 0.1f;
                                            specialTileObj.transform.rotation =
                                                Quaternion.Euler(new Vector3(0, 0, 270));
                                            break;

                                        case EDirection.RIGHT:
                                            targetMatrix.x -= TileSize.x;
                                            targetMatrix.x += 0.1f;
                                            specialTileObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                                            break;
                                    }

                                    specialTileObj.transform.position = targetMatrix;

                                    etcObjects.Add(specialTileObj);
                                }
                            }

                            if ((EID)objID == EID.TARGET_RELIC)
                            {
                                tileDictionary[xy].IsRelicTarget = true;
                                var arrow = DynamicObjectPool.GetInstance.GetObjectForType("Arrow", false);
                                arrow.transform.SetParent(tileDictionary[xy].gameObject.transform);

                                var arrowScript = arrow.GetComponent<Arrow>();
                                if (arrowScript != null) arrowScript.Setting(xy);

                                switch (tileDictionary[xy].DropDirection)
                                {
                                    case EDirection.DOWN:
                                        arrow.transform.localPosition = new Vector3(0, -0.4f, 0);
                                        break;

                                    case EDirection.UP:
                                        arrow.transform.localPosition = new Vector3(0, 0.4f, 0);
                                        arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                                        break;

                                    case EDirection.LEFT:
                                        arrow.transform.localPosition = new Vector3(-0.4f, 0f, 0);
                                        arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
                                        break;

                                    case EDirection.RIGHT:
                                        arrow.transform.localPosition = new Vector3(0.4f, 0f, 0);
                                        arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                                        break;
                                }
                            }

                            if ((EID)objID == EID.TUNNEL_ENTRANCE)
                            {
                                var exitTileMatrix = new Vector2Int();
                                exitTileMatrix.x = objEtc / 100;
                                exitTileMatrix.y = objEtc - exitTileMatrix.x * 100;

                                tileDictionary[xy].SetExitTunnel(tileDictionary[exitTileMatrix]);

                                var tunnelObj =
                                    DynamicObjectPool.GetInstance.GetObjectForType("TunnelEntrance", false);

                                tunnelObj.transform.SetParent(tileDictionary[xy].gameObject.transform);
                                tunnels.Add(tunnelObj);
                                switch (tileDictionary[xy].DropDirection)
                                {
                                    case EDirection.DOWN:
                                        tunnelObj.transform.localPosition = new Vector3(0, -0.18f, 0);
                                        break;

                                    case EDirection.UP:
                                        tunnelObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180f));
                                        tunnelObj.transform.localPosition = new Vector3(0, 0.18f, 0);
                                        break;

                                    case EDirection.RIGHT:
                                        tunnelObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90f));
                                        tunnelObj.transform.localPosition = new Vector3(0.18f, 0f, 0);
                                        break;

                                    case EDirection.LEFT:
                                        tunnelObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270f));
                                        tunnelObj.transform.localPosition = new Vector3(-0.18f, 0f, 0);
                                        break;
                                }
                            }

                            if ((EID)objID == EID.TUNNEL_EXIT)
                            {
                                var entranceTileMatrix = new Vector2Int();
                                entranceTileMatrix.x = objEtc / 100;
                                entranceTileMatrix.y = objEtc - entranceTileMatrix.x * 100;

                                tileDictionary[xy].SetEntranceTunnel(tileDictionary[entranceTileMatrix]);

                                var tunnelObj =
                                    DynamicObjectPool.GetInstance.GetObjectForType("TunnelExit", false);
                                tunnels.Add(tunnelObj);
                                tunnelObj.transform.SetParent(tileDictionary[xy].gameObject.transform);

                                switch (tileDictionary[xy].DropDirection)
                                {
                                    case EDirection.DOWN:
                                        tunnelObj.transform.localPosition = new Vector3(0, 0.18f, 0);
                                        break;

                                    case EDirection.UP:
                                        tunnelObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180f));
                                        tunnelObj.transform.localPosition = new Vector3(0, -0.18f, 0);
                                        break;

                                    case EDirection.RIGHT:
                                        tunnelObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90f));
                                        tunnelObj.transform.localPosition = new Vector3(-0.18f, 0f, 0);
                                        break;

                                    case EDirection.LEFT:
                                        tunnelObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270f));
                                        tunnelObj.transform.localPosition = new Vector3(0.18f, 0f, 0);
                                        break;
                                }
                            }

                            if ((EID)objID == EID.RAIL)
                            {
                                var railOnMatrix = new RailOnMatrix();
                                railOnMatrix.matrix = xy;
                                railOnMatrix.property.order = objEtc / 10;
                                var isRemoveValue = false;
                                if (objEtc - railOnMatrix.property.order * 10 == 1) isRemoveValue = true;
                                railOnMatrix.property.isRemove = isRemoveValue;

                                railOnMatrix.property.power = objColor;
                                railOnMatrix.property.railKind = objHp;

                                var railObj = DynamicObjectPool.GetInstance.GetObjectForType("Rail", false);
                                var rail = railObj.GetComponent<Rail>();
                                railObj.transform.SetParent(tileDictionary[xy].gameObject.transform);
                                railObj.transform.localPosition = Vector3.zero;

                                rail.Setting(tileDictionary[xy], railOnMatrix);

                                rails.Add(rail);
                            }

                            if ((EID)objID == EID.CONNECTION)
                            {
                                var connectionObj =
                                    DynamicObjectPool.GetInstance.GetObjectForType("ObConnection", false);
                                if (connectionObj != null)
                                {
                                    var connection = connectionObj.GetComponent<ObConnection>();
                                    if (connection != null)
                                    {
                                        BlockManager.GetInstance.AddBlock(connection);

                                        var connectionOnMatrix = new ConnectionOnMatrix();
                                        connectionOnMatrix.matrix = xy;
                                        connectionOnMatrix.property.hp = objHp;
                                        connectionOnMatrix.property.order = objEtc / 100;
                                        connectionOnMatrix.property.direction =
                                            (EOneWay)(objEtc - connectionOnMatrix.property.order * 100);

                                        connection.Init();
                                        connection.SetTile(tileDictionary[xy]);
                                        connection.Setting((EColor)objColor, (EID)objID, objHp, objEtc);
                                        connection.SetProperty(
                                            DataContainer.GetInstance.GetObstacleDataOrNULL((EID)objID));
                                        connection.ApplySprite();
                                        connection.SetPosition();
                                        connection.SettingConnection(tileDictionary[xy], connectionOnMatrix);

                                        connections.Add(connection);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        foreach (var item in tileDictionary)
        {
            var matrix = item.Key;
            var tile = item.Value;

            if (tile.NormalBlock == null && !blankList.Contains(matrix)
                                         && tile.TileKind != ETileKind.LADDER && tile.TileKind != ETileKind.VOID)
            {
                var data = StageManager.GetInstance.GetBlockInfo();
                while (data.ID != EID.NORMAL)
                {
                    var tempData = StageManager.GetInstance.GetBlockInfo();
                    data.ID = tempData.ID;
                    data.ETC = tempData.ETC;
                    data.HP = tempData.HP;
                    data.color = tempData.color;
                }

                var block = BlockManager.GetInstance.GetGameBlockOrNull(data.ID);

                if (block != null)
                {
                    //block.Init();
                    block.Setting(data.color, data.ID);
                    block.SetProperty(DataContainer.GetInstance.GetObstacleDataOrNULL(data.ID));
                    block.ApplySprite();
                    block.SetTile(tile);
                    block.SetPosition();
                }
            }
        }

        var lavaCount = 0;
        var bandageCount = 0;

        if (BlockManager.GetInstance.GetBlocksToUseID(EID.LAVA).Count != 0)
            foreach (var item in BlockManager.GetInstance.GetBlocksToUseID(EID.LAVA))
                lavaCount += item.HP;
        if (BlockManager.GetInstance.GetBlocksToUseID(EID.BANDAGE).Count != 0)
            foreach (var item in BlockManager.GetInstance.GetBlocksToUseID(EID.BANDAGE))
                bandageCount += item.HP;
        ObstacleLogic.LavaCount = lavaCount;
        ObstacleLogic.BandageCount = bandageCount;

        SetRailUnion();
        SetConnectionUnions();
    }

    private void ConnectTile()
    {
        foreach (var tile in tileDictionary)
            for (var d = 0; d < (int)EDirection.NONE; d++)
            {
                var connectTile = GetConnectedTileOrNull(tile.Key, (EDirection)d);
                if (connectTile != null) tile.Value.ConnectTile(connectTile, (EDirection)d);
            }

        foreach (var tile in tileDictionary) tile.Value.LineSetting();
    }

    private void ConnectEndToStartTile()
    {
        var collapseTiles = new List<GameTile>();

        foreach (var startTile in startTiles)
        {
            var list = new List<GameTile>();
            var iter = true;
            var tile = startTile;

            while (iter)
            {
                if (!list.Contains(tile))
                {
                    list.Add(tile);
                    
                    //collapseTiles.Add(tile);
                    
                    if (!collapseTiles.Contains(tile))
                    {       
                        collapseTiles.Add(tile);
                    }

                }

                if (tile.TileKind == ETileKind.END)
                {
                    iter = false;
                    break;
                }

                if (tile.ExitTile != null)
                {
                    tile = tile.ExitTile;
                }
                else
                {
                    tile = tile.GetConnectTileOrNull(tile.DropDirection);
                    if (tile == null) break;
                    while (tile.TileKind == ETileKind.LADDER) tile = tile.GetConnectTileOrNull(tile.DropDirection);
                }

                if (tile == null) break;
                //if (tile.TileKind == ETileKind.VOID) break;

                //if (collapseTiles.Contains(tile))
                //{
                //    DebugX.LogError("Gravity Error"); 
                //    break;
                //}
            }

            list.Reverse();
            endToStartTiles.Add(list);
        }

        foreach (var endTile in endTiles)
            if (!IsExistEndToStartTile(endTile))
            {
                var list = new List<GameTile>();
                var iter = true;
                var tile = endTile;
                GameTile nextTile = null;
                var isTunnel = false;

                var maybeError = 0;
                while (iter)
                {
                    isTunnel = false;
                    list.Add(tile);

                    if (tile.EntranceTile != null) isTunnel = true;

                    if (isTunnel)
                    {
                        nextTile = tile.EntranceTile;

                        if (nextTile == null)
                        {
                            iter = false;
                        }
                        else if (nextTile.ExitTile != null)
                        {
                            if (nextTile.ExitTile.Matrix == tile.Matrix)
                                tile = nextTile;
                            else
                                iter = false;
                        }
                    }
                    else
                    {
                        nextTile = tile.GetConnectTileOrNull(tile.ReverseDirection);
                        if (nextTile == null)
                        {
                            iter = false;
                        }
                        else if (nextTile.GetConnectTileOrNull(nextTile.DropDirection) != null)
                        {
                            if (nextTile.GetConnectTileOrNull(nextTile.DropDirection).Matrix == tile.Matrix)
                                tile = tile.GetConnectTileOrNull(tile.ReverseDirection);
                            else
                                iter = false;
                        }
                    }

                    maybeError++;
                    if (maybeError > 999)
                    {
                        DebugX.LogError("Gravity Error!");
                        break;
                    }
                }

                endToStartTiles.Add(list);
            }

        //[도토리] 노말 블럭 조사해서 도토리 부여하는곳
        var nomalBlockList = new List<GameBlock>();
        foreach (var tileList in endToStartTiles)
        {
            foreach (var item in tileList)
            {
                GameBlock block = item.NormalBlock;
                if (block != null)
                {
                    if (block.ID == EID.NORMAL)
                    {
                        nomalBlockList.Add(block);
                    }
                }
            }
        }

        if (AcornSystem.GetInstance != null)
        {
            var stageNumber = StageManager.StageNumber;
            if (stageNumber >= 10 && !PlayerData.GetInstance.IsGotAcornStage(stageNumber))
            {
                var num = Random.Range(0, nomalBlockList.Count);
                if (nomalBlockList.Count != 0)
                {
                    nomalBlockList[num].SetACorn(true);
                    PrefabRegister.GetInstance.GetPrefab("Acorn", nomalBlockList[num].transform);
                }
            }
        }
    }

    private bool IsExistEndToStartTile(GameTile tile)
    {
        foreach (var list in endToStartTiles)
            if (list.Contains(tile))
                return true;
        return false;
    }

    public Vector3 GetTilePosition(Vector2Int matrix)
    {
        if (tileDictionary.ContainsKey(matrix))
            return tileDictionary[matrix].GetPosition();
        return Vector3.zero;
    }

    public GameTile GetConnectedTileOrNull(Vector2Int matrix, EDirection direction)
    {
        if (tileDictionary.ContainsKey(matrix))
        {
            var value_x = 0;
            var value_y = 0;

            switch (direction)
            {
                case EDirection.UP:
                    value_y = -1;
                    break;

                case EDirection.DOWN:
                    value_y = 1;
                    break;

                case EDirection.LEFT:
                    value_x = -1;
                    break;

                case EDirection.RIGHT:
                    value_x = 1;
                    break;

                case EDirection.UP_LEFT:
                    value_y = -1;
                    value_x = -1;
                    break;

                case EDirection.UP_RIGHT:
                    value_y = -1;
                    value_x = 1;
                    break;

                case EDirection.DOWN_LEFT:
                    value_y = 1;
                    value_x = -1;
                    break;

                case EDirection.DOWN_RIGHT:
                    value_y = 1;
                    value_x = 1;
                    break;
            }

            var targetMatrix = new Vector2Int(matrix.x + value_x, matrix.y + value_y);
            if (tileDictionary.ContainsKey(targetMatrix)) return tileDictionary[targetMatrix];
        }

        return null;
    }

    public void SetCameraPosition()
    {
        var cameraPosition = tileStartPosition + new Vector2((preViewMap_leftTopTile_matrix.x + 4) * tileSize.x,
            -(preViewMap_leftTopTile_matrix.y + 2) * tileSize.y - 0.6f);
        mainCamera.Move(cameraPosition, 1f);
    }

    public bool IsPreViewTile(GameTile tile)
    {
        if (tile == null) return false;
        if (preViewMap_leftTopTile_matrix == null) return false;

        var tileMatrix = tile.Matrix;

        if (tileMatrix.x < preViewMap_leftTopTile_matrix.x
            || tileMatrix.x > preViewMap_leftTopTile_matrix.x + 8)
            return false;
        if (tileMatrix.y < preViewMap_leftTopTile_matrix.y
            || tileMatrix.y > preViewMap_leftTopTile_matrix.y + 8)
            return false;

        return true;
    }

    public int GetPreViewTile(ETileKind tileKind)
    {
        var result = 0;
        foreach (var item in tileDictionary)
            if (IsPreViewTile(item.Value))
                if (item.Value.TileKind == tileKind)
                    result++;
        return result;
    }

    public bool DropCheck()
    {
        var returnValue = false;

        if (CheckDirectionDrop(EDirection.LEFT)
            || CheckDirectionDrop(EDirection.RIGHT))
            returnValue = true;

        if (CheckDrop())
            returnValue = true;

        if (CheckDirectionDrop(EDirection.LEFT)
            || CheckDirectionDrop(EDirection.RIGHT))
            returnValue = true;

        return returnValue;
    }

    public bool CheckDrop()
    {
        var returnValue = false;

        foreach (var tiles in endToStartTiles)
        {
            var tileCount = -1;
            foreach (var tile in tiles)
            {
                var isTunnel = false;
                tileCount++;

                if (tile.ExitTile != null) isTunnel = true;

                if (!tile.IsDropAble(tile.DropDirection)) continue;
                if (tileCount == 0) continue;

                GameTile goalTile = null;
                if (isTunnel)
                    goalTile = tile.ExitTile;
                else
                    goalTile = GetConnectedTileOrNull(tile.Matrix, tile.DropDirection);
                if (goalTile == null) continue;
                while (goalTile.TileKind == ETileKind.LADDER)
                    goalTile = GetConnectedTileOrNull(goalTile.Matrix, goalTile.DropDirection);
                if (goalTile.IsDroppedAble())
                {
                    //TODO
                    tile.NormalBlock.IsDropped = true;
                    returnValue = true;
                }
            }
        }

        return returnValue;
    }

    public void Drop()
    {
        GameTile tile = null;
        var tiles = new List<GameTile>();

        foreach (var endtile in endTiles)
        {
            tile = endtile;
            if (tile != null) tiles.Add(tile);
        }

        var iter = true;
        while (iter)
        {
            var tempList = new List<GameTile>();

            foreach (var item in tiles) tempList.Add(item);
            tiles.Clear();

            foreach (var t in tempList)
            {
                t.Drop();

                if (GetPreviousTileOrNullDrop(t) != null) tiles.Add(GetPreviousTileOrNullDrop(t));
            }

            if (tiles.Count == 0) iter = false;
        }
    }

    public GameTile GetPreviousTileOrNull(GameTile _tile)
    {
        foreach (var tiles in endToStartTiles)
            if (tiles.Contains(_tile))
            {
                var tileNum = tiles.FindIndex(x => x == _tile);
                tileNum++;
                if (tileNum >= tiles.Count) return null;
                return tiles[tileNum];
            }

        return null;
    }
    
    public GameTile GetPreviousTileOrNullDrop(GameTile _tile)
    {
        foreach (var tiles in endToStartTiles)
            if (tiles.Contains(_tile))
            {
                foreach (var tile in tiles)
                {
                    if (tile.NormalBlock != null)
                    {
                        if (tile.NormalBlock.IsDropped)
                        {
                            var tileNum = tiles.FindIndex(x => x == _tile);
                            tileNum++;
                            if (tileNum >= tiles.Count) return null;
                            return tiles[tileNum];
                        }
                    }
                }
            }

        return null;
    }


    public GameTile GetNextTileOrNull(GameTile tile)
    {
        foreach (var list in endToStartTiles)
            for (var i = 0; i < list.Count; i++)
                if (list[i].Matrix == tile.Matrix)
                {
                    var nextNumber = i - 1;
                    if (nextNumber < 0) return null;
                    return list[nextNumber];
                }

        return null;
    }

    public void CreateBlock()
    {
        var createID = EID.NORMAL;
        var IsSpecialCreate = false;

        if (StageManager.GetInstance.IsMissionBeing(EID.DROP_RELIC1)
            && !StageManager.GetInstance.IsMissionClear(EID.DROP_RELIC1)
            && BlockManager.GetInstance.RelicCoolTime1 <= 0)
        {
            createID = EID.DROP_RELIC1;
            IsSpecialCreate = true;
        }

        if (StageManager.GetInstance.IsMissionBeing(EID.DROP_RELIC2)
            && !StageManager.GetInstance.IsMissionClear(EID.DROP_RELIC2)
            && BlockManager.GetInstance.RelicCoolTime2 <= 0)
        {
            createID = EID.DROP_RELIC2;
            IsSpecialCreate = true;
        }

        var ableTiles = new List<GameTile>();
        if (IsSpecialCreate)
        {
            foreach (var tile in startTiles)
                if (tile.IsCreateBlock())
                    if (!tile.IsSpecialTile)
                        ableTiles.Add(tile);

            if (ableTiles.Count != 0)
            {
                var ranValue = Random.Range(0, ableTiles.Count);
                var startBlock = BlockManager.GetInstance.GetGameBlockOrNull(createID);
                var data = new BlockInfo(createID, EColor.NONE, 1, 0);

                startBlock.Init();
                startBlock.Setting(data.color, data.ID);
                startBlock.SetProperty(DataContainer.GetInstance.GetObstacleDataOrNULL(data.ID));
                startBlock.SetStartTile(ableTiles[ranValue]);
                startBlock.ApplySprite();

                if (createID == EID.DROP_RELIC1) BlockManager.GetInstance.RelicCoolTime1 = 2;
                if (createID == EID.DROP_RELIC2) BlockManager.GetInstance.RelicCoolTime2 = 2;
            }
        }

        foreach (var tile in startTiles)
            if (tile.IsCreateBlock())
            {
                if (!tile.IsSpecialTile)
                {
                    var data = StageManager.GetInstance.GetBlockInfo();
                    if (data.ID == EID.CHAMELEON)
                    {
                        //TODO: Add Color Range
                        //data.color = (EColor)Random.Range(0, (int)EColor.ORANGE);
                    }

                    var startBlock = BlockManager.GetInstance.GetGameBlockOrNull(data.ID);
                    if (startBlock != null)
                    {
                        startBlock.Init();
                        if (StageManager.GetInstance.GetBlockColorCount() <= (int)data.color)
                        {
                            var ranColor = EColor.RED;
                            ranColor = (EColor)Random.Range(0,
                                StageManager.GetInstance.GetBlockColorCount());
                            startBlock.Setting(ranColor, data.ID, _etc: data.ETC);
                        }
                        else
                        {
                            startBlock.Setting(data.color, data.ID, _etc: data.ETC);
                        }

                        startBlock.SetProperty(DataContainer.GetInstance.GetObstacleDataOrNULL(data.ID));
                        startBlock.SetStartTile(tile);
                        startBlock.ApplySprite();
                    }
                }
                else
                {
                    var data = StageManager.GetInstance.GetBlockInfo();
                    var blockInfo = new BlockInfo(EID.NONE, EColor.NONE, 1, 0);
                    var specialItems = SpecialTileItemList.GetInstance.GetSpecialItems(tile.SpecialTileValue);
                    GameBlock startBlock = null;
                    if (tile.SpecialTileValue == 1 || tile.SpecialTileValue == 2)
                    {
                        if (specialItems != null)
                        {
                            var relicList = BlockManager.GetInstance.GetBlocksToUseID(EID.DROP_RELIC3);
                            var RandomPerset = Random.Range(0, 100);
                            foreach (var item in specialItems)
                            {
                                if (item.blockInfo.ID == EID.DROP_RELIC3)
                                    switch (GetDropRelicCount)
                                    {
                                        case 0:
                                            RandomPerset -= item.persent + GetDropRelicCount * 0;
                                            break;

                                        case 1:
                                            RandomPerset -= item.persent + GetDropRelicCount * 5;
                                            break;

                                        case 2:
                                            RandomPerset -= item.persent + GetDropRelicCount * 5;
                                            break;

                                        case 3:
                                            RandomPerset -= item.persent + GetDropRelicCount * 10;
                                            break;

                                        case 4:
                                            RandomPerset -= item.persent + 50;
                                            break;

                                        default:
                                            RandomPerset -= 100;
                                            break;
                                    }
                                else
                                    RandomPerset -= item.persent;

                                if (RandomPerset < 0)
                                {
                                    var info = new BlockInfo(item.blockInfo.ID, item.blockInfo.color, item.blockInfo.HP,
                                        item.blockInfo.ETC);
                                    blockInfo.ID = info.ID;
                                    blockInfo.color = info.color;
                                    blockInfo.HP = info.HP;
                                    blockInfo.ETC = info.ETC;
                                    break;
                                }
                            }

                            if (blockInfo.ID != EID.DROP_RELIC1 && blockInfo.ID != EID.DROP_RELIC2 &&
                                blockInfo.ID != EID.DROP_RELIC3 &&
                                blockInfo.ID != EID.SHIELD && blockInfo.ID != EID.COLOR_BOMB &&
                                blockInfo.color == EColor.NONE)
                                blockInfo.color = data.color;
                            else blockInfo.color = EColor.NONE;

                            if (blockInfo.ID == EID.NONE || StageManager.GetInstance.IsMissionClear(EID.DROP_RELIC3) ||
                                StageManager.GetInstance.IsStageClear()) blockInfo = data;

                            if (StageManager.GetInstance.IsMissionBeing(EID.DROP_RELIC3))
                            {
                                var isAbleRelic3 = false;
                                var missions = StageManager.GetInstance.GetMissions();

                                var list = BlockManager.GetInstance.GetBlocksToUseID(EID.DROP_RELIC3);
                                var missionCount = 0;
                                foreach (var mission in missions)
                                    if (mission.ID == EID.DROP_RELIC3)
                                        missionCount = mission.count;

                                missionCount /= 2;
                                missionCount = Mathf.Max(missionCount, 1);

                                if (list.Count < missionCount) isAbleRelic3 = true;

                                if (!isAbleRelic3)
                                {
                                    GetDropRelicCount = 0;
                                    if (blockInfo.ID == EID.DROP_RELIC3 || StageManager.GetInstance.IsStageClear())
                                        blockInfo = data;
                                }
                            }
                            else
                            {
                                if (blockInfo.ID == EID.DROP_RELIC3 || StageManager.GetInstance.IsStageClear())
                                    blockInfo = data;
                            }

                            if (blockInfo.ID == EID.DROP_RELIC3) GetDropRelicCount = 0;

                            startBlock = BlockManager.GetInstance.GetGameBlockOrNull(blockInfo.ID);
                            if (startBlock != null)
                            {
                                startBlock.Init();

                                startBlock.Setting(blockInfo.color, blockInfo.ID, blockInfo.HP, blockInfo.ETC);
                                startBlock.SetProperty(DataContainer.GetInstance.GetObstacleDataOrNULL(blockInfo.ID));
                                startBlock.SetStartTile(tile);
                                startBlock.ApplySprite();
                            }
                        }
                        else
                        {
                            startBlock = BlockManager.GetInstance.GetGameBlockOrNull(data.ID);
                            if (startBlock != null)
                            {
                                startBlock.Init();
                                startBlock.Setting(data.color, data.ID);
                                startBlock.SetProperty(DataContainer.GetInstance.GetObstacleDataOrNULL(data.ID));
                                startBlock.SetStartTile(tile);
                                startBlock.ApplySprite();
                            }
                        }
                    }
                    else
                    {
                        if (specialItems != null)
                        {
                            var RandomPerset = Random.Range(0, 100);
                            foreach (var item in specialItems)
                            {
                                RandomPerset -= item.persent;
                                if (RandomPerset < 0)
                                {
                                    var info = new BlockInfo(item.blockInfo.ID, item.blockInfo.color, item.blockInfo.HP,
                                        item.blockInfo.ETC);
                                    blockInfo.ID = info.ID;
                                    blockInfo.color = info.color;
                                    blockInfo.HP = info.HP;
                                    blockInfo.ETC = info.ETC;
                                    break;
                                }
                            }
                        }

                        if (blockInfo.ID != EID.DROP_RELIC1 && blockInfo.ID != EID.DROP_RELIC2 &&
                            blockInfo.ID != EID.DROP_RELIC3 &&
                            blockInfo.ID != EID.SHIELD && blockInfo.ID != EID.COLOR_BOMB &&
                            blockInfo.color == EColor.NONE)
                            blockInfo.color = data.color;
                        else blockInfo.color = EColor.NONE;

                        if (blockInfo.ID == EID.NONE || StageManager.GetInstance.IsStageClear()) blockInfo = data;
                        startBlock = BlockManager.GetInstance.GetGameBlockOrNull(blockInfo.ID);
                        if (startBlock != null)
                        {
                            startBlock.Init();
                            if (DoubleClickSystem.GetInstance != null)
                            {
                                foreach (var item in DoubleClickSystem.GetInstance.GetBlockList())
                                {
                                    if (blockInfo.ID == item)
                                    {
                                        startBlock.Setting(EColor.NONE, blockInfo.ID, blockInfo.HP, blockInfo.ETC);
                                        break;
                                    }
                                    else
                                    {
                                        startBlock.Setting(blockInfo.color, blockInfo.ID, blockInfo.HP, blockInfo.ETC);
                                    }
                                }
                            }
                            else
                            {
                                startBlock.Setting(blockInfo.color, blockInfo.ID, blockInfo.HP, blockInfo.ETC);
                            }
                            startBlock.SetProperty(DataContainer.GetInstance.GetObstacleDataOrNULL(blockInfo.ID));
                            startBlock.SetStartTile(tile);
                            startBlock.ApplySprite();

                            if (startBlock.ID == EID.COLOR_BOMB)
                            {
                                var DummyObj = DynamicObjectPool.GetInstance.GetObjectForType("RainbowDummy", false);
                                DummyObj.transform.SetParent(startBlock.transform);
                                DummyObj.transform.position = startBlock.transform.position;
                                startBlock.GetComponent<SpriteRenderer>().sprite = null;
                            }
                        }
                    }
                }
            }
    }

    public bool IsAbleToCreateBlock()
    {
        //TODO
        var returnValue = false;

        foreach (var tile in startTiles)
            if (tile.IsCreateBlock())
                returnValue = true;
        return returnValue;
    }

    public bool IsAbleToMatchWhenSwap()
    {
        //TODO: 레인보우 조합, 폭탄 조합, 물고기 조건 추가 필요
        var returnValue = true;

        var info = BlockManager.GetInstance.HintCheck();
        if (info == null) returnValue = false;

        return returnValue;
    }

    public bool CheckNotBlockState(GameTile targetTile, GameTile basicTile, EDirection direction)
    {
        if (!targetTile.IsDroppedAble()) return false;
        if (targetTile.TileKind == ETileKind.START) return false;

        var targetLineTile = GetPreviousTileOrNull(targetTile);
        if (targetLineTile == null) return true;

        var basicNextTile = GetNextTileOrNull(basicTile);
        if (basicNextTile != null)
        {
            // TL | B
            // T  | BN
            if (CheckBarricade(basicTile.Matrix, direction) && CheckBarricade(basicNextTile.Matrix, direction))
                return false;

            // TL B
            // --
            // T | BN

            if (CheckBarricade(targetLineTile.Matrix, targetLineTile.DropDirection)
                && CheckBarricade(basicNextTile.Matrix, direction))
                return false;
        }

        if (targetLineTile.TileKind == ETileKind.VOID) return true;

        // TL B
        // ----
        // T  BN
        if (CheckBarricade(basicTile.Matrix, basicTile.DropDirection)
            && CheckBarricade(targetLineTile.Matrix, targetLineTile.DropDirection))
            return false;

        // B | TL
        // --
        // BN  T
        if (CheckBarricade(basicTile.Matrix, basicTile.DropDirection) &&
            CheckBarricade(basicTile.Matrix, direction)) return false;

        var iter = true;
        while (iter)
        {
            if (targetLineTile.TileKind == ETileKind.START)
            {
                // 시작 타일에 상자같은 블록이 있는지 확인. 없으면 false
                if (targetLineTile.NormalBlock != null)
                {
                    if (!targetLineTile.NormalBlock.IsDropAble()) return true;
                    if (!targetLineTile.IsDropAble()) return true;
                }

                if (targetLineTile.GetTopBlockOrNull() != null)
                    if (!targetLineTile.GetTopBlockOrNull().Property.moveProperty.AnotherDrop)
                        return true;
                return false;
            }

            if (targetLineTile.IsDropAble(targetLineTile.DropDirection)) return false;

            if (targetLineTile.NormalBlock == null)
            {
                targetLineTile = GetPreviousTileOrNull(targetLineTile);
                if (targetLineTile == null) iter = false;
            }
            else
            {
                return true;
            }
        }

        return true;
    }

    public bool CheckDirectionDrop(EDirection direction)
    {
        var returnValue = false;

        foreach (var list in endToStartTiles)
            foreach (var tile in list)
            {
                if (!tile.IsDropAble()) continue;
                var targetTile = GetNextTileOrNull(tile);
                if (targetTile != null)
                    if (targetTile.IsDroppedAble())
                        continue;

                var leftDirection = EDirection.NONE;
                var rightDirection = EDirection.RIGHT;

                switch (tile.DropDirection)
                {
                    case EDirection.DOWN:
                        leftDirection = EDirection.DOWN_LEFT;
                        rightDirection = EDirection.DOWN_RIGHT;
                        break;

                    case EDirection.UP:
                        leftDirection = EDirection.UP_RIGHT;
                        rightDirection = EDirection.UP_LEFT;
                        break;

                    case EDirection.LEFT:
                        leftDirection = EDirection.UP_LEFT;
                        rightDirection = EDirection.DOWN_LEFT;
                        break;

                    case EDirection.RIGHT:
                        leftDirection = EDirection.DOWN_RIGHT;
                        rightDirection = EDirection.UP_RIGHT;
                        break;
                }

                var resultDirection = EDirection.NONE;
                if (direction == EDirection.LEFT) resultDirection = leftDirection;
                if (direction == EDirection.RIGHT) resultDirection = rightDirection;

                var nextTile = GetNextTileOrNull(tile);
                var isDroppedAble = false;
                while (true)
                    if (nextTile != null)
                    {
                        if (nextTile.IsDroppedAble())
                        {
                            isDroppedAble = true;
                            break;
                        }

                        if (!nextTile.IsDropAble()) break;
                        nextTile = GetNextTileOrNull(nextTile);
                    }
                    else
                    {
                        break;
                    }

                if (isDroppedAble) continue;

                var goalTile = tile.GetConnectTileOrNull(resultDirection);
                if (goalTile == null) continue;
                if (!goalTile.IsDroppedAble()) continue;
                if (!CheckNotBlockState(goalTile, tile, direction)) continue;

                if (goalTile.NormalBlock == null)
                {
                    tile.SideDropDirection = resultDirection;
                    tile.IsSideDrop = true;
                    tile.NormalBlock.IsDropped = true;
                    returnValue = true;
                    goalTile.IsDroppedBlock = true;
                }
                else
                {
                    if (!goalTile.NormalBlock.IsDropped) continue;
                    tile.SideDropDirection = resultDirection;
                    tile.IsSideDrop = true;
                    tile.NormalBlock.IsDropped = true;
                    returnValue = true;
                    goalTile.IsDroppedBlock = true;
                }
            }

        return returnValue;
    }

    public void SetShuffleTile()
    {
        StartCoroutine(ShuffleBlockCoroutine());
    }

    private IEnumerator ShuffleBlockCoroutine()
    {
        ShuffleOn = true;
        yield return new WaitForSeconds(0.6f);

        var tiles = new List<GameTile>();
        var blocks = new List<GameBlock>();

        var blockInitPosition = new Dictionary<GameBlock, GameTile>();
        var destinationPosition = new Dictionary<GameBlock, GameTile>();

        var randNum = 0;
        var loop = true;

        foreach (var item in tileDictionary)
        {
            if (!IsPreViewTile(item.Value)) continue;
            if (!item.Value.IsSwapAble()) continue;

            var block = item.Value.NormalBlock;
            if (block != null)
                if (block.ID == EID.NORMAL)
                    blockInitPosition.Add(block, item.Value);
        }

        var maxCount = 0;

        while (loop)
        {
            tiles.Clear();
            blocks.Clear();
            destinationPosition.Clear();

            foreach (var item in tileDictionary)
            {
                if (!IsPreViewTile(item.Value)) continue;
                if (!item.Value.IsSwapAble()) continue;
                if (!item.Value.IsPopAble()) continue;

                var block = item.Value.NormalBlock;
                if (block != null)
                    if (block.ID == EID.NORMAL)
                    {
                        blocks.Add(block);
                        tiles.Add(item.Value);
                    }
            }

            foreach (var block in blocks)
            {
                randNum = Random.Range(0, tiles.Count);
                block.SetTile(tiles[randNum]);
                destinationPosition.Add(block, tiles[randNum]);
                tiles.RemoveAt(randNum);
            }

            if (IsAbleToMatchWhenSwap() && !BlockManager.GetInstance.PopCheck()) loop = false;
            BlockManager.GetInstance.ClearPopCondition();

            if (maxCount > 1000)
            {
                //gameOver
                StageManager.GetInstance.GameOver();
                loop = false;
                yield return null;
            }

            maxCount++;
        }

        foreach (var item in blockInitPosition) item.Key.SetTile(item.Value);

        yield return new WaitForSeconds(0.3f);

        foreach (var item in destinationPosition)
            //moving
            item.Key.MoveShuffle(item.Value);
        //yield return new WaitForSeconds(0.01f);

        yield return new WaitUntil(() => BlockManager.GetInstance.IsMovingEnd());

        foreach (var item in destinationPosition) item.Key.SetTile(item.Value);

        ShuffleOn = false;
    }

    public bool IsBombing()
    {
        foreach (var tile in tileDictionary)
            if (tile.Value.IsBombing)
                return true;
        return false;
    }

    public void ClearBombCondition()
    {
        foreach (var tile in tileDictionary) tile.Value.IsBombActive = false;
    }

    public Sprite GetTileSpriteOrNull(ETileKind kind)
    {
        if (kind == ETileKind.NORMAL1 && (int) kind >= tileSprites.Length) kind = ETileKind.NORMAL;
        if ((int)kind >= tileSprites.Length || (int)kind < 0) return null;
        return tileSprites[(int)kind];
    }

    public bool IsJamTiles(List<GameBlock> blocks)
    {
        foreach (var block in blocks)
        {
            if (block.Tile == null) return false;
            if (block.Tile.IsFreeJam()) return true;
        }

        return false;
    }

    public void SetRailUnion()
    {
        var orders = new List<int>();

        foreach (var rail in rails)
        {
            var order = rail.RailProperty.property.order;
            if (!orders.Contains(order)) orders.Add(order);
        }

        foreach (var order in orders)
        {
            var railUnion = new RailUnion();
            railUnion.Setting(order);
            foreach (var rail in rails)
                if (rail.RailProperty.property.order == order)
                    railUnion.AddRail(rail);
            railUnion.ConnectRail();
            railUnions.Add(railUnion);
        }
    }

    public void SetConnectionUnions()
    {
        var orders = new List<int>();

        foreach (var connection in connections)
        {
            var order = connection.ConnectionProperty.property.order;
            if (!orders.Contains(order)) orders.Add(order);
        }

        foreach (var order in orders)
        {
            var connectionUnion = new ObConnectionUnion();
            connectionUnion.Order = order;
            foreach (var connect in connections)
                if (connect.ConnectionProperty.property.order == order)
                    connectionUnion.AddConnection(connect);
            connectionUnion.ConnectConnections();
            connectionUnions.Add(connectionUnion);
        }

        foreach (var connection in connections) connection.WrapUpConnection();
    }

    public void Clear()
    {
        ShuffleOn = false;
        IsMultiCameraWorkEnd = true;
        IsRailEnd = true;
        IsClimberEnd = true;
        settingBlockList.Clear();

        //endToStartTiles.Clear();
        startTiles.Clear();
        endTiles.Clear();
        foreach (var item in bigObjectDictionary)
        {
            item.Value.Clear();
            Destroy(item.Key.gameObject);
        }

        bigObjectDictionary.Clear();
        foreach (var item in tileDictionary)
        {
            Destroy(item.Value.gameObject);
            item.Value.Clear();
        }

        tileDictionary.Clear();

        foreach (var item in endToStartTiles) item.Clear();
        endToStartTiles.Clear();

        foreach (var item in barricadesVerticalObjs) Destroy(item.Value.gameObject);
        barricadesVerticalObjs.Clear();
        foreach (var item in barricadesHorizontalObjs) Destroy(item.Value.gameObject);
        barricadesHorizontalObjs.Clear();

        foreach (var rail in rails) Destroy(rail.gameObject);
        rails.Clear();
        railUnions.Clear();

        foreach (var connection in connections) Destroy(connection.gameObject);
        connections.Clear();
        connectionUnions.Clear();

        foreach (var gear in gears) Destroy(gear.gameObject);
        gears.Clear();

        foreach (var obj in etcObjects) Destroy(obj);
        etcObjects.Clear();

        preMapNumber = 0;
        StopAllCoroutines();
    }

    public void BigSidePop(GameBlock block, EColor color, EDirection direction)
    {
        BigObject bigObject = null;

        foreach (var item in bigObjectDictionary)
            if (item.Value.Contains(block))
                bigObject = item.Key;

        if (bigObject != null)
            bigObject.SidePop(color, direction, block.Tile.Matrix);
    }

    public void BigBombPop(GameBlock block)
    {
        BigObject bigObject = null;

        foreach (var item in bigObjectDictionary)
            if (item.Value.Contains(block))
                bigObject = item.Key;

        if (bigObject != null)
            bigObject.BombPop(block.Tile.Matrix);
    }

    public void BigClear(BigObject bigObj)
    {
        if (!bigObjectDictionary.ContainsKey(bigObj)) return;
        foreach (var block in bigObjectDictionary[bigObj])
        {
            block.Tile.RemoveBlock(block);
            block.Clear();
        }

        bigObjectDictionary.Remove(bigObj);
        Destroy(bigObj.gameObject);
    }

    public void TabletClear(Tablet tablet)
    {
        if (!tablets.Contains(tablet)) return;
        tablets.Remove(tablet);
    }

    public GameBlock GetLeftTopBlockInBigObjOrNull(BigObject bigObj)
    {
        if (!bigObjectDictionary.ContainsKey(bigObj)) return null;

        return bigObjectDictionary[bigObj][0];
    }

    public bool CheckRelicDrop()
    {
        var returnValue = false;
        foreach (var tileItem in tileDictionary)
            if (tileItem.Value.IsRelicTarget)
            {
                if (tileItem.Value.NormalBlock == null) continue;
                if (tileItem.Value.NormalBlock.isDropMoving) continue;
                if (tileItem.Value.NormalBlock.ID == EID.DROP_RELIC1
                    || tileItem.Value.NormalBlock.ID == EID.DROP_RELIC2
                    || tileItem.Value.NormalBlock.ID == EID.DROP_RELIC3)
                    returnValue = true;
            }

        return returnValue;
    }

    public void RelicException()
    {
        if (StageManager.GetInstance.IsMissionBeing(EID.DROP_RELIC1)
            || StageManager.GetInstance.IsMissionBeing(EID.DROP_RELIC2)
            || StageManager.GetInstance.IsMissionBeing(EID.DROP_RELIC3))
        {
            var isMisstake = true;
            foreach (var item in endTiles)
                if (item.IsRelicTarget)
                    isMisstake = false;

            if (isMisstake)
                foreach (var item in endTiles)
                {
                    item.IsRelicTarget = true;

                    var arrow = DynamicObjectPool.GetInstance.GetObjectForType("Arrow", false);
                    arrow.transform.SetParent(item.gameObject.transform);

                    var arrowScript = arrow.GetComponent<Arrow>();
                    if (arrowScript != null) arrowScript.Setting(item.Matrix);

                    switch (item.DropDirection)
                    {
                        case EDirection.DOWN:
                            arrow.transform.localPosition = new Vector3(0, -0.4f, 0);
                            break;

                        case EDirection.UP:
                            arrow.transform.localPosition = new Vector3(0, 0.4f, 0);
                            arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                            break;

                        case EDirection.LEFT:
                            arrow.transform.localPosition = new Vector3(-0.4f, 0f, 0);
                            arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
                            break;

                        case EDirection.RIGHT:
                            arrow.transform.localPosition = new Vector3(0.4f, 0f, 0);
                            arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                            break;
                    }
                }
        }
    }

    public bool MovingRail()
    {
        IsRailEnd = false;
        StartCoroutine(MovingRailCoroutine());

        if (railUnions.Count > 0) return true;
        return false;
    }

    private IEnumerator MovingRailCoroutine()
    {
        if (railUnions.Count == 0) IsRailEnd = true;
        yield return new WaitForSeconds(0.3f);
        if (railUnions.Count > 0)
            if (SoundManager.GetInstance != null)
                SoundManager.GetInstance.Play("RailActive");
        foreach (var union in railUnions)
        {
            for (var i = 0; i < union.Power; i++)
            {
                union.Moving();
                yield return new WaitUntil(() => BlockManager.GetInstance.IsMovingEnd()
                //&& !BlockManager.GetInstance.IsDropMoving()
                );
                //yield return new WaitForSeconds(0.005f);
                yield return new WaitForSeconds(0.2f);
            }

            yield return new WaitForSeconds(0.1f);
        }

        IsRailEnd = true;
    }

    public void MovingGear()
    {
        IsRailEnd = false;
        StartCoroutine(MovingGearCoroutine());
    }

    private IEnumerator MovingGearCoroutine()
    {
        if (gears.Count != 0)
            if (SoundManager.GetInstance != null)
                SoundManager.GetInstance.Play("GearActive");
        foreach (var gear in gears)
            gear.Moving();
        //yield return new WaitForSeconds(0.2f);
        yield return new WaitForSeconds(0.2f);
        IsRailEnd = true;
    }

    public void MovingClimber()
    {
        IsClimberEnd = false;
        StartCoroutine(ClimberCoroutine());
    }

    private Tweener climberAttackTween = null;

    public void ActionClimberAttack(Sprite sp)
    {
        string strColor = "red";
        if (sp.name.Contains("1"))
        {
            strColor = "red";
        }
        else if (sp.name.Contains("2"))
        {
            strColor = "yellow";
        }
        else if (sp.name.Contains("3"))
        {
            strColor = "green";
        }
        else if (sp.name.Contains("4"))
        {
            strColor = "blue";
        }
        else if (sp.name.Contains("5"))
        {
            strColor = "purple";
        }
        else if (sp.name.Contains("6"))
        {
            strColor = "orange";
        }

        string climberAttackParticle = string.Format($"Image_ghost_{strColor.ToString().ToLower()}");

        GameObject gobTarget = GameObject.Find("Image - Move");
        var climberAttackPrefab = PrefabRegister.GetInstance.GetPrefab(climberAttackParticle, gobTarget.transform);

        GameObject gobTargetText = GameObject.Find("Text - Count");
        if (null != gobTargetText)
        {
            Text targetText = gobTargetText.GetComponent<Text>();
            if (null != targetText)
            {
                Color color;
                ColorUtility.TryParseHtmlString("#FF4204", out color);
                DOTween.Kill(climberAttackTween);
                climberAttackTween = targetText.DOColor(color, 0.2f).OnComplete(() =>
                {
                    ColorUtility.TryParseHtmlString("#FFEDAA", out color);
                    targetText.DOColor(color, 0.3f);
                });
            }
        }
        AnimationManager.AutoTargetBombCount--;
        StageManager.GetInstance.DecreasePreMoveCount();
        //climberAttackPrefab.transform.localScale = new UnityEngine.Vector3
        //if (climberAttackPrefab != null) climberAttackPrefab.transform.position = sp.
    }

    private IEnumerator ClimberCoroutine()
    {
        /// 클라이머(유령) 이동
        List<GameBlock> climbers = BlockManager.GetInstance.GetBlockList(EID.CLIMBER);
        climbers.Sort(delegate (GameBlock a, GameBlock b)
        {
            if (a.Tile.Matrix.y > b.Tile.Matrix.y)
                return 1;
            else if (a.Tile.Matrix.y < b.Tile.Matrix.y)
                return -1;
            else
                return 0;
        });
        foreach (var iclimber in climbers)
        {
            if (MoveChangeClimber(iclimber.Tile, iclimber.Tile.ReverseDirection) == false)
            {
                if (iclimber.Tile.TileKind == ETileKind.START)
                {
                    Debug.Log("ClimberReached");
                    GameObject gobTarget = GameObject.Find("Image - Move");
                    if (null != gobTarget)
                    {
                        Vector3 vecTargetPosition = gobTarget.transform.position;
                        SpriteMover.StartMoveTarget(iclimber.BlockRenderer.sprite, 0.9f, iclimber.Tile.transform.position, vecTargetPosition, ActionClimberAttack);
                        RemoveBlock(iclimber.Tile, false);
                        AnimationManager.AutoTargetBombCount++;
                    }
                    else
                    {
                        RemoveBlock(iclimber.Tile, false);
                        StageManager.GetInstance.DecreasePreMoveCount();
                    }
                }
                else
                {
                    if (false == MoveChangeClimber(iclimber.Tile, EDirection.LEFT, EDirection.RIGHT))
                    {
                        if (false == MoveChangeClimber(iclimber.Tile, EDirection.RIGHT, EDirection.LEFT))
                        {
                            if (false == MoveChangeClimber(iclimber.Tile, EDirection.UP, EDirection.DOWN))
                            {
                                MoveChangeClimber(iclimber.Tile, EDirection.DOWN, EDirection.UP);
                            }
                        }
                    }
                }
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// 클라이머(유령) 소환
        List<GameBlock> climberNests = BlockManager.GetInstance.GetBlockList(EID.CLIMBER_NEST);
        if (climberNests.Count > 0)
        {
            yield return new WaitForSeconds(0.2f);
            if (IsClimberDamaged == false)
            {
                if (iDelayClimberCounter >= 2)
                {
                    iDelayClimberCounter = 0;
                    foreach (var iclimberNest in climberNests)
                    {
                        var normalblock = iclimberNest.Tile.NormalBlock;
                        if (normalblock == null)
                            continue;
                        if (normalblock.ID == EID.NORMAL)
                        {
                            normalblock.Init();
                            normalblock.Setting(normalblock.Color, EID.CLIMBER);
                            normalblock.SetProperty(DataContainer.GetInstance.GetObstacleDataOrNULL(EID.CLIMBER));
                            normalblock.ApplySprite();

                            var dummy = normalblock.GetComponentInChildren<DummyObject>();
                            if (dummy != null)
                            {
                                dummy.Clear();
                                DynamicObjectPool.GetInstance.PoolObject(dummy.gameObject, false);
                            }

                            normalblock.SetTile(iclimberNest.Tile);
                            normalblock.SetPosition();
                            normalblock.SetAnimTrigger("Scale");
                        }
                    }
                }
            }
            else
            {
            }

            iDelayClimberCounter++;

            yield return new WaitForSeconds(0.2f);
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        IsClimberEnd = true;
        IsClimberDamaged = false;
    }

    public static bool isSpecialBlock(EID _ID)
    {
        bool r_isSpecial = false;
        switch (_ID)
        {
            case EID.FISH:
            case EID.HORIZONTAL:
            case EID.VERTICAL:
            case EID.RHOMBUS:
            case EID.X:
                {
                    r_isSpecial = true;
                    break;
                }
        }

        return r_isSpecial;
    }

    private bool MoveChangeClimber(GameTile _climber, EDirection _eDirection, EDirection _eCheckDirection = EDirection.NONE)
    {
        bool r_isClimberSwaped = false;

        bool _isSwapAble = false;

        GameTile _targettile = _climber.GetConnectTileOrNull(_eDirection);
        if (_targettile != null && _targettile.NormalBlock != null)
        {
            if (_targettile.NormalBlock.ID != EID.CLIMBER)
            {
                if (true == _targettile.IsSwapAble())
                {
                    if (_eCheckDirection == EDirection.NONE)
                    {
                        _isSwapAble = true;
                    }
                    else
                    {
                        if (_targettile.DropDirection == _eCheckDirection)
                        {
                            _isSwapAble = true;
                        }
                    }
                }

                if (true == _isSwapAble)
                {
                    _targettile.NormalBlock.Swap(_climber);
                    _climber.NormalBlock.Swap(_targettile);
                    _climber.IsSwap = true;
                    _targettile.IsSwap = true;
                    r_isClimberSwaped = true;
                }
            }
        }

        return r_isClimberSwaped;
    }

    public void RemoveBlock(GameTile _tile, bool isCollectMission = true)
    {
        var block = _tile.NormalBlock;
        if (isCollectMission == true)
        {
            block.Remove();
        }

        _tile.RemoveBlock(block);
        block.Clear();
    }

    public void RelicRemove()
    {
        foreach (var tileItem in tileDictionary)
            if (tileItem.Value.IsRelicTarget)
            {
                if (tileItem.Value.NormalBlock == null) continue;
                if (tileItem.Value.NormalBlock.isDropMoving) continue;
                if (tileItem.Value.NormalBlock.ID == EID.DROP_RELIC1
                    || tileItem.Value.NormalBlock.ID == EID.DROP_RELIC2
                    || tileItem.Value.NormalBlock.ID == EID.DROP_RELIC3)
                {
                    var relicBlock = tileItem.Value.NormalBlock;
                    relicBlock.Remove();
                    tileItem.Value.RemoveBlock(relicBlock);
                    relicBlock.Clear();
                }
            }
    }

    public bool CheckEndBigAnim()
    {
        foreach (var item in bigObjectDictionary)
            if (!item.Key.IsEndAnim)
                return false;
        return true;
    }

    public bool CheckBarricade(Vector2Int matrix, EDirection direction)
    {
        var targetMatrix = matrix;
        switch (direction)
        {
            case EDirection.DOWN:
                targetMatrix.y++;
                break;

            case EDirection.RIGHT:
                targetMatrix.x++;
                break;
        }

        if (direction == EDirection.UP || direction == EDirection.DOWN)
        {
            if (barricadesHorizontalObjs.ContainsKey(targetMatrix))
                return true;
            return false;
        }

        if (barricadesVerticalObjs.ContainsKey(targetMatrix))
            return true;
        return false;
    }

    public void Down()
    {
        preViewMap_leftTopTile_matrix.y += 9;
        SetCameraPosition();
    }

    public void Up()
    {
        preViewMap_leftTopTile_matrix.y -= 9;
        SetCameraPosition();
    }

    public void Right()
    {
        preViewMap_leftTopTile_matrix.x += 9;
        SetCameraPosition();
    }

    public void Left()
    {
        preViewMap_leftTopTile_matrix.x -= 9;
        SetCameraPosition();
    }

    public void DropTest()
    {
        if (CheckDrop()) Drop();
    }

    public void CreateBlockTest()
    {
        if (IsAbleToCreateBlock()) CreateBlock();
    }

    public void ShuffleTest()
    {
        SetShuffleTile();
    }

    public void GoNextMap()
    {
        if (!IsMultiMap()) return;

        preMapNumber++;
        var x = multimapOrder[preMapNumber] / 10;
        var y = multimapOrder[preMapNumber] - x * 10;

        preViewMap_leftTopTile_matrix.x = x * 9;
        preViewMap_leftTopTile_matrix.y = y * 9;
        SetCameraPosition();
    }

    public void StartMultiCameraWork()
    {
        IsMultiCameraWorkEnd = false;
        StartCoroutine(MultiMapStartCoroutine());
    }

    private IEnumerator MultiMapStartCoroutine()
    {
        var lastNumber = multimapOrder.Length - 1;
        var x = multimapOrder[lastNumber] / 10;
        var y = multimapOrder[lastNumber] - x * 10;
        var cameraPosition = tileStartPosition + new Vector2((x * 9 + 4) * tileSize.x,
            -(y * 9 + 2) * tileSize.y - 0.6f);
        mainCamera.SetPosition(cameraPosition);

        for (var i = multimapOrder.Length - 1; i >= 0; i--)
        {
            x = multimapOrder[i] / 10;
            y = multimapOrder[i] - x * 10;

            cameraPosition = tileStartPosition + new Vector2((x * 9 + 4) * tileSize.x,
                -(y * 9 + 2) * tileSize.y - 0.6f);
            mainCamera.Move(cameraPosition, 1f);
            yield return new WaitUntil(() => !CameraWork.IsMoving);
        }

        IsMultiCameraWorkEnd = true;
    }

    public bool IsTabletInPreViewTile()
    {
        foreach (var item in tablets)
            if (item != null)
                if (IsPreViewTile(tileDictionary[item.LeftTopPosition]))
                    return true;
        return false;
    }

    public bool IsBigObjInPreViewTile(EID bigID)
    {
        foreach (var item in bigObjectDictionary)
            if (IsPreViewTile(tileDictionary[item.Key.LeftTopPosition]))
                if (item.Key.ID == bigID)
                    return true;
        return false;
    }

    public bool IsMultiMap()
    {
        if (multimapOrder.Length > 1) return true;
        return false;
    }

    public void ClickBlock(GameBlock block)
    {
        if (LogicManager.GetInstance != null)
            if (LogicManager.GetInstance.GetStateName() != null)
                if (!LogicManager.GetInstance.GetStateName().Equals("WAITUSERINPUT"))
                    return;

        switch (StageManager.GetInstance.SelectItem)
        {
            case EUseItem.HAMMER:
                if (block.Tile != null && block.Property.popProperty.Break)
                    if (block.Tile)
                    {
                        if (PlayerData.GetInstance != null)
                        {
                            PlayerData.GetInstance.ItemHammer--;
                            if (ChallengeSystem.GetInstance != null)
                            {
                                if (ChallengeSystem.GetInstance.IsChallengeStage)
                                {
                                    FirebaseManager.GetInstance.FirebaseLogEvent(string.Format($"{StageManager.StageNumber}_hardmode_1item_use"));
                                }
                            }
                            StageManager.GetInstance.SetUI();
                        }

                        //_isItemSelect = true;
                        //ItemPresentationManager.GetInstance.ShowHammer(block.Matrix);
                        AnimationManager.AnimCount++;
                        AnimationManager.GetInstance.ShowHammer(block);
                        StartCoroutine(ItemCoroutine(block, EUseItem.HAMMER));
                        //FirebaseManager.GetInstance.FirebaseLogEvent("Item1_Use");
                    }

                break;

            case EUseItem.CROSS:
                if (block.Tile != null)
                {
                    if (PlayerData.GetInstance != null)
                    {
                        PlayerData.GetInstance.ItemCross--;
                        StageManager.GetInstance.SetUI();
                    }

                    AnimationManager.AnimCount++;
                    AnimationManager.GetInstance.ShowCross(block);

                    StartCoroutine(ItemCoroutine(block, EUseItem.CROSS));
                    //FirebaseManager.GetInstance.FirebaseLogEvent("Item2_Use");
                }

                break;

            case EUseItem.BOMB:
                if (block.Tile != null)
                {
                    if (PlayerData.GetInstance != null)
                    {
                        PlayerData.GetInstance.ItemBomb--;
                        if (ChallengeSystem.GetInstance != null)
                        {
                            if (ChallengeSystem.GetInstance.IsChallengeStage)
                            {
                                FirebaseManager.GetInstance.FirebaseLogEvent(string.Format($"{StageManager.StageNumber}_hardmode_2item_use"));
                            }
                        }
                        StageManager.GetInstance.SetUI();
                    }

                    AnimationManager.AnimCount++;
                    AnimationManager.GetInstance.ShowCircle(block);
                    StartCoroutine(ItemCoroutine(block, EUseItem.BOMB));
                    //FirebaseManager.GetInstance.FirebaseLogEvent("Item3_Use");
                }

                break;

            case EUseItem.COLOR:
                if (block.Tile != null &&
                    (block.ID == EID.NORMAL || block.ID == EID.HORIZONTAL || block.ID == EID.VERTICAL ||
                     block.ID == EID.RHOMBUS || block.ID == EID.X || block.ID == EID.COLOR_BOMB || block.ID == EID.FISH))
                {
                    if (PlayerData.GetInstance != null)
                    {
                        PlayerData.GetInstance.ItemColor--;
                        if (ChallengeSystem.GetInstance != null)
                        {
                            if (ChallengeSystem.GetInstance.IsChallengeStage)
                            {
                                FirebaseManager.GetInstance.FirebaseLogEvent(string.Format($"{StageManager.StageNumber}_hardmode_3item_use"));
                            }
                        }
                        StageManager.GetInstance.SetUI();
                    }

                    AnimationManager.AnimCount++;
                    AnimationManager.GetInstance.ShowRainbow(block);
                    StartCoroutine(ItemCoroutine(block, EUseItem.COLOR));
                    //FirebaseManager.GetInstance.FirebaseLogEvent("Item4_Use");
                }

                break;
        }
    }

    public bool CheckRelicMultimap(EID relicID)
    {
        if (relicID != EID.DROP_RELIC1 && relicID != EID.DROP_RELIC2 && relicID != EID.DROP_RELIC3) return false;

        if (preMapNumber >= multimapOrder.Length - 1) return false;
        var reviseValue = 4;

        var x = preViewMap_leftTopTile_matrix.x;
        var y = preViewMap_leftTopTile_matrix.y;

        var nextX = multimapOrder[preMapNumber + 1] / 10;
        var nextY = multimapOrder[preMapNumber + 1] - nextX * 10;

        var isY = false;
        var isX = false;
        if (nextY - y != 0) isY = true;
        if (nextX - x != 0) isX = true;

        var relics = BlockManager.GetInstance.GetBlocksToUseID(relicID);
        var relicMatrices = new List<Vector2Int>();

        foreach (var relic in relics)
            if (IsPreViewTile(relic.Tile))
                relicMatrices.Add(relic.Tile.Matrix);

        if (isY)
        {
            var isDown = true;
            if (nextY * 9 - y < 0) isDown = false;

            if (isDown)
            {
                var maxY = -1;
                foreach (var item in relicMatrices)
                    if (maxY == -1 || maxY > item.y)
                        maxY = item.y;

                if (maxY > preViewMap_leftTopTile_matrix.y + 9 - reviseValue)
                {
                    preViewMap_leftTopTile_matrix.y += maxY - reviseValue + 2;

                    if (preViewMap_leftTopTile_matrix.y >= nextY * 9) return true;

                    SetCameraPosition();
                    return false;
                }
            }
            else
            {
                var minY = -1;
                foreach (var item in relicMatrices)
                    if (minY == -1 || minY < item.y)
                        minY = item.y;

                if (minY < preViewMap_leftTopTile_matrix.y + reviseValue)
                {
                    preViewMap_leftTopTile_matrix.y = minY - reviseValue - 2;

                    if (preViewMap_leftTopTile_matrix.y <= nextY * 9) return true;

                    SetCameraPosition();
                    return false;
                }
            }
        }

        if (isX)
        {
            var isRight = true;
            if (nextX * 9 - x < 0) isRight = false;

            if (isRight)
            {
                var maxX = -1;
                foreach (var item in relicMatrices)
                    if (maxX == -1 || maxX > item.x)
                        maxX = item.x;

                if (maxX > preViewMap_leftTopTile_matrix.x + 9 - reviseValue)
                {
                    preViewMap_leftTopTile_matrix.x = maxX - reviseValue + 2;

                    if (preViewMap_leftTopTile_matrix.x >= nextX * 9) return true;

                    SetCameraPosition();
                    return false;
                }
            }
            else
            {
                var minX = -1;
                foreach (var item in relicMatrices)
                    if (minX == -1 || minX < item.x)
                        minX = item.x;

                if (minX < preViewMap_leftTopTile_matrix.x + reviseValue)
                {
                    preViewMap_leftTopTile_matrix.x = minX - reviseValue - 2;

                    if (preViewMap_leftTopTile_matrix.x <= nextX * 9)
                    {
                        return true;
                    }

                    SetCameraPosition();
                    return false;
                }
            }
        }

        return false;
    }

    private IEnumerator ItemCoroutine(GameBlock block, EUseItem useItem)
    {
        //ItemEnd = false;
        /*
        if (StageManager.GetInstance != null)
        {
            GameObject PopupManager = StageManager.GetInstance.PopupManager;
            for (int i = PopupManager.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(PopupManager.transform.GetChild(i).gameObject);
            }
        }
        */

        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("CashBlockSelect");
        StageManager.GetInstance.CancelItem();
        StageManager.GetInstance.CloseItemPopup();

        yield return new WaitUntil(() => AnimationManager.AnimCount == 0);
        if (block == null) yield break;
        if (block.Tile == null) yield break;
        if (!tileDictionary.ContainsKey(block.Tile.Matrix)) yield break;

        var tile = tileDictionary[block.Tile.Matrix];
        switch (useItem)
        {
            case EUseItem.HAMMER:
                if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("UseHammer");
                block.Tile.BombPop();
                break;

            case EUseItem.CROSS:
                BlockManager.GetInstance.MoneyItemJamApply(block.Tile);
                if (block.ID == EID.RHOMBUS)
                    block.Tile.RegisterCombinePop(ECombine.BIG_CROSS, block.Color);
                else
                    block.Tile.RegisterCombinePop(ECombine.CROSS, block.Color);
                //tile.RemoveBlock(block);
                // block.BombPop();
                break;

            case EUseItem.BOMB:
                BlockManager.GetInstance.MoneyItemJamApply(block.Tile);
                block.Tile.RegisterCombinePop(ECombine.BIG_RHOMBUS, block.Color);
                //tile.RemoveBlock(block);
                //block.BombPop();
                break;

            case EUseItem.COLOR:
                BlockManager.GetInstance.MoneyItemJamApply(block.Tile);
                var rainColor = block.Color;
                if (block.ID == EID.NORMAL)
                {
                    tile.RegisterCombinePop(ECombine.RAINBOW_NORMAL, rainColor);
                }
                else if (block.ID == EID.HORIZONTAL || block.ID == EID.VERTICAL)
                {
                    tile.RegisterCombinePop(ECombine.RAINBOW_DIRECTION, rainColor);
                }
                else if (block.ID == EID.RHOMBUS)
                {
                    tile.RegisterCombinePop(ECombine.RAINBOW_RHOMBUS, rainColor);
                }
                else if (block.ID == EID.X)
                {
                    tile.RegisterCombinePop(ECombine.RAINBOW_X, rainColor);
                }
                else if (block.ID == EID.FISH)
                {
                    tile.RegisterCombinePop(ECombine.RAINBOW_FISH, rainColor);
                }
                else if (block.ID == EID.COLOR_BOMB)
                {
                    block.Remove();
                    block.Clear();
                    tile.RemoveBlock(block);
                    tile.RegisterCombinePop(ECombine.RAINBOW_RAINBOW, rainColor);
                }

                break;
        }

        yield return new WaitUntil(() => AnimationManager.AnimCount == 0);
        if (useItem == EUseItem.BOMB)
            if (block != null)
                if (block.Tile != null)
                    block.Tile.BombPop();
        yield return new WaitUntil(() => AnimationManager.RainbowCount == 0);

        yield return new WaitUntil(() => !IsBombing());

        LogicManager.GetInstance.ChangeLogicState(new DropLogic());
    }

    public void ShowGravity()
    {
        StartCoroutine(CreateGravityCoroutine());
    }

    private IEnumerator CreateGravityCoroutine()
    {
        var tiles = new List<GameTile>();
        foreach (var tile in startTiles) tiles.Add(tile);

        var iter = true;
        while (iter)
        {
            var tempTiles = new List<GameTile>();
            foreach (var item in tiles) tempTiles.Add(item);
            tiles.Clear();

            foreach (var item in tempTiles)
            {
                var obj = DynamicObjectPool.GetInstance.GetObjectForType("GravityObj", false);
                var obTransform = obj.GetComponent<Transform>();
                obTransform.position = item.GetPosition();

                switch (item.DropDirection)
                {
                    case EDirection.RIGHT:
                        obTransform.rotation = Quaternion.Euler(0, 0, 90);
                        break;

                    case EDirection.LEFT:
                        obTransform.rotation = Quaternion.Euler(0, 0, 270);
                        break;

                    case EDirection.UP:
                        obTransform.rotation = Quaternion.Euler(0, 0, 180);
                        break;

                    case EDirection.DOWN:
                        obTransform.rotation = Quaternion.Euler(0, 0, 0);
                        break;
                }

                //if (item.TileKind != ETileKind.END)
                {
                    var nextTile = GetNextTileOrNull(item);
                    if (nextTile != null)
                    {
                        while (nextTile.TileKind == ETileKind.VOID)
                        {
                            nextTile = GetConnectedTileOrNull(nextTile.Matrix, nextTile.DropDirection);
                            if (nextTile == null) break;
                        }

                        if (nextTile != null) tiles.Add(nextTile);
                    }
                }
            }

            if (tiles.Count == 0) iter = false;

            yield return new WaitForSeconds(0.15f);
        }
    }

    public GameTile GetTileOrNull(Vector2Int tilePosition)
    {
        if (tileDictionary.ContainsKey(tilePosition))
            return tileDictionary[tilePosition];
        return null;
    }

    public void SwapReset()
    {
        foreach (var item in tileDictionary)
            if (item.Value.IsSwap)
                item.Value.IsSwap = false;
    }

    public void SetSpecialHighlight(bool value, Vector2Int matrix)
    {
        foreach (var barricade in barricadesVerticalObjs)
        {
            barricade.Value.GetComponent<Highlight>().Init();
            barricade.Value.GetComponent<Highlight>().SetHighlight(value);
        }

        foreach (var barricade in barricadesHorizontalObjs)
        {
            barricade.Value.GetComponent<Highlight>().Init();
            barricade.Value.GetComponent<Highlight>().SetHighlight(value);
        }

        foreach (var tunnel in tunnels)
        {
            tunnel.GetComponent<Highlight>().Init();
            tunnel.GetComponent<Highlight>().SetHighlight(value);
        }

        foreach (var rail in rails)
            if (rail.RailProperty.matrix == matrix)
                rail.SetHighlight(value);

        foreach (var item in bigObjectDictionary)
            if (item.Key.LeftTopPosition == matrix)
                item.Key.SetHighlight(value);

        foreach (var gear in gears) gear.SetHighlight(matrix, value);
    }
}