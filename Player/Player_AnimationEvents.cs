using System;
using UnityEngine;

public class Player_AnimationEvents : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    public void OnBasicAttackEnd()
    {
        player.OnBasicAttackEnd();
    }
}