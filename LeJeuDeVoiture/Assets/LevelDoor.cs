using System;
using ManagerNameSpace;
using TMPro;
using UnityEngine;

public class LevelDoor : MonoBehaviour, IDamageable
{
    [SerializeField] private bool isWeak;
    [SerializeField] private bool isDestructionReady;

    [SerializeField] private int maxLifePoints;
    [SerializeField] private int currentLifePoints;

    [SerializeField] private TextMeshProUGUI lifeText;

    [Header("Weakness Attribute Section")]
    [SerializeField] private float limitPercentageToSwitchDestructMode = 10;
    [SerializeField] private float weaknessDuration = 20;
    private float weakTimer;
    [SerializeField] private float weaknessDamageMultiplyer = 2;

    [Header("Regeneration Attribute Section")] 
    [SerializeField] private int regenerationTowersCount;
    [SerializeField] private float regenerationTimer;
    [SerializeField] private int hpRegenPerSeconds;
    
    
    private void Start()
    {
        if (isDestructionReady) GetComponent<Collider>().isTrigger = true;
        currentLifePoints = maxLifePoints;
        UpdateCanvas();
    }

    private void Update()
    {
        if (isWeak)
        {
            weakTimer -= Time.deltaTime;
            if (weakTimer < 0)
            {
                UnsetWeakMode();
            }
        }

        if (regenerationTowersCount > 0)
        {
            
        }
    }

    public void SetWeakness()
    {
        Debug.Log("Set Weakness");
        weakTimer += weaknessDuration;
        isWeak = true;
    }
    
    private void UnsetWeakMode()
    {
        Debug.Log("Unset Weakness");
        isWeak = false;
        weakTimer = 0;
    }

    public void AddRegenerationTower()
    {
        regenerationTowersCount++;
    }

    public void RemoveRegenerationTower()
    {
        regenerationTowersCount--;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isDestructionReady) return;
        if (!other.CompareTag("Player")) return;
        if (!GameManager.instance.controller.abilitiesManager.isInBulletMode) return;

        Kill();
    }

    public void TakeDamage(int damages)
    {
        if (isWeak) currentLifePoints -= Mathf.FloorToInt(damages * weaknessDamageMultiplyer);
        else currentLifePoints -= damages;
        
        UpdateCanvas();

        Debug.Log((float)currentLifePoints / maxLifePoints);
        Debug.Log((float)currentLifePoints / maxLifePoints <= limitPercentageToSwitchDestructMode / 100);
        
        if ((float)currentLifePoints / maxLifePoints <= limitPercentageToSwitchDestructMode / 100)
        {
            SwitchDestructMode();
        }
    }

    private void SwitchDestructMode()
    {
        isDestructionReady = true;
        GetComponent<Collider>().isTrigger = true;
    }

    private void UpdateCanvas()
    {
        lifeText.text = $"{currentLifePoints}/{maxLifePoints}";
        GameManager.instance.uiManager.SetDoorLife(currentLifePoints, maxLifePoints, isDestructionReady);
    }

    public void Kill()
    {
        LevelManager.Instance.GoNextLevel();
        Destroy(gameObject);
    }
}
