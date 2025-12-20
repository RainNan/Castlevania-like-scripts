public class StateMachine
{
    public EntityState CurrentState { get; private set; }

    /// <summary>
    /// 是否处于状态转移中
    /// </summary>
    private bool _isTransitioning;

    public void Initialize(EntityState startState)
    {
        CurrentState = startState;
        _isTransitioning = true;
        CurrentState.Enter();
        _isTransitioning = false;
    }

    public void ChangeState(EntityState newState)
    {
        if (newState == null) return;
        if (_isTransitioning) return;
        if (ReferenceEquals(CurrentState, newState)) return;

        _isTransitioning = true;

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();

        _isTransitioning = false;
    }

    public void LogicUpdate() => CurrentState?.LogicUpdate();
    public void PhysicUpdate() => CurrentState?.PhysicUpdate();
}