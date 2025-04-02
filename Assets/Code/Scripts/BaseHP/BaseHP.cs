using UnityEngine;

public class BaseHP : HPSystem
{
    public static event System.Action OnBaseDestroyed;

    protected override void Die()
    {
        base.Die();
        OnBaseDestroyed?.Invoke();
        Debug.Log("Base has been destroyed.");
    }
    protected override void Start()
    {
        base.Start();
        gameObject.layer = LayerMask.NameToLayer("Base");
        gameObject.tag = "Base";
    }

    public override void Update()
    {
        base.Update();
        
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        BaseHitEffect hitEffect = GetComponent<BaseHitEffect>();
        if (hitEffect != null)
        {
            hitEffect.TriggerHitEffect();
        }
    }

}
