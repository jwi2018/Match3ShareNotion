using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class AutoTargetBomb : MonoBehaviour
{
    [SerializeField] private AudioClip soundFire;
    [SerializeField] private AudioClip soundExplosion;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationCurve curveX;
    [SerializeField] private AnimationCurve curveY;
    [SerializeField] private float rocketTime = 1.5f;

    [SerializeField] private float curveInputAngle = 1.6f;
    [SerializeField] private float curveOutputAngle = 0.02f;
    [SerializeField] private float curveYValue = 0.3f;
    [SerializeField] private MissileParticleCreater particleCreater;

    private SpriteAnimationCustom spriteAnimation = null;

    public float speed = 10f;
    private EColor color;
    private ECombine combine = ECombine.NONE;
    private float distanceLength;
    private Vector3 endPosition;

    private bool isAnimEnd;
    private bool isSpread;
    private Keyframe keyEndX;
    private Keyframe keyEndY;
    private Keyframe keyMiddleX;
    private Keyframe keyMiddleY;
    private Keyframe keyStartX;
    private Keyframe keyStartY;
    private float moveTime;
    private Vector3 startPosition;
    private float startTime;

    private GameTile targetTile;
    private float TotalTime;

    private AnimationCurve XPositionCurved = new AnimationCurve();
    private AnimationCurve YPositionCurved = new AnimationCurve();

    private void Awake()
    {
        spriteAnimation = GetComponent<SpriteAnimationCustom>();
    }

    private void OnEnable()
    {
        AnimationManager.AutoTargetBombCount++;
    }

    private void OnDisable()
    {
        AnimationManager.AutoTargetBombCount--;
        if (AnimationManager.AutoTargetBombCount < 0) AnimationManager.AutoTargetBombCount = 0;
    }

    private void FixedUpdate()
    {
        //if (targetTile == null || !isAnimEnd) return;
        if (!IsBombPopAble())
        {
            var vector = BlockManager.GetInstance.GetAutoTargetVector2Int(isSpread);

            if (vector.x == -1 && vector.y == -1) return;

            var newTargetTile = TileManager.GetInstance.GetTileOrNull(vector);
            if (newTargetTile != null)
            {
                targetTile = newTargetTile;

                startPosition = transform.position;
                endPosition = targetTile.GetWorldPosition();
                transform.rotation = Quaternion.Euler(0, 0, 0);

                ResetMove();
                SettingMove(true);
            }
        }
    }

    public void Setting(Vector2 position, EColor c, ECombine com, bool isspread)
    {
        transform.position = position;
        spriteRenderer.sprite = BlockManager.GetInstance.GetBlockSprite(EID.FISH, c, 1);
        color = c;
        spriteAnimation.idxFrame = (int)c;
        if (DoubleClickSystem.GetInstance != null)
        {
            spriteAnimation.idxFrame = 0;
        }
        combine = com;
        if (combine != ECombine.FISH_FISH && combine != ECombine.NONE)
            spriteRenderer.sprite = BlockManager.GetInstance.GetBlockSprite(EID.FISH, c, 2);
        isSpread = isspread;
        particleCreater.isActive = true;
    }

    public void SetTarget(GameTile tile)
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("AutoTargetBombFire");
        if (tile == null)
        {
            Active();
            return;
        }

        targetTile = tile;
        PositionSetting();

        if (animator != null) animator.SetTrigger("Active");
    }

    public void EndAnimation()
    {
        isAnimEnd = true;
    }

    public void PositionSetting()
    {
        startPosition = transform.position;
        endPosition = targetTile.GetWorldPosition();
        var moving = Moving();
        ResetMove();
        SettingMove(false);
        StartCoroutine(moving);

        string TrailParticle = "";

        //TrailParticle = string.Format($"Particle_Butterfly_{color.ToString().ToLower()}");
        //TrailParticle = string.Format("Particle_Rocket");
        //TrailParticle = "Particle_Rocket";

        if (BaseSystem.GetInstance != null)
        {
            if (ChallengeSystem.GetInstance != null)
            {
                TrailParticle = "Particle_Rocket";
            }
            else
            {
                TrailParticle = string.Format($"Particle_Butterfly_{color.ToString().ToLower()}");
            }
        }

        //var whiteSmoke = DynamicObjectPool.GetInstance.GetObjectForType("WhiteSmoke", false);
        var whiteSmoke = PrefabRegister.GetInstance.GetPrefab(TrailParticle, transform);
        if (whiteSmoke != null) whiteSmoke.transform.position = transform.position;
    }

    private bool IsBombPopAble()
    {
        if (targetTile == null) return false;
        if (!targetTile.IsBombPopAble())
        {
            if (targetTile.NormalBlock != null)
            {
                if (targetTile.NormalBlock.ID != EID.ACTINIARIA)
                    return false;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    private void Active()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("AutoTargetBombExplosion");
        BlockManager.GetInstance.ResetFishPosition();

        if (combine == ECombine.NONE)
        {
            var bombParticle = string.Format($"Particle - Butterfly bomb_{color.ToString().ToLower()}");

            var fishRhombus = PrefabRegister.GetInstance.GetPrefab(bombParticle);
            if (fishRhombus != null) fishRhombus.transform.position = transform.position;

            ParticleManager.GetInstance.ShowParticle(EID.FISH, color, 1, transform.position);
            if (targetTile != null)
            {
                if (isSpread) targetTile.RegisterJamPop();
                targetTile.BombPop();
                targetTile = null;
            }
        }
        else
        {
            ParticleManager.GetInstance.ShowParticle(EID.FISH, color, 1, transform.position);
            var id = EID.NONE;
            switch (combine)
            {
                case ECombine.FISH_HORIZONTAL:
                    id = EID.HORIZONTAL;
                    break;

                case ECombine.FISH_VERTICAL:
                    id = EID.VERTICAL;
                    break;

                case ECombine.FISH_RHOMBUS:
                    id = EID.RHOMBUS;
                    break;

                case ECombine.FISH_X:
                    id = EID.X;
                    break;
            }

            if (isSpread) targetTile.RegisterJamPop();
            targetTile.RegisterBombPop(id, color);
            targetTile.BombPop();
            targetTile = null;
        }

        Destroy(gameObject);
    }

    private void ResetMove()
    {
        XPositionCurved = new AnimationCurve();
        YPositionCurved = new AnimationCurve();
        keyStartX = new Keyframe();
        keyStartY = new Keyframe();

        keyMiddleX = new Keyframe();
        keyMiddleY = new Keyframe();
        keyEndX = new Keyframe();
        keyEndY = new Keyframe();
        TotalTime = 0.0f;
        moveTime = 1.3f;
    }

    private void SettingMove(bool isSecond)
    {
        moveTime = Vector2.Distance(transform.position, endPosition) / speed;
        moveTime = Mathf.Max(1.3f, moveTime);
        keyStartX.time = 0.0f;
        keyStartX.value = transform.position.x;
        keyStartY.time = 0.0f;
        keyStartY.value = transform.position.y;
        var secondValue = 1.0f;
        if (isSecond) secondValue = 0.5f;

        if (startPosition.x > endPosition.x)
            keyStartX.outTangent = curveInputAngle * -1 * secondValue;
        else
            keyStartX.outTangent = curveInputAngle * secondValue;

        keyMiddleX.time = moveTime * 0.5f;
        if (startPosition.x - endPosition.x > 0)
            keyMiddleX.value =
                transform.position.x - curveYValue;
        else if ((int)startPosition.x - (int)endPosition.x == 0)
            keyMiddleX.value =
                transform.position.x + curveYValue * Random.Range(-1, 2) * secondValue;
        else
            keyMiddleX.value =
                transform.position.x + curveYValue;

        keyMiddleY.time = moveTime * 0.5f;
        if (startPosition.y - endPosition.y > 0)
            keyMiddleY.value =
                transform.position.y + curveYValue;
        else if ((int)startPosition.y - (int)endPosition.y == 0)
            keyMiddleY.value =
                transform.position.y + curveYValue * Random.Range(-1, 2) * secondValue;
        else
            keyMiddleY.value =
                transform.position.y - curveYValue;

        keyEndX.time = moveTime;
        keyEndX.value = endPosition.x;
        if (startPosition.x > endPosition.x)
            keyEndX.inTangent = curveOutputAngle * secondValue;
        else
            keyEndX.inTangent = curveOutputAngle * -1 * secondValue;

        keyEndY.time = moveTime;
        keyEndY.value = endPosition.y;

        XPositionCurved.AddKey(keyStartX);
        YPositionCurved.AddKey(keyStartY);

        if (Mathf.Abs(startPosition.x - endPosition.x) < Mathf.Abs(startPosition.y - endPosition.y))
            XPositionCurved.AddKey(keyMiddleX);
        else
            YPositionCurved.AddKey(keyMiddleY);

        XPositionCurved.AddKey(keyEndX);
        YPositionCurved.AddKey(keyEndY);

        curveX.keys = XPositionCurved.keys;
        curveY.keys = YPositionCurved.keys;
    }

    private IEnumerator Moving()
    {
        while (TotalTime < moveTime - 0.1f)
        {
            var nextPosition = new Vector3(curveX.Evaluate(TotalTime), curveY.Evaluate(TotalTime), 0);
            TotalTime += Time.deltaTime;
            var dirToTarget = nextPosition - transform.position;

            transform.rotation =
                Quaternion.Euler(0.0f, 0.0f, -Mathf.Atan2(dirToTarget.x, dirToTarget.y) * Mathf.Rad2Deg);
            transform.position = nextPosition;
            yield return new WaitForEndOfFrame();
        }

        transform.position = new Vector3(endPosition.x, endPosition.y, 0);
        if (VibrationConnector.GetInstance != null)
        {
            VibrationConnector.StartVibration();
        }
        Active();
    }
}