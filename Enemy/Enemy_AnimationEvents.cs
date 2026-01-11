using System;
using UnityEngine;

public class Enemy_AnimationEvents:MonoBehaviour
{
    private Enemy _enemy;

    private void Awake()
    {
        _enemy = GetComponentInParent<Enemy>();
    }

    public void OnAttackEnd()
    {
        _enemy.OnAttackEnd();
        Debug.Log("OnAttackEnd");
    }

    public void OnAttack() => _enemy.OnAttack();

    public void OnTakeDamageEnd() => _enemy.OnTakeDamageEnd();
}
