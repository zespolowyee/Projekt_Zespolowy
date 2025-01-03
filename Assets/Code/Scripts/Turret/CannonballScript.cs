using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class CannonballScript : NetworkBehaviour
{
	[SerializeField] private LayerMask whatIsTarget;
	[SerializeField] private int ballDamage;
	[Tooltip("After this time the ball will be destroyed (Helpful when ball doesn't hit the target)")]
	[SerializeField] private float cannonballLifetime = 5f;
	private void OnCollisionEnter(Collision other)
	{

		if (((1 << other.gameObject.layer) & whatIsTarget) == 0)
		{
			return;	
		}

		if (other.gameObject.TryGetComponent<HPSystem>(out var otherHp))
		{
			otherHp.TakeDamage(ballDamage);
			Destroy(gameObject);
		}

	}

	public CannonballScript SetTargetLayer(LayerMask layer)
	{
		whatIsTarget = layer;
		return this;
	}
	
	public CannonballScript SetBallDamage(int damage)
	{
		ballDamage = damage;
		return this;
	}

	public void Start()
	{
		Destroy(gameObject, cannonballLifetime);
	}
}
