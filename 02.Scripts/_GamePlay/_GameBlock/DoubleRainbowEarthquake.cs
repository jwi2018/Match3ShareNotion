using UnityEngine;

public class DoubleRainbowEarthquake : MonoBehaviour
{
    [SerializeField] private Vector2 minPosition = new Vector2(-0.1f, -0.1f);
    [SerializeField] private Vector2 maxPosition = new Vector2(0.1f, 0.1f);
    [SerializeField] private Vector2 minScale = new Vector2(0f, 0f);
    [SerializeField] private Vector2 maxScale = new Vector2(0f, 0f);
    [SerializeField] private Transform myTransform;

    public bool IsActive;

    private Vector2 initScale;

    private void Start()
    {
        initScale = transform.localScale;
    }

    private void FixedUpdate()
    {
        if (IsActive)
        {
            myTransform.localPosition = new Vector2(Random.Range(minPosition.x, maxPosition.x),
                Random.Range(minPosition.y, maxPosition.y));
            myTransform.localScale = new Vector2(initScale.x + Random.Range(minScale.x, maxScale.x),
                initScale.y + Random.Range(minScale.y, maxScale.y));
        }
    }
}