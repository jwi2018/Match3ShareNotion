using UnityEngine;

public class ObstacleFaction : ObstacleBlock
{
    [SerializeField] private SpriteRenderer mainRenderer;
    [SerializeField] private Animator factionAnimator;

    public override void Init()
    {
        base.Init();
        blockRenderer = mainRenderer;
    }

    public override void Setting(EColor _color, EID _id, int _hp = 1, int _etc = 0)
    {
        etc = Mathf.Min(2, etc);
        base.Setting(_color, _id, _hp, _etc);
        factionAnimator.SetInteger("ETCValue", _etc);
        factionAnimator.SetInteger("HPValue", _hp);
    }

    public override void SidePop(EColor _color, EDirection _direction)
    {
        base.SidePop(_color, _direction);
        factionAnimator.SetTrigger("Damage");
    }

    public override void BombPop()
    {
        if (IsBombed) return;
        IsBombed = true;

        if (tile != null)
            if (tile.IsDamagedThisTurn)
                return;
        Attacked();
        factionAnimator.SetTrigger("Damage");
    }

    public override void ActiveAbility()
    {
        factionAnimator.SetTrigger("Destroy");
    }

    public void EndDestroyAnim()
    {
        if (tile != null)
            tile.RemoveBlock(this);
        AnimationManager.FactionCount--;
        Remove();
        Clear();
    }
}