using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class CannonballScript : NetworkBehaviour
{
	[SerializeField] private LayerMask whatIsTarget;
	private void OnCollisionEnter(Collision other)
	{

		if (((1 << other.gameObject.layer) & whatIsTarget) == 0)
		{
			return;	
		}

		if (other.gameObject.TryGetComponent<HPSystem>(out var otherHp))
		{
			otherHp.TakeDamage(20);
			Destroy(gameObject);
		}

	}

	public void SetTargetLayer(LayerMask layer)
	{
		whatIsTarget = layer;
	}

	public void Start()
	{
		Destroy(gameObject, 5f);
	}
}
