using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "InteractiveSettings", menuName = "Enviro_WIP/InteractiveSettings", order = 1)]
public class InteractiveSettings_WIP : ScriptableObject
{
    [Header("Energy Barrel")] 
    public float energyAmount = 10;
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
    public Color jumpPadColor = Color.magenta;
    //TODO : Add a Boolean to check if the player is in the air or not
}
