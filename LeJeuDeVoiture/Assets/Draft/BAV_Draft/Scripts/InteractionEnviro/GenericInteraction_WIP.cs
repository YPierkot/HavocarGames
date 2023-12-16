using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarNameSpace;
using ManagerNameSpace;
using UnityEngine;

public class GenericInteraction_WIP : EnvironmentInteraction
{
    public InteractionsType interactionType = InteractionsType.None;
    public InteractiveSettings_WIP interactiveSettings;
    public List<GameObject> visualPadRenderer;
    
    //Private
    private bool canBePickedUp = true;
    private bool isBoosting = false;
    private bool isRotating = false;
    
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    private void Awake()
    {
        DisplayPadRenderer();
    }

    public override void Interact(CarController player)
    {
        base.Interact(player);
        switch (interactionType)
        {
            case InteractionsType.None:
                break;
            //Implement Energy Item
            case InteractionsType.EnergyItem:
                GameManager.instance.abilitiesManager.AddEnergy(interactiveSettings.energyAmount);
                break;
            //Implement Shield Item
            case InteractionsType.ShieldItem:
                _ = ActivateShieldAsync(player);
                break;
            //Implement Jump Pad
            case InteractionsType.JumpPad:
                _ = ActivateJumpAsync(player, interactiveSettings.jumpPadForce);
                break;
            //Implement Booster Pad
            case InteractionsType.BoosterPad:
                _ = StartBoosterAsync(player);
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
        SelectPrefabToDisplay(interactionType, visualPadRenderer);
    }
    
    private void DisableAllVisualPad()
    {
        foreach (GameObject pad in visualPadRenderer)
        {
            pad.SetActive(false);
        }
    }

    private static void SelectPrefabToDisplay(InteractionsType type, List<GameObject> visualPad)
    {
        switch (type)
        {
            case InteractionsType.None:
                break;
            case InteractionsType.EnergyItem:
                visualPad[0].SetActive(true);
                break;
            case InteractionsType.ShieldItem:
                visualPad[1].SetActive(true);
                break;
            case InteractionsType.BoosterPad:
                visualPad[2].SetActive(true);
                break;
            case InteractionsType.JumpPad:
                visualPad[3].SetActive(true);
                break;
        }
    }
}
