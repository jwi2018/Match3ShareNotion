using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private SpriteRenderer arrowRenderer;

    public Vector2Int matrix = new Vector2Int(-1, -1);

    private GameTile tile;

    private void Update()
    {
        if (arrowRenderer == null) return;

        if (TileManager.GetInstance.IsPreViewTile(tile))
            arrowRenderer.maskInteraction = SpriteMaskInteraction.None;
        else
            arrowRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
    }

    public void Setting(Vector2Int m)
    {
        matrix = m;
        tile = TileManager.GetInstance.GetTileOrNull(m);
    }

    public void SetHighlightArrow(bool isHighlight)
    {
        if (isHighlight)
        {
            //arrowRenderer.sortingOrder += 900;
            arrowRenderer.gameObject.layer = 5;
        }
        else
        {
            //arrowRenderer.sortingOrder += 0;
            arrowRenderer.gameObject.layer = 0;
        }
    }
}