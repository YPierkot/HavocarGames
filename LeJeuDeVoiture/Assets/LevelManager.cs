using System;
using System.Collections;
using System.Collections.Generic;
using EnemyNamespace;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private static LevelManager instance;
    public static LevelManager Instance => instance;

    public int currentLevelIndex = 0;
    public Level[] levels = Array.Empty<Level>();
    
    [Serializable]
    public struct Level
    {
        public int levelCount;
        public LevelDoor levelDoor;
        public Enemy[] levelEnnemies;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void StartLevel()
    {
        
    }
}

