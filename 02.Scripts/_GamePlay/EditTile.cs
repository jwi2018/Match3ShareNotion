using System.Collections.Generic;
using UnityEngine;

public class EditTile
{
    private readonly Dictionary<EDepth, BlockInfo> blockDictionary = new Dictionary<EDepth, BlockInfo>();

    private readonly Tile tile = new Tile();
    public Vector2Int Matrix => tile.Matrix;
    public ETileKind TileKind => tile.TileKind;
    public EDirection DropDirection => tile.DropDirection;

    public void Init()
    {
    }

    public void Setting(Vector2Int _matrix, ETileKind _tileKind, EDirection _dropDirection)
    {
        tile.Setting(_matrix, _tileKind, _dropDirection);
    }

    public bool IsExistBlock(EDepth depth)
    {
        if (blockDictionary.ContainsKey(depth))
            return true;
        return false;
    }

    public bool InputBlock(EDepth depth, BlockInfo block)
    {
        if (!blockDictionary.ContainsKey(depth))
        {
            blockDictionary.Add(depth, block);
            return true;
        }

        return false;
    }

    public BlockInfo GetObjectOrNull(EDepth depth)
    {
        if (blockDictionary.ContainsKey(depth))
            return blockDictionary[depth];
        return null;
    }

    public BlockInfo PopBlockOrNull(EDepth depth)
    {
        if (blockDictionary.ContainsKey(depth))
        {
            var info = blockDictionary[depth];
            blockDictionary.Remove(depth);
            return info;
        }

        return null;
    }

    public void SetDropDirection(EDirection direction)
    {
        tile.SetDropDirection(direction);
    }

    public void SetTileKind(ETileKind kind)
    {
        tile.SetTileKind(kind);
    }

    public Dictionary<EDepth, BlockInfo> GetObjectDictionary()
    {
        return blockDictionary;
    }

    public void Clear()
    {
        blockDictionary.Clear();
    }

    public void Clone(EditTile _tile)
    {
        blockDictionary.Clear();

        foreach (var item in _tile.GetObjectDictionary()) blockDictionary.Add(item.Key, item.Value);

        tile.Setting(_tile.Matrix, _tile.TileKind, _tile.DropDirection);
    }
}