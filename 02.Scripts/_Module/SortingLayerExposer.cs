using UnityEngine;

/// <summary>
///     Place on a 3D object to modify its visibility relative to Sprite 'sorting
///     layers'.
/// </summary>
[ExecuteInEditMode]
public sealed class SortingLayerExposer : MonoBehaviour
{
    [SerializeField] private string SortingLayerName = "Default";

    [SerializeField] private int SortingOrder;

    public void OnEnable()
    {
        apply();
    }

    public void OnValidate()
    {
        apply();
    }

    private void apply()
    {
        var meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.sortingLayerName = SortingLayerName;
        meshRenderer.sortingOrder = SortingOrder;
    }
}