using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] targetRenderers;

    public EDepth depth = EDepth.NORMAL;

    private readonly List<int> correctList = new List<int>();

    public void Init()
    {
        if (correctList.Count != 0) return;

        for (var i = 0; i < targetRenderers.Length; i++) correctList.Add(targetRenderers[i].sortingOrder);
    }

    public void SetHighlight(bool value)
    {
        if (value)
            for (var i = 0; i < targetRenderers.Length; i++)
            {
                if (targetRenderers[i] == null) continue;
                if (correctList.Count < i) continue;

                targetRenderers[i].sortingOrder = (int) depth + 1000 + correctList[i];
                targetRenderers[i].gameObject.layer = 5;
                targetRenderers[i].maskInteraction = SpriteMaskInteraction.None;
            }
        else
            for (var i = 0; i < targetRenderers.Length; i++)
            {
                if (targetRenderers[i] == null) continue;
                if (correctList.Count < i) continue;

                targetRenderers[i].sortingOrder = (int) depth + correctList[i];
                targetRenderers[i].gameObject.layer = 0;
                targetRenderers[i].maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
    }
}