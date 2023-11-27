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
    
    //Private
    private bool canBePickedUp = true;
    
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
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
                ActivateJump(player, interactiveSettings.jumpPadForce);
                break;
            //Implement Booster Pad
            case InteractionsType.BoosterPad:
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
    
    private void ActivateJump(CarController player, float jumpForce)
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
