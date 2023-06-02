using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class ScrollRect_fix : ScrollRect
{
    private float verticalScrollbarHandlerSize;
    private float horizontalScrollbarHandlerSize;

    override protected void LateUpdate()
    {
        if (this.horizontalScrollbar)
            horizontalScrollbarHandlerSize = this.horizontalScrollbar.size;
        if (this.verticalScrollbar)
            verticalScrollbarHandlerSize = this.verticalScrollbar.size;

        base.LateUpdate();

        if (this.horizontalScrollbar)
            this.horizontalScrollbar.size = horizontalScrollbarHandlerSize;
        if (this.verticalScrollbar)
            this.verticalScrollbar.size = verticalScrollbarHandlerSize;
    }

    override public void Rebuild(CanvasUpdate executing)
    {
        if (this.horizontalScrollbar)
            horizontalScrollbarHandlerSize = this.horizontalScrollbar.size;
        if (this.verticalScrollbar)
            verticalScrollbarHandlerSize = this.verticalScrollbar.size;

        base.Rebuild(executing);
        if( this.verticalScrollbar.size < 0.06f)
        {
            this.verticalScrollbar.size = 0.06f;
        }


    }
}