using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] float cooldown;

    [SerializeField] private Vector3 attackPos1;
    [SerializeField] private Vector3 attackPos2;
    [SerializeField] private float attackRadius;
    public virtual bool CheckAttackConditon()
    {
        return true;
    }

    public virtual void PerformAttack()
    {
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
