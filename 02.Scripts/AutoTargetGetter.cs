using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public enum ETargetType
{
    DESTROY,
    MOVE,
    SPREAD,
    OBSTACLE
}

public class AutoTargetGetter : MonoBehaviour
{
    private readonly Dictionary<EID, ETargetType> missionTypeIDListDictionary = new Dictionary<EID, ETargetType>();

    private readonly List<Vector2Int> preAutoTargetVectorList = new List<Vector2Int>();

    public void Init()
    {
        missionTypeIDListDictionary.Add(EID.NORMAL, ETargetType.DESTROY);
        missionTypeIDListDictionary.Add(EID.RELIC_IN_INVISIBLE_BOX, ETargetType.DESTROY);
        missionTypeIDListDictionary.Add(EID.TABLET_FLOOR, ETargetType.DESTROY);
        missionTypeIDListDictionary.Add(EID.TABLET, ETargetType.DESTROY);
        missionTypeIDListDictionary.Add(EID.BIG_SIDE, ETargetType.DESTROY);
        missionTypeIDListDictionary.Add(EID.COIN_BOX, ETargetType.DESTROY);
        missionTypeIDListDictionary.Add(EID.GOLD, ETargetType.DESTROY);
        missionTypeIDListDictionary.Add(EID.GLASS_COLOR, ETargetType.DESTROY);
        missionTypeIDListDictionary.Add(EID.CLAM, ETargetType.DESTROY);
        missionTypeIDListDictionary.Add(EID.BIG_SIDE_COLOR, ETargetType.DESTROY);
        missionTypeIDListDictionary.Add(EID.CHAMELEON, ETargetType.DESTROY);
        missionTypeIDListDictionary.Add(EID.CLIMBER, ETargetType.DESTROY);

        missionTypeIDListDictionary.Add(EID.TURN_BOX, ETargetType.DESTROY);
        missionTypeIDListDictionary.Add(EID.DOUBLE, ETargetType.DESTROY);
        missionTypeIDListDictionary.Add(EID.ACTINIARIA, ETargetType.DESTROY);

        missionTypeIDListDictionary.Add(EID.JAM, ETargetType.SPREAD);

        missionTypeIDListDictionary.Add(EID.DROP_RELIC1, ETargetType.MOVE);
        missionTypeIDListDictionary.Add(EID.DROP_RELIC2, ETargetType.MOVE);
        missionTypeIDListDictionary.Add(EID.DROP_RELIC3, ETargetType.MOVE);

        missionTypeIDListDictionary.Add(EID.BOX, ETargetType.OBSTACLE);
        missionTypeIDListDictionary.Add(EID.JAIL, ETargetType.OBSTACLE);
        missionTypeIDListDictionary.Add(EID.SHIELD, ETargetType.OBSTACLE);
        missionTypeIDListDictionary.Add(EID.LAVA, ETargetType.OBSTACLE);
        missionTypeIDListDictionary.Add(EID.BANDAGE, ETargetType.OBSTACLE);
        missionTypeIDListDictionary.Add(EID.CREATOR_LAVA, ETargetType.OBSTACLE);
        missionTypeIDListDictionary.Add(EID.TIMEBOMB_ICE, ETargetType.OBSTACLE);
        missionTypeIDListDictionary.Add(EID.METAL_BOX, ETargetType.OBSTACLE);
        missionTypeIDListDictionary.Add(EID.GIFTBOX, ETargetType.OBSTACLE);
        missionTypeIDListDictionary.Add(EID.OAK, ETargetType.OBSTACLE);
        missionTypeIDListDictionary.Add(EID.BOX_COLOR, ETargetType.OBSTACLE);
        missionTypeIDListDictionary.Add(EID.FIZZ, ETargetType.OBSTACLE);
    }

    public Vector2Int GetAutoTargetPosition(in List<GameBlock> blockList, bool isSpread = false)
    {
        var returnValue = new Vector2Int(0, 0);

        returnValue = GetMatrixMissionBlock(blockList, isSpread);
        if (returnValue.x == -1 && returnValue.y == -1)
        {
            returnValue = GetMatrixObstacleBlock(blockList);

            if (returnValue.x == -1 && returnValue.y == -1)
            {
                var randomBlock = BlockManager.GetInstance.GetRandomNormalBlockOrNull();
                if (randomBlock != null) returnValue = randomBlock.Tile.Matrix;
            }
        }

        preAutoTargetVectorList.Add(returnValue);
        return returnValue;
    }

