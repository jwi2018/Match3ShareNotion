using UnityEngine;

public class SkipConnecter : MonoBehaviour
{
    [SerializeField] private CongratulationClearPopup clearPopup;

    private void OnMouseDown()
    {
        if (clearPopup != null) clearPopup.TouchSkipScreen();
    }
}