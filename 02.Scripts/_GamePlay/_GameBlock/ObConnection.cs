using System;
using System.Collections;
using UnityEngine;

public struct ConnectionOnMatrix
{
    public Vector2Int matrix;
    public ConnectionProperty property;
}

public struct ConnectionProperty
{
    public int hp;
    public int order;
    public EOneWay direction;
}

[Serializable]
public struct ConnectionHpSetting
{
    public SpriteRenderer renderer;
    public Sprite hpSprite;
}

public class ObConnection : ObstacleBlock
{
    private static int DestroySoundCount = 0;
    [SerializeField] private SpriteRenderer connectionRenderer;
    [SerializeField] private ConnectionHpSetting[] connectionHpSettings;
    [SerializeField] private GameObject leftRope;
    [SerializeField] private GameObject rightRope;
    [SerializeField] private GameObject upRope;
    [SerializeField] private GameObject downRope;

    [SerializeField] private Highlight highlight;
    [SerializeField] private AudioClip destroySound;

    private ConnectionOnMatrix connectionOnMatrix;

    private EDirection nextDirection = EDirection.NONE;
    private EDirection preDirection = EDirection.NONE;

    public ConnectionOnMatrix ConnectionProperty => connectionOnMatrix;
    public ObConnection NextConnection { get; private set; }

    public ObConnection PreConnection { get; private set; }

    public bool IsChainBomb { get; set; }

    public override void Init()
    {
        blockRenderer = connectionRenderer;
        base.Init();
        IsChainBomb = false;

        if (highlight != null) highlight.Init();
    }

    public override void Setting(EColor _color, EID _id, int _hp = 1, int _etc = 0)
    {
        base.Setting(_color, _id, _hp, _etc);
        SetSprite();
    }

    public override void SidePop(EColor _color, EDirection _direction)
    {
        if (hp == 1)
        {
            if (tile != null) StartChainBomb(true, true);
        }
        else
        {
            if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ObConnectionDestroy");
            base.SidePop(_color, _direction);
            SetSprite();
            //SetAnim();
        }
    }

    public override void BombPop()
    {
        if (hp == 1)
        {
            if (tile != null) StartChainBomb(true, true);
        }
        else
        {
            if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ObConnectionDestroy");
            base.BombPop();
            SetSprite();
            //SetAnim();
        }
    }

    public void SetSprite()
    {
        for (var i = 1; i < 5; i++)
            if (hp <= i)
            {
                connectionHpSettings[i - 1].renderer.sprite = connectionHpSettings[i - 1].hpSprite;
                switch (i)
                {
                    case 1:
                        connectionHpSettings[i - 1].renderer.gameObject.transform.localPosition =
                            new Vector3(-0.1f, -0.1f);
                        break;
                    case 2:
                        connectionHpSettings[i - 1].renderer.gameObject.transform.localPosition =
                            new Vector3(0.1f, 0.1f);
                        break;
                    case 3:
                        connectionHpSettings[i - 1].renderer.gameObject.transform.localPosition =
                            new Vector3(0.1f, -0.1f);
                        break;
                    case 4:
                        connectionHpSettings[i - 1].renderer.gameObject.transform.localPosition =
                            new Vector3(-0.1f, 0.1f);
                        break;
                }
            }
    }

