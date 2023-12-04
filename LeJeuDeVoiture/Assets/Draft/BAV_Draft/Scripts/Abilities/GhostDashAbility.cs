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
    public float dashThroughTime;

    public override void StartAbility()
    {
        base.StartAbility();
        GameManager.instance.controller.abilitiesManager.dash.SetDashThroughWallsTimer(dashThroughTime);
    }

}