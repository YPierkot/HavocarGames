using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarNameSpace;
using ManagerNameSpace;
using UnityEngine;
using UnityEngine.Serialization;

public class GenericInteraction_WIP : EnvironmentInteraction
{
    public InteractionsType interactionType = InteractionsType.EnergyItem;
    public InteractiveSettings_WIP interactiveSettings;
    public List<GameObject> padVisualRenderer; 
    public List<ParticleSystem> padPSRenderer = new List<ParticleSystem>();
    public List<Material> padVisualMat = new List<Material>();
    
    [Header("--- Collider Size ---")]
    [Tooltip("If the collider size is not correct, use the context menu 'Update Collide' to update it")]
    public GameObject scalingRenderer; //Attach the Scaling Renderer
    
    [Header("Curve Jump Pad")]
    public Transform controlTransform;
    public Transform endTransform;
    
    //Private
    private bool canBePickedUp = true;
    private bool isBoosting = false;
    private bool isRotating = false;
    private Color[] padColors; //Store the color
    private string padColorName = "_BaseColor"; //Name of the color in the shader 
    private BoxCollider padBoxCollider; // Get the Box Collider

    

    private void Awake()
    {
        //DisplayPadRenderer();
        //PopulateColorArray();
    }
    

    public override void Interact(CarController player)
    {
        base.Interact(player);
        switch (interactionType)
        {
            //Implement Energy Item
            case InteractionsType.EnergyItem:
                GameManager.instance.abilitiesManager.AddEnergy(interactiveSettings.energyAmount);
                break;
            //Implement Shield Item
            case InteractionsType.ShieldItem:
                _ = ActivateShieldAsync(player);
                EnableParticleSystems((int)interactionType); 
                break;
            //Implement Jump Pad
            case InteractionsType.JumpPad:
                _ = ActivateJumpQuadraticBAsync(player, controlTransform, endTransform, interactiveSettings.jumpPadDuration);
                EnableParticleSystems((int)interactionType); 
                break;
            //Implement Booster Pad
            case InteractionsType.BoosterPad:
                _ = StartBoosterAsync(player);
                EnableParticleSystems((int)interactionType); 
                break;
        }
    }
    
    private async Task ActivateShieldAsync(CarController player)
    {
        // TODO : Implement the function for activate the shield
        //player.ActivateShield(interactiveSettings.shieldDuration);
        
        gameObject.SetActive(false); //Disable the object

        await Task.Delay((int)(interactiveSettings.shieldRespawnValue * 1000));

        gameObject.SetActive(true); //Enable

        canBePickedUp = true;
    }
    
    private async Task ActivateJumpAsync(CarController player, float jumpForce)
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Debug.Log(jumpForce);
            Quaternion originalRotation = rb.rotation;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            rb.freezeRotation = true;

            await Task.Delay((int)(3.0f * 1000));
            
