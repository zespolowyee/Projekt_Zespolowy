using UnityEngine;
using UnityEngine.Events;

public class BaseHP : HPSystem
{
    public static event System.Action OnBaseDestroyed;

    public UnityEvent OnBaseTakeDamage;
    public UnityEvent OnBaseDie;

    protected override void Die()
    {
        base.Die();
        OnBaseDestroyed?.Invoke();
        Debug.Log("Base has been destroyed.");
        OnBaseDie?.Invoke();
    }
    protected override void Start()
    {
        base.Start();
        gameObject.layer = LayerMask.NameToLayer("Base");
        gameObject.tag = "Base";
    }

    //public override void Update()
    //{
    //    base.Update();
        
    //}

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        BaseHitEffect hitEffect = GetComponent<BaseHitEffect>();
        if (hitEffect != null)
        {
            hitEffect.TriggerHitEffect();
        }
        OnBaseTakeDamage?.Invoke();
    }

}
