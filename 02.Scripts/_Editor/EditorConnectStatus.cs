using UnityEngine;

public class EditorConnectStatus : MonoBehaviour
{
    [SerializeField] private EOneWay direction;

    [SerializeField] private EditorConnectController _controller;

    public EOneWay GetDirection => direction;
}