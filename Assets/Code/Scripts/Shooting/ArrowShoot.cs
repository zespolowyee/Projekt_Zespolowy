using UnityEngine;

public class ArrowShoot : MonoBehaviour
{
     [Header("Settings")]
    [SerializeField] private GameObject arrowPrefab;          // Prefab strzały
    [SerializeField] private Transform arrowSpawnPoint;       // Punkt, z którego wystrzeliwana jest strzała
    [SerializeField] private float shootForce = 30f;          // Siła strzału
    [SerializeField] private float shootCooldown = 1f;        // Czas odnowienia między strzałami

    private BowShooterAnimationController animationController; // Referencja do skryptu animacji

    private float lastShootTime = 0f;                         // Czas ostatniego strzału
    [SerializeField] private GameObject cameraHolder;
    [SerializeField] public GameObject HandArrow;
    void Start()
    {
        animationController = GetComponent<BowShooterAnimationController>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            HandArrow.SetActive(true);
        }
        // Sprawdź, czy gracz puszcza prawy przycisk myszy i czy minął cooldown
        if (Input.GetButtonUp("Fire1") && animationController.IsDrawing && Time.time >= lastShootTime + shootCooldown)
        {
            Shoot();
            lastShootTime = Time.time;  // Zapisz czas ostatniego strzału
        }
    }


    private void Shoot()
    {
        // Utwórz strzałę w punkcie wystrzału
        HandArrow.SetActive(false);

        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        Rigidbody rb = arrow.GetComponent<Rigidbody>();

        Vector3 targetDirection = cameraHolder.transform.forward; 
        // Dodaj siłę do strzały z lekkim uwzględnieniem grawitacji
        if (rb != null)
        {
            rb.AddForce(targetDirection * shootForce, ForceMode.Impulse);  // Proste wystrzelenie w kierunku wskazywanym przez cameraHolder

            //rb.AddForce(Vector3.down * 1f, ForceMode.Impulse);  // Ustawienie siły grawitacji
        }
        // Zniszcz strzałę po 10 sekundach, aby nie zaśmiecała sceny
        Destroy(arrow, 10f);
    }

}
