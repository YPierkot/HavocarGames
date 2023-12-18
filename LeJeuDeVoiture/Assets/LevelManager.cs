using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnemyNamespace;
using ManagerNameSpace;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private static LevelManager instance;
    public static LevelManager Instance => instance;

    [SerializeField] private int currentLevelIndex = 0;
    public Level[] levels = Array.Empty<Level>();

    [SerializeField] private GameObject guardianTowerPrefab;
    
    [Serializable]
    public struct Level
    {
        public int levelCount;
        public LevelDoor levelDoor;
        public List<Enemy> levelEnemies;
        public List<Vector3> enemiesPos;
        public List<bool> isEnemyAtPosAlive;
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

    private void Start()
    {
        InitGame();
    }

    private void InitGame()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            for (int j = 0; j < levels[i].levelEnemies.Count; j++)
            {
                levels[i].levelEnemies[j].enabled = false;
                levels[i].enemiesPos.Add(levels[i].levelEnemies[j].transform.position);
                levels[i].isEnemyAtPosAlive.Add(true);
            }
        }

        StartLevel();
    }

    private void StartLevel()
    {
        for (int i = 0; i < levels[currentLevelIndex].levelEnemies.Count; i++)
            levels[currentLevelIndex].levelEnemies[i].enabled = true;

        for (int i = 0; i < levels[currentLevelIndex].levelEnemies.Count; i++)
        {
            if (levels[currentLevelIndex].levelEnemies[i].GetComponent<Tower>().attribute == Tower.EnemyAttribute.Regeneration)
            {
                levels[currentLevelIndex].levelDoor.AddRegenerationTower();
            }
        }
        
        GameManager.instance.uiManager.SetCurrentLevelDoorName(levels[currentLevelIndex].levelDoor.name);
        levels[currentLevelIndex].levelDoor.UpdateCanvas();
    }

    public void ReviveTowers()
    {
        for (int i = 0; i < levels[currentLevelIndex].enemiesPos.Count; i++)
        {
            if (levels[currentLevelIndex].isEnemyAtPosAlive[i] == false)
            {
                var newEnemy = Instantiate(guardianTowerPrefab, levels[currentLevelIndex].enemiesPos[i], Quaternion.identity).GetComponent<Enemy>();
                levels[currentLevelIndex].isEnemyAtPosAlive[i] = true;
                levels[currentLevelIndex].levelEnemies.Add(newEnemy);
            }
        }
    }
    
    public Task OnTowerDie(Tower tower, int damages)
    {
        for (int i = 0; i < levels[currentLevelIndex].isEnemyAtPosAlive.Count; i++)
        {
            if (tower == null || !tower.enabled)
            {
                Debug.Log("No more towers in level");
                for (int j = 0; j < levels[currentLevelIndex].isEnemyAtPosAlive.Count; j++) levels[currentLevelIndex].isEnemyAtPosAlive[j] = false;
            }
            
            if (levels[currentLevelIndex].levelEnemies[i] == tower)
            {
                levels[currentLevelIndex].levelDoor.TakeDamage(damages);
                levels[currentLevelIndex].isEnemyAtPosAlive[i] = false;
                levels[currentLevelIndex].levelEnemies.RemoveAt(i);
                
                Debug.Log(tower.attribute);
                switch (tower.attribute)
                {
                    case Tower.EnemyAttribute.None: break;
                    case Tower.EnemyAttribute.Fragile: levels[currentLevelIndex].levelDoor.SetWeakness(); break;
                    case Tower.EnemyAttribute.Regeneration: levels[currentLevelIndex].levelDoor.RemoveRegenerationTower(); break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        return Task.CompletedTask;
    }

    public void GoNextLevel()
    {
        currentLevelIndex++;
        GameManager.instance.controller.maxSpeed += 25;
        StartLevel();
    }
}
