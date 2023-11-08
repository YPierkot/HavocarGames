using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Car;
public class CameraManager : MonoBehaviour
{
    private float dirCam;
    private CarController controller;
    
    // Start is called before the first frame update
    void Start()
    {
        controller = GameManager.instance.controller;
    }

    // Update is called once per frame
    void FixedUpdate()
    {  
        dirCam = Mathf.Lerp(dirCam, controller.rb.velocity.magnitude,Time.fixedDeltaTime*3);
        transform.position = Vector3.Lerp(transform.position,controller.transform.position + controller.rb.velocity.normalized * dirCam * 0.5f,5*Time.fixedDeltaTime);
    }
}
