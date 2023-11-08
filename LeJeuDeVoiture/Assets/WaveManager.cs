using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    [SerializeField] private Transform[] spawners;
    [SerializeField] private Vector2Int enemyPerWaveBoundsCount;
    [SerializeField] private float maxDistFromSpawner = 2.5f;
    [SerializeField] private float spawningInterval = 7f;
    private float timer = 7f;
    
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > spawningInterval)
        {
            timer = 0;
            var spawnTr = spawners[Random.Range(0, spawners.Length)];
            var nbThisWave = Random.Range(enemyPerWaveBoundsCount.x, enemyPerWaveBoundsCount.y + 1);
            
            for (int i = 0; i < nbThisWave; i++)
            {
                Vector2 spawnPos =  Random.insideUnitCircle * maxDistFromSpawner;
                Vector3 initPos = spawnTr.position + new Vector3(spawnPos.x, 0, spawnPos.y);

                Instantiate(enemyPrefab, initPos, Quaternion.identity);
            }
        }
    }
}

