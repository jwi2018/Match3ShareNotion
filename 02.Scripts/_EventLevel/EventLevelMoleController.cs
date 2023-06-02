using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class EventLevelMoleController : MonoBehaviour
{
    [SerializeField] private Transform _rectHairDust;
    [SerializeField] private Vector2 _hairDustShake;
    [SerializeField] private Transform _rectBody;
    [SerializeField] private Vector2 _bodyBounce;

    [SerializeField] private DOTweenAnimation _hairDustAnimation;

    [SerializeField] private float hairFollowDuration = 0.5f;

    private Vector3 prevTargetPos;
    private Tweener followTween;

    [SerializeField] private GameObject gobNotClear = null;
    [SerializeField] private GameObject gobClearParticle = null;
    [SerializeField] private GameObject gobClear = null;
    private bool isSetedClear = false;

    public void SetComplete(bool showEffect, bool delay = false)
    {
        if (showEffect == false)
        {
            StaticScript.SetActiveCheckNULL(gobClear, true);
            StaticScript.SetActiveCheckNULL(gobNotClear, false);
            StaticScript.SetActiveCheckNULL(gobClearParticle, false);
        }
        if (delay == true)
        {
            StaticScript.SetActiveCheckNULL(gobClear, false);
            StaticScript.SetActiveCheckNULL(gobNotClear, true);
            StaticScript.SetActiveCheckNULL(gobClearParticle, true);
        }

        if (isSetedClear == false)
        {
            if (showEffect == true)
            {
                Invoke("DelayStartClearParticle", 1.0f);
                //StartCoroutine(DelayStartClearParticle());
            }
        }

        isSetedClear = true;
    }

    private void DelayStartClearParticle()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ClearStar");
        StaticScript.SetActiveCheckNULL(gobClear, true);
        StaticScript.SetActiveCheckNULL(gobNotClear, false);
        StaticScript.SetActiveCheckNULL(gobClearParticle, true);
    }

    public void SetNotComplete()
    {
        StaticScript.SetActiveCheckNULL(gobClear, false);
        StaticScript.SetActiveCheckNULL(gobNotClear, true);
    }

    private void OnDestroy()
    {
    }

    private void Start()
    {
        prevTargetPos = _rectBody.position;
        //followTween = _rectHairDust.DOMove(_rectBody.position, hairFollowDuration).SetEase(Ease.InOutBounce).SetAutoKill(false);
        StaticScript.SetActiveCheckNULL(gobClearParticle, false);
    }

    private void OnEnable()
    {
        StartCoroutine(DelayStart());
    }

    private IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(2.0f);
        if (_rectBody != null && gameObject != null)
            StartCoroutine(IEMoleJump());
    }

    private void MoleJump()
    {
        if (_rectBody == null)
            return;

        _rectBody.transform.DOJump(_rectBody.transform.position, UnityEngine.Random.Range(_bodyBounce.x, _bodyBounce.y), 1, 0.5f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                if (_rectBody != null && gameObject != null)
                    StartCoroutine(IEMoleJump());
            });
    }

    private IEnumerator IEMoleJump()
    {
        _hairDustAnimation.loops = -1;
        _hairDustAnimation.DORestart();
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.8f, 2.0f));
        _hairDustAnimation.DOPause();
        MoleJump();
    }

    private void Update()
    {
        if (prevTargetPos != _rectBody.position)
        {
            if (followTween == null)
            {
                followTween = _rectHairDust.DOMove(_rectBody.position, hairFollowDuration).SetEase(Ease.OutBounce).SetAutoKill(false);
            }
            prevTargetPos = _rectBody.position;
            followTween.ChangeEndValue(_rectBody.position, hairFollowDuration, true).SetEase(Ease.OutBounce).Restart();
        }
    }
}