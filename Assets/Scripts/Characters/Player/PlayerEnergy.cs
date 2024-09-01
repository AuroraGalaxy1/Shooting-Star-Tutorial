using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergy : Singleton<PlayerEnergy>
{
    [SerializeField] EnergyBar energyBar;
    [SerializeField] float overdriveInterval = 0.1f;

    bool available = true;

    public const int MAX = 100;
    public const int PERCENT = 1;
    int energy;

    WaitForSeconds waitForOverdriveInterval;

    protected override void Awake()
    {
        base.Awake();
        waitForOverdriveInterval = new WaitForSeconds(overdriveInterval);
    }

    void OnEnable()
    {
        PlayerOverdrive.on += PlayerOverdriveOn;
        PlayerOverdrive.off += PlayerOverdriveOff;
    }

    void OnDisable()
    {
        PlayerOverdrive.on -= PlayerOverdriveOn;
        PlayerOverdrive.off -= PlayerOverdriveOff;
    }

    void Start()
    {
        energyBar.Initialize(energy, MAX);
        Obtain(MAX);
    }

    public void Obtain(int value)
    {
        if (energy == MAX || !available || !gameObject.activeSelf) return;

        energy = Mathf.Clamp(energy + value, 0, MAX);
        energyBar.UpdateStats(energy, MAX);
    }

    public void Use(int value)
    {
        energy -= value;
        energyBar.UpdateStats(energy, MAX);
        if (energy == 0 && !available)
        {
            PlayerOverdrive.off.Invoke();
        }
    }

    public bool IsEnough(int value) => energy >= value;

    void PlayerOverdriveOn()
    {
        available = false;
        StartCoroutine(nameof(KeepUsingCoroutine));
    }

    void PlayerOverdriveOff()
    {
        available = true;
        StopCoroutine(nameof(KeepUsingCoroutine));
    }

    IEnumerator KeepUsingCoroutine()
    {
        while (gameObject.activeSelf && energy > 0)
        {
            yield return waitForOverdriveInterval;
            Use(PERCENT);
        }
    }
}