using System.Threading;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Serialization;

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
        public bool enabledDebugLogs = false;        
        
        //Material Renderer
        [Header("Shield Material")]
        public Material shieldMaterial;
        public float shieldAppearSpeed = 0.2f;
        public string positionMaskName = "_PositionMask";
        
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
                if (enabledDebugLogs) Debug.Log("Not enough energy to activate the shield ability.");
            }
        }

        private async Task ActivateShieldAsync()
        {
            //TODO : Do the energy cost
            ActivateShield();
            
            await Task.Delay((int)(shieldDuration * 1000));
            Debug.Log("Hello");
            DeactivateShield();
        }

        private async void ActivateShield()
        {
            isShieldActive = true;
            shieldVisualBody.gameObject.SetActive(true);
            await LerpMaterialPropertyAsync(0f, 1f, shieldAppearSpeed);
            if (enabledDebugLogs) Debug.Log("Shield activated. Material property lerped from 0 to 1.");
        }

        private async void DeactivateShield()
        {
            // Turn off the display 
            isShieldActive = false;
            await LerpMaterialPropertyAsync(1f, 0f, shieldAppearSpeed);
            if (enabledDebugLogs) Debug.Log("Shield deactivated. Material property lerped from 1 to 0.");
        }
        
        private async Task LerpMaterialPropertyAsync(float startValue, float endValue, float duration)
        {
            float startTime = Time.realtimeSinceStartup;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                float currentValue = Mathf.Lerp(startValue, endValue, t);
                shieldMaterial.SetFloat(positionMaskName, currentValue);

                // Wait for the next frame
                await Task.Yield();

                // Update elapsed time using Time.realtimeSinceStartup
                elapsedTime = Time.realtimeSinceStartup - startTime;
                Debug.Log(t);
            }


            if (endValue == 0.0f)
            {
                shieldVisualBody.gameObject.SetActive(false);
            }
            // Ensure the final value is set
            shieldMaterial.SetFloat(positionMaskName, endValue);
        }

        public bool IsShieldActive()
        {
            return isShieldActive;
        }
    }
}
