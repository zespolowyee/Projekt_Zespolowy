using UnityEngine;

public class StringPull : MonoBehaviour
{
    public GameObject BowString;           // Linka łuku
    public Transform StringIdlePos;        // Pozycja linki w stanie spoczynku
    public Transform StringPullPos;        // Pozycja linki przy pełnym naciągu
    public float pullSpeed = 5f;           // Szybkość naciągania linki
    private bool isPulling = false;        // Czy łuk jest naciągany

    private void Update()
    {
        // Jeśli przycisk naciągania jest wciśnięty
        if (Input.GetButton("Fire1"))
        {
            isPulling = true;
        }
        else
        {
            isPulling = false;
        }

        // Wywołanie metody naciągania lub resetowania
        if (isPulling)
        {
            StringPullEffect();  // Naciąganie linki
        }
        else
        {
            ResetStringPosition();  // Resetowanie linki
        }
    }

    void StringPullEffect()
    {
        // Płynne naciąganie linki za pomocą interpolacji
        BowString.transform.position = Vector3.Lerp(BowString.transform.position, StringPullPos.position, pullSpeed * Time.deltaTime);
        BowString.transform.SetParent(StringPullPos);  // Ustawienie linki w odpowiedniej hierarchii
    }

    void ResetStringPosition()
    {
        // Płynne zwrócenie linki do pozycji neutralnej
        BowString.transform.position = Vector3.Lerp(BowString.transform.position, StringIdlePos.position, pullSpeed * Time.deltaTime);
        BowString.transform.SetParent(StringIdlePos);  // Resetowanie pozycji i hierarchii
    }
}
