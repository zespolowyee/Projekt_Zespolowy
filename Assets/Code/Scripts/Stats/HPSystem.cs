using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class HPSystem : NetworkBehaviour
{
	NetworkVariable<int> currentHP = new NetworkVariable<int>(100);
	public int maxHP = 100;
	private bool isDead = false;

	public Animator animator;
	//private int currentHP;
	
	protected virtual void Start()
	{
		if (IsServer)
		{
			currentHP.Value = maxHP;
		}

		animator = GetComponent<Animator>();
	}

	public virtual void Update()
	{
		if (!isDead && Input.GetKeyDown(KeyCode.P) && animator != null)
		{
			TakeDamage(100);
		}
	}

	public virtual void TakeDamage(int damage)
	{
		if (IsServer)
		{
			currentHP.Value -= damage;
			Debug.Log(gameObject.name + " took: " + damage + " damage" + " hp left:" + currentHP.Value);
			if (currentHP.Value <= 0)
			{
				currentHP.Value = 0;
				Die();
			}
		}
	}

	protected virtual void Die()
	{
		isDead = true;
		if (animator != null)
		{
			animator.Play("Die", -1, 0f);
			Debug.Log($"{gameObject.name} has died.");
		}
	}
}