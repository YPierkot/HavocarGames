using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AbilityNameSpace;
using ManagerNameSpace;
using UnityEngine;
using UnityEngine.InputSystem;

public class DashAbility : MonoBehaviour
{
    public int directionIndex;
    public float dashDuration,dashSpeed;
    
    public Vector2 stickValue = Vector2.up;
    public GameObject forwardParticles, rightParticles, leftParticles;
    public GameObject particleObj;
    public AnimationCurve speedCurve,particleSizeCurve;
    public float collisionCheckRadius;
    public LayerMask wallMask,dashThroughMask;
    public bool dashThroughWall;
    public float dashThroughWallTimer;


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
        if(context.started) GameManager.instance.controller.steeringInputEnabled = false;
        
        else if (context.canceled)
        {
            ReleaseDash();
        }
    }
    
    public async void ReleaseDash()
    {
        GameManager.instance.controller.steeringInputEnabled = true;
        
        Vector2 carForwardCamera = Quaternion.Euler(0, 0, -45) * new Vector2(
            GameManager.instance.controller.transform.forward.x,
            GameManager.instance.controller.transform.forward.z);
            

        float angleDiff = Vector2.Dot(stickValue.normalized,carForwardCamera.normalized);

        float sign = Vector2.SignedAngle(stickValue, carForwardCamera);

        bool dashThrough = dashThroughWall;
        if (dashThrough) GameManager.instance.controller.gameObject.layer = 11;
            
        if (angleDiff > 0.5)
        {
            directionIndex = 0;
        }
        else if (angleDiff > -0.5f)
        {
            directionIndex = sign > 0 ? 2 : 1;
        }
        
        Vector3 direction = GameManager.instance.controller.transform.forward;
        switch (directionIndex)
        {
            case 0 :
                direction = GameManager.instance.controller.transform.forward;
                particleObj = forwardParticles;
                break;
            case 1 :
                direction = -GameManager.instance.controller.transform.right;
                particleObj = rightParticles;
                break;
            case 2 :
                direction = GameManager.instance.controller.transform.right;
                particleObj = leftParticles;
                break;
        }

        direction = new Vector3(direction.x, 0, direction.z);

        particleObj.SetActive(true);
        particleObj.transform.localScale = Vector3.zero;
        Collider[] results;
        
        float i = 0;
        while (i < dashDuration)
        {
            if (dashThrough)
            {
                results = Physics.OverlapSphere(GameManager.instance.controller.rb.position, collisionCheckRadius,  wallMask );
                if(results.Length == 0) i += Time.deltaTime;
            }
            else
            {
                i += Time.deltaTime;   
            }

            Vector3 newPos = GameManager.instance.controller.rb.position + direction * Time.deltaTime * dashSpeed * speedCurve.Evaluate(i / dashDuration);
            results = Physics.OverlapSphere(newPos, collisionCheckRadius, dashThrough ? dashThroughMask : wallMask );
            Debug.Log(results.Length);
            if (results.Length == 0)
            {
                GameManager.instance.controller.rb.position = newPos;
            }
            else
            {
                break;
            }
            particleObj.transform.localScale = Vector3.one * particleSizeCurve.Evaluate(i / dashDuration);
            await Task.Yield();
        }
        
        if (dashThrough) GameManager.instance.controller.gameObject.layer = 8;
        particleObj.SetActive(false);
    }
    
    
    
    
    
    
    
    
    
    
    /*public override async void StartAbility()
    {
        base.StartAbility();
        
        Vector2 carForwardCamera = Quaternion.Euler(0, 0, -45) * new Vector2(
            GameManager.instance.controller.transform.forward.x,
            GameManager.instance.controller.transform.forward.z);
            

        float angleDiff = Vector2.Dot(stickValue.normalized,carForwardCamera.normalized);

        float sign = Vector2.SignedAngle(stickValue, carForwardCamera);

        
            
        if (angleDiff > 0.5)
        {
            directionIndex = 0;
        }
        else if (angleDiff > -0.5f)
        {
            directionIndex = sign > 0 ? 2 : 1;
        }
        
        Vector3 direction = GameManager.instance.controller.transform.forward;
        switch (directionIndex)
        {
            case 0 :
                direction = GameManager.instance.controller.transform.forward;
                break;
            case 1 :
                direction = -GameManager.instance.controller.transform.right;
                break;
            case 2 :
                direction = GameManager.instance.controller.transform.right;
                break;
        }

        direction = new Vector3(direction.x, 0, direction.z);
        Vector3 newPos = FindClosestDashablePosition(direction);
        Vector3 oldPos = GameManager.instance.controller.rb.position;
        
        GameManager.instance.controller.rb.angularVelocity = Vector3.zero;
        
        Debug.DrawLine(GameManager.instance.controller.transform.position,newPos,Color.magenta,5);
        GameManager.instance.controller.collider.enabled = false;
        GameManager.instance.controller.wheelForcesApply = false;
        
        //GameManager.instance.controller.transform.position = newPos;
        //GameManager.instance.controller.rb.velocity = Vector3.zero;
        //Debug.Break();
        
        dashVisualBody.gameObject.SetActive(true);
        
        float i = 0;
        while (i < 1)
        {
            dashVisualBody.position = oldPos;
            dashMaterial.SetFloat("_Dissolve",i);
            i += Time.deltaTime * dashSpeed;
            GameManager.instance.controller.rb.position = Vector3.Lerp(oldPos,newPos,speedCurve.Evaluate(i));
            await Task.Yield();
        }
        
        dashVisualBody.gameObject.SetActive(false);
        
        GameManager.instance.controller.rb.position = newPos;
        
        GameManager.instance.controller.gameObject.layer = 0;
        GameManager.instance.controller.collider.enabled = true;
        GameManager.instance.controller.wheelForcesApply = true;
        
    }

    public Vector3 FindClosestDashablePosition(Vector3 direction)
    {
        Vector3 startPos = GameManager.instance.controller.transform.position;
        Vector3 position = GameManager.instance.controller.transform.position + direction * dashInitialDistance;
        Collider[] results;
        
        for (int i = 0; i < maxBonusIterations; i++)
        {
            Debug.DrawLine(position,position+Vector3.up,Color.yellow,5);
            results = Physics.OverlapSphere(position,collisionCheckRadius,dashThroughMask);
            if (results.Length == 0)
            {
                break;
            }
            position += direction * checkDistance;

            if (i == maxBonusIterations - 1)
            {
                position = startPos;
            }
        }
        
        return position;
    }*/
}
