using System;
using UnityEngine;

public class Entity_AnimationEvents : MonoBehaviour
{
    private Entity entity;

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }

    public void OnBasicAttackEnd()
    {
        entity.OnBasicAttackEnd();
    }

    public void OnAttack()
    {
        entity.OnAttack();
    }
}