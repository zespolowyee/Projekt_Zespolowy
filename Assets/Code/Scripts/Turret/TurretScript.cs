using System;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class TurretScript : NetworkBehaviour, ITurretScript
{
	[Header("Rotation objects")]
	[SerializeField] private Transform barrelObject;
	[SerializeField] private Transform barrelClamp;
	[SerializeField] private Transform barrelShootingPoint;
	[SerializeField] private Transform cannonPivot;
	
	[Header("Cannonball info")]
	[SerializeField] private GameObject cannonball;
	[SerializeField] private float cannonballVelocity;
	
	[Header("Turret's target")]
	[SerializeField] private LayerMask targetLayer;
	
	[Header("Up and down rotation limits")]
	[SerializeField] private float maxUpRotation = -10;
	[SerializeField] private float maxDownRotation = 20;
	
	[Header("Turret's stats")]
	[SerializeField] private float detectionRange = 5;
	[SerializeField] private float shootingInterval = 3;
	[SerializeField] private int damage = 5;

	private Collider _currentTarget;
	private float timeElapsed = 0f;

	public ITurretScript SetDetectionRange(float detectionRange)
	{
		this.detectionRange = detectionRange;
		return this;
	}

	public ITurretScript SetShootingInterval(float shootingInterval)
	{
		this.shootingInterval = shootingInterval;
		return this;
	}

	public ITurretScript SetDamage(int damage)
	{
		this.damage = damage;
		return this;
	}
	
	
	bool FindClosestTarget()
	{
		Collider[] targets = Physics.OverlapSphere(transform.position, detectionRange, targetLayer);
		if (targets.Length > 0)
		{

			//Find the closest target
			float smallestDistance = Vector3.Distance(transform.position, targets[0].transform.position);
			Collider closestTarget = targets[0];
			foreach (Collider target in targets)
			{
				var distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
				if (distanceToTarget < smallestDistance)
				{
					smallestDistance = distanceToTarget;
					closestTarget = target;
				}
			}

			_currentTarget = closestTarget;
			return true;
		}

		return false;
	}

	void LookAtTarget()
	{
		//Rotate the cannon without moving lowering or raising the barrel
		Vector3 targetPostition = new Vector3(_currentTarget.transform.position.x,
			transform.position.y,
			_currentTarget.transform.position.z);
		cannonPivot.LookAt(targetPostition);

		//Lower or raise the hidden element that is at the same position as barrel
		barrelClamp.LookAt(_currentTarget.transform.position);

		//Get the rotation from the hidden element
		Vector3 clampedRotation = barrelClamp.eulerAngles;

		//Clamp the x rotation of barrel
		float xRotation = clampedRotation.x;

		if (xRotation > 180)
		{
			xRotation -= 360;
		}

		clampedRotation.x = Math.Clamp(xRotation, maxUpRotation, maxDownRotation);

		//Apply the rotation with clamped x to the barrel
		barrelObject.eulerAngles = clampedRotation;
	}

	void ShootAtTarget()
	{
		var ball = Instantiate(cannonball, barrelShootingPoint.position, barrelShootingPoint.rotation);
		ball.GetComponent<CannonballScript>()
			.SetTargetLayer(targetLayer)
			.SetBallDamage(damage);
		ball.GetComponent<Rigidbody>().linearVelocity = barrelShootingPoint.transform.forward * cannonballVelocity;
	}

	[ServerRpc]
	void ShootAtTargetServerRpc()
	{
		if (IsServer && IsClient)
		{
			ShootAtTargetClientRpc();
		}
		else
		{
			ShootAtTargetClientRpc();
			ShootAtTarget();
		}
	}

	[ClientRpc]
	void ShootAtTargetClientRpc()
	{
		ShootAtTarget();
	}

	void Update()
	{
		timeElapsed += Time.deltaTime;

		var found = FindClosestTarget();
		if (!found)
		{
			return;
		}

		LookAtTarget();

		if (timeElapsed >= shootingInterval)
		{
			if (IsServer)
			{
				ShootAtTargetServerRpc();
			}

			timeElapsed = 0f;
		}

	}

}
