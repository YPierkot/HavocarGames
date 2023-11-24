using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace CarNameSpace
{
    public class CarController : MonoBehaviour
    {
        #region Variables

        [Header("REFERENCES")] [SerializeField]
        public Rigidbody rb;
        public Collider collider;
        [SerializeField] private CarAbilitiesManager abilitiesManager;

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
        [SerializeField] private float steeringSpeed = 50f;
        public bool wheelForcesApply = true;

        public float speed => rb.velocity.magnitude;

        [Header("PHYSICVALUES")] [SerializeField]
        private Vector3 localCenterOfMass;

        [Header("BRAKEDRIFT")] [SerializeField]
        private float dampeningMultiplier = 0.25f;

        [SerializeField] private float steeringMultiplier = 0.95f;
        [SerializeField] private float accelMultiplier = 1.5f;
        [SerializeField] private float angleMinToExitDrift = 0.1f;

        private bool driftEngaged;
        private float driftValue;
        
        [Header("NITROMODE")] [SerializeField]
        private float nitroAcceleration = 2;
        [SerializeField] private float nitroEnergyConsuption = 0.5f;
        [SerializeField] private AnimationCurve nitroAccelerationByNitroTime;

        // INPUT VALUES
        private Vector2 stickValue;
        private float brakeForce;
        private bool driftBrake,nitroMode;
        private float nitroModeEntryEnergy;
        private float speedFactor => speed / maxSpeed;
        
        
        // DRAFT VALUES - To be Deleted
        public TMP_Text speedDisplay;
        

        #endregion

        #region Updates

        private void Start()
        {
            rb.centerOfMass = localCenterOfMass;
        }

        private void Update()
        {
            // TOURNER LES ROUES
            for (int i = 0; i < wheels.Length; i++)
            {
                if (wheels[i].steeringFactor > 0)
                {
                    wheels[i].wheelVisual.localRotation = wheels[i].transform.localRotation = Quaternion.Lerp(
                        wheels[i].transform.localRotation,
                        Quaternion.Euler(0,
                            stickValue.x                                      // Valeur du Stick ( 0 - 1 )
                            * wheels[i].steeringFactor                          // SteeringFactor de la roue ( 0 - 1 )
                            * steeringBySpeedFactor.Evaluate(speedFactor)       // Courbe de steering par speedFactor ( 0 - 1 )
                            * (baseMaxSpeed / maxSpeed)                         // Facteur de Vitesse Bonus ( 0 - 1 )
                            * (driftBrake ? steeringMultiplier : 1), 0)       // DriftBrake Multiplier
                        , Time.deltaTime * steeringSpeed);
                }
            }

            // EXECUTION DU NITRO MODE
            if (nitroMode) ExecuteNitroMode();

            // EXECUTION DU DRIFT BRAKE
            if (driftBrake) ExecuteDrift();

            speedDisplay.text = ((int) speed) + "/" + ((int) maxSpeed);

            // SI PRISE D'UN MUR OU BRAKE AU DELA DE 20% DE VITESSE MAX, PERTE DU BONUS
            if (speedFactor < 0.2f) maxSpeed = baseMaxSpeed;
        }

        void FixedUpdate()
        {
            if (!wheelForcesApply) return;
            
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

            // SI ROUE AU SOL, ALORS FORCES
            if (Physics.Raycast(wheel.transform.position, -wheel.transform.up, out RaycastHit hit,
                suspensionLenght + anchoring))
            {
                // CALCUL DES FORCES
                float suspension = GetWheelSuspensionForce(wheel, hit);
                float directionalDamp = GetWheelDirectionalDampening(wheel);
                float drivingForce = wheel.drivingFactor > 0 ? GetWheelAcceleration(wheel) : 0;
                wheelForce = wheel.transform.up * suspension +
                             wheel.transform.right * directionalDamp +
                             wheel.transform.forward * drivingForce;

                // DEBUG RAYS
                Debug.DrawRay(wheel.transform.position, wheel.transform.up * suspension, Color.green);
                Debug.DrawRay(wheel.transform.position, wheel.transform.right * directionalDamp, Color.red);
                Debug.DrawRay(wheel.transform.position, wheel.transform.forward * drivingForce, Color.blue);
                Debug.DrawRay(wheel.transform.position, wheelForce, Color.white);
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

            force = wheelMass * counterAcceleration * (driftBrake ? dampeningMultiplier : 1);

            return force;
        }

        float GetWheelAcceleration(Wheel wheel)
        {
            float force = 0;

            float accel = (1 - brakeForce) * acceleration * (speedFactor >= 1 ? 0 : 1) * accelerationBySpeedFactor.Evaluate(speedFactor) * wheel.drivingFactor *
                          (driftBrake ? accelMultiplier : 1);

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
        
        public void RShoulder(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                StartNitroMode();
            }

            if (context.canceled)
            {
                EndNitroMode();
            }
        }
        public void LShoulder(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if ((stickValue.x > 0.6f || stickValue.x < -0.6f) && speedFactor > 0.5f)
                {
                    StartDrift();
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
            if (context.performed)
            {
                stickValue = context.ReadValue<Vector2>();
            }
            else
            {
                stickValue = Vector2.zero;
            }
        }

        #endregion

        #region Drift

        void StartDrift()
        {
            driftBrake = true;
            bodyMat.color = Color.cyan;
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].tireMarksGenerator.StartTireMark();
            }
        }

        void ExecuteDrift()
        {
            driftValue = 1 - Mathf.Abs(Vector3.Dot(new Vector3(rb.velocity.normalized.x, 0, rb.velocity.normalized.z),
                transform.forward));
            
            if (!driftEngaged && driftValue > angleMinToExitDrift + 0.1f)
            {
                driftEngaged = true;
            }
            else if (driftEngaged && driftValue < angleMinToExitDrift)
            {
                driftEngaged = false;
                EndDrift();
            }
        }
        
        void EndDrift()
        {
            driftBrake = false;
            bodyMat.color = new Color(1, 0.6f, 0);
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].tireMarksGenerator.EndTireMark();
            }
        }

        #endregion

        #region Nitro Mode

        void StartNitroMode()
        {
            nitroMode = true;
            nitroModeEntryEnergy = abilitiesManager.energy;
        }

        void ExecuteNitroMode()
        {
            if (abilitiesManager.UseEnergy(Time.deltaTime * nitroEnergyConsuption))
            {
                maxSpeed += Time.deltaTime * nitroAcceleration *
                            nitroAccelerationByNitroTime.Evaluate((nitroModeEntryEnergy - abilitiesManager.energy) /
                                                                  abilitiesManager.energySegments);
            }
            else
            {
                EndNitroMode();
            }
        }
        
        void EndNitroMode()
        {
            if (!nitroMode) return;
            nitroMode = false;
        }

        #endregion
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
