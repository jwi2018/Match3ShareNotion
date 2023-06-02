using UnityEngine;

public class Spin : MonoBehaviour
{
    public bool IsActive;
    public float spinSpeed = 720.0f;

    [SerializeField] private Transform[] transforms;

    private void FixedUpdate()
    {
        if (transform == null) return;

        if (IsActive)
            for (var i = 0; i < transforms.Length; i++)
            {
                if (transforms[i] == null) continue;

                transforms[i].Rotate(Vector3.forward, spinSpeed * Time.deltaTime);
            }
    }
}