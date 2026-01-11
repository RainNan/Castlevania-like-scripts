public class Enemy_Skeleton : Enemy
{
    protected override void Start()
    {
        base.Start();
        
        StateMachine.Initialize(Idle);
    }

    protected override void Update()
    {
        base.Update();
    }
}