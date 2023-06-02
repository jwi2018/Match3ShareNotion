using UnityEngine;

public class SideDirectionBigObject : BigObject
{
    public override void Setting(EColor color, Vector2Int matrix, Vector2Int _size)
    {
        base.Setting(color, matrix, _size);
        ID = EID.BIG_SIDE_DIRECTION;
        leftTopPosition = matrix;
    }

    public override void SidePop(EColor color, EDirection direction, Vector2Int matrix)
    {
        base.SidePop(color, direction, matrix);
        var position = matrix - leftTopPosition;
        foreach (var item in subParts)
            if (item.Matrix == position && item.Direction == direction
                                        && item.gameObject.activeSelf)
            {
                item.gameObject.SetActive(false);
                break;
            }

        Damaged();
    }

    public override void BombPop(Vector2Int matrix)
    {
        base.BombPop(matrix);

        var position = matrix - leftTopPosition;
        foreach (var item in subParts)
            if (item.Matrix == position && item.gameObject.activeSelf)
                item.gameObject.SetActive(false);
        Damaged();
    }

    public override void Damaged()
    {
        base.Damaged();

        var isClear = true;
        foreach (var item in subParts)
        foreach (var test in subParts)
            if (test.gameObject.activeSelf)
                isClear = false;
        if (isClear) Clear();
    }

    public override void Clear()
    {
        base.Clear();
        BlockManager.GetInstance.AddRainbowBox(this);
    }
}