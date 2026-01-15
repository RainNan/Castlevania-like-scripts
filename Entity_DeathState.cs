using UnityEngine;

public class Entity_DeathState : EntityState
{
    public Entity_DeathState(StateMachine stateMachine, Entity entity) : base(stateMachine, entity)
    {
    }

    public override void Enter()
    {
        base.Enter();
        entity.OnDead();
        var collider2D = entity.GetComponent<Collider2D>();
        collider2D.enabled = false;
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
    }
}