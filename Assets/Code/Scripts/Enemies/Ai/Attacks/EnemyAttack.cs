using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] float cooldown;

    
    [SerializeField] private Vector3 attackPos1;
    [SerializeField] private Vector3 attackPos2;
    [SerializeField] private float attackRadius;

    private float lastPerformedTime;
    private EnemyNavigation controller;

    public virtual void SetupAttack(EnemyNavigation controller)
    {
        this.controller = controller;
        lastPerformedTime = Time.time;
    }
    public virtual bool CheckAttackConditon()
    {
        if (Time.time >= lastPerformedTime + cooldown)
        {
            return true;
        }

        return false;
    }

    public virtual void PerformAttack()
    {
        lastPerformedTime = Time.time;
        Debug.Log("performedAttack");
        Collider[] targets = Physics.OverlapCapsule(attackPos1, attackPos2, attackRadius);
        foreach (Collider target in targets)
        {
            if (target.gameObject.TryGetComponent<HPSystem>(out var targetHp))
            {
                targetHp.TakeDamage(damage);
            }
        }
        
    }

}
