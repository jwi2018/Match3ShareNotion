using System.Collections.Generic;
using UnityEngine;

public class Tablet : BigObject
{
    [SerializeField] private SpriteRenderer bigObjSprite;

    private void Update()
    {
        Check();
    }

    public void SetSprite(Sprite sprite)
    {
        bigObjSprite.sprite = sprite;
    }

    public void Check()
    {
        var isNotClear = false;

        var matrixList = new List<Vector2Int>();

        for (var x = 0; x < size.x; x++)
        for (var y = 0; y < size.y; y++)
            matrixList.Add(LeftTopPosition + new Vector2Int(x, y));
        var list = BlockManager.GetInstance.GetBlocksToUseID(EID.TABLET_FLOOR);

        foreach (var block in list)
        foreach (var matrix in matrixList)
            if (block.Tile.Matrix == matrix)
                isNotClear = true;


        if (!isNotClear) Clear();
    }

    public override void Clear()
    {
        TileManager.GetInstance.TabletClear(this);
        base.Clear();
        var sizeValue = size.x * size.y;
        if (sizeValue == 1) sizeValue = 0;
        else if (sizeValue == 2) sizeValue = 1;
        else if (sizeValue == 4) sizeValue = 2;
        else if (sizeValue == 6) sizeValue = 3;
        else if (sizeValue == 9) sizeValue = 4;
        StageManager.GetInstance.CollectMission(EID.TABLET, EColor.NONE, transform, sizeValue);

        TileManager.GetInstance.BigClear(this);
    }
}