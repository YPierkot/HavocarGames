using System.Threading;
using UnityEngine;
using System.Threading.Tasks;

namespace AbilityNameSpace
{
    public class ShieldAbility : Ability
    {
        [Header("Shield Parameters")]
        public float shieldDuration = 5f; 
        public int energyCost = 2; 
        public Transform shieldVisualBody; 

        private bool isShieldActive = false;
        private float shieldTimer = 0f;
        
        [Header("Enable Debug Logs")]
        public bool debugLogs = false;        
        public override void SetupAbility(AbilitySocket currentSocket)
        {
            base.SetupAbility(currentSocket);
            
        }

        public override async void StartAbility()
        {
            base.StartAbility();
            
            if (!isShieldActive)
            {
                await ActivateShieldAsync();
            }
            else
            {
                if (debugLogs) Debug.Log("Not enough energy to activate the shield ability.");
            }
        }

        private async Task ActivateShieldAsync()
        {
            //TODO : Do the energy cost
            ActivateShield();


            
            await Task.Delay((int)(shieldDuration * 1000));
            DesactivateShield();
        }

        private void ActivateShield()
        {
            isShieldActive = true;
            shieldVisualBody.gameObject.SetActive(true);
            if (debugLogs) Debug.Log("Shield activated!");
        }

        private void DesactivateShield()
        {
            // Turn the display off 
            shieldVisualBody.gameObject.SetActive(false);
            isShieldActive = false;
            if (debugLogs) Debug.Log("Shield desactivated!");
        }

        public bool IsShieldActive()
        {
            return isShieldActive;
        }
    }
}
