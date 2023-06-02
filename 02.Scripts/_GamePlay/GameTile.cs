using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AdvancedFPSCounter.CountersData;
using UnityEngine;

public struct DirectionTile
{
    public EDirection direction;
    public GameTile tile;
    public int count;

    public DirectionTile(EDirection _direction, GameTile _tile, int _count)
    {
        direction = _direction;
        tile = _tile;
        count = _count;
    }
}

public class GameTile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;

    public UnityEngine.SpriteRenderer TileRenderer
    {
        get { return _renderer; }
        set { _renderer = value; }
    }

    [SerializeField] private GameObject[] lines;

    public bool IsSpecialTile;
    public int SpecialTileValue = -1;

    public bool IsRelicTarget;
    private readonly List<GameBlock> blocks = new List<GameBlock>();

    private readonly List<List<GameTile>> bombPopList = new List<List<GameTile>>();
    private int bomingCount;
    private readonly Dictionary<EDirection, GameTile> connectTiles = new Dictionary<EDirection, GameTile>();

    private readonly Dictionary<DirectionTile, bool> jamTiles = new Dictionary<DirectionTile, bool>();
    private readonly Dictionary<List<GameTile>, bool> rocketJamTiles = new Dictionary<List<GameTile>, bool>();

    private readonly Dictionary<DirectionTile, bool> shieldDirectionTiles = new Dictionary<DirectionTile, bool>();
    private readonly Dictionary<List<GameTile>, bool> shieldTileLists = new Dictionary<List<GameTile>, bool>();

    private readonly Tile tile = new Tile();

    public Vector2Int Matrix => tile.Matrix;

    public GameBlock NormalBlock
    {
        get
        {
            foreach (var block in blocks)
                if (block.Property.depth == EDepth.NORMAL)
                    return block;
            return null;
        }
    }

    public ETileKind TileKind => tile.TileKind;
    public EDirection DropDirection => tile.DropDirection;
    public EDirection ReverseDirection { get; private set; } = EDirection.UP;

    public EDirection SideDropDirection { get; set; }
    public GameTile EntranceTile { get; private set; }

    public GameTile ExitTile { get; private set; }

    public bool IsDamagedThisTurn { get; set; }

    public bool IsSideDrop { get; set; }
    public bool IsDroppedBlock { get; set; }
    public bool IsBombActive { get; set; }

    public bool IsBombing
    {
        get
        {
            if (bomingCount <= 0)
                return false;
            return true;
        }
        set
        {
            if (value)
            {
                bomingCount++;
            }
            else
            {
                bomingCount--;
                bomingCount = Mathf.Max(0, bomingCount);
            }
        }
    }

    public bool IsDoubleRainbow { get; set; }
    public bool IsSwap { get; set; }

    public void Setting(Vector2Int _matrix, ETileKind _tileKind, EDirection _dropDirection)
    {
        IsBombActive = false;
        tile.Setting(_matrix, _tileKind, _dropDirection);
        IsBombing = false;
        bomingCount = 0;
        IsSideDrop = false;
        IsDamagedThisTurn = false;
        IsSwap = false;
        ApplyImage();

        switch (_dropDirection)
        {
            case EDirection.DOWN:
                ReverseDirection = EDirection.UP;
                break;

            case EDirection.UP:
                ReverseDirection = EDirection.DOWN;
                break;

            case EDirection.RIGHT:
                ReverseDirection = EDirection.LEFT;
                break;

            case EDirection.LEFT:
                ReverseDirection = EDirection.RIGHT;
                break;
        }
    }

    public void LineSetting()
    {
        for (var i = 0; i < lines.Length; i++) lines[i].SetActive(false);

        if (TileKind != ETileKind.VOID && TileKind != ETileKind.LADDER)
        {
            var NoUp = false;
            var NoDown = false;
            var NoLeft = false;
            var NoRight = false;

            if (connectTiles.ContainsKey(EDirection.UP))
            {
                if (connectTiles[EDirection.UP].TileKind == ETileKind.VOID ||
                    connectTiles[EDirection.UP].TileKind == ETileKind.LADDER)
                {
                    lines[(int)EDirection.UP].SetActive(true);
                    NoUp = true;
                }
            }
            else
            {
                lines[(int)EDirection.UP].SetActive(true);
                NoUp = true;
            }

            if (connectTiles.ContainsKey(EDirection.DOWN))
            {
                if (connectTiles[EDirection.DOWN].TileKind == ETileKind.VOID ||
                    connectTiles[EDirection.DOWN].TileKind == ETileKind.LADDER)
                {
                    lines[(int)EDirection.DOWN].SetActive(true);
                    NoDown = true;
                }
            }
            else
            {
                lines[(int)EDirection.DOWN].SetActive(true);
                NoDown = true;
            }

            if (connectTiles.ContainsKey(EDirection.LEFT))
            {
                if (connectTiles[EDirection.LEFT].TileKind == ETileKind.VOID ||
                    connectTiles[EDirection.LEFT].TileKind == ETileKind.LADDER)
                {
                    lines[(int)EDirection.LEFT].SetActive(true);
                    NoLeft = true;
                }
            }
            else
            {
                lines[(int)EDirection.LEFT].SetActive(true);
                NoLeft = true;
            }

            if (connectTiles.ContainsKey(EDirection.RIGHT))
            {
                if (connectTiles[EDirection.RIGHT].TileKind == ETileKind.VOID ||
                    connectTiles[EDirection.RIGHT].TileKind == ETileKind.LADDER)
                {
                    lines[(int)EDirection.RIGHT].SetActive(true);
                    NoRight = true;
                }
            }
            else
            {
                lines[(int)EDirection.RIGHT].SetActive(true);
                NoRight = true;
            }

            if (NoUp && NoLeft) lines[(int)EDirection.UP_LEFT].SetActive(true);
            if (NoUp && NoRight) lines[(int)EDirection.UP_RIGHT].SetActive(true);
            if (NoDown && NoLeft) lines[(int)EDirection.DOWN_LEFT].SetActive(true);
            if (NoDown && NoRight) lines[(int)EDirection.DOWN_RIGHT].SetActive(true);

            if (BaseSystem.GetInstance != null)
            {
                if (BaseSystem.GetInstance.GetSystemList("CircusSystem") || BaseSystem.GetInstance.GetSystemList("Fantasy"))
                {
                    SetTileCircus();
                }
            }

            if (MapEditor.GetInstance == null)
                foreach (var line in lines)
                    if (line != null)
                        if (!line.activeSelf)
                            DestroyImmediate(line);
        }
    }

    public void SetTileCircus()
    {
        int numX, numY, numMinusY, numMinusX = 0;
        Vector3 xPositionVector3 = new Vector3(0.07f, 0f, 0f);
        Vector3 yPositionVector3 = new Vector3(0f, 0.07f, 0f);
        Vector3 xScaleVector3 = new Vector3(16.5f, 1f, 1f);
        Vector3 yScaleVector3 = new Vector3(1f, 16.5f, 1f);

        for (int i = 4; i < lines.Length; i++)
        {
            if (i * 2 < 11)
            {
                numX = 0;
                numMinusY = -1;
            }
            else
            {
                numX = 3;
                numMinusY = 1;
            }

            if (i % 2 == 0)
            {
                numY = 1;
                numMinusX = 1;
            }
            else
            {
                numY = 2;
                numMinusX = -1;
            }

            if (lines[i].gameObject.activeSelf)
            {
                lines[numX].transform.localPosition += numMinusX * xPositionVector3;
                lines[numX].transform.localScale = xScaleVector3;
                lines[numY].transform.localPosition += numMinusY * yPositionVector3;
                lines[numY].transform.localScale = yScaleVector3;
            }
        }
        /*
        if (lines[4].gameObject.activeSelf)
        {
            lines[0].transform.localPosition += xPositionVector3;
            lines[0].transform.localScale = xScaleVector3;
            lines[1].transform.localPosition += -1 * yPositionVector3;
            lines[1].transform.localScale = yScaleVector3;
        }
        if (lines[5].gameObject.activeSelf)
        {
            lines[0].transform.localPosition += -1 * xPositionVector3;
            lines[0].transform.localScale = xScaleVector3;
            lines[2].transform.localPosition += -1 * yPositionVector3;
            lines[2].transform.localScale = yScaleVector3;
        }
        if (lines[6].gameObject.activeSelf)
        {
            lines[3].transform.localPosition += xPositionVector3;
            lines[3].transform.localScale = xScaleVector3;
            lines[1].transform.localPosition += yPositionVector3;
            lines[1].transform.localScale = yScaleVector3;
        }
        if (lines[7].gameObject.activeSelf)
        {
            lines[3].transform.localPosition += -1 * xPositionVector3;
            lines[3].transform.localScale = xScaleVector3;
            lines[2].transform.localPosition += yPositionVector3;
            lines[2].transform.localScale = yScaleVector3;
        }
        */
    }

    public void SetEntranceTunnel(GameTile entranceGameTile)
    {
        EntranceTile = entranceGameTile;
    }

    public void SetExitTunnel(GameTile exitGameTile)
    {
        ExitTile = exitGameTile;
    }

    private void ApplyImage()
    {
        //_renderer = this.GetComponent<SpriteRenderer>();
        int cellNumber = (tile.Matrix.x + tile.Matrix.y) % 2;

        if (tile.TileKind != ETileKind.VOID && tile.TileKind != ETileKind.LADDER)
        {
            if (cellNumber == 0)
            {
                _renderer.sprite = TileManager.GetInstance.GetTileSpriteOrNull(tile.TileKind);
            }
            else
            {
                _renderer.sprite = TileManager.GetInstance.GetTileSpriteOrNull(ETileKind.NORMAL1);
            }
        }
        else
        {
            _renderer.sprite = TileManager.GetInstance.GetTileSpriteOrNull(tile.TileKind);
        }

        if (tile.TileKind == ETileKind.LADDER &&
            (tile.DropDirection == EDirection.LEFT || tile.DropDirection == EDirection.RIGHT))
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        else
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

        foreach (var block in blocks)
        {
        }
    }

    public void ConnectTile(GameTile tile, EDirection direction)
    {
        if (connectTiles.ContainsKey(direction))
            return;
        connectTiles.Add(direction, tile);
    }

    public GameTile GetConnectTileOrNull(EDirection direction)
    {
        if (connectTiles.ContainsKey(direction))
            return connectTiles[direction];
        return null;
    }

    public Vector3 GetPosition()
    {
        return transform.localPosition;
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    public GameBlock GetTopBlockOrNull()
    {
        GameBlock topBlock = null;

        foreach (var block in blocks)
            if (block.Property.depth == EDepth.TOP)
                topBlock = block;

        return topBlock;
    }

    public bool IsPopAble()
    {
        //TODO
        /*
        if (normalBlock == null) return false;
        if (normalBlock.IsDropAble()) return false;
        foreach (var block in blocks)
        {
        }
        */

        foreach (var block in blocks)
        {
            if (!block.Property.popProperty.AnotherPop
                && !block.Property.popProperty.ColorPop) return false;

            if (block.IsMoving) return false;
            if (block.isDropMoving) return false;
        }

        foreach (var block in blocks)
            if (block.Property.popProperty.ColorPop && !block.IsCombine)
                return true;

        return true;
    }

    public void Pop()
    {
        //TODO
        /*
        if (normalBlock != null)
        {
            normalBlock.Pop();
        }
        */
    }

    public void SidePop(EColor color, EDirection direction)
    {
        if (IsDamagedThisTurn) return;
        //TODO: 높낮이에 따라서 우선 순위 지정 필요
        GameBlock topBlock = null;
        foreach (var block in blocks)
            if (topBlock == null)
            {
                topBlock = block;
            }
            else
            {
                if ((int)topBlock.Property.depth < (int)block.Property.depth) topBlock = block;
            }

        if (topBlock != null)
        {
            if (topBlock.isUseSp != null) topBlock.isUseSp.BombPop();
            topBlock.SidePop(color, direction);
        }
    }

    public void RegisterFloorPop()
    {
        GameBlock floor = null;
        foreach (var block in blocks)
            if (block.Property.depth == EDepth.FLOOR
                && block.Property.popProperty.Break)
                floor = block;
        if (floor != null) floor.BombPop();
    }

    public void RegisterSidePop(EColor _color)
    {
        if (connectTiles.ContainsKey(EDirection.DOWN)) connectTiles[EDirection.DOWN].SidePop(_color, EDirection.DOWN);
        if (connectTiles.ContainsKey(EDirection.LEFT)) connectTiles[EDirection.LEFT].SidePop(_color, EDirection.LEFT);
        if (connectTiles.ContainsKey(EDirection.RIGHT))
            connectTiles[EDirection.RIGHT].SidePop(_color, EDirection.RIGHT);
        if (connectTiles.ContainsKey(EDirection.UP)) connectTiles[EDirection.UP].SidePop(_color, EDirection.UP);
    }

    public void RegisterBombSidePop()
    {
        if (connectTiles.ContainsKey(EDirection.DOWN)) connectTiles[EDirection.DOWN].BombSidePop();
        if (connectTiles.ContainsKey(EDirection.LEFT)) connectTiles[EDirection.LEFT].BombSidePop();
        if (connectTiles.ContainsKey(EDirection.RIGHT)) connectTiles[EDirection.RIGHT].BombSidePop();
        if (connectTiles.ContainsKey(EDirection.UP)) connectTiles[EDirection.UP].BombSidePop();
    }

    public bool IsBombPopAble()
    {
        GameBlock highestBlock = null;
        foreach (var block in blocks)
            if (highestBlock == null)
            {
                highestBlock = block;
            }
            else
            {
                if ((int)highestBlock.Property.depth < (int)block.Property.depth) highestBlock = block;
            }

        if (highestBlock != null) return highestBlock.Property.popProperty.BombPop;

        return false;
    }

    public void BombSidePop()
    {
        if (IsDamagedThisTurn) return;

        GameBlock highestBlock = null;

        foreach (var block in blocks)
            if (highestBlock == null)
            {
                highestBlock = block;
            }
            else
            {
                if ((int)highestBlock.Property.depth < (int)block.Property.depth) highestBlock = block;
            }

        if (highestBlock != null) highestBlock.BombSidePop();
    }

    public void Clear()
    {
        //StopAllCoroutines();
        bombPopList.Clear();
        blocks.Clear();
        connectTiles.Clear();
        IsDamagedThisTurn = false;
        //normalBlock = null;
    }

    public void RegisterJamPop()
    {
        if (TileKind == ETileKind.VOID || TileKind == ETileKind.LADDER) return;
        foreach (var block in blocks)
        {
            if (block.ID == EID.JAM)
                return;
            if (BlockManager.GetInstance.IsJamHater(block.ID))
                return;
        }

        var jam = BlockManager.GetInstance.GetGameBlockOrNull(EID.JAM);

        if (jam != null)
        {
            jam.Init();
            jam.Setting(EColor.NONE, EID.JAM);
            jam.SetProperty(DataContainer.GetInstance.GetObstacleDataOrNULL(EID.JAM));
            jam.ApplySprite();
            jam.SetTile(this);
            jam.SetPosition();

            StageManager.GetInstance.CollectMission(EID.JAM, EColor.NONE, transform);
            if (SoundManager.GetInstance != null)
                SoundManager.GetInstance.Play("Jam");
        }
    }

    public void RegisterBombPop(GameBlock block)
    {
        if (IsBombing) return;

        if (block.ID != EID.FISH)
        {
            bomingCount++;
            AnimationManager.GetInstance.ShowBomb(block.ID, block.Color, this);
            RegisterBombStartTile(block.ID);
            StartCoroutine(BombCoroutine());
        }
        else
        {
            BlockManager.GetInstance.ActiveAutoTargetBomb(this, block.Color, ECombine.NONE);
        }
    }

    public void RegisterBombPop(EID id, EColor color)
    {
        if (IsBombing) return;

        if (id != EID.FISH)
        {
            bomingCount++;
            AnimationManager.GetInstance.ShowBomb(id, color, this);
            RegisterBombStartTile(id);
            StartCoroutine(BombCoroutine());
        }
        else
        {
            BlockManager.GetInstance.ActiveAutoTargetBomb(this, color, ECombine.NONE);
        }
    }

    public void RegisterCombinePop(ECombine combine, EColor color)
    {
        var isJam = BlockManager.GetInstance.HasJamInCombine;

        switch (combine)
        {
            case ECombine.CROSS:
            case ECombine.ASTERISK:
            case ECombine.BIG_CROSS:
            case ECombine.BIG_X:
            case ECombine.X_X:
            case ECombine.BIG_RHOMBUS:
            case ECombine.ACTIVE_ASTERISK:
            case ECombine.ACTIVE_GREATBOMB:
            case ECombine.RAINBOW_RAINBOW:
                IsBombing = true;
                AnimationManager.GetInstance.ShowBomb(combine, color, this);
                RegisterBombStartTile(combine);
                StartCoroutine(BombCoroutine());
                break;

            case ECombine.RAINBOW_NORMAL:
                if (NormalBlock != null)
                    BlockManager.GetInstance.RegisterRainbow(color, EID.NORMAL, NormalBlock, isJam);
                break;

            case ECombine.RAINBOW_DIRECTION:
                if (NormalBlock != null)
                    BlockManager.GetInstance.RegisterRainbow(color, EID.HORIZONTAL, NormalBlock, isJam);
                break;

            case ECombine.RAINBOW_RHOMBUS:
                if (NormalBlock != null)
                    BlockManager.GetInstance.RegisterRainbow(color, EID.RHOMBUS, NormalBlock, isJam);
                break;

            case ECombine.RAINBOW_X:
                if (NormalBlock != null)
                    BlockManager.GetInstance.RegisterRainbow(color, EID.X, NormalBlock, isJam);
                break;

            case ECombine.RAINBOW_FISH:
                if (NormalBlock != null)
                    BlockManager.GetInstance.RegisterRainbow(color, EID.FISH, NormalBlock, isJam);
                break;

            case ECombine.FISH_FISH:
                BlockManager.GetInstance.ActiveAutoTargetBomb(this, color, ECombine.NONE);
                BlockManager.GetInstance.ActiveAutoTargetBomb(this, color, ECombine.NONE);
                BlockManager.GetInstance.ActiveAutoTargetBomb(this, color, ECombine.NONE);
                break;

            case ECombine.FISH_VERTICAL:
                BlockManager.GetInstance.ActiveAutoTargetBomb(this, color, ECombine.FISH_VERTICAL);
                break;

            case ECombine.FISH_HORIZONTAL:
                BlockManager.GetInstance.ActiveAutoTargetBomb(this, color, ECombine.FISH_HORIZONTAL);
                break;

            case ECombine.FISH_RHOMBUS:
                BlockManager.GetInstance.ActiveAutoTargetBomb(this, color, ECombine.FISH_RHOMBUS);
                break;

            case ECombine.FISH_X:
                BlockManager.GetInstance.ActiveAutoTargetBomb(this, color, ECombine.FISH_X);
                break;

            default:
                return;
        }
    }

    public void BombPop()
    {
        //TODO
        //if (IsBombActive) return;
        //IsBombActive = true;
        if (IsDamagedThisTurn) return;

        if (IsDoubleRainbow) IsDoubleRainbow = false;

        GameBlock highestBlock = null;

        foreach (var block in blocks)
            if (highestBlock == null)
            {
                highestBlock = block;
            }
            else
            {
                if ((int)highestBlock.Property.depth < (int)block.Property.depth) highestBlock = block;
            }

        if (highestBlock != null)
        {
            highestBlock.BombPop();
        }
        else
        {
            if (SoundManager.GetInstance != null)
            {
                SoundManager.GetInstance.Play("BlockDestroy");
            }
        }
    }

    public void Swap(EDirection _direction)
    {
        Debug.Log("[버그 추적] 스왑 시 이동 방향 : " + _direction);
        if (!IsSwapAble() || !BlockManager.GetInstance.IsSwapAble || IsSwap) return;
        if (!StageManager.GetInstance.IsSwapAbleInTuto(Matrix, _direction)) return;
        if (StageManager.GetInstance.PreMoveCount < 1) return;

        var objectTile = GetConnectTileOrNull(_direction);
        if (objectTile == null) return;

        if (!objectTile.IsSwapAble()) return;
        if (!TileManager.GetInstance.IsPreViewTile(objectTile)) return;

        if (SoundManager.GetInstance != null)
            SoundManager.GetInstance.Play("BlockSwap");

        var objectBlock = objectTile.NormalBlock;

        if (StageManager.GetInstance.IsTutorialActive() == false)
        {
            StageManager.GetInstance.MoveCountForRewardItemAD++;
        }

        objectBlock.Swap(this);
        NormalBlock.Swap(objectTile);
        IsSwap = true;
        objectTile.IsSwap = true;

        BlockManager.GetInstance.InputSwapBlock(NormalBlock, objectBlock);
    }

    public void RemoveBlock(GameBlock block)
    {
        //normalBlock = null;
        if (blocks.Contains(block)) blocks.Remove(block);
    }

    public void AddBlock(GameBlock newBlock)
    {
        GameBlock destroyBlock = null;

        foreach (var block in blocks)
            if (block.Property.depth == newBlock.Property.depth)
                destroyBlock = block;
        if (destroyBlock != null) blocks.Remove(destroyBlock);

        blocks.Add(newBlock);
    }

    public bool IsDropAble(EDirection direction = EDirection.NONE)
    {
        var returnValue = true;
        if (NormalBlock == null) return false;
        foreach (var block in blocks)
            if (!block.Property.moveProperty.AnotherDrop)
                return false;
        foreach (var block in blocks)
            if (block.Property.depth == EDepth.NORMAL)
                if (!block.Property.moveProperty.Drop)
                    return false;
        if (direction != EDirection.NONE && ExitTile == null)
        {
            var directionTile = GetConnectTileOrNull(direction);

            if (TileManager.GetInstance.CheckBarricade(Matrix, direction)) return false;
            if (directionTile == null) return false;
            while (directionTile.TileKind == ETileKind.LADDER)
                directionTile = directionTile.GetConnectTileOrNull(directionTile.DropDirection);
            if (!directionTile.IsDroppedAble()) return false;
        }

        if (ExitTile != null)
            if (!ExitTile.IsDroppedAble())
                return false;

        return returnValue;
    }

    public bool IsDroppedAble()
    {
        if (tile.TileKind == ETileKind.VOID || tile.TileKind == ETileKind.LADDER ||
            tile.TileKind == ETileKind.START) return false;

        if (blocks.Count == 0) return true;

        foreach (var block in blocks)
            if (!block.Property.moveProperty.AnotherDrop)
                return false;
        foreach (var block in blocks)
            //if (block.IsDropAble()) return true;
            if (block.Property.depth == EDepth.NORMAL
                && !block.IsDropped)
                return false;

        return true;
    }

    public void Drop()
    {
        if (!IsDropAble()) return;

        if (NormalBlock == null) return;
        var direction = EDirection.NONE;
        if (IsSideDrop)
        {
            IsSideDrop = false;
            direction = SideDropDirection;
        }
        else
        {
            direction = DropDirection;
            if (!IsDropAble(direction)) return;
        }

        GameTile targetTile = null;
        GameBlock droppedBlock = null;

        if (ExitTile != null)
        {
            targetTile = ExitTile;

            foreach (var block in blocks)
                if (block.Property.depth == EDepth.NORMAL &&
                    block.IsDropAble())
                    if (block.EnterTunnel(targetTile))
                    {
                        droppedBlock = block;
                        break;
                    }

            if (droppedBlock != null) blocks.Remove(droppedBlock);
        }
        else
        {
            if (!connectTiles.ContainsKey(direction)) return;
            if (connectTiles[direction].TileKind == ETileKind.VOID) return;

            var isLadder = false;

            targetTile = connectTiles[direction];
            while (targetTile.TileKind == ETileKind.LADDER)
            {
                targetTile = targetTile.GetConnectTileOrNull(targetTile.DropDirection);
                isLadder = true;
            }

            foreach (var block in blocks)
                if (block.Property.depth == EDepth.NORMAL &&
                    block.IsDropAble())
                {
                    if (!isLadder)
                    {
                        if (block.Drop(targetTile))
                        {
                            droppedBlock = block;
                            break;
                        }
                    }
                    else
                    {
                        if (block.EnterTunnel(targetTile))
                        {
                            droppedBlock = block;
                            break;
                        }
                    }
                }

            if (droppedBlock != null) blocks.Remove(droppedBlock);
        }
    }

    public bool IsSwapAble(EDirection direction = EDirection.NONE)
    {
        var returnValue = false;

        if (blocks.Count == 0) return false;

        foreach (var block in blocks)
            if (block.Property.depth == EDepth.NORMAL
                && block.Property.moveProperty.Swap)
                returnValue = true;
        foreach (var block in blocks)
            if (!block.Property.moveProperty.AnotherSwap)
                return false;

        if (direction != EDirection.NONE) return IsSwapAbleToUseDirection(direction);

        return returnValue;
    }

    private bool IsSwapAbleToUseDirection(EDirection direction)
    {
        //TODO
        var returnValue = true;
        if (TileManager.GetInstance.CheckBarricade(Matrix, direction)) returnValue = false;
        return returnValue;
    }

    public bool IsCreateBlock()
    {
        var returnValue = true;
        /*
        if (normalBlock == null)
        {
            returnValue = true;
        }
        */
        foreach (var block in blocks)
        {
            if (block.Property.depth == EDepth.NORMAL) returnValue = false;
            if (block.ID == EID.JAIL) returnValue = false;
            if (block.ID == EID.INVISIBLE_BOX) returnValue = false;
            if (block.ID == EID.RELIC_IN_INVISIBLE_BOX) returnValue = false;
            if (block.ID == EID.ICE) returnValue = false;
            if (block.ID == EID.BANDAGE) returnValue = false;
        }

        return returnValue;
    }

    public void RegisterBombStartTile(EID id)
    {
        var startTiles = new List<DirectionTile>();
        var leftTile = GetConnectTileOrNull(EDirection.LEFT);
        var rightTile = GetConnectTileOrNull(EDirection.RIGHT);
        var downTile = GetConnectTileOrNull(EDirection.DOWN);
        var upTile = GetConnectTileOrNull(EDirection.UP);

        switch (id)
        {
            case EID.VERTICAL:
                startTiles.Add(new DirectionTile(EDirection.UP, this, 99));
                startTiles.Add(new DirectionTile(EDirection.DOWN, this, 99));
                break;

            case EID.HORIZONTAL:
                startTiles.Add(new DirectionTile(EDirection.LEFT, this, 99));
                startTiles.Add(new DirectionTile(EDirection.RIGHT, this, 99));
                break;

            case EID.RHOMBUS:
                if (SoundManager.GetInstance != null)
                    SoundManager.GetInstance.Play("BombCircle");

                startTiles.Add(new DirectionTile(EDirection.UP, this, 4));
                startTiles.Add(new DirectionTile(EDirection.DOWN, this, 4));
                startTiles.Add(new DirectionTile(EDirection.LEFT, this, 4));
                startTiles.Add(new DirectionTile(EDirection.RIGHT, this, 4));
                startTiles.Add(new DirectionTile(EDirection.UP_LEFT, this, 2));
                startTiles.Add(new DirectionTile(EDirection.UP_RIGHT, this, 2));
                startTiles.Add(new DirectionTile(EDirection.DOWN_LEFT, this, 2));
                startTiles.Add(new DirectionTile(EDirection.DOWN_RIGHT, this, 2));

                if (leftTile != null)
                {
                    startTiles.Add(new DirectionTile(EDirection.UP, leftTile, 3));
                    startTiles.Add(new DirectionTile(EDirection.DOWN, leftTile, 3));
                }

                if (rightTile != null)
                {
                    startTiles.Add(new DirectionTile(EDirection.UP, rightTile, 3));
                    startTiles.Add(new DirectionTile(EDirection.DOWN, rightTile, 3));
                }

                if (downTile != null)
                {
                    startTiles.Add(new DirectionTile(EDirection.LEFT, downTile, 3));
                    startTiles.Add(new DirectionTile(EDirection.RIGHT, downTile, 3));
                }

                if (upTile != null)
                {
                    startTiles.Add(new DirectionTile(EDirection.LEFT, upTile, 3));
                    startTiles.Add(new DirectionTile(EDirection.RIGHT, upTile, 3));
                }

                break;

            case EID.X:
                startTiles.Add(new DirectionTile(EDirection.UP_LEFT, this, 99));
                startTiles.Add(new DirectionTile(EDirection.UP_RIGHT, this, 99));
                startTiles.Add(new DirectionTile(EDirection.DOWN_LEFT, this, 99));
                startTiles.Add(new DirectionTile(EDirection.DOWN_RIGHT, this, 99));
                break;

            case EID.FIZZ:
                startTiles.Add(new DirectionTile(EDirection.UP, this, 2));
                startTiles.Add(new DirectionTile(EDirection.DOWN, this, 2));
                startTiles.Add(new DirectionTile(EDirection.LEFT, this, 2));
                startTiles.Add(new DirectionTile(EDirection.RIGHT, this, 2));
                startTiles.Add(new DirectionTile(EDirection.UP_LEFT, this, 2));
                startTiles.Add(new DirectionTile(EDirection.UP_RIGHT, this, 2));
                startTiles.Add(new DirectionTile(EDirection.DOWN_LEFT, this, 2));
                startTiles.Add(new DirectionTile(EDirection.DOWN_RIGHT, this, 2));
                break;

            case EID.FISH:

                break;
        }

        if (startTiles.Count != 0)
        {
            var isJam = false;
            if (IsFreeJam())
                isJam = true;

            foreach (var directionTile in startTiles)
            {
                if (jamTiles.ContainsKey(directionTile)) continue;

                jamTiles.Add(directionTile, isJam);
            }

            foreach (var directionTile in startTiles)
            {
                if (shieldDirectionTiles.ContainsKey(directionTile)) continue;

                shieldDirectionTiles.Add(directionTile, true);
            }

            RegisterBombTile(startTiles);
        }
    }

    public void RegisterBombStartTile(ECombine combine)
    {
        var startTiles = new List<DirectionTile>();

        var leftTile = GetConnectTileOrNull(EDirection.LEFT);
        var rightTile = GetConnectTileOrNull(EDirection.RIGHT);
        var downTile = GetConnectTileOrNull(EDirection.DOWN);
        var upTile = GetConnectTileOrNull(EDirection.UP);

        switch (combine)
        {
            case ECombine.CROSS:
                startTiles.Add(new DirectionTile(EDirection.UP, this, 99));
                startTiles.Add(new DirectionTile(EDirection.DOWN, this, 99));
                startTiles.Add(new DirectionTile(EDirection.LEFT, this, 99));
                startTiles.Add(new DirectionTile(EDirection.RIGHT, this, 99));
                break;

            case ECombine.ASTERISK:
                startTiles.Add(new DirectionTile(EDirection.UP, this, 99));
                startTiles.Add(new DirectionTile(EDirection.DOWN, this, 99));
                startTiles.Add(new DirectionTile(EDirection.LEFT, this, 99));
                startTiles.Add(new DirectionTile(EDirection.RIGHT, this, 99));
                startTiles.Add(new DirectionTile(EDirection.UP_LEFT, this, 99));
                startTiles.Add(new DirectionTile(EDirection.UP_RIGHT, this, 99));
                startTiles.Add(new DirectionTile(EDirection.DOWN_LEFT, this, 99));
                startTiles.Add(new DirectionTile(EDirection.DOWN_RIGHT, this, 99));
                break;

            case ECombine.ACTIVE_ASTERISK:
                startTiles.Add(new DirectionTile(EDirection.UP, this, 99));
                startTiles.Add(new DirectionTile(EDirection.DOWN, this, 99));
                startTiles.Add(new DirectionTile(EDirection.LEFT, this, 99));
                startTiles.Add(new DirectionTile(EDirection.RIGHT, this, 99));
                startTiles.Add(new DirectionTile(EDirection.UP_LEFT, this, 99));
                startTiles.Add(new DirectionTile(EDirection.UP_RIGHT, this, 99));
                startTiles.Add(new DirectionTile(EDirection.DOWN_LEFT, this, 99));
                startTiles.Add(new DirectionTile(EDirection.DOWN_RIGHT, this, 99));
                break;

            case ECombine.BIG_CROSS:
                startTiles.Add(new DirectionTile(EDirection.UP, this, 99));
                startTiles.Add(new DirectionTile(EDirection.DOWN, this, 99));
                startTiles.Add(new DirectionTile(EDirection.LEFT, this, 99));
                startTiles.Add(new DirectionTile(EDirection.RIGHT, this, 99));
                if (leftTile != null)
                {
                    startTiles.Add(new DirectionTile(EDirection.UP, leftTile, 99));
                    startTiles.Add(new DirectionTile(EDirection.DOWN, leftTile, 99));
                }

                if (rightTile != null)
                {
                    startTiles.Add(new DirectionTile(EDirection.UP, rightTile, 99));
                    startTiles.Add(new DirectionTile(EDirection.DOWN, rightTile, 99));
                }

                if (downTile != null)
                {
                    startTiles.Add(new DirectionTile(EDirection.RIGHT, downTile, 99));
                    startTiles.Add(new DirectionTile(EDirection.LEFT, downTile, 99));
                }

                if (upTile != null)
                {
                    startTiles.Add(new DirectionTile(EDirection.LEFT, upTile, 99));
                    startTiles.Add(new DirectionTile(EDirection.RIGHT, upTile, 99));
                }

                break;

            case ECombine.BIG_X:
                startTiles.Add(new DirectionTile(EDirection.UP, this, 1));
                startTiles.Add(new DirectionTile(EDirection.DOWN, this, 1));
                startTiles.Add(new DirectionTile(EDirection.LEFT, this, 1));
                startTiles.Add(new DirectionTile(EDirection.RIGHT, this, 1));
                startTiles.Add(new DirectionTile(EDirection.UP_LEFT, this, 99));
                startTiles.Add(new DirectionTile(EDirection.UP_RIGHT, this, 99));
                startTiles.Add(new DirectionTile(EDirection.DOWN_LEFT, this, 99));
                startTiles.Add(new DirectionTile(EDirection.DOWN_RIGHT, this, 99));
                if (leftTile != null)
                {
                    startTiles.Add(new DirectionTile(EDirection.UP_LEFT, leftTile, 99));
                    startTiles.Add(new DirectionTile(EDirection.DOWN_LEFT, leftTile, 99));
                }

                if (rightTile != null)
                {
                    startTiles.Add(new DirectionTile(EDirection.UP_RIGHT, rightTile, 99));
                    startTiles.Add(new DirectionTile(EDirection.DOWN_RIGHT, rightTile, 99));
                }

                if (downTile != null)
                {
                    startTiles.Add(new DirectionTile(EDirection.DOWN_LEFT, downTile, 99));
                    startTiles.Add(new DirectionTile(EDirection.DOWN_RIGHT, downTile, 99));
                }

                if (upTile != null)
                {
                    startTiles.Add(new DirectionTile(EDirection.UP_LEFT, upTile, 99));
                    startTiles.Add(new DirectionTile(EDirection.UP_RIGHT, upTile, 99));
                }

                break;

            case ECombine.BIG_RHOMBUS:
                startTiles.Add(new DirectionTile(EDirection.UP, this, 5));
                startTiles.Add(new DirectionTile(EDirection.DOWN, this, 5));
                startTiles.Add(new DirectionTile(EDirection.LEFT, this, 5));
                startTiles.Add(new DirectionTile(EDirection.RIGHT, this, 5));
                startTiles.Add(new DirectionTile(EDirection.UP_LEFT, this, 3));
                startTiles.Add(new DirectionTile(EDirection.UP_RIGHT, this, 3));
                startTiles.Add(new DirectionTile(EDirection.DOWN_LEFT, this, 3));
                startTiles.Add(new DirectionTile(EDirection.DOWN_RIGHT, this, 3));

                if (leftTile != null)
                {
                    startTiles.Add(new DirectionTile(EDirection.UP, leftTile, 4));
                    startTiles.Add(new DirectionTile(EDirection.DOWN, leftTile, 4));
                }

                if (rightTile != null)
                {
                    startTiles.Add(new DirectionTile(EDirection.UP, rightTile, 4));
                    startTiles.Add(new DirectionTile(EDirection.DOWN, rightTile, 4));
                }

                if (downTile != null)
                {
                    startTiles.Add(new DirectionTile(EDirection.LEFT, downTile, 4));
                    startTiles.Add(new DirectionTile(EDirection.RIGHT, downTile, 4));
                }

                if (upTile != null)
                {
                    startTiles.Add(new DirectionTile(EDirection.LEFT, upTile, 4));
                    startTiles.Add(new DirectionTile(EDirection.RIGHT, upTile, 4));
                }

                break;

            case ECombine.ACTIVE_GREATBOMB:
                startTiles.Add(new DirectionTile(EDirection.UP, this, 5));
                startTiles.Add(new DirectionTile(EDirection.DOWN, this, 5));
                startTiles.Add(new DirectionTile(EDirection.LEFT, this, 5));
                startTiles.Add(new DirectionTile(EDirection.RIGHT, this, 5));
                startTiles.Add(new DirectionTile(EDirection.UP_LEFT, this, 3));
                startTiles.Add(new DirectionTile(EDirection.UP_RIGHT, this, 3));
                startTiles.Add(new DirectionTile(EDirection.DOWN_LEFT, this, 3));
                startTiles.Add(new DirectionTile(EDirection.DOWN_RIGHT, this, 3));

                if (leftTile != null)
                {
                    startTiles.Add(new DirectionTile(EDirection.UP, leftTile, 4));
                    startTiles.Add(new DirectionTile(EDirection.DOWN, leftTile, 4));
                }

                if (rightTile != null)
                {
                    startTiles.Add(new DirectionTile(EDirection.UP, rightTile, 4));
                    startTiles.Add(new DirectionTile(EDirection.DOWN, rightTile, 4));
                }

                if (downTile != null)
                {
                    startTiles.Add(new DirectionTile(EDirection.LEFT, downTile, 4));
                    startTiles.Add(new DirectionTile(EDirection.RIGHT, downTile, 4));
                }

                if (upTile != null)
                {
                    startTiles.Add(new DirectionTile(EDirection.LEFT, upTile, 4));
                    startTiles.Add(new DirectionTile(EDirection.RIGHT, upTile, 4));
                }

                break;

            case ECombine.X_X:
                startTiles.Add(new DirectionTile(EDirection.UP, this, 99));
                startTiles.Add(new DirectionTile(EDirection.DOWN, this, 99));
                startTiles.Add(new DirectionTile(EDirection.LEFT, this, 99));
                startTiles.Add(new DirectionTile(EDirection.RIGHT, this, 99));
                startTiles.Add(new DirectionTile(EDirection.UP_LEFT, this, 99));
                startTiles.Add(new DirectionTile(EDirection.UP_RIGHT, this, 99));
                startTiles.Add(new DirectionTile(EDirection.DOWN_LEFT, this, 99));
                startTiles.Add(new DirectionTile(EDirection.DOWN_RIGHT, this, 99));
                if (upTile != null)
                {
                    var upupTile = upTile.GetConnectTileOrNull(EDirection.UP);
                    if (upupTile != null)
                    {
                        startTiles.Add(new DirectionTile(EDirection.RIGHT, upupTile, 1));
                        startTiles.Add(new DirectionTile(EDirection.LEFT, upupTile, 1));
                    }
                }

                if (downTile != null)
                {
                    var ddTile = downTile.GetConnectTileOrNull(EDirection.DOWN);
                    if (ddTile != null)
                    {
                        startTiles.Add(new DirectionTile(EDirection.RIGHT, ddTile, 1));
                        startTiles.Add(new DirectionTile(EDirection.LEFT, ddTile, 1));
                    }
                }

                if (rightTile != null)
                {
                    var rrTile = rightTile.GetConnectTileOrNull(EDirection.RIGHT);
                    if (rrTile != null)
                    {
                        startTiles.Add(new DirectionTile(EDirection.UP, rrTile, 1));
                        startTiles.Add(new DirectionTile(EDirection.DOWN, rrTile, 1));
                    }
                }

                if (leftTile != null)
                {
                    var llTile = leftTile.GetConnectTileOrNull(EDirection.LEFT);
                    if (llTile != null)
                    {
                        startTiles.Add(new DirectionTile(EDirection.UP, llTile, 1));
                        startTiles.Add(new DirectionTile(EDirection.DOWN, llTile, 1));
                    }
                }

                break;

            case ECombine.RAINBOW_RAINBOW:
                startTiles.Add(new DirectionTile(EDirection.NONE, this, 99));
                break;
        }

        if (startTiles.Count != 0)
        {
            var isJam = false;
            if (combine != ECombine.RAINBOW_RAINBOW)
            {
                if (IsFreeJam()) isJam = true;
                if (BlockManager.GetInstance.HasJamInCombine) isJam = true;

                foreach (var directionTile in startTiles)
                {
                    jamTiles.Add(directionTile, isJam);
                    shieldDirectionTiles.Add(directionTile, true);
                }

                RegisterBombTile(startTiles);
            }
            else
            {
                if (BlockManager.GetInstance.HasJamInCombine) isJam = true;
                if (isJam) jamTiles.Add(new DirectionTile(EDirection.NONE, this, 99), true);
                shieldDirectionTiles.Add(new DirectionTile(EDirection.NONE, this, 99), true);
                RegisterDoubleBombTile(startTiles);
            }
        }
    }

    public void RegisterBombTile(List<DirectionTile> dTiles)
    {
        var iter = true;

        var firstPopList = new List<GameTile>();
        var nextTiles = new List<DirectionTile>();

        foreach (var item in jamTiles)
        {
            var d = item.Key;
            if (d.direction != EDirection.NONE)
            {
                var tiles = new List<GameTile>();
                var t = d.tile;
                var count = d.count;

                while (iter)
                {
                    if (count <= 0) break;
                    if (t == null) break;
                    if (!TileManager.GetInstance.IsPreViewTile(t)) break;

                    tiles.Add(t);
                    t = t.GetConnectTileOrNull(d.direction);
                    count--;
                }

                rocketJamTiles.Add(tiles, item.Value);
            }
        }

        foreach (var item in shieldDirectionTiles)
        {
            var d = item.Key;
            if (d.direction != EDirection.NONE)
            {
                var tiles = new List<GameTile>();
                var t = d.tile;
                var count = d.count;

                while (iter)
                {
                    if (count <= 0) break;
                    if (t == null) break;
                    if (!TileManager.GetInstance.IsPreViewTile(t)) break;

                    tiles.Add(t);
                    t = t.GetConnectTileOrNull(d.direction);
                    count--;
                }

                shieldTileLists.Add(tiles, item.Value);
            }
        }

        foreach (var item in dTiles)
            if (item.count > 0)
            {
                var count = item.count - 1;
                var t = item.tile.GetConnectTileOrNull(item.direction);
                if (t != null)
                    if (TileManager.GetInstance.IsPreViewTile(t))
                    {
                        nextTiles.Add(new DirectionTile(item.direction, t, count));
                        firstPopList.Add(t);
                    }
            }

        bombPopList.Add(firstPopList);

        while (iter)
        {
            var popList = new List<GameTile>();
            var tempList = new List<DirectionTile>();

            foreach (var item in nextTiles)
            {
                popList.Add(item.tile);
                if (item.count > 0)
                {
                    var count = item.count - 1;
                    var t = item.tile.GetConnectTileOrNull(item.direction);
                    if (t != null)
                        if (TileManager.GetInstance.IsPreViewTile(t))
                            tempList.Add(new DirectionTile(item.direction, t, count));
                }
            }

            bombPopList.Add(popList);

            if (tempList.Count == 0)
            {
                iter = false;
            }
            else
            {
                nextTiles.Clear();
                foreach (var item in tempList) nextTiles.Add(item);
            }
        }
    }

    public void RegisterDoubleBombTile(List<DirectionTile> dTiles)
    {
        AnimationManager.GetInstance.ShowBomb(ECombine.RAINBOW_RAINBOW, EColor.NONE, this);

        var iter = true;
        var nextTiles = new List<GameTile>();

        dTiles[0].tile.IsDoubleRainbow = true;
        nextTiles.Add(dTiles[0].tile);

        while (iter)
        {
            var popTiles = new List<GameTile>();
            var tempTiles = new List<GameTile>();
            foreach (var nextTile in nextTiles)
                for (var i = 0; i < (int)EDirection.NONE; i++)
                {
                    var targetTile = nextTile.GetConnectTileOrNull((EDirection)i);
                    if (targetTile == null) continue;
                    if (targetTile.IsDoubleRainbow) continue;
                    if (!TileManager.GetInstance.IsPreViewTile(targetTile)) continue;
                    popTiles.Add(targetTile);
                    targetTile.IsDoubleRainbow = true;
                    tempTiles.Add(targetTile);
                }

            bombPopList.Add(popTiles);
            nextTiles.Clear();

            if (tempTiles.Count == 0)
                iter = false;
            else
                foreach (var item in tempTiles)
                    nextTiles.Add(item);
        }
    }

    private IEnumerator BombCoroutine(EID id = EID.NONE, ECombine combine = ECombine.NONE)
    {
        yield return new WaitUntil(() => AnimationManager.AnimCount == 0);
        RegisterFloorPop();

        if (bomingCount == 1)
        {
            foreach (var list in bombPopList)
            {
                foreach (var bTile in list)
                {
                    var jamTileKey = new List<GameTile>();
                    var antibombKey = new List<GameTile>();

                    var isHate = false;
                    var isJam = false;
                    var isAntibomb = false;

                    foreach (var item in rocketJamTiles)
                        if (item.Key.Contains(bTile))
                        {
                            if (item.Value)
                            {
                                bTile.RegisterJamPop();
                                if (bTile.IsJamHateBlock())
                                {
                                    isHate = true;
                                    jamTileKey = item.Key;
                                }
                            }
                            else
                            {
                                if (bTile.IsFreeJam())
                                {
                                    isJam = true;
                                    jamTileKey = item.Key;
                                }
                            }
                        }

                    if (jamTiles.Count == 1)
                        foreach (var jam in jamTiles)
                            if (jam.Key.direction == EDirection.NONE)
                                bTile.RegisterJamPop();
                    if (bTile.IsAntiBomb()) isAntibomb = true;

                    if (shieldDirectionTiles.Count != 1)
                    {
                        foreach (var item in shieldTileLists)
                            if (item.Key.Contains(bTile))
                            {
                                if (item.Value)
                                {
                                    if (combine == ECombine.ACTIVE_GREATBOMB)
                                    {
                                        bTile.BombPop();
                                        bTile.IsDamagedThisTurn = false;
                                        bTile.BombPop();
                                        bTile.IsDamagedThisTurn = false;
                                        bTile.BombPop();
                                        bTile.IsDamagedThisTurn = false;
                                        bTile.BombPop();
                                        bTile.IsDamagedThisTurn = false;
                                        bTile.BombPop();
                                        bTile.IsDamagedThisTurn = false;
                                        bTile.BombPop();
                                        bTile.IsDamagedThisTurn = false;
                                    }

                                    bTile.BombPop();
                                }

                                antibombKey = item.Key;
                            }
                    }
                    else
                    {
                        bTile.BombPop();
                    }

                    if (isHate) rocketJamTiles[jamTileKey] = false;
                    if (isJam) rocketJamTiles[jamTileKey] = true;
                    if (isAntibomb) shieldTileLists[antibombKey] = false;
                }

                yield return new WaitForSeconds(0.065f);
            }

            bombPopList.Clear();
            rocketJamTiles.Clear();
            shieldDirectionTiles.Clear();
            shieldTileLists.Clear();
            jamTiles.Clear();
            bomingCount--;
        }
    }

    public bool IsFreeJam()
    {
        var hasJam = false;

        foreach (var block in blocks)
        {
            if (block.ID == EID.JAM) hasJam = true;
            if (BlockManager.GetInstance.IsJamHater(block.ID)) return false;
        }

        if (hasJam) return true;

        return false;
    }

    public bool IsJamHateBlock()
    {
        foreach (var block in blocks)
            if (BlockManager.GetInstance.IsJamHater(block.ID))
                return true;

        return false;
    }

    public GameBlock GetBlockOrNULL(EDepth depth)
    {
        foreach (var block in blocks)
            if (block.Property.depth == depth)
                return block;

        return null;
    }

    public bool IsAntiBomb()
    {
        foreach (var block in blocks)
            if (block.Property.popProperty.AntiBomb)
                return true;
        return false;
    }
}