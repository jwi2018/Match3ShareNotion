using UnityEngine;

public struct RailOnMatrix
{
    public Vector2Int matrix;
    public RailProperty property;
}

public struct RailProperty
{
    public int power;
    public bool isRemove;
    public int order;
    public int railKind;
}

public class Rail : MonoBehaviour
{
    [SerializeField] private SpriteRenderer railRenderer;

    private RailOnMatrix railOnMatrix;

    public RailOnMatrix RailProperty => railOnMatrix;
    public Rail NextRail { get; private set; }

    public Rail PreRail { get; private set; }

    public GameTile Tile { get; private set; }

    public EDirection NextDirection { get; private set; } = EDirection.NONE;

    public EDirection PreDirection { get; private set; } = EDirection.NONE;

    public void Setting(GameTile _tile, RailOnMatrix _railOnMatrix)
    {
        Tile = _tile;
        railOnMatrix = _railOnMatrix;

        switch ((EOneWay)railOnMatrix.property.railKind)
        {
            case EOneWay.RIGHT_TO_LEFT:
                NextDirection = EDirection.LEFT;
                PreDirection = EDirection.RIGHT;
                railRenderer.sprite =
                    BlockManager.GetInstance.GetBlockSprite(EID.RAIL, EColor.NONE, railOnMatrix.property.railKind);
                break;

            case EOneWay.LEFT_TO_RIGHT:
                NextDirection = EDirection.RIGHT;
                PreDirection = EDirection.LEFT;
                railRenderer.sprite =
                    BlockManager.GetInstance.GetBlockSprite(EID.RAIL, EColor.NONE, railOnMatrix.property.railKind);
                break;

            case EOneWay.DOWN_TO_UP:
                NextDirection = EDirection.UP;
                PreDirection = EDirection.DOWN;
                railRenderer.sprite =
                    BlockManager.GetInstance.GetBlockSprite(EID.RAIL, EColor.NONE, railOnMatrix.property.railKind);
                break;

            case EOneWay.UP_TO_DOWN:
                NextDirection = EDirection.DOWN;
                PreDirection = EDirection.UP;
                railRenderer.sprite =
                    BlockManager.GetInstance.GetBlockSprite(EID.RAIL, EColor.NONE, railOnMatrix.property.railKind);
                break;

            case EOneWay.RIGHT_TO_DOWN:
                NextDirection = EDirection.DOWN;
                PreDirection = EDirection.RIGHT;
                railRenderer.sprite =
                    BlockManager.GetInstance.GetBlockSprite(EID.RAIL, EColor.NONE, railOnMatrix.property.railKind);
                break;

            case EOneWay.DOWN_TO_LEFT:
                NextDirection = EDirection.LEFT;
                PreDirection = EDirection.DOWN;
                railRenderer.sprite =
                    BlockManager.GetInstance.GetBlockSprite(EID.RAIL, EColor.NONE, railOnMatrix.property.railKind);
                break;

            case EOneWay.DOWN_TO_RIGHT:
                NextDirection = EDirection.RIGHT;
                PreDirection = EDirection.DOWN;
                railRenderer.sprite =
                    BlockManager.GetInstance.GetBlockSprite(EID.RAIL, EColor.NONE, railOnMatrix.property.railKind);
                break;

            case EOneWay.LEFT_TO_DOWN:
                NextDirection = EDirection.DOWN;
                PreDirection = EDirection.LEFT;
                railRenderer.sprite =
                    BlockManager.GetInstance.GetBlockSprite(EID.RAIL, EColor.NONE, railOnMatrix.property.railKind);
                break;

            case EOneWay.UP_TO_RIGHT:
                NextDirection = EDirection.RIGHT;
                PreDirection = EDirection.UP;
                railRenderer.sprite =
                    BlockManager.GetInstance.GetBlockSprite(EID.RAIL, EColor.NONE, railOnMatrix.property.railKind);
                break;

            case EOneWay.LEFT_TO_UP:
                NextDirection = EDirection.UP;
                PreDirection = EDirection.LEFT;
                railRenderer.sprite =
                    BlockManager.GetInstance.GetBlockSprite(EID.RAIL, EColor.NONE, railOnMatrix.property.railKind);
                break;

            case EOneWay.RIGHT_TO_UP:
                NextDirection = EDirection.UP;
                PreDirection = EDirection.RIGHT;
                railRenderer.sprite =
                    BlockManager.GetInstance.GetBlockSprite(EID.RAIL, EColor.NONE, railOnMatrix.property.railKind);
                break;

            case EOneWay.UP_TO_LEFT:
                NextDirection = EDirection.LEFT;
                PreDirection = EDirection.UP;
                railRenderer.sprite =
                    BlockManager.GetInstance.GetBlockSprite(EID.RAIL, EColor.NONE, railOnMatrix.property.railKind);
                break;
        }
        railRenderer.sortingOrder = (int)EDepth.OVER_RAIL; // 레일과 잔디가 동시에 존재할경우 문제가 발생
    }

    public void SetPreRail(Rail rail)
    {
        PreRail = rail;
    }

    public void SetNextRail(Rail rail)
    {
        NextRail = rail;
    }

    public void SetHighlight(bool value)
    {
        if (value)
        {
            railRenderer.sortingOrder = (int)EDepth.OVER_RAIL + 1000;
            //railRenderer.sortingOrder = (int) EDepth.OVER_RAIL;
            railRenderer.maskInteraction = SpriteMaskInteraction.None;
            gameObject.layer = 5;
            //gameObject.layer = 0;
        }
        else
        {
            railRenderer.sortingOrder = (int)EDepth.OVER_RAIL;
            railRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            gameObject.layer = 0;
        }
    }
}