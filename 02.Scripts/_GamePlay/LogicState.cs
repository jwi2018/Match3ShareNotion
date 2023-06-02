public abstract class LogicState
{
    protected string STATE_NAME;
    protected float StateTime;
    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();

    public string GetStateName()
    {
        return STATE_NAME;
    }
}