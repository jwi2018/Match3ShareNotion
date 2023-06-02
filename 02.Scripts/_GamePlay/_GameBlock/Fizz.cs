using UnityEngine;

public class Fizz : ObstacleBlock
{
    [SerializeField] private SpriteRenderer fizzRender;
    private bool isBomb;

    public override void Init()
    {
        base.Init();
        blockRenderer = fizzRender;
        isBomb = false;
    }

    public override void SidePop(EColor _color, EDirection _direction)
    {
        if (isBomb) return;
        if (hp == 1)
        {
            if (tile != null)
            {
                isBomb = true;
                AnimationManager.AnimCount++;
                animator.SetTrigger("Destroy");
                ParticleManager.GetInstance.ShowParticle(ID, Color, HP, transform.position);
                //tile.RegisterBombPop(this);
                //base.BombPop();
            }
        }
        else if (hp > 1)
        {
            base.SidePop(_color, _direction);
        }
    }

    public override void BombPop()
    {
        if (isBomb) return;
        if (hp == 1)
        {
            if (tile != null)
            {
                isBomb = true;
                AnimationManager.AnimCount++;
                animator.SetTrigger("Destroy");
                ParticleManager.GetInstance.ShowParticle(ID, Color, HP, transform.position);
                //tile.RegisterBombPop(this);
                //base.BombPop();
            }
        }
        else if (hp > 1)
        {
            base.BombPop();
        }
    }

    public void ActiveBomb()
    {
        tile.RegisterBombPop(this);
        base.BombPop();
        AnimationManager.AnimCount--;
    }
}