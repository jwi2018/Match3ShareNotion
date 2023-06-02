using UnityEngine;

public class TabletContainer : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Vector2Int[] size;

    public Sprite GetSprite(int num)
    {
        return sprites[num];
    }

    public Vector2Int GetSize(int num)
    {
        return size[num];
    }
}