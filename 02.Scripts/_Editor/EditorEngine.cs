using UnityEngine;

public class EditorEngine : MonoBehaviour
{
    [SerializeField] private MapEditor editor;

    private void Start()
    {
        DataContainer.GetInstance.InitData();
        StageManager.GetInstance.Init();
        DynamicObjectPool.GetInstance.Init();

        if (editor != null) editor.Init();
    }
}