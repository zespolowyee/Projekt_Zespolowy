using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    [SerializeField] private HPSystem hpSystem;
    [SerializeField] private GameObject bar;
    [SerializeField] private float maxBarLenght;
    [SerializeField] private float barHeight;

    [SerializeField] private float visibleTime;
    private int maxHp;
    void Start()
    {
        maxHp = hpSystem.maxHP;
        UpdateHpBarValue(maxHp, maxHp);
    }

    private void OnEnable()
    {
        hpSystem.currentHP.OnValueChanged += UpdateHpBarValue;
    }

    private void OnDisable()
    {
        hpSystem.currentHP.OnValueChanged -= UpdateHpBarValue;
    }

    private void UpdateHpBarValue(int previousValue, int newValue)
    {
        float fillAmount = (float)newValue / (float)maxHp;
        Math.Clamp(fillAmount, 0, 1);
        bar.transform.localScale = new Vector3(maxBarLenght * fillAmount, barHeight, 1f);
        StartCoroutine(ShowHpBar());
    }

    IEnumerator ShowHpBar()
    {
        GameObject playerBody = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponentInChildren<RBController>().gameObject;
        bar.transform.forward = playerBody.transform.forward;
        bar.SetActive(true);
        yield return new WaitForSeconds(visibleTime);
        bar.SetActive(false);
    }

}
