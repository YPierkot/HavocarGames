using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AbilityNameSpace;
using ManagerNameSpace;
using UnityEngine.Events;

namespace CarNameSpace
{
    public class CarAbilitiesManager : MonoBehaviour
    {
        [Header("KIT")] 
        public Ability xAbility, bAbility, yAbility, aAbility;

        [Header("ENERGY")] 
        public int energySegments;
        public float energy;

        [SerializeField] private float delayBeforeRegen;
        private float timerBeforeRegen;
        [SerializeField] private float regenSpeed;

        // DELEGATES
        public delegate void AbilityUsed(AbilitySocket socket);
        public AbilityUsed AbilityActivated;

        public bool UseEnergy(float energyAmount)
        {
            if (energy > energyAmount)
            {
                energy -= energyAmount;
                GameManager.instance.uiManager.SetEnergyJauge(energy);
                timerBeforeRegen = delayBeforeRegen;
                return true;
            }
            return false;
        }
        
        public void AddEnergy(float energyAmount)
        {
            energy += energyAmount;
            energy = Mathf.Clamp(energy, 0, energySegments);
            GameManager.instance.uiManager.SetEnergyJauge(energy);
        }

        public bool UseEnergySegments(int segments)
        {
            if (energy > segments)
            {
                energy -= segments;
                GameManager.instance.uiManager.SetEnergyJauge(energy);
                timerBeforeRegen = delayBeforeRegen;
                return true;
            }
            return false;
        }

        public float GetSegmentCurrentEnergy(int segment)
        {
            return Mathf.Clamp01(energy - segment);
        }
        
        
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
           
        }

        private void Update()
        {
            aAbility.UpdateAbility();
            bAbility.UpdateAbility();
            xAbility.UpdateAbility();
            yAbility.UpdateAbility();

            if (timerBeforeRegen > 0)
            {
                timerBeforeRegen -= Time.deltaTime;
            }
            else
            {
                AddEnergy(Time.deltaTime * regenSpeed);
            }
        }
    }
}
