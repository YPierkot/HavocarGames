using System.Collections;
using System.Collections.Generic;
using EnemyNamespace;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Enemy))]
public class EnemyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Enemy enemy = (Enemy)target;
        
        enemy.holdSentinel = GUILayout.Toggle(enemy.holdSentinel, "Flag");
        
        if (enemy.holdSentinel)
        {
            // Draw sentinelsList
            // enemy.sentinelsList = GUILayout.
            // Draw sentinelRandomRange
            // Draw currentSentinelCount
            // Draw spawningRadius
            // Draw sentinelsPrefab
            
        }
    }
}
