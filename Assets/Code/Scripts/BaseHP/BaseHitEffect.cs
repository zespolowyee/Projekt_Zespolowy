using System.Collections;
using UnityEngine;

public class BaseHitEffect : MonoBehaviour
{
    private Renderer rend;
    private Color originalColor;
    public Color hitColor = Color.red;
    public float effectDuration = 0.2f;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
    }

    public void TriggerHitEffect()
    {
        StopAllCoroutines();
        StartCoroutine(HitFlash());
    }

    private IEnumerator HitFlash()
    {
        rend.material.color = hitColor;
        yield return new WaitForSeconds(effectDuration);
        rend.material.color = originalColor;
    }
}
