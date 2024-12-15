using UnityEngine;
using UnityEngine.Events;

public class TMP_EnemyHp : HPSystem
{
	public UnityEvent OnEnemyDeath;
	protected override void Die()
	{
		OnEnemyDeath.Invoke();
		base.Die();
	}
}
