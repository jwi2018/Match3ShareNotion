using UnityEngine;
using UnityEngine.UI;

public class EditorTunnelCount : MonoBehaviour
{
    [SerializeField] private Text countText;

    public void SetCount(int _count)
    {
        countText.text = _count.ToString();
    }
}