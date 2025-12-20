using UnityEngine;

namespace DefaultNamespace
{
    public class Player_FallState : EntityState
    {
        public Player_FallState(StateMachine stateMachine, Player player) : base(stateMachine, player)
        {
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // 落地 -> Idle/Move
            if (_player.IsGrounded)
            {
                if (Mathf.Abs(_player.MoveInput.x) > 0.01f)
                    _stateMachine.ChangeState(_player.Move);
                else
                    _stateMachine.ChangeState(_player.Idle);

                // 及其重要！！！
                return;
            }

            if (_player.IsWallTouched && _player.rb.velocity.y < 0)
            {
                _stateMachine.ChangeState(_player.Slide);
                return;
            }
        }

        public override void PhysicUpdate()
        {
            // 空中水平控制
            var v = _player.rb.velocity;
            v.x = _player.MoveInput.x * _player.MoveSpeed;
            _player.rb.velocity = v;

            _player.UpdateFacing();
        }
    }
}