using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private Player _player;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
    }

    private void DamageEnemies() => _player.DamageEnemy();

    private void EnableMoveAndJump() => _player.EnableMoveAndJump(true);
    private void DisableMoveAndJump() => _player.EnableMoveAndJump(false);
}