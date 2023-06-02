using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TogglesActivator : MonoBehaviour
{
    [SerializeField] private GameObject gobActiveTarget;
    private Toggle m_Toggle;

    //[System.Serializable] public class ToggleEvent : UnityEvent<Toggle> { }
    //[SerializeField] private ToggleEvent onActiveTogglesChanged;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        m_Toggle = GetComponent<Toggle>();
        m_Toggle.onValueChanged.AddListener(delegate
        {
            ToggleValueChanged(m_Toggle);
        });
    }

    public void ToggleValueChanged(Toggle change)
    {
        gobActiveTarget.SetActiveSelf(m_Toggle.isOn);
    }
}