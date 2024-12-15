using UnityEngine;

public class ArrowShoot : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject arrowPrefab;          // Prefab strzały
    [SerializeField] private Transform arrowSpawnPoint;       // Punkt, z którego wystrzeliwana jest strzała
    [SerializeField] private float baseShootForce = 5f;      // Bazowa siła strzału
    [SerializeField] private float maxShootForce = 40f;       // Maksymalna siła strzału
    [SerializeField] private float chargeSpeed = 5f;         // Szybkość ładowania siły strzału
    [SerializeField] private float shootCooldown = 1f;        // Czas odnowienia między strzałami

    [Header("Trajectory")]
    [SerializeField] private LineRenderer trajectoryRenderer; // LineRenderer do rysowania trajektorii
    [SerializeField] private int lineSegmentCount = 20;       // Ilość segmentów trajektorii

    [Header("Hand Arrow")]
    [SerializeField] private GameObject handArrow;            // Strzała trzymana w ręku podczas celowania

    private BowShooterAnimationController animationController; // Referencja do skryptu animacji
    private float lastShootTime = 0f;                         // Czas ostatniego strzału
    private float currentShootForce;                          // Aktualna siła strzału

    void Start()
    {
        animationController = GetComponent<BowShooterAnimationController>();
        currentShootForce = baseShootForce;

        if (trajectoryRenderer != null)
        {
            trajectoryRenderer.positionCount = 0; // Ukryj trajektorię na początku
        }
    }

    void Update()
    {
        // Rozpoczęcie celowania i ładowanie siły strzału
        if (Input.GetButton("Fire1"))
        {
            handArrow.SetActive(true);
            currentShootForce = Mathf.Min(currentShootForce + chargeSpeed * Time.deltaTime, maxShootForce);
            DrawTrajectory();
        }

        // Wystrzał po puszczeniu przycisku i sprawdzeniu cooldownu
        if (Input.GetButtonUp("Fire1") && animationController.IsDrawing && Time.time >= lastShootTime + shootCooldown)
        {
            Shoot(currentShootForce);
            lastShootTime = Time.time;
            currentShootForce = baseShootForce; // Reset siły strzału do bazowej wartości
        }

        // Ukrywanie trajektorii, gdy nie celujesz
        if (!Input.GetButton("Fire1") && trajectoryRenderer != null)
        {
            trajectoryRenderer.positionCount = 0;
        }
    }

    private void Shoot(float force)
    {
        handArrow.SetActive(false);

        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        Rigidbody rb = arrow.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Dodaj siłę z lekkim kątem w górę dla lepszego celowania
            Vector3 shootDirection = arrowSpawnPoint.forward + arrowSpawnPoint.up * 0.05f;
            rb.AddForce(shootDirection.normalized * force, ForceMode.Impulse);
        }

        // Zniszcz strzałę po 10 sekundach, aby nie zaśmiecała sceny
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
