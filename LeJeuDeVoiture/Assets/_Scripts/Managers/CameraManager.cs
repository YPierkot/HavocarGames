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
        

        void Start()
        {
            controller = GameManager.instance.controller;
        }

        void FixedUpdate()
        {
            dirCam = Mathf.Lerp(dirCam, controller.speed, Time.fixedDeltaTime * 5);
            transform.position = Vector3.Lerp(transform.position,
                controller.transform.position + controller.rb.velocity.normalized * dirCam * 0.5f,
                5 * Time.fixedDeltaTime);
            float speedValue = controller.maxSpeed / controller.baseMaxSpeed;
            transform.localScale = Vector3.Lerp(transform.localScale,Vector3.one * (cameraSize + (controller.maxSpeed - controller.baseMaxSpeed) *  zoomByBonusSpeed),Time.fixedDeltaTime * 5);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, Mathf.Lerp(fovMin, fovMax,speedValue - 1),Time.fixedDeltaTime * 5);
            //transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(0,controller.transform.eulerAngles.y,0),Time.fixedDeltaTime * 5 );
        }
    }
}
