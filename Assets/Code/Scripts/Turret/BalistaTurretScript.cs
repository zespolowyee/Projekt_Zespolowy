using System;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class BalistaTurretScript : NetworkBehaviour
{
	[Header("Rotation objects")]
	[SerializeField] private Transform bowObject;
	[SerializeField] private Transform bowClamp;
	[SerializeField] private Transform bowShootingPoint;
	[SerializeField] private Transform balistaPivot;
	
	[Header("Turret's target")]
	[SerializeField] private LayerMask targetLayer;
	
	[Header("Up and down rotation limits")]
	[SerializeField] private float maxUpRotation = -10;
	[SerializeField] private float maxDownRotation = 20;
	
	[SerializeField] private LineRenderer lineRendererPrefab;
	[SerializeField] private float beamDuration = 0.1f;
	
	private Collider _currentTarget;
	private float _timeElapsed = 0f;
	private NetStatController _turretStats;

	bool FindClosestTarget()
	{
		Collider[] targets = Physics.OverlapSphere(transform.position, _turretStats.GetNetStatValue(NetStatType.Range), targetLayer);
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
		balistaPivot.LookAt(targetPostition);

		//Lower or raise the hidden element that is at the same position as barrel
		bowClamp.LookAt(_currentTarget.transform.position);

		//Get the rotation from the hidden element
		Vector3 clampedRotation = bowClamp.eulerAngles;

		//Clamp the x rotation of barrel
		float xRotation = clampedRotation.x;

		if (xRotation > 180)
		{
			xRotation -= 360;
		}

		clampedRotation.x = Math.Clamp(xRotation, maxUpRotation, maxDownRotation);

		//Apply the rotation with clamped x to the barrel
		bowObject.eulerAngles = clampedRotation;
	}
	
	void DrawBeam(Vector3 start, Vector3 end)
	{
		var beam = Instantiate(lineRendererPrefab);
		beam.SetPosition(0, start);
		beam.SetPosition(1, end);
		Destroy(beam.gameObject, beamDuration);
	}

	void ShootAtTarget()
	{
		Ray ray = new Ray(bowShootingPoint.transform.position, bowShootingPoint.transform.forward);
		RaycastHit hit;

		// Perform the raycast
		if (Physics.Raycast(ray, out hit, _turretStats.GetNetStatValue(NetStatType.Range), targetLayer))
		{
			if (hit.collider.TryGetComponent<HPSystem>(out var otherHp))
			{
				otherHp.TakeDamage((int)_turretStats.GetNetStatValue(NetStatType.Damage));
				DrawBeam(bowShootingPoint.transform.position, hit.transform.position);
			}
		}

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

	void Awake()
	{
		_turretStats = GetComponent<TurretStats>();
	}

	void Update()
	{
		_timeElapsed += Time.deltaTime;

		var found = FindClosestTarget();
		if (!found)
		{
			return;
		}

		LookAtTarget();

		if (_timeElapsed >= _turretStats.GetNetStatValue(NetStatType.ShootingInterval))
		{
			if (IsServer)
			{
				ShootAtTargetServerRpc();
			}

			_timeElapsed = 0f;
		}

	}

}
