using UnityEngine;
using CarNameSpace;

namespace ManagerNameSpace
{
    public class CameraManager : MonoBehaviour
    {
        private float dirCam;
        private CarController controller;
        public float cameraSize;
        public Camera cam;
        public int fovMin, fovMax;
        public float zoomByBonusSpeed;
        public Vector3 cameraOffset;
        

        void Start()
        {
            controller = GameManager.instance.controller;
        }

        void FixedUpdate()
        {
            transform.position = Vector3.Lerp(transform.position,
                controller.transform.position + controller.rb.velocity.normalized * dirCam * 0.5f + cameraOffset,
                5 * Time.fixedDeltaTime);
            
        }
    }
}
