using UnityEngine;
using CarNameSpace;

namespace ManagerNameSpace
{
    public class CameraManager : MonoBehaviour
    {
        private float dirCam;
        private CarController controller;

        void Start()
        {
            controller = GameManager.instance.controller;
        }

        void FixedUpdate()
        {
            dirCam = Mathf.Lerp(dirCam, controller.rb.velocity.magnitude, Time.fixedDeltaTime * 3);
            transform.position = Vector3.Lerp(transform.position,
                controller.transform.position + controller.rb.velocity.normalized * dirCam * 0.5f,
                5 * Time.fixedDeltaTime);
        }
    }
}
