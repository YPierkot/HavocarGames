using System;
using EnemyNamespace;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    private Vector3[] positions;
    
    [Header("Spawning Attributes")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Vector2Int enemyPerWaveBoundsCount;
    [SerializeField] private float maxDistFromSpawner = 2.5f;
    [SerializeField] private float spawningInterval = 3f;

    private float timer = 10;

    [SerializeField] private SpawningShape SpawningShape = SpawningShape.Rect;

    [Space]
    [Header("CircleSpawnAttributes")]
    [SerializeField] private int spawnPointsCount = 10;
    [SerializeField] private float spawningRadius = 5f;
    
    [Space]
    [Header("RectSpawnAttributes")]
    [SerializeField] public int spawningPointPerSideCount = 7;
    [SerializeField] public float height = 30f;
    [SerializeField] public float width = 20f;
    
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > spawningInterval)
        {
            SpawnWave();
        }
    }

    /// <summary>
    /// Method qui est appelée pour faire spawn une vague d'ennemis
    /// </summary>
    void SpawnWave()
    {
        UpdateEnemySpawingPos();
        timer = 0;
        var temp = Random.Range(0, spawnPointsCount);
        var spawnTr = positions[temp] + transform.InverseTransformPoint(positions[temp]);
        var nbThisWave = Random.Range(enemyPerWaveBoundsCount.x, enemyPerWaveBoundsCount.y + 1);
            
        for (int i = 0; i < nbThisWave; i++)
        {
            Vector2 spawnPos =  Random.insideUnitCircle * maxDistFromSpawner;
            Vector3 initPos = spawnTr + new Vector3(spawnPos.x, 0, spawnPos.y);

            var GO = Instantiate(enemyPrefab, initPos, Quaternion.identity);
        }
    }
    
    /// <summary>
    /// Calculer la position des points de spawn des entités ennemis
    /// </summary>
    void UpdateEnemySpawingPos()
    {
        var currentPos = transform.position;
        
        if (SpawningShape == SpawningShape.Circle)
        {
            positions = new Vector3[spawnPointsCount];
            
            for (int i = 0; i < spawnPointsCount; i++)
            {
                float angle = i * (2 * Mathf.PI / spawnPointsCount);
                float x = Mathf.Cos(angle) * spawningRadius;
                float z = Mathf.Sin(angle) * spawningRadius;

                positions[i] = new Vector3(currentPos.x + x, 0, currentPos.z + z);
            }
        }
        else if (SpawningShape == SpawningShape.Rect)
        {
            positions = new Vector3[spawningPointPerSideCount*4];
            
            float semiHeight = height / 2;
            float semiWidth = width / 2;

            // Créer les points sur le côté supérieur
            for (int i = 0; i < spawningPointPerSideCount; i++)
            {
                float t = i / (float)(spawningPointPerSideCount - 1);
                float x = Mathf.Lerp(-semiHeight, semiHeight, t);
                float y = semiWidth;
                
                positions[i] = new Vector3(currentPos.x + x, 0, currentPos.z + y);
            }

            // Créer les points sur le côté droit
            for (int i = 0; i < spawningPointPerSideCount; i++)
            {
                float t = i / (float)(spawningPointPerSideCount - 1);
                float x = semiHeight;
                float y = Mathf.Lerp(semiWidth, -semiWidth, t);
                
                positions[i + spawningPointPerSideCount] = new Vector3(currentPos.x + x, 0, currentPos.z + y);
            }
            
            // Créer les points sur le côté inférieur
            for (int i = 0; i < spawningPointPerSideCount; i++)
            {
                float t = i / (float)(spawningPointPerSideCount - 1);
                float x = Mathf.Lerp(semiHeight, -semiHeight, t);
                float y = -semiWidth;

                positions[i + spawningPointPerSideCount * 2] = new Vector3(currentPos.x + x, 0, currentPos.z + y);
            }

            // Créer les points sur le côté gauche
            for (int i = 0; i < spawningPointPerSideCount; i++)
            {
                float t = i / (float)(spawningPointPerSideCount - 1);
                float x = -semiHeight;
                float y = Mathf.Lerp(-semiWidth, semiWidth, t);

                positions[i + spawningPointPerSideCount * 3] = new Vector3(currentPos.x + x, 0, currentPos.z + y);
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        UpdateEnemySpawingPos();
        
        for (int i = 0; i < positions.Length; i++)
        {
            Gizmos.DrawSphere( positions[i]+ transform.InverseTransformPoint(positions[i]), 1);
        }
    }
}

enum SpawningShape
{
    Circle,
    Rect
}

