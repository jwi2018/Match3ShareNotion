using UnityEngine;

public class EditorRailItemStatus : MonoBehaviour
{
    [SerializeField] private int TileKind;

    [SerializeField] private EditorRailItemController _controller;

    public int GetTileKind => TileKind;
}