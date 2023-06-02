using LogicStates;
using UnityEngine;
using UnityEngine.UI;

public class LogicManager : Singleton<LogicManager>
{
    [SerializeField] private Text stateName;

    private readonly StateMachine _stateMachine = new StateMachine();

    private void Update()
    {
        _stateMachine.Update();
        //Debug.Log(string.Format("[버그 추적] LogicManager :: Update : {0}", GetStateName()));
        if (stateName != null) stateName.text = GetStateName();
    }

    private void OnDestroy()
    {
        Debug.Log("[버그 추적] LogicManager :: OnDestroy");
    }

    public void Init()
    {
        _stateMachine.Init(new MapSettingLogic());
    }

    public void ChangeLogicState(LogicState logicState)
    {
        //Debug.LogWarningFormat("KKI  ChangeLogicState :: {0}", logicState);
        _stateMachine.ChangeState(logicState);
    }

    public void Clear()
    {
        ObstacleLogic.ObstacleActive = true;
        _stateMachine.Clear();
    }

    public string GetStateName()
    {
        return _stateMachine.GetStateName();
    }
}