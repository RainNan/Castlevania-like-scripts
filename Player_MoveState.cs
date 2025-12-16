using UnityEngine;

public class Player_MoveState:EntityState
{
    public Player_MoveState(StateMachine stateMachine, Player player) : base(stateMachine, player)
    {
    }


    public override void Update()
    {
        Debug.Log("move state update");
        if (Input.GetMouseButtonDown(0))
        {
            _stateMachine.ChangeState(_player.Idle);
        }
    }
}