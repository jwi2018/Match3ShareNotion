using UnityEngine;

public class BigObjectSub : MonoBehaviour
{
    [SerializeField] private SpriteRenderer subRenderer;
    public Animator animator;
    public Vector2Int Matrix = new Vector2Int(0, 0);
    public EDirection Direction = EDirection.NONE;
    public bool Damaged;

    public EColor Color { get; set; }
    public SpriteRenderer Renderer { get; set; }

    public void SetSprite(Sprite sprite)
    {
        subRenderer.sprite = sprite;
    }
}