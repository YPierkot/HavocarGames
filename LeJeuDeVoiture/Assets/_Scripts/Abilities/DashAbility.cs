using System.Threading.Tasks;
using ManagerNameSpace;
using UnityEngine;
using UnityEngine.InputSystem;


namespace AbilityNameSpace
{
    public class DashAbility : MonoBehaviour
    {
        [SerializeField] private int directionIndex;
        [SerializeField] private float dashLenght, dashSpeed;

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
            Debug.Log(dir);
            Vector3 velocity = GameManager.instance.controller.rb.velocity;
            float speed = directionIndex == 0 ? 0 :velocity.magnitude;
            //GameManager.instance.controller.rb.velocity = Vector3.zero;
            Vector3 pos1 = transform.position;
            
            isDashing = true;
            Collider[] results;

            bool dashThrough = dashThroughWall;
            if (dashThrough) GameManager.instance.controller.gameObject.layer = 11;

            particleObj.SetActive(true);
            particleObj.transform.localScale = Vector3.zero;
            
            float duration = 0;
            while (duration < dashLenght)
            {
                Vector3 newPos = GameManager.instance.controller.rb.position +
                                 dir * Time.deltaTime * (dashSpeed + speed) * speedCurve.Evaluate(duration / dashLenght);
                results = Physics.OverlapSphere(newPos, collisionCheckRadius, dashThrough ? dashThroughMask : wallMask);
                if (results.Length == 0)
                {
                    GameManager.instance.controller.rb.position = newPos;
                }
                else
                {
                    Debug.Log("HIT SMTHING");
                    break;
                }

                particleObj.transform.localScale = Vector3.one * particleSizeCurve.Evaluate(duration / dashLenght);
                
                duration = Vector3.Distance(pos1,transform.position);
                await Task.Yield();
            }
            
            if (dashThrough) GameManager.instance.controller.gameObject.layer = 8;
            particleObj.SetActive(false);
            isDashing = false;

            GameManager.instance.controller.rb.velocity = velocity;
            Vector3 pos2 = transform.position;
            Debug.DrawLine(pos1,pos2,Color.green,300);
            Debug.Log("DASH DISTANCE : "+Vector3.Distance(pos1,pos2));
        }
    }
}
