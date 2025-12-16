using UnityEngine;

public class Player : MonoBehaviour
{
    public StateMachine StateMachine { get; private set; }

    public Player_IdleState Idle { get; private set; }
    public Player_MoveState Move { get; private set; }

    private void Awake()
    {
        StateMachine = new StateMachine();

        Idle = new Player_IdleState(StateMachine, this);
        Move = new Player_MoveState(StateMachine, this);
    }

    private void Start()
    {
        StateMachine.Initialize(Idle);
    }

    private void Update()
    {
        StateMachine.CurrentState?.Update(); // 空值保护
    }
}