            await Task.Yield();
            rb.rotation = originalRotation;
            rb.freezeRotation = false;
        }
    }
    
    private async Task ActivateJumpLinerBAsync(CarController player, Transform endTransform, float curveDuration)
    {
        Vector3 startPos = player.transform.position;
        Vector3 endPos = endTransform.position;

        float elapsedTime = 0f;

        while (elapsedTime < curveDuration)
        {
            float t = elapsedTime / curveDuration;
            
            Vector3 jumpPoint = CalculateBezierPoint(startPos, startPos + Vector3.up * interactiveSettings.jumpPadForce, endPos, t);
            player.transform.position = jumpPoint;

            elapsedTime += Time.deltaTime;
            await Task.Yield(); 

            if (!gameObject.activeSelf) return; 
        }

        player.transform.position = endPos;
    }
    
    
    private async Task ActivateJumpQuadraticBAsync(CarController player, Transform controlTransform, Transform endTransform, float curveDuration)
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.freezeRotation = true;
            
            Vector3 startPos = player.transform.position;
            Vector3 controlPos = controlTransform.position;
            Vector3 endPos = endTransform.position;

            float elapsedTime = 0f;
            
            while (elapsedTime < curveDuration)
            {
                float t = elapsedTime / curveDuration;

                Vector3 jumpPoint = CalculateQuadraticBezierPoint(startPos, controlPos, endPos, t);
                player.transform.position = jumpPoint;

                elapsedTime += Time.deltaTime;
                await Task.Yield(); 

                if (!gameObject.activeSelf) return; 
            }
            player.transform.position = endPos;
            rb.freezeRotation = false;
        }
    }



    
    private async Task StartBoosterAsync(CarController player)
    {
        if (!isBoosting)
        {
            player.maxSpeed += interactiveSettings.increaseSpeedMax;
            isBoosting = true;

            await Task.Delay((int)(interactiveSettings.boosterDuration * 1000));

            await StopBoosterAsync(player);
        }
    }
    

    private async Task StopBoosterAsync(CarController player)
    {
        if (isBoosting)
        {
            //player.maxSpeed = player.baseMaxSpeed;
            isBoosting = false;
        }
    }

    [ContextMenu("Update Collider and Display")]
    private void UpdateDisplayAndCollider()
    {
        UpdateCollider();
        DisplayPadRenderer();
    }
    
    private void DisplayPadRenderer()
    {
        DisableAllVisualPad();
        DisableAllParticulesSystemPad();
        SelectPrefabToDisplay(interactionType, padVisualRenderer);
    }
    
    private void DisableAllVisualPad()
    {
        foreach (GameObject pad in padVisualRenderer)
        {
            pad.SetActive(false);
        }
    }

   
    private void SelectPrefabToDisplay(InteractionsType type, List<GameObject> visualPad)
    {
        switch (type)
        {
            case InteractionsType.EnergyItem:
                visualPad[0].SetActive(true);
                padVisualMat[0].SetColor(padColorName, padColors[0]); //TODO : Refactor this part and make a function
                break;
            case InteractionsType.ShieldItem:
                visualPad[1].SetActive(true);
                padVisualMat[1].SetColor(padColorName, padColors[1]);
                break;
            case InteractionsType.BoosterPad:
                visualPad[2].SetActive(true);
                padVisualMat[2].SetColor(padColorName, padColors[2]);
                break;
            case InteractionsType.JumpPad:
                visualPad[3].SetActive(true);
                padVisualMat[3].SetColor(padColorName, padColors[3]);
                break;
        }
    }
    
    private void EnableParticleSystems(int type)
    {
        foreach (var ps in padPSRenderer)
        {
            ps.Stop(); 
            ps.gameObject.SetActive(false);
            if (padPSRenderer.IndexOf(ps) == (int)type - 1)
            {
                ps.gameObject.SetActive(true);
                ps.Play(); 
            }
        }
    }
    
    private void DisableAllParticulesSystemPad()
    {
        foreach (ParticleSystem pad in padPSRenderer)
        {
            pad.Stop();
            pad.gameObject.SetActive(false);
        }
    }

    [ContextMenu("Populate Color Array")]
    //TODO : Redo this function
    private void PopulateColorArray()
    {
        padColors = new Color[Enum.GetValues(typeof(InteractionsType)).Length];
        padColors[0] = interactiveSettings.energyPadColor;
        padColors[1] = interactiveSettings.shieldPadColor;
        padColors[2] = interactiveSettings.boostPadColor;
        padColors[3] = interactiveSettings.jumpPadColor;
    }
    
    private void UpdateCollider()
    {
        padBoxCollider = GetComponent<BoxCollider>();
        padBoxCollider.isTrigger = true;
        Vector3 localScale = scalingRenderer.transform.localScale;
        padBoxCollider.size = new Vector3(localScale.x, 2, localScale.z);
    }
    
    private Vector3 CalculateQuadraticBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0; // (1-t)^2 * P0
        p += 2 * u * t * p1; // 2 * (1-t) * t * P1
        p += tt * p2; // t^2 * P2

        return p;
    }
    
    private Vector3 CalculateBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0; // (1-t)^3 * P0
        p += 3 * uu * t * p1; // 3 * (1-t)^2 * t * P1
        p += 3 * u * tt * p2; // 3 * (1-t) * t^2 * P2
        p += ttt * endTransform.position; // t^3 * P3

        return p;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (endTransform != null)
        {
            Vector3 startPos = transform.position;
            Vector3 controlPos = controlTransform.position;
            Vector3 endPos = endTransform.position;

            for (float t = 0; t <= 1; t += 0.1f)
            {
                //Vector3 point = CalculateBezierPoint(startPos, startPos + Vector3.up * interactiveSettings.jumpPadForce, endPos, t);
                Vector3 point = CalculateQuadraticBezierPoint(startPos, controlPos, endPos, t);
                Gizmos.DrawSphere(point, 0.1f);
            }
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(endPos, 0.2f);
        }
    }
}
