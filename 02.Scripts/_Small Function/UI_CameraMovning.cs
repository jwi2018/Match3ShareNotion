using UnityEngine;

public class UI_CameraMovning : MonoBehaviour
{
    [SerializeField] private Transform _MainCamera_Transform;

    public void Update()
    {
        transform.position = _MainCamera_Transform.position;
        transform.GetComponent<Camera>().orthographicSize =
            _MainCamera_Transform.GetComponent<Camera>().orthographicSize;
    }
}