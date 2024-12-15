using UnityEngine;

public class ArrowHit : MonoBehaviour
{
    public int damage = 25;  // Ilość obrażeń zadawanych przez strzałę
    private Rigidbody rb;
    private bool hasHit = false;  // Flaga, aby upewnić się, że strzała zatrzymuje się tylko raz

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
         if (hasHit) return;  // Jeśli już trafiono, zakończ funkcję

        hasHit = true;  // Ustaw flagę na true po pierwszym trafieniu

        // Sprawdź, czy trafiono obiekt z systemem HP
        HPSystem hpSystem = collision.gameObject.GetComponent<HPSystem>();
        if (hpSystem != null)
        {
            hpSystem.TakeDamage(damage);  // Zadaj obrażenia przeciwnikowi
            Debug.Log($"Zadano obrazenia obiektowi {collision.gameObject.name}");
        }
        else
        {
            Debug.Log($"Trafiono w {gameObject.name}");
        }

        // Zatrzymaj strzałę na trafionym obiekcie
        rb.isKinematic = true;  // Dezaktywuj fizykę, aby strzała zatrzymała się

        // Przytwierdź strzałę do trafionego obiektu
        transform.parent = collision.transform;

        // Opcjonalnie: zniszcz strzałę po pewnym czasie, aby nie zaśmiecała sceny
        Destroy(gameObject, 20f);
    }
}



