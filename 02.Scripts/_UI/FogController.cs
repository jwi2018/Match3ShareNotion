using UnityEngine;

public class FogController : MonoBehaviour
{
    [SerializeField] private Camera weatherCamera;

    private void Start()
    {
        weatherCamera.enabled = true;
    }
}