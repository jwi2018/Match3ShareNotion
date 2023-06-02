using UnityEngine;

public class Tile
{
    protected EDirection dropDirection = EDirection.DOWN;

    protected Vector2Int matrix;
    protected ETileKind tileKind;
    public Vector2Int Matrix => matrix;
    public ETileKind TileKind => tileKind;
    public EDirection DropDirection => dropDirection;

    public void Setting(Vector2Int _matrix, ETileKind _tileKind, EDirection _direction)
    {
        matrix = _matrix;
        tileKind = _tileKind;
        dropDirection = _direction;
    }

    public void SetDropDirection(EDirection direction)
    {
        dropDirection = direction;
    }

    public void SetTileKind(ETileKind kind)
    {
        tileKind = kind;
    }
}