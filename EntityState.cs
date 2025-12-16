using UnityEngine;

public abstract class EntityState
{
    protected StateMachine _stateMachine;
    protected Player _player;

    public EntityState(StateMachine stateMachine, Player player)
    {
        _stateMachine = stateMachine;
        _player = player;
    }

    public virtual void Enter()
    {
        Debug.Log("Enter entity state");
    }

    public virtual void Update()
    {
        Debug.Log("Update from EntityState");
    }

    public virtual void Exit()
    {
        Debug.Log("Exit entity state");

        // 这里要注意虽然没有继承MonoBehaviour但是我们引入了UnityEngine可以使用Input方法
        // 如果继承了MonoBehaviour，游戏运行会反射运行相关固定方法名的方法
        // Input.GetButtonDown()
    }
}