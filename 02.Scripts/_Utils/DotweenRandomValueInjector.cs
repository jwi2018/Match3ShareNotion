using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

[RequireComponent(typeof(DOTweenAnimation))]
public class DotweenRandomValueInjector : MonoBehaviour
{
    public Vector2 _randDuration;
    public Vector2 _randDelay;

    private DOTweenAnimation _targetDotweenAnimation;

    private void Awake()
    {
        _targetDotweenAnimation = GetComponent<DOTweenAnimation>();
    }

    private void Start()
    {
        _targetDotweenAnimation.duration = UnityEngine.Random.Range(_randDuration.x, _randDuration.y);
        _targetDotweenAnimation.delay = UnityEngine.Random.Range(_randDelay.x, _randDelay.y);
    }
}