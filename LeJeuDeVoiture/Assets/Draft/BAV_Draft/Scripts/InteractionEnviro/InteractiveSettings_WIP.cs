using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractiveSettings", menuName = "Enviro_WIP/InteractiveSettings", order = 1)]
public class InteractiveSettings_WIP : ScriptableObject
{
    [Header("Energy Barrel")] 
    public float energyAmount = 10;
    
    [Header("Shield Settings")]
    public float shieldDuration = 5;
    public float shieldRespawnValue = 5;
    
    [Header("Booster Pad Settings")]
    public float boosterForce = 10;
    public float boosterDuration = 5;
    public float increaseSpeedMax = 10;
    
    [Header("Jump Pad Settings")] 
    public float jumpPadForce = 10;
    //TODO : Add a Boolean to check if the player is in the air or not
}
