using UnityEngine;
using CarNameSpace;

namespace ManagerNameSpace
{
    public class CameraManager : MonoBehaviour
    {
        private float dirCam;
        private CarController controller;
        public Camera cam;
        public int zoomMin, zoomMax;
        public int minMaxSpeed, maxMaxSpeed;
        public Vector3 cameraOffset;
        public float camPrevision;


        void Start()
        {
            controller = GameManager.instance.controller;
        }

        void FixedUpdate()
        {
            dirCam = controller.speed *camPrevision;
            
            transform.position = Vector3.Lerp(transform.position,
                controller.transform.position + controller.rb.velocity.normalized * dirCam * 0.5f + cameraOffset,
                5 * Time.fixedDeltaTime);

            float speedValue = Mathf.InverseLerp(minMaxSpeed,maxMaxSpeed,controller.maxSpeed);
            transform.localScale = Vector3.Lerp(transform.localScale,Vector3.Lerp(Vector3.one * zoomMin,Vector3.one * zoomMax, speedValue),Time.fixedDeltaTime * 5 );
        }
    }
}
