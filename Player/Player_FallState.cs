using UnityEngine;

namespace DefaultNamespace
{
    public class Player_FallState : PlayerState
    {
        public Player_FallState(StateMachine stateMachine, Player player) : base(stateMachine, player)
        {
        }

        public override void Enter()
        {
            base.Enter();
            player.SetIdleMove(false);
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            // => Dash
            if (player.DashPressed)
            {
                stateMachine.ChangeState(player.Dash);
                return;
            }

            // => Idle/Move
            if (player.IsGrounded)
            {
                if (Mathf.Abs(player.MoveInput.x) > 0.01f)
                    stateMachine.ChangeState(player.Move);
                else
                    stateMachine.ChangeState(player.Idle);

                // 及其重要！！！
                return;
            }

            if (player.IsWallTouched && player.rb.velocity.y < 0)
            {
                stateMachine.ChangeState(player.Slide);
                return;
            }
        }

        public override void PhysicUpdate()
        {
            // 空中水平控制
            var v = player.rb.velocity;
            v.x = player.MoveInput.x * player.MoveSpeed;
            player.rb.velocity = v;

            player.UpdateFacing();
        }
    }
}