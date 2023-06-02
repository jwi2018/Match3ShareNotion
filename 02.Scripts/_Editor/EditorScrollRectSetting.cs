using UnityEngine;
using UnityEngine.UI;

public class EditorScrollRectSetting : MonoBehaviour
{
    [SerializeField] private RectTransform _viewPort;

    private void Start()
    {
        GetComponent<ScrollRect>().content = _viewPort;
    }
}