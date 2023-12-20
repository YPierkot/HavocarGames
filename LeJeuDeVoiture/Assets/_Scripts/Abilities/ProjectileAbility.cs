using System;
using System.Numerics;
using System.Threading.Tasks;
using ManagerNameSpace;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;


namespace AbilityNameSpace
{
    public class ProjectileAbility : MonoBehaviour
    {
        [SerializeField] private int directionIndex;
        [SerializeField] private float projectileDuration, projectileSpeed;
        [SerializeField] private CarProjectileBehavior projectileObject;
        
        [SerializeField] private Vector2 stickValue = Vector2.up;
        
        [SerializeField] private float dashAngleSwitch = 0.5f;
        [SerializeField] private float cooldown;
        [Header("Indicator Direction Projectile")]
        [SerializeField]private float maxScaleZ = 3.0f;
        [SerializeField] private float defaultScaleZ = 1.0f;
        [SerializeField] private float scaleTransitionTime = 0.3f;
        [SerializeField] private GameObject indicatorGD;
        private float timer;
        private int previousDirectionIndex;


        
        Vector2 carForwardCamera => Quaternion.Euler(0, 0, -45) * new Vector2(
            GameManager.instance.controller.transform.forward.x,
            GameManager.instance.controller.transform.forward.z);
        
        
        private void Update()
        {
            // TODO : Extraire ça de cette fonction Update, le mettre dans celle de AbilityManager
            
            if (timer > 0) timer -= Time.deltaTime;
        }

        public void LStick(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                stickValue = context.ReadValue<Vector2>();
                
                float signedAngle = Vector2.SignedAngle(stickValue.normalized, carForwardCamera.normalized);

                if (Mathf.Abs(signedAngle) < 45)
                {
                    directionIndex = 0;
                    RotateIndicator();
                }
                else if (Mathf.Abs(signedAngle) < 135)
                {
                    directionIndex = signedAngle > 0 ? 2 : 1;
                    RotateIndicator();
                }
            }
        }

        public void PressProjectile(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                GameManager.instance.controller.steeringInputEnabled = false;
                SetIndicatorActive(true);
            }

            if (context.performed)
            {
                RotateIndicator();
            }

            else if (context.canceled)
            {
                GameManager.instance.controller.steeringInputEnabled = true;
                SetIndicatorActive(false);
                ReleaseProjectile();
            }
        }

        public void ReleaseProjectile()
        {
            if (timer > 0) return;
            timer = cooldown;
            
            Vector3 direction = GameManager.instance.controller.transform.forward;
            switch (directionIndex)
            {
                case 0:
                    direction = GameManager.instance.controller.transform.forward;
                    break;
                case 1:
                    direction = -GameManager.instance.controller.transform.right;
                    break;
                case 2:
                    direction = GameManager.instance.controller.transform.right;
                    break;
            }

            direction = new Vector3(direction.x, 0, direction.z);

            ShootProjectile(direction);
        }
        
        public async void ShootProjectile(Vector3 dir)
        {
            projectileObject.gameObject.SetActive(true);
            projectileObject.transform.position = transform.position;
            projectileObject.transform.rotation = transform.rotation;

            projectileObject.movement = dir * projectileSpeed;
            projectileObject.transform.rotation = Quaternion.LookRotation(projectileObject.movement);
            projectileObject.damages = (int) GameManager.instance.controller.maxSpeed;
            projectileObject.trail.Clear();

            await Task.Delay(Mathf.RoundToInt(1000 * projectileDuration));
            
            if (projectileObject.gameObject.activeSelf)
            {
                projectileObject.gameObject.SetActive(false);   
            }
        }


        private async Task RotateIndicatorAsync(Vector3 targetDirection)
        {
            // Scale to default value
            float elapsedScaleTime = 0f;
            while (elapsedScaleTime < scaleTransitionTime)
            {
                float scaleValue = Mathf.Lerp(maxScaleZ, defaultScaleZ, elapsedScaleTime / scaleTransitionTime);
                indicatorGD.transform.localScale = new Vector3(1f, 1f, scaleValue);
                elapsedScaleTime += Time.deltaTime;
                await Task.Yield();
            }

            // Rotate
            indicatorGD.transform.rotation = Quaternion.LookRotation(targetDirection);

            // Scale to max value
            float elapsedRotateTime = 0f;
            while (elapsedRotateTime < scaleTransitionTime)
            {
                float scaleValue = Mathf.Lerp(defaultScaleZ, maxScaleZ, elapsedRotateTime / scaleTransitionTime);
                indicatorGD.transform.localScale = new Vector3(1f, 1f, scaleValue);
                elapsedRotateTime += Time.deltaTime;
                await Task.Yield();
            }
            previousDirectionIndex = directionIndex;
        }

        private void RotateIndicator()
        {
            Vector3 targetDirection = transform.forward;

            /*
            switch (directionIndex)
            {
                case 0:
                    targetDirection = GameManager.instance.controller.transform.forward;
                    break;
                case 1:
                    targetDirection = -GameManager.instance.controller.transform.right;
                    break;
                case 2:
                    targetDirection = GameManager.instance.controller.transform.right;
                    break;
            }
            */
            
            switch (directionIndex)
            {
                case 0:
                    targetDirection = transform.forward;
                    break;
                case 1:
                    targetDirection = -transform.right;
                    break;
                case 2:
                    targetDirection = transform.right;
                    break;
                default:
                    targetDirection = transform.forward;
                    break;
            }
            
            if (directionIndex != previousDirectionIndex)
            {
               _ = RotateIndicatorAsync(targetDirection);
            }
        }

        
        private void SetIndicatorActive(bool isActive)
        {
            indicatorGD.SetActive(isActive);
        }
    }
}
