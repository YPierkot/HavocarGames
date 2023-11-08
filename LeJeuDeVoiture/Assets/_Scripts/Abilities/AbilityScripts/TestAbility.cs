using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestAbility : Ability
{
    [Header("ABILITY PARAMETERS")] 
    [SerializeField] private string message;

    public override void StartAbility()
    {
        base.StartAbility();
        Debug.Log(message);
    }
}
