using UnityEngine;

public class RainbowLineController : MonoBehaviour
{
    [SerializeField] private GameObject endObject;
    [SerializeField] private LineRenderer whiteLightningBolt;
    [SerializeField] private LineRenderer colorLightningBolt;
    [SerializeField] private GameObject endParticle;

    [SerializeField] private float endObjectSpeed = 2.0f;
    [SerializeField] private float destroyDelaytime = 0.1f;
    [SerializeField] private float lineRange = 0.5f;
    private EID changeID = EID.NORMAL;
    private float currentDestroyDelaytime;
    private float currentLerpDistance;
    private bool isArrived;
    private bool isClear;

    private GameBlock mainBlock;
    private GameBlock targetBlock;
    private GameObject targetObject;

    private void FixedUpdate()
    {
        if (isClear) return;

        if (!isArrived)
        {
            currentLerpDistance += endObjectSpeed * Time.deltaTime;
            //endObject.transform.position = Vector3.Lerp(endObject.transform.position, targetObject.transform.position, currentLerpDistance);
            endObject.transform.Translate((targetObject.transform.position - endObject.transform.position) *
                                          endObjectSpeed * Time.deltaTime);

            if (IsArrive())
            {
                targetBlock.SetIDWhenActiveRainbow(changeID);

                if (mainBlock != null && targetBlock != null)
                    if (mainBlock.Tile != null && targetBlock.Tile != null)
                        if (mainBlock.Tile.Matrix != targetBlock.Tile.Matrix)
                            targetBlock.SetAnimTrigger("TimeBomb");
                ParticleManager.GetInstance.ShowParticle(EID.COLOR_BOMB, EColor.NONE, 1, endObject.transform.position);
                ParticleManager.GetInstance.CreateRainbowActivePartice(targetObject.transform.position);
                ParticleManager.GetInstance.ShowParticle(EID.COLOR_BOMB, EColor.NONE, 2,
                    targetObject.transform.position);
                isArrived = true;
            }
        }
        else
        {
            currentDestroyDelaytime += Time.deltaTime;
            if (currentDestroyDelaytime > destroyDelaytime) Clear();
        }
    }

    public void Init(GameBlock mainblock, EID id)
    {
        mainBlock = mainblock;
        endObject.transform.localPosition = Vector3.zero;
        changeID = id;
    }

    public void SetTarget(GameObject targetObj, GameBlock targetblock)
    {
        targetObject = targetObj;
        targetBlock = targetblock;

        currentDestroyDelaytime = 0f;
        isArrived = false;
        currentLerpDistance = 0f;
    }

    private bool IsArrive()
    {
        if ((endObject.transform.position - targetObject.transform.position).sqrMagnitude < lineRange)
            return true;
        return false;
    }

    private void Clear()
    {
        if (mainBlock != null) mainBlock.SetEndLightning();
        whiteLightningBolt.enabled = false;
        colorLightningBolt.enabled = false;
        isClear = true;
    }
}