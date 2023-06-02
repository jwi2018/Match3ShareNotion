using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEditor;

public class UISlideTween : MonoBehaviour
{
    public Ease easeType_Enter = Ease.Linear;
    public Ease easeType_Exit = Ease.Linear;
    private RectTransform rectTr = null;
    public GameObject activeTarget = null;
    public Vector3 enterPosition, enterScale, targetPosition, targetScale, exitPosition, exitScale;
    public float enterWidth, enterHeight, targetWidth, targetHeight, exitWidth, exitHeight;

    public bool isEnterInit = false;
    public float enterDuration = 1;
    public float enterDelay = 0;
    public float exitDuration = 1;
    public float autoExitAfter = 0;
    
    private bool isInitial = false;

    void Awake()
    {
        rectTr = GetComponent<RectTransform>();
        if (activeTarget == null)
        {
            activeTarget = gameObject;
        }
        DOTween.Init();
    }

    private void OnEnable()
    {
        if (isInitial == false)
        {
            isInitial = true;
            if (isEnterInit == false)
            {
                rectTr.anchoredPosition = exitPosition;
                activeTarget.SetActive(false);
            }
        }
        else
        {
            EnterTween();
        }
    }

    public void EnterTween()
    {
        activeTarget.SetActive(true);
        StartCoroutine(EnterAnimation());
    }
    
    public void ExitTween()
    {
        transform.DOKill();
        transform.DORestart();
        transform.DOScale(exitScale, exitDuration);
        // transform.DOLocalMove(exitPosition, exitDuration, false).SetEase(easeType_Exit).
        //     OnComplete(()=> gameObject.SetActive(false))
        //     .Play();
        
        rectTr.DOAnchorPos(exitPosition, exitDuration, false).SetEase(easeType_Exit).
            OnComplete(()=> activeTarget.SetActive(false))
            .Play();
    }

    private IEnumerator EnterAnimation()
    {
        // transform.SetAsLastSibling();
        rectTr.anchoredPosition = enterPosition;
        transform.localScale = enterScale;

        transform.DOKill();
        transform.DORestart();

        yield return new WaitForSeconds(enterDelay);

        transform.DOScale(targetScale, enterDuration);
        // transform.DOLocalMove(targetPosition, enterDuration, false).SetEase(easeType_Enter).Play();
        // rectTr.DOLocalMove(targetPosition, enterDuration, false).SetEase(easeType_Enter).Play();
        rectTr.DOAnchorPos(targetPosition, enterDuration, false).SetEase(easeType_Enter).Play();

        if (autoExitAfter > 0)
        {
            yield return new WaitForSeconds(autoExitAfter);
            ExitTween();
        }
    }
}