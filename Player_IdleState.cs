using UnityEngine;

public class Player_IdleState : EntityState
{
    public Player_IdleState(StateMachine stateMachine, Player player) : base(stateMachine, player)
    {
    }

    public override void Update()
    {
        Debug.Log("idle state update");

        if (Input.GetMouseButtonDown(0))
        {
            _stateMachine.ChangeState(_player.Move);
        }
    }
}