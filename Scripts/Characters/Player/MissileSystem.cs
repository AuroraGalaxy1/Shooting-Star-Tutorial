using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSystem : MonoBehaviour
{
    [SerializeField] int defaultAmount = 5;
    [SerializeField] float cooldownTime = 1f;
    [SerializeField] GameObject missilePrefab = null;
    [SerializeField] AudioData lauchSFX = null;
    [SerializeField] AudioData getMissileSFX = null;
    [SerializeField] AudioData errorSFX = null;
    int amount;
    bool isReady = true;
    void Awake()
    {
        amount = defaultAmount;
    }
    void Start()
    {
        MissileDisplay.UpdateAmountText(amount);
    }
    public void PickUp()
    {
        amount++;
        MissileDisplay.UpdateAmountText(amount);
        if (amount == 1)
        {
            MissileDisplay.UpdateCooldownImage(0f);
            isReady = true;
        }
    }

    public void Lauch(Transform muzzleTransform)
    {
        if (amount == 0 || !isReady) return;
        isReady = false;
        PoolManager.Release(missilePrefab, muzzleTransform.position);
        AudioManager.Instance.PlayRandomSFX(lauchSFX); ;
        amount--;
        MissileDisplay.UpdateAmountText(amount);

        if (amount == 0)
        {
            MissileDisplay.UpdateCooldownImage(1f);
        }
        else
        {
            StartCoroutine(CooldownCoroutine());
        }
    }
    IEnumerator CooldownCoroutine()
    {
        var cooldownValue = cooldownTime;
        while (cooldownValue > 0f)
        {
            MissileDisplay.UpdateCooldownImage(cooldownValue / cooldownTime);
            cooldownValue = Mathf.Max(cooldownValue - Time.deltaTime, 0f);
            yield return null;
        }
        isReady = true;
    }
    void Update()
    {
        GetMissile();
    }
    void GetMissile()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (PlayerEnergy.Instance.IsEnough(50))
            {
                PlayerEnergy.Instance.Use(50);
                amount++;
                MissileDisplay.UpdateAmountText(amount);
                AudioManager.Instance.PlaySFX(getMissileSFX);
            }
            else
            {
                AudioManager.Instance.PlaySFX(errorSFX);
            }
        }
    }
}
