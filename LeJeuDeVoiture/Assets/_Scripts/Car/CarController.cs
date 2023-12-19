using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ManagerNameSpace;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace CarNameSpace
{
    public class CarController : MonoBehaviour, IDamageable
    {
        #region Variables

        [Header("REFERENCES")] [SerializeField]
        public Rigidbody rb;
        public Collider collider;
        [SerializeField] public CarAbilitiesManager abilitiesManager;

        [SerializeField] public Wheel[] wheels;
        [SerializeField] private Material bodyMat;

        [Header("SUSPENSION")] [SerializeField]
        private float suspensionLenght = 0.5f;

        [SerializeField] private float suspensionForce = 70f;
        [SerializeField] private float suspensionDampening = 3f;
        [SerializeField] private float anchoring = 0.2f;

        [Header("WHEEL")] [SerializeField] private float wheelMass = 0.1f;
        public float baseMaxSpeed = 25f;
        [HideInInspector] public float maxSpeed = 25f;
        [SerializeField] private AnimationCurve accelerationBySpeedFactor;
        [SerializeField] private float acceleration = 12f;
        [SerializeField] private float braking = 12f;
        [SerializeField] private float decceleration = 4f;
        [SerializeField] private AnimationCurve steeringBySpeedFactor;
        [SerializeField] private AnimationCurve steeringByMaxSpeed;
        [SerializeField] private float steeringSpeed = 50f;
        [SerializeField] private float bulletModeSteeringFactor = 0.3f;
        public bool wheelForcesApply = true;
        public bool steeringInputEnabled = true;
        public bool directionalDampeningEnabled = true;
        [SerializeField] private float maxRotation;
        
        [Header("WALLBOUNCE")]
        [SerializeField] private float speedRetained = 0.7f;
        [SerializeField] private float maxSpeedRetained = 0.8f;
        [SerializeField] private float minAngleToBounce = 0.3f;
        [SerializeField] private GameObject fxBounce;

        public float speed => rb.velocity.magnitude;

        [Header("PHYSICVALUES")] [SerializeField]
        private Vector3 localCenterOfMass;

        // INPUT VALUES
        private Vector2 stickValue;
        private float brakeForce;
        private bool driftBrake,nitroMode,onGround;
        private float nitroModeEntryEnergy;
        private float speedFactor => speed / maxSpeed;
        
        
        // DRAFT VALUES - To be Deleted
        public TMP_Text speedDisplay;
        public Transform whiteCircle, whiteIndicator;
        

        #endregion

        #region Updates

        private void Start()
        {
            rb.centerOfMass = localCenterOfMass;
        }

        private void Update()
        {
            // TOURNER LES ROUES
            
            Vector2 carForwardCamera = Quaternion.Euler(0, 0, -45) * new Vector2(transform.forward.x, transform.forward.z);
            float angleDiff = Vector2.SignedAngle(carForwardCamera, stickValue);
            float rotationValue = -Mathf.Clamp(angleDiff / 90,-1,1);
            whiteCircle.position = new Vector3(transform.position.x, 1, transform.position.z);
            whiteCircle.rotation = Quaternion.Euler(90,transform.eulerAngles.y,0);
            whiteIndicator.position = whiteCircle.position + Quaternion.Euler(0,-45,0) * new Vector3(stickValue.x,0,stickValue.y).normalized * 7;
            
            
            
            for (int i = 0; i < wheels.Length; i++)
            {
                if (wheels[i].steeringFactor > 0)
                {
                    wheels[i].wheelVisual.localRotation = wheels[i].transform.localRotation = Quaternion.Lerp(
                        wheels[i].transform.localRotation,
                        Quaternion.Euler(0,
                            rotationValue                                                                                 // Valeur de rotation Stick ( -1 / 1 )
                            * wheels[i].steeringFactor                                                                      // SteeringFactor de la roue ( value )
                            * steeringBySpeedFactor.Evaluate(speedFactor)                                                   // Courbe de steering par speedFactor ( 0 / 1 )
                            * (baseMaxSpeed / maxSpeed)                                                                     // Facteur de Vitesse Bonus ( 0 / 1 )
                            * steeringByMaxSpeed.Evaluate(maxSpeed)                                                         // Courbe de steering par maxSpeed ( 1 / X )
                            * (abilitiesManager.isInRage ? bulletModeSteeringFactor : 1), 0)                            // Si en Bullet Mode driving factor reduit ( Value )
                        , Time.deltaTime * steeringSpeed);
                }
            }

            speedDisplay.text = ((int) speed) + "/" + ((int) maxSpeed);

            if(speedFactor > 1) rb.velocity = Vector3.Lerp(rb.velocity,Vector3.ClampMagnitude(rb.velocity,maxSpeed),Time.deltaTime);
            
            
            if(onGround) transform.rotation = Quaternion.Euler(Mathf.Clamp(transform.eulerAngles.x,-maxRotation,maxRotation),transform.eulerAngles.y,Mathf.Clamp(transform.eulerAngles.z,-maxRotation,maxRotation));

            
        }

        void FixedUpdate()
        {
            if (!wheelForcesApply) return;
            
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit,1))
            {
                onGround = true;
            }
            else
            {
                onGround = false;
            }
            
            for (int i = 0; i < wheels.Length; i++)
            {
                // APPLICATION DES FORCES
                Vector3 wheelForce = GetWheelForces(wheels[i]);
                rb.AddForceAtPosition(wheelForce, wheels[i].transform.position);
            }
        }

        #endregion

        #region Wheel Methods

        Vector3 GetWheelForces(Wheel wheel)
        {
            Vector3 wheelForce = Vector3.zero;

            float suspension = 0;
            
            // SI ROUE AU SOL, ALORS FORCES
            if (Physics.Raycast(wheel.transform.position, -wheel.transform.up, out RaycastHit hit,
                suspensionLenght + anchoring))
            {
                // GET SUSPENSION
                suspension = GetWheelSuspensionForce(wheel, hit);
            }

            if (onGround)
            {
                // CALCUL DES FORCES
                float directionalDamp = GetWheelDirectionalDampening(wheel);
                float drivingForce = wheel.drivingFactor > 0 ? GetWheelAcceleration(wheel) : 0;
                wheelForce = wheel.transform.up * suspension +
                             wheel.transform.right * directionalDamp +
                             wheel.transform.forward * drivingForce;   
            }

            return wheelForce;
        }

        float GetWheelSuspensionForce(Wheel wheel, RaycastHit hit)
        {
            float force = 0;

            float offset = suspensionLenght - hit.distance;
            Vector3 wheelWorldVelocity = rb.GetPointVelocity(wheel.transform.position);
            float velocity = Vector3.Dot(wheel.transform.up, wheelWorldVelocity);
            force = (offset * suspensionForce) - (velocity * suspensionDampening);
            wheel.wheelVisual.position = wheel.transform.position - wheel.transform.up * (hit.distance - 0.25f);

            return force;
        }

        float GetWheelDirectionalDampening(Wheel wheel)
        {
            float force = 0;

            Vector3 wheelWorldVelocity = rb.GetPointVelocity(wheel.transform.position);
            float tangentSpeed = Vector3.Dot(wheelWorldVelocity, wheel.transform.right);
            float counterAcceleration = (-tangentSpeed * wheel.directionalDampening) / Time.fixedDeltaTime;

            force = wheelMass * counterAcceleration * (directionalDampeningEnabled ? 1 : 0f);

            return force;
        }

        float GetWheelAcceleration(Wheel wheel)
        {
            float force = 0;

            float accel = (1 - brakeForce) * acceleration * (speedFactor >= 1 ? 0 : 1) * accelerationBySpeedFactor.Evaluate(speedFactor) * wheel.drivingFactor;

            float brake = 0;
            if (Vector3.Dot(rb.velocity, transform.forward) > 0.1f)
            {
                brake = brakeForce * -braking * wheel.drivingFactor * (driftBrake ? 0 : 1);
            }
            else
            {
                brake = brakeForce * (speedFactor >= 1 ? 0 : 1) * accelerationBySpeedFactor.Evaluate(speedFactor) * -decceleration * wheel.drivingFactor;
            }

            force = accel + brake;

            return force;
        }

        #endregion

        #region Inputs
        
        public void LShoulder(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if ((stickValue.x > 0.6f || stickValue.x < -0.6f) && speedFactor > 0.5f)
                {
                    //StartDrift();
                }
                else
                {
                    brakeForce = context.ReadValue<float>();
                }
            }
            else
            {
                brakeForce = 0;
            }
        }

        public void LStick(InputAction.CallbackContext context)
        {
            if (context.performed && steeringInputEnabled)
            {
                stickValue = context.ReadValue<Vector2>();
            }
            else
            {
                stickValue = Vector2.zero;
            }
        }

        #endregion

        #region Collisions

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Wall"))
            {
                Debug.Log(other.relativeVelocity.magnitude);
                if (Vector3.Dot(other.contacts[0].normal, transform.forward) < -minAngleToBounce)
                {
                    
                    Vector2 reflect = Vector2.Reflect(new Vector2(transform.forward.x, transform.forward.z),
                        new Vector2(other.contacts[0].normal.x,other.contacts[0].normal.z));
                    transform.forward = new Vector3(reflect.x,0, reflect.y);
                    maxSpeed = Mathf.Clamp(maxSpeed * maxSpeedRetained,baseMaxSpeed,Mathf.Infinity);
                    rb.velocity = transform.forward * other.relativeVelocity.magnitude * speedRetained;
                    rb.angularVelocity = Vector3.zero;
                    
                    for (int i = 0; i < wheels.Length; i++)
                    {
                        if (wheels[i].steeringFactor > 0)
                        {
                            wheels[i].wheelVisual.localRotation =
                                wheels[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
                        }
                    }

                    Destroy(Instantiate(fxBounce, other.contacts[0].point, Quaternion.LookRotation(other.contacts[0].normal)),2);
                }
                
                transform.rotation = Quaternion.Euler(Mathf.Clamp(transform.eulerAngles.x,-maxRotation,maxRotation),transform.eulerAngles.y,Mathf.Clamp(transform.eulerAngles.z,-maxRotation,maxRotation));
            }
        }

        #endregion

        public void TakeDamage(int damages)
        {
            for (int i = 0; i < damages; i++)
            {
                if (maxSpeed > 10)
                {
                    maxSpeed -= 1;
                }
            }
        }

        public void Kill()
        {
        }
    }

    [Serializable]
    public struct Wheel
    {
        public Transform transform, wheelVisual;
        public float directionalDampening;
        public float drivingFactor;
        public float steeringFactor;
        public TireMarksGenerator tireMarksGenerator;
    }
}
