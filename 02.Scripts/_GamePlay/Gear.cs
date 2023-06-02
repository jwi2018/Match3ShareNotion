using System.Collections.Generic;
using UnityEngine;

public class Gear : BigObject
{
    [SerializeField] private SpriteRenderer directionSprite;
    [SerializeField] private Sprite clockwise;

    [SerializeField] private Sprite counterClockwise;

    //[SerializeField] private Rigidbody2D rigidbody = null;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer gearSprite;
    public bool isClockwise = true;
    private GearCore core;
    private float rotationZ = 0;

    private List<GameTile> tiles = new List<GameTile>();

    public void Setting(GearCore _core, bool _isClockwise)
    {
        if (isClockwise != _isClockwise) tiles.Reverse();
        isClockwise = _isClockwise;

        directionSprite = _core.gameObject.GetComponent<SpriteRenderer>();
        _core.SettingGear(this);

        core = _core;
        ApplyGear();
    }

    public void AddTiles(List<GameTile> list)
    {
        tiles = list;
    }

    public void ChangeDirection()
    {
        isClockwise = !isClockwise;
        tiles.Reverse();

        ApplyGear();
    }

    public void ApplyGear()
    {
        if (isClockwise)
            directionSprite.sprite = clockwise;
        else
            directionSprite.sprite = counterClockwise;
    }

    public void Moving()
    {
        if (isClockwise)
            //rotationZ -= 45f;
            animator.SetTrigger("TurnR");
        else
            //rotationZ += 45f;
            animator.SetTrigger("TurnL");

        //rigidbody.DORotate(rotationZ, 0.1f);

        var firstTile = tiles[0];

        var preTile = firstTile;
        GameTile nextTile = null;

        GameBlock tempBlock = null;
        var moveWaitBlock = preTile.NormalBlock;

        GameBlock tempTopBlock = null;
        var moveWaitTopBlock = preTile.GetTopBlockOrNull();

        for (var i = 0; i < tiles.Count; i++)
        {
            if (i == tiles.Count - 1)
                nextTile = firstTile;
            else
                nextTile = tiles[i + 1];

            tempBlock = nextTile.NormalBlock;
            tempTopBlock = nextTile.GetTopBlockOrNull();

            if (moveWaitBlock != null)
            {
                nextTile.RemoveBlock(nextTile.NormalBlock);
                moveWaitBlock.Drop(nextTile);
            }
            else
            {
                nextTile.RemoveBlock(nextTile.NormalBlock);
            }

            if (moveWaitTopBlock != null)
            {
                nextTile.RemoveBlock(nextTile.GetTopBlockOrNull());
                moveWaitTopBlock.Move(nextTile);
            }
            else
            {
                nextTile.RemoveBlock(nextTile.GetTopBlockOrNull());
            }

            moveWaitBlock = tempBlock;
            moveWaitTopBlock = tempTopBlock;
        }
    }

    public void SetHighlight(Vector2Int position, bool value)
    {
        var isExist = false;
        foreach (var tile in tiles)
            if (tile.Matrix == position)
                isExist = true;

        if (isExist)
        {
            if (value)
            {
                gearSprite.sortingOrder = (int) EDepth.FLOOR + 1000;
                gearSprite.maskInteraction = SpriteMaskInteraction.None;
                gearSprite.gameObject.layer = 5;
            }
            else
            {
                gearSprite.sortingOrder = (int) EDepth.FLOOR;
                gearSprite.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                gearSprite.gameObject.layer = 0;
            }

            core.SetHighlight(value);
        }
    }
}