using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] protected float explosionRange;
    [SerializeField] protected int explosionDamage;
    [SerializeField] protected LayerMask whatIsTarget;
    protected ParticleSystem explosionParticles;
    protected AudioSource audioSource;

    public virtual void Start()
    {
        explosionParticles = GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }

    public virtual IEnumerator Explode()
    {
        explosionParticles.Play();
        audioSource.Play();
        DisableVisuals();

        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, explosionRange, whatIsTarget);
        List<GameObject> damagedObjects = new List<GameObject>();

        foreach (Collider col in objectsInRange)
        {

            HPSystem target = col.GetComponent<HPSystem>();
            if (target != null && !damagedObjects.Contains(col.gameObject))
            {
                target.TakeDamage(explosionDamage);
                damagedObjects.Add(col.gameObject);
            }
        }
        yield return new WaitForSeconds(audioSource.clip.length);
        Destroy(gameObject);
    }

    protected virtual void DisableVisuals(){}
}
