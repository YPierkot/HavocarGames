using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TilingSetter : MonoBehaviour
{
    public float maxAngle;
    public int maxQuarter;
    public GameObject[] possibleTiles;
    
    [ContextMenu("SetRandomAngle")]
    public void SetRandomAngles()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).rotation = Quaternion.Euler(-90,Random.Range(0,maxQuarter)*90 + Random.Range(-maxAngle,maxAngle),0);
        }
    }
    
    
}
