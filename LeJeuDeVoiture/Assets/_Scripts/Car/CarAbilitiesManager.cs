using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AbilityNameSpace;
using UnityEngine.Events;

namespace CarNameSpace
{
    public class CarAbilitiesManager : MonoBehaviour
    {
        [Header("KIT")] 
        public Ability xAbility, bAbility, yAbility, aAbility;

        // DELEGATES
        public delegate void AbilityUsed(AbilitySocket socket);
        public AbilityUsed AbilityActivated;
        
        public void ActivateXAbility()
        {
            if (!xAbility.activable) return;
            xAbility.StartAbility();
            AbilityActivated(AbilitySocket.ABILITY_X);
        }

        public void ActivateBAbility()
        {
            if (!bAbility.activable) return;
            bAbility.StartAbility();
            AbilityActivated(AbilitySocket.ABILITY_B);
        }

        public void ActivateYAbility()
        {
            if (!yAbility.activable) return;
            yAbility.StartAbility();
            AbilityActivated(AbilitySocket.ABILITY_Y);
        }

        public void ActivateAAbility()
        {
            if (!aAbility.activable) return;
            aAbility.StartAbility();
            AbilityActivated(AbilitySocket.ABILITY_A);
        }

        public void Setup()
        {
            aAbility.SetupAbility(AbilitySocket.ABILITY_A);
            xAbility.SetupAbility(AbilitySocket.ABILITY_X);
            bAbility.SetupAbility(AbilitySocket.ABILITY_B);
            yAbility.SetupAbility(AbilitySocket.ABILITY_Y);
        }

        private void Update()
        {
            aAbility.UpdateAbility();
            xAbility.UpdateAbility();
            bAbility.UpdateAbility();
            yAbility.UpdateAbility();
        }
    }
}
