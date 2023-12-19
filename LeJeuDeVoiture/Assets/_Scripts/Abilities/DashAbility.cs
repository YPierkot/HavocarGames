using System.Threading.Tasks;
using ManagerNameSpace;
using UnityEngine;
using UnityEngine.InputSystem;


namespace AbilityNameSpace
{
    public class DashAbility : MonoBehaviour
    {
        [SerializeField] private int directionIndex;
        [SerializeField] private float dashDuration, dashSpeed;

        [SerializeField] private Vector2 stickValue = Vector2.up;
        [SerializeField] private GameObject forwardParticles, rightParticles, leftParticles;
        [SerializeField] private GameObject particleObj;
        [SerializeField] private AnimationCurve speedCurve, particleSizeCurve;
        [SerializeField] private float collisionCheckRadius;
        [SerializeField] private float dashExpansion;
        [SerializeField] private LayerMask wallMask, dashThroughMask;
        [SerializeField] private bool dashThroughWall;
        private float dashThroughWallTimer;
        [SerializeField] private float dashAngleSwitch = 0.5f;
        [SerializeField] private float cooldown;
        private float timer;
        public bool isDashing;

        public int boostedDashs;
        
        Vector2 carForwardCamera => Quaternion.Euler(0, 0, -45) * new Vector2(
            GameManager.instance.controller.transform.forward.x,
            GameManager.instance.controller.transform.forward.z);


        public void SetDashThroughWallsTimer(float time)
        {
            dashThroughWallTimer = time;
            dashThroughWall = true;
        }

        private void Update()
        {
            if (dashThroughWallTimer > 0)
            {
                dashThroughWallTimer -= Time.deltaTime;
            }
            else
            {
                dashThroughWall = false;
            }

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
                }
                else if (Mathf.Abs(signedAngle) < 135)
                {
                    directionIndex = signedAngle > 0 ? 2 : 1;
                }
            }
        }

        public void PressDash(InputAction.CallbackContext context)
        {
            if (context.started) GameManager.instance.controller.steeringInputEnabled = false;

            else if (context.canceled)
            {
                GameManager.instance.controller.steeringInputEnabled = true;
                ReleaseDash();
            }
        }

        void ReleaseDash()
        {
            if (timer > 0) return;
            timer = cooldown;
            
            Vector3 direction = GameManager.instance.controller.transform.forward;
            switch (directionIndex)
            {
                case 0:
                    direction = GameManager.instance.controller.transform.forward;
                    particleObj = forwardParticles;
                    break;
                case 1:
                    direction = -GameManager.instance.controller.transform.right;
                    particleObj = rightParticles;
                    break;
                case 2:
                    direction = GameManager.instance.controller.transform.right;
                    particleObj = leftParticles;
                    break;
            }

            direction = new Vector3(direction.x, 0, direction.z);
            Dash(direction);
        }

        public async void Dash(Vector3 dir)
        {
            
            isDashing = true;
            Collider[] results;

            bool dashThrough = dashThroughWall;
            if (dashThrough) GameManager.instance.controller.gameObject.layer = 11;

            bool dashBoosted = boostedDashs > 0;

            particleObj.SetActive(true);
            particleObj.transform.localScale = Vector3.zero;

            if (dashBoosted)
            {
                GameManager.instance.controller.abilitiesManager.EnterBulletMode(BulletModeSources.DashCapacity);
                boostedDashs--;
            }

            float i = 0;
            float duration = dashDuration * (dashBoosted ? dashExpansion : 1);

            while (i < duration)
            {
                if (dashThrough)
                {
                    results = Physics.OverlapSphere(GameManager.instance.controller.rb.position, collisionCheckRadius,
                        wallMask);
                    if (results.Length == 0) i += Time.deltaTime;
                }
                else
                {
                    i += Time.deltaTime;
                }

                Vector3 newPos = GameManager.instance.controller.rb.position +
                                 dir * Time.deltaTime * dashSpeed * speedCurve.Evaluate(i / duration);
                results = Physics.OverlapSphere(newPos, collisionCheckRadius, dashThrough ? dashThroughMask : wallMask);
                if (results.Length == 0)
                {
                    GameManager.instance.controller.rb.position = newPos;
                }
                else
                {
                    break;
                }

                particleObj.transform.localScale = Vector3.one * particleSizeCurve.Evaluate(i / duration);
                await Task.Yield();
            }

            if (dashBoosted)
                GameManager.instance.controller.abilitiesManager.QuitBulletMode(BulletModeSources.DashCapacity);
            if (dashThrough) GameManager.instance.controller.gameObject.layer = 8;
            particleObj.SetActive(false);
            isDashing = false;
        }
    }
}
