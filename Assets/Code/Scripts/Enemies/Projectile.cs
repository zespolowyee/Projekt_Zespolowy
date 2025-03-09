using System.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : NetworkBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float speed;
    [SerializeField] private float lifetime;
    [SerializeField] private LayerMask whatIsTarget;
    private Rigidbody rb;
    public float Speed { get => speed; set => speed = value; }
    public int Damage { get => damage; set => damage = value; }
    public LayerMask WhatIsTarget { get => whatIsTarget; set => whatIsTarget = value; }
    public float Lifetime { get => lifetime; set => lifetime = value; }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Shoot();
        if (IsServer)
        {
            StartCoroutine(DestroyAfterDelay(lifetime));
        }

    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & whatIsTarget) == 0)
        {
            return;
        }

        if (other.gameObject.TryGetComponent<HPSystem>(out var otherHp))
        {

            otherHp.TakeDamage(damage);


            Destroy(gameObject);
        }
    }

    public void SetTargetLayer(LayerMask layer)
    {
        whatIsTarget = layer;
    }

    protected virtual void Shoot()
    {
        rb.linearVelocity = transform.forward * speed;
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
