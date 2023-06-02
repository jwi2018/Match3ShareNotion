using UnityEngine;

public class PlayManager : Singleton<PlayManager>
{
    private void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        DynamicObjectPool.GetInstance.Init();
        DataContainer.GetInstance.InitData();
        LogicManager.GetInstance.Init();
    }

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.P))
        {
            BlockManager.GetInstance.PopTest();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            TileManager.GetInstance.DropTest();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            TileManager.GetInstance.CreateBlockTest();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            TileManager.GetInstance.Down();
        }
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            TileManager.GetInstance.Up();
        }
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            TileManager.GetInstance.Right();
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            TileManager.GetInstance.Left();
        }
        */
    }
}