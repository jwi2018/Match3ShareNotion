using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyObject : MonoBehaviour
{
    public void Start()
    {
        this.transform.localScale = new Vector3(1, 1, 1);
    }

    public SpriteRenderer dummyRenderer = null;
    public SpriteRenderer dummyRendererAdditive = null;
    public SpriteRenderer dummyLight = null;

    public void SetHighlight(bool value)
    {
        if (dummyRenderer == null || dummyLight == null) return;

        if (value)
        {
            dummyRenderer.sortingOrder = (int)EDepth.NORMAL + 1000;
            dummyRenderer.gameObject.layer = 5;
            dummyRenderer.maskInteraction = SpriteMaskInteraction.None;

            if (dummyRendererAdditive != null)
            {
                dummyRendererAdditive.sortingOrder = (int)EDepth.NORMAL + 1000;
                dummyRendererAdditive.gameObject.layer = 5;
                dummyRendererAdditive.maskInteraction = SpriteMaskInteraction.None;
            }

            dummyLight.sortingOrder = (int)EDepth.NORMAL + 999;
            dummyLight.gameObject.layer = 5;
            dummyLight.maskInteraction = SpriteMaskInteraction.None;
        }
        else
        {
            dummyRenderer.sortingOrder = (int)EDepth.NORMAL;
            dummyRenderer.gameObject.layer = 0;
            dummyRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

            if (dummyRendererAdditive != null)
            {
                dummyRendererAdditive.sortingOrder = (int)EDepth.NORMAL;
                dummyRendererAdditive.gameObject.layer = 0;
                dummyRendererAdditive.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }

            dummyLight.sortingOrder = (int)EDepth.NORMAL - 1;
            dummyLight.gameObject.layer = 0;
            dummyLight.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
    }

    public void Clear()
    {
        Destroy(this.gameObject);
    }
}