using UnityEngine;

public class Player_MoveState : EntityState
{
    public Player_MoveState(StateMachine stateMachine, Player player) : base(stateMachine, player)
    {
    }


    public override void Update()
    {
        Debug.Log("move state update");
        if (_player.MoveInput.x == 0)
        {
            _stateMachine.ChangeState(_player.Idle);
            return;
        }

        var pos = _player.transform.position;
        pos.x += _player.MoveInput.x * _player.MoveSpeed * Time.deltaTime;
        _player.transform.position = pos;
    }
}