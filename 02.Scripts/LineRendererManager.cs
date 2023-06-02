using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererManager : MonoBehaviour
{
    [SerializeField] private float DelayTime = 0.07f;

    [SerializeField] private List<Material> LightningMat;

    [SerializeField] private Transform ParentsTransform;

    private void Start()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("BombRainbow");
        StartCoroutine("ChangeOffSet");
    }

    private void Update()
    {
        if (ParentsTransform != null) transform.position = ParentsTransform.position;
    }

    private void OnDestroy()
    {
        for (var i = transform.childCount - 1; i > 0; i--) Destroy(transform.GetChild(i).gameObject);
        foreach (var item in LightningMat)
        {
            var offset = item.GetTextureOffset("_MainTex");
            offset.x = 0;
            offset.y = 0;
            item.SetTextureOffset("_MainTex", offset);
        }
    }

    public void SetParentsTransform(Transform transform)
    {
        ParentsTransform = transform;
        if (ParentsTransform != null) this.transform.position = ParentsTransform.position;
    }

    private IEnumerator ChangeOffSet()
    {
        while (true)
        {
            foreach (var item in LightningMat)
            {
                var offset = item.GetTextureOffset("_MainTex");
                offset.y += 0.167f;
                if (offset.y >= 1) offset.y = 0;
                item.SetTextureOffset("_MainTex", offset);
            }

            yield return new WaitForSeconds(DelayTime);
        }
    }

    private void DestroyChild()
    {
        for (var i = transform.childCount - 1; i > 0; i--) Destroy(transform.GetChild(i).gameObject);
        foreach (var item in LightningMat)
        {
            var offset = item.GetTextureOffset("_MainTex");
            offset.x = 0;
            offset.y = 0;
            item.SetTextureOffset("_MainTex", offset);
        }
    }
}