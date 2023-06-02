using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SpriteMover : MonoBehaviour
{
    [SerializeField] private float MoveTime = 0.8f;

    [SerializeField] private Image _mImage;

    private Vector3 vecTargetPosition = Vector3.zero;
    public Action<Sprite> endAction;
    [HideInInspector] private Sprite savedSprite;

    public static void StartMoveTarget(Sprite sprite, float fsize, Vector3 StartPosition, Vector3 TargetPosition, Action<Sprite> _endAction)
    {
        GameObject gobUICanvas = GameObject.Find("UICanvas");

        GameObject gob = PrefabRegister.GetInstance.GetPrefab("SpriteMover", gobUICanvas.transform);
        gob.transform.position = StartPosition;
        SpriteMover loadedScript = gob.GetComponent<SpriteMover>();
        loadedScript._mImage.sprite = sprite;
        loadedScript.endAction = _endAction;
        loadedScript.StartMove(TargetPosition, fsize);
    }

    private void StartMove(Vector3 TargetPosition, float ExceptionSize = 1.0f)
    {
        vecTargetPosition = TargetPosition;

        var Min = Mathf.Min(100.0f * ExceptionSize / _mImage.GetComponent<RectTransform>().sizeDelta.x,
            100.0f * ExceptionSize / _mImage.GetComponent<RectTransform>().sizeDelta.y);
        var Sizex = _mImage.GetComponent<RectTransform>().sizeDelta.x * Min;
        var Sizey = _mImage.GetComponent<RectTransform>().sizeDelta.y * Min;
        _mImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Sizex, Sizey);
        _mImage.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        _mImage.GetComponent<RectTransform>().localRotation = new Quaternion(0, 0, 0, 0);
        MoveCubicBezier();
        //StartCoroutine(MoveAnimation(transform.position));
    }

    private float randomRangeX = 0.4f;
    private float pivotRangeX = 1.0f;

    private void MoveCubicBezier()
    {
        var wayPoints = new Vector3[3];

        float pivotminx = Mathf.Lerp(vecTargetPosition.x, transform.position.x, 0.5f) - pivotRangeX;
        float minX = Random.Range(pivotminx - randomRangeX, pivotminx + randomRangeX);

        float pivotmaxx = Mathf.Lerp(vecTargetPosition.x, transform.position.x, 0.5f) + pivotRangeX;
        float maxX = Random.Range(pivotmaxx - randomRangeX, pivotmaxx + randomRangeX);

        float decideX = Random.Range(0, 100) < 50 ? minX : maxX;

        wayPoints.SetValue(transform.position, 0);
        wayPoints.SetValue(new Vector3(decideX, Mathf.Lerp(vecTargetPosition.y, transform.position.y, 0.5f), 0), 1);
        wayPoints.SetValue(vecTargetPosition, 2);

        transform.DOPath(wayPoints, 100f, PathType.CatmullRom).SetSpeedBased(true).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            if (null != endAction)
            {
                endAction.Invoke(_mImage.sprite);
            }
            Destroy(gameObject);
        });
    }

    private void MoveEnd()
    {
        if (null != endAction)
        {
            endAction.Invoke(_mImage.sprite);
        }
        Destroy(gameObject);
    }

    private IEnumerator DelayClose(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}