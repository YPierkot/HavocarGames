using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AbilityNameSpace;
using ManagerNameSpace;
using UnityEngine;
using UnityEngine.InputSystem;

public class GhostDashAbility : Ability
{
    [Header("Ghost Dash Parameters")]
    public string dashThroughTag; // Tag for objects that can be passed through

    public override void StartAbility()
    {
        base.StartAbility();
        if (activable)
        {
            Dash();
        }
    }

    private void Dash()
    {
        // Check if the player can pass through walls with the specified tag
        if (CanPassThroughWalls())
        {
            //TODO: Implement Ghost Dash logic here
            Debug.Log("Ghost Dash activated!");
        }
        else
        { 
            Debug.Log("Normal Dash activated!");
        }
    }

    private bool CanPassThroughWalls()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag(dashThroughTag))
            {
                return true; 
            }
        }

        return false;
    }
}