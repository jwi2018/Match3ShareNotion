using DG.Tweening;
using LogicStates;
using UnityEngine;

public class CameraWork : MonoBehaviour
{
    public static bool IsMoving;
    [SerializeField] private Transform cameraTransform;

    private void Start()
    {
        var TARGET_WIDTH = 750.0f;
        var TARGET_HEIGHT = 1334.0f;
        var PIXELS_TO_UNITS = 100; // 1:1 ratio of pixels to units

        var desiredRatio = TARGET_WIDTH / TARGET_HEIGHT;
        var currentRatio = Screen.width / (float) Screen.height;
        /*
        Camera camera = GetComponent<Camera>();
        float value = Screen.currentResolution.height * 0.5f * 0.01f;
        camera.orthographicSize = value;
        */

        if (currentRatio >= desiredRatio)
        {
            // Our resolution has plenty of width, so we just need to use the height to determine the camera size
            Camera.main.orthographicSize = TARGET_HEIGHT / 2 / PIXELS_TO_UNITS;
        }
        else
        {
            // Our camera needs to zoom out further than just fitting in the height of the image.
            // Determine how much bigger it needs to be, then apply that to our original algorithm.
            var differenceInSize = desiredRatio / currentRatio;
            Camera.main.orthographicSize = TARGET_HEIGHT / 2 / PIXELS_TO_UNITS * differenceInSize;
        }
    }

    public void Move(Vector2 destination, float duration)
    {
        var d = new Vector3(destination.x, destination.y, -10);
        IsMoving = true;
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(d, duration));
        sequence.AppendCallback(() => IsMoving = false);
        sequence.AppendCallback(() => LogicManager.GetInstance.ChangeLogicState(new PopLogic()));
    }

    public void SetPosition(Vector2 position)
    {
        transform.position = position;
    }
}