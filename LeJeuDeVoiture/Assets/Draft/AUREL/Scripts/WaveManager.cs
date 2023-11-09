using System;
using EnemyNamespace;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    
    [SerializeField] private Vector2Int enemyPerWaveBoundsCount;
    [SerializeField] private float maxDistFromSpawner = 2.5f;
    [SerializeField] private float spawningInterval = 3f;
    
    private float timer = 10;

    private Vector3[] positions;

    [SerializeField] private SpawningShape SpawningShape = SpawningShape.Rect;
    [Header("CircleSpawnAttributes")]
    [SerializeField] private int spawnPointsCount = 10;
    [SerializeField] private float spawningRadius = 5f;
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
            UpdateEnemySpawingPos();
            timer = 0;
            var spawnTr = positions[Random.Range(0, spawnPointsCount)];
            var nbThisWave = Random.Range(enemyPerWaveBoundsCount.x, enemyPerWaveBoundsCount.y + 1);
            
            for (int i = 0; i < nbThisWave; i++)
            {
                Vector2 spawnPos =  Random.insideUnitCircle * maxDistFromSpawner;
                Vector3 initPos = spawnTr + new Vector3(spawnPos.x, 0, spawnPos.y);

                var GO = Instantiate(enemyPrefab, initPos, Quaternion.identity);
                GO.GetComponent<Enemy>().playerPos = transform;
            }
        }
    }
    
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
            
            float demiLongueur = height / 2;
            float demiLargeur = width / 2;

            // Créer les points sur le côté supérieur
            for (int i = 0; i < spawningPointPerSideCount; i++)
            {
                float t = i / (float)(spawningPointPerSideCount - 1);
                float x = Mathf.Lerp(-demiLongueur, demiLongueur, t);
                float y = demiLargeur;
                
                positions[i] = new Vector3(currentPos.x + x, 0, currentPos.z + y);
            }

            // Créer les points sur le côté droit
            for (int i = 0; i < spawningPointPerSideCount; i++)
            {
                float t = i / (float)(spawningPointPerSideCount - 1);
                float x = demiLongueur;
                float y = Mathf.Lerp(demiLargeur, -demiLargeur, t);
                
                positions[i + spawningPointPerSideCount] = new Vector3(currentPos.x + x, 0, currentPos.z + y);
            }
            
            // Créer les points sur le côté inférieur
            for (int i = 0; i < spawningPointPerSideCount; i++)
            {
                float t = i / (float)(spawningPointPerSideCount - 1);
                float x = Mathf.Lerp(demiLongueur, -demiLongueur, t);
                float y = -demiLargeur;

                positions[i + spawningPointPerSideCount * 2] = new Vector3(currentPos.x + x, 0, currentPos.z + y);
            }

            // Créer les points sur le côté gauche
            for (int i = 0; i < spawningPointPerSideCount; i++)
            {
                float t = i / (float)(spawningPointPerSideCount - 1);
                float x = -demiLongueur;
                float y = Mathf.Lerp(-demiLargeur, demiLargeur, t);

                positions[i + spawningPointPerSideCount * 3] = new Vector3(currentPos.x + x, 0, currentPos.z + y);
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        UpdateEnemySpawingPos();
        
        for (int i = 0; i < positions.Length; i++)
        {
            Gizmos.DrawSphere(positions[i], 1);
        }
    }
}

enum SpawningShape
{
    Circle,
    Rect
}

