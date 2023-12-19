using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarNameSpace;
using ManagerNameSpace;
using UnityEngine;

public class GenericInteraction_WIP : EnvironmentInteraction
{
    public InteractionsType interactionType = InteractionsType.EnergyItem;
    public InteractiveSettings_WIP interactiveSettings;
    public List<GameObject> padVisualRenderer; 
    public List<ParticleSystem> padPSRenderer = new List<ParticleSystem>();
    public List<Material> padVisualMat = new List<Material>();
    
    //Private
    private bool canBePickedUp = true;
    private bool isBoosting = false;
    private bool isRotating = false;
    public Color[] padColors; //Store the color
    private string padColorName = "_BaseColor"; //Name of the color in the shader 
    

    private void Awake()
    {
        DisplayPadRenderer();
        PopulateColorArray();
    }

#if UNITY_EDITOR    
    private void OnValidate() {
        DisplayPadRenderer();
        PopulateColorArray();
    }
#endif
    
    public override void Interact(CarController player)
    {
        base.Interact(player);
        switch (interactionType)
        {
            //Implement Energy Item
            case InteractionsType.EnergyItem:
                GameManager.instance.abilitiesManager.AddEnergy(interactiveSettings.energyAmount);
                break;
            //Implement Shield Item
            case InteractionsType.ShieldItem:
                _ = ActivateShieldAsync(player);
                EnableParticleSystems((int)interactionType); 
                break;
            //Implement Jump Pad
            case InteractionsType.JumpPad:
                _ = ActivateJumpAsync(player, interactiveSettings.jumpPadForce);
                EnableParticleSystems((int)interactionType); 
                break;
            //Implement Booster Pad
            case InteractionsType.BoosterPad:
                _ = StartBoosterAsync(player);
                EnableParticleSystems((int)interactionType); 
                break;
        }
    }
    
    private async Task ActivateShieldAsync(CarController player)
    {
        // TODO : Implement the function for activate the shield
        //player.ActivateShield(interactiveSettings.shieldDuration);
        
        gameObject.SetActive(false); //Disable the object

        await Task.Delay((int)(interactiveSettings.shieldRespawnValue * 1000));

        gameObject.SetActive(true); //Enable

        canBePickedUp = true;
    }
    
    private async Task ActivateJumpAsync(CarController player, float jumpForce)
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Debug.Log(jumpForce);
            Quaternion originalRotation = rb.rotation;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            rb.freezeRotation = true;

            await Task.Delay((int)(3.0f * 1000));
            
            await Task.Yield();
            rb.rotation = originalRotation;
            rb.freezeRotation = false;
        }
    }
    
    private async Task StartBoosterAsync(CarController player)
    {
        if (!isBoosting)
        {
            player.maxSpeed += interactiveSettings.increaseSpeedMax;
            isBoosting = true;

            await Task.Delay((int)(interactiveSettings.boosterDuration * 1000));

            await StopBoosterAsync(player);
        }
    }

    private async Task StopBoosterAsync(CarController player)
    {
        if (isBoosting)
        {
            //player.maxSpeed = player.baseMaxSpeed;
            isBoosting = false;
        }
    }

    private void DisplayPadRenderer()
    {
        DisableAllVisualPad();
        DisableAllParticulesSystemPad();
        SelectPrefabToDisplay(interactionType, padVisualRenderer);
    }
    
    private void DisableAllVisualPad()
    {
        foreach (GameObject pad in padVisualRenderer)
        {
            pad.SetActive(false);
        }
    }

   
    private void SelectPrefabToDisplay(InteractionsType type, List<GameObject> visualPad)
    {
        switch (type)
        {
            case InteractionsType.EnergyItem:
                visualPad[0].SetActive(true);
                padVisualMat[0].SetColor(padColorName, padColors[0]); //TODO : Refactor this part and make a function
                break;
            case InteractionsType.ShieldItem:
                visualPad[1].SetActive(true);
                padVisualMat[1].SetColor(padColorName, padColors[1]);
                break;
            case InteractionsType.BoosterPad:
                visualPad[2].SetActive(true);
                padVisualMat[2].SetColor(padColorName, padColors[2]);
                break;
            case InteractionsType.JumpPad:
                visualPad[3].SetActive(true);
                padVisualMat[3].SetColor(padColorName, padColors[3]);
                break;
        }
    }
    
    private void EnableParticleSystems(int type)
    {
        foreach (var ps in padPSRenderer)
        {
            ps.Stop(); 
            ps.gameObject.SetActive(false);
            if (padPSRenderer.IndexOf(ps) == (int)type - 1)
            {
                ps.gameObject.SetActive(true);
                ps.Play(); 
            }
        }
    }
    
    private void DisableAllParticulesSystemPad()
    {
        foreach (ParticleSystem pad in padPSRenderer)
        {
            pad.Stop();
            pad.gameObject.SetActive(false);
        }
    }

    [ContextMenu("Populate Color Array")]
    //TODO : Redo this function
    private void PopulateColorArray()
    {
        padColors = new Color[Enum.GetValues(typeof(InteractionsType)).Length];
        padColors[0] = interactiveSettings.energyPadColor;
        padColors[1] = interactiveSettings.shieldPadColor;
        padColors[2] = interactiveSettings.boostPadColor;
        padColors[3] = interactiveSettings.jumpPadColor;
    }
}
