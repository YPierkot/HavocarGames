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
    [SerializeField] private GameObject guardianTowerVariantPrefab;
    
    [Serializable]
    public struct Level
    {
        [Tooltip("A REMPLIR")]      public LevelDoor levelDoor;
        [Tooltip("A REMPLIR")]      public List<Tower> levelEnemies;
        [Tooltip("NE PAS REMPLIR")] public List<Vector3> enemiesPos;
        [Tooltip("NE PAS REMPLIR")] public List<bool> isEnemyAtPosAlive;
        [Tooltip("NE PAS REMPLIR")] public List<EnemyType> enemyTypes;
        [Tooltip("NE PAS REMPLIR")] public List<Tower.EnemyAttribute> enemyAttribute;
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
                levels[i].enemiesPos.Add(levels[i].levelEnemies[j].transform.position); // Set enemy WorldPos
                levels[i].isEnemyAtPosAlive.Add(true); // Set enemy isAlive
                levels[i].enemyAttribute.Add(levels[i].levelEnemies[j].enemyAttribute); // Set enemy attribute
                //Set enemyTypes
                if (levels[i].levelEnemies[j].GetComponent<GuardianTower>())       levels[i].enemyTypes.Add(EnemyType.Guardian);
                else if(levels[i].levelEnemies[j].GetComponent<GuardianVariant>()) levels[i].enemyTypes.Add(EnemyType.GuardianVariant);
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
            if (levels[currentLevelIndex].levelEnemies[i].GetComponent<Tower>().enemyAttribute == Tower.EnemyAttribute.Regeneration)
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
                Tower newEnemy = null;
                
                switch (levels[currentLevelIndex].enemyTypes[i])
                {
                    case EnemyType.Guardian:
                        newEnemy = Instantiate(guardianTowerPrefab, levels[currentLevelIndex].enemiesPos[i], Quaternion.identity).GetComponent<Tower>();
                        break;
                    case EnemyType.GuardianVariant:
                        newEnemy = Instantiate(guardianTowerVariantPrefab, levels[currentLevelIndex].enemiesPos[i], Quaternion.identity).GetComponent<Tower>();
                        break;
                    default: Debug.Log("Appeler 0782535570"); break;
                }
                
                newEnemy.enemyAttribute = levels[currentLevelIndex].enemyAttribute[i];
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
                
                Debug.Log(tower.enemyAttribute);
                switch (tower.enemyAttribute)
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
        GameManager.instance.controller.AddMaxSpeed(25);
        StartLevel();
    }

    public enum EnemyType
    {
        Guardian,
        GuardianVariant
    }
}