    private void SetAnim()
    {
        switch (hp)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                //animator.SetTrigger("LeftTop_Damage");
                break;
        }
    }

    public void StartChainBomb(bool isNextDirection, bool isPreDirection)
    {
        if (IsChainBomb) return;
        IsChainBomb = true;
        if (gameObject.activeSelf) StartCoroutine(StartChainBombCoroutine(isNextDirection, isPreDirection));
    }

    private IEnumerator StartChainBombCoroutine(bool isNextDirection, bool isPreDirection)
    {
        yield return new WaitForSeconds(0.09f);
        if (isNextDirection)
            if (NextConnection != null)
                NextConnection.StartChainBomb(true, false);
        if (isPreDirection)
            if (PreConnection != null)
                PreConnection.StartChainBomb(false, true);

        if (tile != null)
        {
            ParticleManager.GetInstance.ShowParticle(EID.CONNECTION, EColor.NONE, 1, gameObject.transform.position);
            tile.RemoveBlock(this);
            Remove();
            Clear();
            tile = null;
        }
    }


    public void SettingConnection(GameTile _tile, ConnectionOnMatrix _connectionOnMatrix)
    {
        tile = _tile;

        connectionOnMatrix = _connectionOnMatrix;

        switch (connectionOnMatrix.property.direction)
        {
            case EOneWay.RIGHT_TO_LEFT:
                nextDirection = EDirection.LEFT;
                preDirection = EDirection.RIGHT;
                leftRope.SetActive(true);
                rightRope.SetActive(true);
                upRope.SetActive(false);
                downRope.SetActive(false);
                break;
            case EOneWay.LEFT_TO_RIGHT:
                nextDirection = EDirection.RIGHT;
                preDirection = EDirection.LEFT;
                leftRope.SetActive(true);
                rightRope.SetActive(true);
                upRope.SetActive(false);
                downRope.SetActive(false);
                break;
            case EOneWay.DOWN_TO_UP:
                nextDirection = EDirection.UP;
                preDirection = EDirection.DOWN;
                leftRope.SetActive(false);
                rightRope.SetActive(false);
                upRope.SetActive(true);
                downRope.SetActive(true);
                break;
            case EOneWay.UP_TO_DOWN:
                nextDirection = EDirection.DOWN;
                preDirection = EDirection.UP;
                leftRope.SetActive(false);
                rightRope.SetActive(false);
                upRope.SetActive(true);
                downRope.SetActive(true);
                break;
            case EOneWay.RIGHT_TO_DOWN:
                nextDirection = EDirection.DOWN;
                preDirection = EDirection.RIGHT;
                leftRope.SetActive(false);
                rightRope.SetActive(true);
                upRope.SetActive(false);
                downRope.SetActive(true);
                break;
            case EOneWay.DOWN_TO_LEFT:
                nextDirection = EDirection.LEFT;
                preDirection = EDirection.DOWN;
                leftRope.SetActive(true);
                rightRope.SetActive(false);
                upRope.SetActive(false);
                downRope.SetActive(true);
                break;
            case EOneWay.DOWN_TO_RIGHT:
                nextDirection = EDirection.RIGHT;
                preDirection = EDirection.DOWN;
                leftRope.SetActive(false);
                rightRope.SetActive(true);
                upRope.SetActive(false);
                downRope.SetActive(true);
                break;
            case EOneWay.LEFT_TO_DOWN:
                nextDirection = EDirection.DOWN;
                preDirection = EDirection.LEFT;
                leftRope.SetActive(true);
                rightRope.SetActive(false);
                upRope.SetActive(false);
                downRope.SetActive(true);
                break;
            case EOneWay.UP_TO_RIGHT:
                nextDirection = EDirection.RIGHT;
                preDirection = EDirection.UP;
                leftRope.SetActive(false);
                rightRope.SetActive(true);
                upRope.SetActive(true);
                downRope.SetActive(false);
                break;
            case EOneWay.LEFT_TO_UP:
                nextDirection = EDirection.UP;
                preDirection = EDirection.LEFT;
                leftRope.SetActive(true);
                rightRope.SetActive(false);
                upRope.SetActive(true);
                downRope.SetActive(false);
                break;
            case EOneWay.RIGHT_TO_UP:
                nextDirection = EDirection.UP;
                preDirection = EDirection.RIGHT;
                leftRope.SetActive(false);
                rightRope.SetActive(true);
                upRope.SetActive(true);
                downRope.SetActive(false);
                break;
            case EOneWay.UP_TO_LEFT:
                nextDirection = EDirection.LEFT;
                preDirection = EDirection.UP;
                leftRope.SetActive(true);
                rightRope.SetActive(false);
                upRope.SetActive(true);
                downRope.SetActive(false);
                break;
        }
    }

    public void WrapUpConnection()
    {
        if (NextConnection == null)
            switch (nextDirection)
            {
                case EDirection.LEFT:
                    leftRope.SetActive(false);
                    break;
                case EDirection.RIGHT:
                    rightRope.SetActive(false);
                    break;
                case EDirection.UP:
                    upRope.SetActive(false);
                    break;
                case EDirection.DOWN:
                    downRope.SetActive(false);
                    break;
            }

        if (PreConnection == null)
            switch (preDirection)
            {
                case EDirection.LEFT:
                    leftRope.SetActive(false);
                    break;
                case EDirection.RIGHT:
                    rightRope.SetActive(false);
                    break;
                case EDirection.UP:
                    upRope.SetActive(false);
                    break;
                case EDirection.DOWN:
                    downRope.SetActive(false);
                    break;
            }
    }

    public void SetPreConnection(ObConnection connection)
    {
        PreConnection = connection;
    }

    public void SetNextConnection(ObConnection connection)
    {
        NextConnection = connection;
    }

    public override void SetHighlight(bool value)
    {
        base.SetHighlight(value);

        if (highlight != null) highlight.SetHighlight(value);
    }
}