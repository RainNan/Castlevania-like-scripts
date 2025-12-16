public class StateMachine
{
    public EntityState CurrentState { get; private set; }


    public void Initialize(EntityState startState)
    {
        CurrentState = startState;
        CurrentState.Enter();
    }

    public void ChangeState(EntityState newState)
    {
        if (newState == null) return;
        if (CurrentState == newState) return;

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}