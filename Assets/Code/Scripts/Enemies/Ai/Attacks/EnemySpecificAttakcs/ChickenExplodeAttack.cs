using UnityEngine;

public class ChickenExplodeAttack : EnemyAttack
{
    [SerializeField] Explosion explosion;
    [SerializeField] EnemyHp hp;
    bool exploded = false;
    public override void SetupAttack(EnemyNavigation controller)
    {
        base.SetupAttack(controller);
        explosion = GetComponent<Explosion>();

    }
    public override void PerformAttack()
    {
        if (!exploded)
        {
            StartCoroutine(explosion.Explode());
        }
        exploded = true;
        transform.parent = null;
        Destroy(gameObject, 1f);
        hp.Die();

    }
}
