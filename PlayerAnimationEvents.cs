using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private Player _player;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
    }

    private void EnableMoveAndJump() => _player.EnableMoveAndJump(true);
    private void DisableMoveAndJump() => _player.EnableMoveAndJump(false);
}