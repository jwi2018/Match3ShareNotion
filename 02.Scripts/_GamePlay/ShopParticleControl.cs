using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AbleLists
{
    public List<GameObject> gameObjects;
}

public class ShopParticleControl : MonoBehaviour
{
    [SerializeField] private List<AbleLists> ableLists = new List<AbleLists>();

    [SerializeField] private float PackageRectY = 300.0f;

    private float StartYPosition;
    private float YPosition;

    private void Start()
    {
        StartYPosition = transform.localPosition.y;
    }

    private void Update()
    {
        //YPosition = transform.localPosition.y - StartYPosition;
        //var Count = (int) YPosition / (int) PackageRectY;
        //YPosition -= Count * PackageRectY;

        //for (var i = 0; i < Count; i++)
        //    foreach (var item in ableLists[i].gameObjects)
        //        item.SetActive(false);
        //for (var i = Count; i < ableLists.Count; i++)
        //{
        //    if (i + Count < 0) break;
        //    if (i <= Count + 1)
        //        foreach (var item in ableLists[i].gameObjects)
        //            item.SetActive(true);
        //    else
        //        foreach (var item in ableLists[i].gameObjects)
        //            item.SetActive(false);
        //}

        //if (YPosition > 50)
        //    if (Count < ableLists.Count)
        //        ableLists[Count].gameObjects[0].SetActive(false);
        //if (YPosition > 100)
        //    if (Count < ableLists.Count && ableLists[Count].gameObjects.Count > 1)
        //        ableLists[Count].gameObjects[1].SetActive(false);
    }
}