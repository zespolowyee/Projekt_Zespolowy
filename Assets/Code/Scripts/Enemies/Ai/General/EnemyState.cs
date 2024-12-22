using UnityEngine;

public class EnemyState: State
{
    public bool CanEnterToItself = false;
    public bool CanExit { get; protected set; }
    protected EnemyNavigation controller;

    protected EnemyState previousState;
    public virtual void Setup(EnemyNavigation controller)
    {
        this.controller = controller;
        CanExit = true;
    }
    public virtual void Enter(EnemyState previousState)
    {
        this.previousState = previousState;
    }
}
