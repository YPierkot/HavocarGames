using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DoorSO", order = 1)]
public class DoorSO : ScriptableObject
{
    [Header("DOOR")]
    [Tooltip("Door Base Health")] public int maxLifePoints;
    [Tooltip("Porte déstructible à partir de X %")] public float limitPercentageToSwitchDestructMode;
    
    [Space]
    [Header("Attribute SECTION")]
    [Header("Weakness Attribute Section")]
    [Tooltip("Attribute duration")] public float weaknessDuration;
    [Tooltip("Damage Multiplyer")] public float weaknessDamageMultiplyer;

    [Space]
    [Header("Regeneration Attribute Section")] 
    [Tooltip("Renegeration Amount")] public int hpRegenPerTick;
    [Tooltip("Duration for the next heal")] public float regenerationCooldown;
    [Tooltip("For each towers, multiply by ->")] public float rengenerationMultiplierPerTower;
}
