using UnityEngine;

public class Rainbow : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    [SerializeField] private RectTransform _rectTransform;

    private Vector2Int _matrix;

    public void Setting(Vector2Int matrix)
    {
        _matrix = matrix;
        //TileManager.GetInstance.SetRectPositionUseMatrix(_rectTransform, matrix);
    }

    public void ActiveLightning(Vector2 endPosition, GameTile tile)
    {
        var lightningObj = DynamicObjectPool.GetInstance.GetObjectForType("RainbowLine", true);
        if (lightningObj != null)
        {
            var lighting = lightningObj.GetComponent<LightningAnimScript>();
            var position = _rectTransform.anchoredPosition;

            lighting.SetStartPosition = position;
            lighting.SetEndPosition = endPosition;
            lighting.Setting(tile);
        }
    }

    public void EndRainbow()
    {
        _animator.SetTrigger("End");
    }
}