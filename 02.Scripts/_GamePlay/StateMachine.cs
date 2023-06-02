using UnityEngine;

public class StateMachine
{
    private LogicState _current_state;

    public void Init(LogicState initState)
    {
        ChangeState(initState);
    }

    public void Update()
    {
        if (_current_state != null) _current_state.Update();
    }

    public void ChangeState(LogicState newState)
    {
        if (newState == null) return;
        if (_current_state != null) _current_state.Exit();
        _current_state = newState;
        _current_state.Enter();
    }

    public void Set_CurrentState(LogicState state)
    {
        _current_state = state;
    }

    public LogicState GetCurrentState()
    {
        return _current_state;
    }

    public void Clear()
    {
        if (_current_state != null) _current_state.Exit();
        _current_state = null;
    }

    public string GetStateName()
    {
        if (_current_state != null)
            return _current_state.GetStateName();
        return null;
    }
}