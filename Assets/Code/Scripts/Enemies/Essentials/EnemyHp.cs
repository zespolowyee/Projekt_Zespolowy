using UnityEngine;
using UnityEngine.Events;

public class EnemyHp : HPSystem
{
    private EnemyDeathHandler deathHandler;

    public UnityEvent OnEnemyDeath;
    public UnityEvent OnEnemyTakeDamage;

    [SerializeField] private GameObject DeathParticleEfect;

    protected override void Start()
    {
        base.Start();
        deathHandler = GetComponent<EnemyDeathHandler>();
    }

    public void TakeDamageFromSource(int damage, GameObject source)
    {
        deathHandler.SetAttacker(source);
        TakeDamage(damage);
    }

    protected override void Die()
    {
        base.Die();
        OnEnemyDeath?.Invoke();
        if(DeathParticleEfect != null)
        {
            Instantiate(DeathParticleEfect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        OnEnemyTakeDamage?.Invoke();

    }
}
