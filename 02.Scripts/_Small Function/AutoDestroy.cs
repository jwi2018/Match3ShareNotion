using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float minTime;
    public float maxTime = 1f;

    public bool isPool = true;

    private float destroyTime;
    [SerializeField] private GameObject gobTarget = null;

    // Use this for initialization
    private void Start()
    {
        if (gobTarget == null)
        {
            gobTarget = gameObject;
        }

        if (minTime > maxTime)
            destroyTime = minTime;
        else
            destroyTime = Random.Range(minTime, maxTime);
        Invoke("DestroySelf", destroyTime);
    }

    private void DestroySelf()
    {
        if (isPool == true)
        {
            if (!DynamicObjectPool.GetInstance.PoolObject(gobTarget, false)) DestroyImmediate(gobTarget);
        }
        else
        {
            DestroyImmediate(gobTarget);
        }
    }
}