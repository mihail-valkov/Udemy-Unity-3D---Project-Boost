using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
    [SerializeField] float fuelConsumptionRate = 1f;
    [SerializeField] float maxHealthValue = 100f;
    [SerializeField] float maxFuelValue = 100f;
    [SerializeField] float hitDamage = 10f;
    [SerializeField] bool isRocketBreakable = false;

    public float MaxHealthValue { get { return maxHealthValue; } }
    public float MaxFuelValue { get { return maxFuelValue; } }
    public float HitDamage { get { return hitDamage; } }
    public bool IsRocketBreakable { get { return isRocketBreakable; } }
    public float FuelConsumptionRate { get { return fuelConsumptionRate; } }
}
