using System.Collections.Generic;
using UnityEngine;

public class BigObject : MonoBehaviour
{
    public bool IsEndAnim;
    public EID ID;

    [SerializeField] protected BigObjectSub[] subParts;
    protected int hp = 1;
    protected Dictionary<Vector2Int, bool> isDamaged = new Dictionary<Vector2Int, bool>();
    protected bool isDamagedOnce;
    protected Vector2Int leftTopPosition;

    protected Vector2Int size = new Vector2Int(2, 2);
    public Vector2Int Size => size;
    public Vector2Int LeftTopPosition => leftTopPosition;

    public virtual void Setting(EColor color, Vector2Int matrix, Vector2Int _size)
    {
        leftTopPosition = matrix;
        size = _size;

        for (var x = 0; x < size.x; x++)
        for (var y = 0; y < size.y; y++)
            isDamaged.Add(leftTopPosition + new Vector2Int(x, y), false);

        IsEndAnim = true;
    }


    public void EndBomb()
    {
        var tempList = new List<Vector2Int>();
        foreach (var item in isDamaged) tempList.Add(item.Key);
        foreach (var item in tempList) isDamaged[item] = false;
        isDamagedOnce = false;
    }

    public virtual void SidePop(EColor color, EDirection direction, Vector2Int matrix)
    {
    }

    public virtual void BombPop(Vector2Int matrix)
    {
    }

    public virtual void Damaged()
    {
    }

    //public virtual void AnimEnd(GameObject)

    public virtual void SetHighlight(bool value)
    {
    }

    public virtual void Clear()
    {
    }
}