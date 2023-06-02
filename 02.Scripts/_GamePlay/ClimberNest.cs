using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimberNest : MonoBehaviour
{
    [SerializeField] private SpriteRenderer climberNestRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHighlightClimberNest(bool isHighlight)
    {
        if(isHighlight)
        {
            climberNestRenderer.maskInteraction = SpriteMaskInteraction.None;
            climberNestRenderer.sortingOrder = 1010;
            climberNestRenderer.gameObject.layer = 5;
        }
        else
        {
            climberNestRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            climberNestRenderer.sortingOrder = 9;
            climberNestRenderer.gameObject.layer = 0;
        }
    }
}
