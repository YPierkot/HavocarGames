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

        public int boostedDashs;


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
            }
        }

        public void PressDash(InputAction.CallbackContext context)
        {
            if (context.started) GameManager.instance.controller.steeringInputEnabled = false;

            else if (context.canceled)
            {
                ReleaseDash();
            }
        }

        public async void ReleaseDash()
        {
            if (timer > 0) return;
            timer = cooldown;

            Collider[] results;
            results = Physics.OverlapSphere(transform.position, 2, LayerMask.NameToLayer("Projectile"));
            if (results.Length > 0)
            {
                GameManager.instance.prowessManager.TriggerProwessEvent(0.1f, "Escaped a Bullet !", 5);
            }


            GameManager.instance.controller.steeringInputEnabled = true;

            Vector2 carForwardCamera = Quaternion.Euler(0, 0, -45) * new Vector2(
                GameManager.instance.controller.transform.forward.x,
                GameManager.instance.controller.transform.forward.z);


            float angleDiff = Vector2.Dot(stickValue.normalized, carForwardCamera.normalized);

            float sign = Vector2.SignedAngle(stickValue.normalized, carForwardCamera.normalized);

            bool dashThrough = dashThroughWall;
            if (dashThrough) GameManager.instance.controller.gameObject.layer = 11;

            bool dashBoosted = boostedDashs > 0;


            if (angleDiff > dashAngleSwitch)
            {
                directionIndex = 0;
            }
            else if (angleDiff > -dashAngleSwitch)
            {
                directionIndex = sign > 0 ? 2 : 1;
            }

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
                                 direction * Time.deltaTime * dashSpeed * speedCurve.Evaluate(i / duration);
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
        }
    }
}