    public Vector2Int GetAutoTargetMissionPositionOrMinus(in List<GameBlock> blockList, bool isSpread = false)
    {
        var returnValue = new Vector2Int(0, 0);

        returnValue = GetMatrixMissionBlock(blockList, isSpread);
        if (returnValue.x == -1 && returnValue.y == -1) returnValue = GetMatrixObstacleBlock(blockList);
        preAutoTargetVectorList.Add(returnValue);
        return returnValue;
    }


    private Vector2Int GetMatrixMissionBlock(in List<GameBlock> blockList, bool isSpread = false)
    {
        var missions = StageManager.GetInstance.GetMissions();
        var targetVector2IntList = new List<Vector2Int>();

        foreach (var item in missions)
        {
            if (item.count == 0) continue;
            if (missionTypeIDListDictionary.ContainsKey(item.ID))
            {
                if (missionTypeIDListDictionary[item.ID] == ETargetType.SPREAD && isSpread)
                {
                    foreach (var block in blockList)
                    {
                        if (block == null) continue;
                        if (block.Tile == null) continue;
                        if (!TileManager.GetInstance.IsPreViewTile(block.Tile)) continue;
                        if (!block.Tile.IsFreeJam()) targetVector2IntList.Add(block.Tile.Matrix);
                    }

                    if (targetVector2IntList.Count != 0) break;
                }

                if (missionTypeIDListDictionary[item.ID] == ETargetType.DESTROY)
                    foreach (var block in blockList)
                        if (block.ID == item.ID && TileManager.GetInstance.IsPreViewTile(block.Tile))
                        {
                            if (block.ID == EID.TABLET)
                            {
                                if (block.Tile.GetBlockOrNULL(EDepth.FLOOR) != null)
                                    targetVector2IntList.Add(block.Tile.Matrix);
                            }
                            else if (block.ID == EID.TURN_BOX)
                            {
                                // block.Color == RED -> Open
                                if (block.Color == EColor.RED) targetVector2IntList.Add(block.Tile.Matrix);
                            }
                            else if (block.ID == EID.NORMAL)
                            {
                                if (block.Color == item.color) targetVector2IntList.Add(block.Tile.Matrix);
                            }
                            else
                            {
                                targetVector2IntList.Add(block.Tile.Matrix);
                            }
                        }

                if (missionTypeIDListDictionary[item.ID] == ETargetType.MOVE)
                    foreach (var block in blockList)
                        if (block.ID == item.ID && TileManager.GetInstance.IsPreViewTile(block.Tile))
                        {
                            var targetTile = TileManager.GetInstance.GetNextTileOrNull(block.Tile);
                            if (targetTile != null)
                                if (targetTile.IsBombPopAble() && TileManager.GetInstance.IsPreViewTile(targetTile))
                                    targetVector2IntList.Add(targetTile.Matrix);
                        }
            }
        }

        if (targetVector2IntList.Count != 0)
        {
            var rng = new Random();
            var n = targetVector2IntList.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                var value = targetVector2IntList[k];
                targetVector2IntList[k] = targetVector2IntList[n];
                targetVector2IntList[n] = value;
            }

            for (var i = 0; i < targetVector2IntList.Count; i++)
                if (preAutoTargetVectorList.Contains(targetVector2IntList[i]))
                    continue;
                else
                    return targetVector2IntList[i];
        }

        return new Vector2Int(-1, -1);
    }

    private Vector2Int GetMatrixObstacleBlock(in List<GameBlock> blockList)
    {
        var targetVector2IntList = new List<Vector2Int>();
        foreach (var item in blockList)
            if (missionTypeIDListDictionary.ContainsKey(item.ID))
                if (missionTypeIDListDictionary[item.ID] == ETargetType.OBSTACLE)
                    foreach (var block in blockList)
                        if (block.ID == item.ID && TileManager.GetInstance.IsPreViewTile(block.Tile))
                            targetVector2IntList.Add(block.Tile.Matrix);

        if (targetVector2IntList.Count != 0)
        {
            var rng = new Random();
            var n = targetVector2IntList.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                var value = targetVector2IntList[k];
                targetVector2IntList[k] = targetVector2IntList[n];
                targetVector2IntList[n] = value;
            }

            for (var i = 0; i < targetVector2IntList.Count; i++)
                if (preAutoTargetVectorList.Contains(targetVector2IntList[i]))
                    continue;
                else
                    return targetVector2IntList[i];
        }

        return new Vector2Int(-1, -1);
    }

    public void EndTurn()
    {
        preAutoTargetVectorList.Clear();
    }

    public void Clear()
    {
        missionTypeIDListDictionary.Clear();
        preAutoTargetVectorList.Clear();
    }
}