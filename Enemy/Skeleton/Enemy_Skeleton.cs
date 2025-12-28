public class Enemy_Skeleton : Enemy
{
    protected override void Start()
    {
        base.Start();
        
        StateMachine.Initialize(idle);
    }

    protected override void Update()
    {
        base.Update();
        
        
    }
}