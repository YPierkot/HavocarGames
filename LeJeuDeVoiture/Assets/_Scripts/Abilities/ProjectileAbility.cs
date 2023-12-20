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
        [SerializeField] private float offsetValue = 20.0f;
        [SerializeField] private float smoothFactor = 2.0f;
        
        private float timer;
        private int previousDirectionIndex;
        private bool isIndicatorActive = false;
        private Vector3 targetDirection;
        private Vector3 cameraOffset = Vector3.zero;
        private Vector3 originalCameraOffset;


        void Start()
        {
            originalCameraOffset = GameManager.instance.cameraManager.cameraOffset;
        }

        
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
                SetIndicatorActive(true);
                stickValue = context.ReadValue<Vector2>();
                
                float signedAngle = Vector2.SignedAngle(stickValue.normalized, carForwardCamera.normalized);

                if (Mathf.Abs(signedAngle) < 45.0f)
                {
                    directionIndex = 0;
                }
                else if (Mathf.Abs(signedAngle) < 135.0f)
                {
                    directionIndex = signedAngle > 0 ? 2 : 1;
                }

                if (isIndicatorActive)
                {
                    RotateIndicator();
                    // Apply the offset to the camera
                }
            }
            
            else if (context.canceled) {
                SetIndicatorActive(false);
            }
        }



        public void PressProjectile(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                GameManager.instance.controller.steeringInputEnabled = false;
                SetIndicatorActive(true);

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
        
        private bool rotationInProgress = false;

        private async void RotateIndicatorAsync(Vector3 targetDirection)
        {
            if (rotationInProgress)
            {
                return; 
            }

            rotationInProgress = true;

            try
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

                indicatorGD.transform.localRotation = directionIndex switch
                {
                    0 => Quaternion.Euler(0, 0.0f, 0),
                    1 => Quaternion.Euler(0, -90.0f, 0),
                    2 => Quaternion.Euler(0, 90.0f, 0),
                };

                // Rotate
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
            finally
            {
                rotationInProgress = false;
            }
        }

        private void RotateIndicator()
        {
            if (directionIndex != previousDirectionIndex)
            {
                RotateIndicatorAsync(targetDirection);
            }
        }

        private void SetIndicatorActive(bool isActive)
        {
            indicatorGD.SetActive(isActive);
            isIndicatorActive = isActive;

            if (isIndicatorActive)
            {
                UpdateCameraPositionAsync();
            }
        }
        

       
       private async void UpdateCameraPositionAsync()
       {
           while (isIndicatorActive)
           {
               switch (directionIndex)
               {
                   case 0:
                       cameraOffset = GameManager.instance.controller.transform.forward * offsetValue;
                       break;
                   case 1:
                       cameraOffset = -GameManager.instance.controller.transform.right * offsetValue;
                       break;
                   case 2:
                       cameraOffset = GameManager.instance.controller.transform.right * offsetValue;
                       break;
                   default:
                       cameraOffset = Vector3.zero;
                       break;
               }

               GameManager.instance.cameraManager.cameraOffset = Vector3.Lerp(GameManager.instance.cameraManager.cameraOffset, cameraOffset, Time.deltaTime * smoothFactor);

               await Task.Yield();
           }

           cameraOffset = originalCameraOffset;

           while (Vector3.Distance(GameManager.instance.cameraManager.cameraOffset, originalCameraOffset) > 0.01f)
           {
               GameManager.instance.cameraManager.cameraOffset = Vector3.Lerp(GameManager.instance.cameraManager.cameraOffset, cameraOffset, Time.deltaTime * smoothFactor);
               await Task.Yield();
           }
       }
    }
}
