using UnityEngine;

public abstract class State
{
    public bool isComplete { get; protected set; }
    

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Handle() { }


}
