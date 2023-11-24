using System;
using System.Collections;
using System.Collections.Generic;
using CarNameSpace;
using Unity.Mathematics;
using UnityEngine;

public class TireMarksGenerator : MonoBehaviour
{
    [SerializeField] private GameObject tireMark;
    private bool generating;
    private LineRenderer tireMarkRenderer;

    public void StartTireMark()
    {
        generating = true;
        tireMarkRenderer = Instantiate(tireMark,Vector3.zero, quaternion.identity).GetComponent<LineRenderer>();
        tireMarkRenderer.SetPosition(0,transform.position);
    }
    
    public void EndTireMark()
    {
        generating = false;
    }

    private void FixedUpdate()
    {
        if (generating)
        {
            tireMarkRenderer.positionCount++;
            tireMarkRenderer.SetPosition(tireMarkRenderer.positionCount-1,transform.position);   
        }
    }
}
