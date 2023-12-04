using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AbilityNameSpace;
using ManagerNameSpace;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace CarNameSpace
{
    public class CarAbilitiesManager : MonoBehaviour
    {
        [Header("KIT")] 
        public Ability xAbility, bAbility, yAbility;

        [Header("ENERGY")] 
        public int energySegments;
        public float energy;

        [SerializeField] private float delayBeforeRegen;
        private float timerBeforeRegen;
        [SerializeField] private float regenSpeed;

        [Header("BULLET MODE")]
        [SerializeField] private float speedAmountToBullet;
        [SerializeField] private float rageModeCooldown;
        private float rageModeCooldownTimer;
        
        [SerializeField] private float rageModeTime;
        private float rageModeTimer;
        
        [SerializeField] private bool bulletMode;
        [SerializeField] private List<BulletModeSources> bulletModeSources;

        public bool isInBulletMode => bulletModeSources.Count > 0;
        public bool isInRage => bulletModeSources.Contains(BulletModeSources.Rage);
        public bool isInGold => bulletModeSources.Contains(BulletModeSources.Speed);
        
        [SerializeField] private GameObject particles;

        [SerializeField] private CarController carController;

        public bool isShielded;
        public DashAbility dash;
        
        
        
        
        // DELEGATES
        public delegate void AbilityUsed(AbilitySocket socket);
        public AbilityUsed AbilityActivated;

        #region Energy

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

        #endregion
        
        
        
        // BULLET MODES

        #region BulletMode

        public void RShoulder(InputAction.CallbackContext context)
        {
            if (context.performed && rageModeCooldownTimer <= 0)
            {
                //GameManager.instance.uiManager.SetRageJauge(0);
                rageModeCooldownTimer = rageModeCooldown;
                rageModeTimer = rageModeTime;
                EnterBulletMode(BulletModeSources.Rage);
                
            }
        }

        private void UpdateBulletMode()
        {
            if (!bulletModeSources.Contains(BulletModeSources.Rage))
            {
                if (rageModeCooldownTimer > 0)
                {
                    rageModeCooldownTimer -= Time.deltaTime;
                    GameManager.instance.uiManager.SetRageJauge(1 - rageModeCooldownTimer / rageModeCooldown,true);
                }
                else
                {
                    GameManager.instance.uiManager.SetRageJauge(1,false);
                }
            }
            else
            {
                if (rageModeTimer > 0)
                {
                    rageModeTimer -= Time.deltaTime;
                    GameManager.instance.uiManager.SetRageJauge(rageModeTimer / rageModeTime,false);
                }
                else
                {
                    QuitBulletMode(BulletModeSources.Rage);
                }
            }

            if (carController.maxSpeed > speedAmountToBullet && !bulletModeSources.Contains(BulletModeSources.Speed))
            {
                EnterBulletMode(BulletModeSources.Speed);
            }
            if (carController.maxSpeed < speedAmountToBullet && bulletModeSources.Contains(BulletModeSources.Speed))
            {
                QuitBulletMode(BulletModeSources.Speed);
            }
        }
        
        public void EnterBulletMode(BulletModeSources source)
        {
            if(!bulletModeSources.Contains(source)) bulletModeSources.Add(source);

            particles.SetActive(true);
        }
        
        public void QuitBulletMode(BulletModeSources source)
        {
            Debug.Log("Quitt " + source);
            bulletModeSources.Remove(source);
            
            if (!isInBulletMode)
            {
                particles.SetActive(false);
            }
        }

        #endregion
        

        #region Abilities

        public void ActivateXAbility()
        {
            if (!xAbility.activable || !UseEnergy(xAbility.energyCost)) return;
            xAbility.StartAbility();
            AbilityActivated(AbilitySocket.ABILITY_X);
        }

        public void ActivateBAbility()
        {
            if (!bAbility.activable || !UseEnergy(bAbility.energyCost)) return;
            bAbility.StartAbility();
            AbilityActivated(AbilitySocket.ABILITY_B);
        }

        public void ActivateYAbility()
        {
            if (!yAbility.activable || !UseEnergy(yAbility.energyCost)) return;
            yAbility.StartAbility();
            AbilityActivated(AbilitySocket.ABILITY_Y);
        }
        

        #endregion

        public void Setup()
        {
            bAbility.SetupAbility(AbilitySocket.ABILITY_B);
            xAbility.SetupAbility(AbilitySocket.ABILITY_X);
            yAbility.SetupAbility(AbilitySocket.ABILITY_Y);
        }

        private void Update()
        {
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

            UpdateBulletMode();
        }
    }
}

public enum BulletModeSources
{
    Speed,
    Rage,
    DashCapacity
}
