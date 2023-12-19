using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
[CreateAssetMenu(fileName = "InteractiveSettings", menuName = "Enviro_WIP/InteractiveSettings", order = 1)]
public class InteractiveSettings_WIP : ScriptableObject
{
    [FormerlySerializedAs("smallEnergyAmount")]
    [Header("Energy Barrel")] 
    [Tooltip("X is for the amount of energy to add, Y is the cooldown before the energy can be collected again")]
    public Vector2 smallEnergy = new Vector2(1,2);
    [FormerlySerializedAs("mediumEnergyAmount")] [Tooltip("X is for the amount of energy to add, Y is the cooldown before the energy can be collected again")]
    public Vector2 mediumEnergy = new Vector2(2,4);
    [FormerlySerializedAs("largeEnergyAmount")] [Tooltip("X is for the amount of energy to add, Y is the cooldown before the energy can be collected again")]
    public Vector2 largeEnergy = new Vector2(3,6);

    public Color energyPadColor = Color.yellow;
    
    [Header("Shield Settings")]
    public float shieldDuration = 5;
    public float shieldRespawnValue = 5;
    public Color shieldPadColor = Color.blue;
    
    [Header("Booster Pad Settings")]
    public float boosterDuration = 5;
    public float increaseSpeedMax = 10;
    public Color boostPadColor = Color.green;
    
    [Header("Jump Pad Settings")] 
    public float jumpPadForce = 10;
    public float jumpPadDuration = 0.5f;
    public Color jumpPadColor = Color.magenta;
    //TODO : Add a Boolean to check if the player is in the air or not
}
