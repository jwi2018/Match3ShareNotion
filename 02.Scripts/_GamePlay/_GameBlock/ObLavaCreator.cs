using UnityEngine;

public class ObLavaCreator : ObstacleBlock
{
    [SerializeField] private Highlight highlight;

    public bool IsDamaged;

    public override void SetProperty(BlockProperty blockProperty)
    {
        base.SetProperty(blockProperty);
        if (highlight != null) highlight.Init();
    }

    public override void ActiveAbility()
    {
    }

    public override void SidePop(EColor _color, EDirection _direction)
    {
        //base.SidePop(_color, _direction);
        IsDamaged = true;
    }

    public override void BombPop()
    {
        //base.BombPop();
        IsDamaged = true;
    }

    public override void BombSidePop()
    {
        // Not Design
        //base.BombSidePop();
        IsDamaged = true;
    }

    public override void SetHighlight(bool value)
    {
        base.SetHighlight(value);

        if (highlight != null) highlight.SetHighlight(value);
    }
}