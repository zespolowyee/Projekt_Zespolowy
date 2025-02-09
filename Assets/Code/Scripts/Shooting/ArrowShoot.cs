using UnityEngine;
using Unity.Netcode;

public class ArrowShoot : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject arrowPrefab;        
    [SerializeField] private Transform arrowSpawnPoint;       
    [SerializeField] private float baseShootForce = 5f;      
    [SerializeField] private float maxShootForce = 40f;       
    [SerializeField] private float chargeSpeed = 5f;         
    [SerializeField] private float shootCooldown = 1f;        

    [Header("Trajectory")]
    [SerializeField] private LineRenderer trajectoryRenderer; 
    [SerializeField] private int lineSegmentCount = 20;       

    [Header("Hand Arrow")]
    [SerializeField] private GameObject handArrow;            

    private BowShooterAnimationController animationController; 
    private float lastShootTime = 0f;                        
    private float currentShootForce;                       

    void Start()
    {
        animationController = GetComponent<BowShooterAnimationController>();
        currentShootForce = baseShootForce;

        if (trajectoryRenderer != null)
        {
            trajectoryRenderer.positionCount = 0; 
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetButton("Fire1"))
        {
            handArrow.SetActive(true);
            currentShootForce = Mathf.Min(currentShootForce + chargeSpeed * Time.deltaTime, maxShootForce);
            DrawTrajectory();
        }

        if (Input.GetButtonUp("Fire1") && animationController.IsDrawing && Time.time >= lastShootTime + shootCooldown)
        {
            ShootServerRpc(currentShootForce);
            lastShootTime = Time.time;
            currentShootForce = baseShootForce; 
        }

        if (!Input.GetButton("Fire1") && trajectoryRenderer != null)
        {
            trajectoryRenderer.positionCount = 0;
        }
    }
    [ServerRpc]
    private void ShootServerRpc(float force)
    {
        ShootClientRpc(force);
    }

    [ClientRpc]
    private void ShootClientRpc(float force)
    {
        handArrow.SetActive(false);

        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        NetworkObject networkObject = arrow.GetComponent<NetworkObject>();

        if (networkObject != null)
        {
            networkObject.Spawn();  // Spawnowanie obiektu na sieci
        }
        Rigidbody rb = arrow.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 shootDirection = arrowSpawnPoint.forward + arrowSpawnPoint.up * 0.05f;
            rb.AddForce(shootDirection.normalized * force, ForceMode.Impulse);
        }
        ArrowHit arrowHit = arrow.GetComponent<ArrowHit>();
        if (arrowHit != null)
        {
            arrowHit.SetAttacker(gameObject);
        }
        Destroy(arrow, 10f);
    }

    private void DrawTrajectory()
    {
        if (trajectoryRenderer == null)
            return;
        Rigidbody arrowrb = arrowPrefab.GetComponent<Rigidbody>();
        float arrowMass = arrowrb.mass;
        Vector3 startPosition = arrowSpawnPoint.position;
        Vector3 startVelocity = (arrowSpawnPoint.forward + arrowSpawnPoint.up * 0.05f) * (currentShootForce / arrowMass);
        trajectoryRenderer.positionCount = lineSegmentCount;

        float timeStep = 0.05f; // Dokładniejszy krok czasu
        Vector3 previousPoint = startPosition;

        for (int i = 0; i < lineSegmentCount; i++)
        {
            float time = i * timeStep;
            Vector3 point = startPosition + startVelocity * time + 0.5f * Physics.gravity * time * time;

            // Sprawdź, czy między poprzednim punktem a obecnym punktem jest kolizja
            if (Physics.Raycast(previousPoint, (point - previousPoint).normalized, out RaycastHit hit, (point - previousPoint).magnitude))
            {
                // Jeśli wykryto kolizję, zakończ rysowanie trajektorii na punkcie kolizji
                trajectoryRenderer.positionCount = i + 1;
                trajectoryRenderer.SetPosition(i, hit.point);
                break;
            }

            trajectoryRenderer.SetPosition(i, point);
            previousPoint = point;
        }
    }

}
