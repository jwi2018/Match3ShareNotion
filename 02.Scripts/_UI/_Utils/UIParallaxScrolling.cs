using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIParallaxScrolling : MonoBehaviour
{
    public Vector3 enterPosition;
    public Vector3 exitPosition;
    public float exitDurationMin = 1;
    public float exitDurationMax = 3;
    private RectTransform rectTr = null;

    public float fFirstDelay_Min = 1.0f;
    public float fFirstDelay_Max = 3.0f;
    void Awake()
    {
        rectTr = GetComponent<RectTransform>();
        DOTween.Init();
    }

    private void Start()
    {
        StartCoroutine(EnterAnimation(Random.Range(fFirstDelay_Min, fFirstDelay_Max)));

    }

    private void OnDisable()
    {
        transform.DOKill();
    }

    public void EnterTween()
    {
        StartCoroutine(EnterAnimation());
    }

    private IEnumerator EnterAnimation(float delay = 0f)
    {
        // transform.SetAsLastSibling();
        rectTr.anchoredPosition = enterPosition;

        yield return new WaitForSeconds(delay);

        transform.DOKill();
        transform.DORestart();
        rectTr.DOAnchorPos(exitPosition, Random.Range(exitDurationMin, exitDurationMax), false).SetEase(Ease.Linear).
            OnComplete(() => EnterTween())
            .Play();
    }

}
