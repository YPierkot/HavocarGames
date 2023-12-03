using UnityEngine;
using System.Threading.Tasks;

namespace AbilityNameSpace
{
    public class ShieldAbility : Ability
    {
        [Header("Shield Parameters")]
        public float shieldDuration = 5f; 
        public int energyCost = 2; 
        public GameObject shieldVisualPrefab; 

        private bool isShieldActive = false;
        private float shieldTimer = 0f;
        private GameObject shieldVisualInstance;
        
        public override void SetupAbility(AbilitySocket currentSocket)
        {
            base.SetupAbility(currentSocket);
            if (shieldVisualInstance == null)
            {
                shieldVisualInstance = Instantiate(shieldVisualPrefab, transform.position, Quaternion.identity, parent: transform.parent);
            }

            shieldVisualInstance = shieldVisualPrefab;
            shieldVisualInstance.SetActive(false);
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
                Debug.Log("Not enough energy to activate the shield ability.");
            }
        }

        private async Task ActivateShieldAsync()
        {
            //TODO : Do the energy cost

            // Turn on the shield
            isShieldActive = true;
            shieldTimer = shieldDuration;

            // Turn on the shield visual
            await DisplayShieldAsync();

            Debug.Log("Shield activated!");
            
            await Task.Delay((int)(shieldDuration * 1000));
            DesactivateShield();
        }

        private async Task DisplayShieldAsync()
        {
            await Task.Run(() =>
            {
                //TODO : Delete this when we are doing the Setup
                shieldVisualInstance = shieldVisualPrefab;
                if (shieldVisualInstance == null)
                {
                    shieldVisualInstance = Instantiate(shieldVisualPrefab, transform.position, Quaternion.identity, parent: transform.parent);
                }
                shieldVisualInstance.SetActive(true);
            });
        }

        private void DesactivateShield()
        {
            // Turn the display off 
            shieldVisualInstance.SetActive(false);
            isShieldActive = false;
            Debug.Log("Shield deactivated!");
        }

        public bool IsShieldActive()
        {
            return isShieldActive;
        }
    }
}
