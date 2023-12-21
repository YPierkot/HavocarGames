using ManagerNameSpace;
using TMPro;
using UnityEngine;

public class LevelDoor : MonoBehaviour
{
    [SerializeField] private DoorSO doorSO;
    
    [SerializeField] private bool isWeak;
    [SerializeField] private bool isDestructionReady;

    private int maxLifePoints;
    [SerializeField] private int currentLifePoints;

    [SerializeField] private TextMeshProUGUI lifeText;

    [Header("Weakness Attribute Section")]
    private float limitPercentageToSwitchDestructMode = 10;
    private float weaknessDuration = 20;
    private float weakTimer;
    private float weaknessDamageMultiplyer = 2;

    [Header("Regeneration Attribute Section")] 
    [SerializeField] private int regenerationTowersCount;
    [SerializeField] private float regenerationTimer;
    private int hpRegenPerTick;
    private float regenerationCooldown = 7;
    private float multiplyerPerTowers;
    
    
    private void Start()
    {
        if( doorSO != null) SetupSO();
        if (isDestructionReady) GetComponent<Collider>().isTrigger = true;
        currentLifePoints = maxLifePoints;
        regenerationTimer = regenerationCooldown;
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

        if (currentLifePoints == maxLifePoints) return;
        if (regenerationTowersCount > 0)
        {
            RegenerationAttribute();
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
    
    private void RegenerationAttribute()
    {
        regenerationTimer -= Time.deltaTime;
        if (regenerationTimer < 0)
        {
            currentLifePoints += Mathf.FloorToInt(hpRegenPerTick * (regenerationTowersCount * multiplyerPerTowers));
            if (currentLifePoints > maxLifePoints) currentLifePoints = maxLifePoints;
            UpdateCanvas();
            regenerationTimer = regenerationCooldown;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isDestructionReady) return;
        if (!other.CompareTag("Player")) return;
        if (!GameManager.instance.controller.abilitiesManager.dash.isDashing) return;

        DestroyTower();
    }

    public void TakeDamage(int damages)
    {
        if (isWeak) currentLifePoints -= Mathf.FloorToInt(damages * weaknessDamageMultiplyer);
        else currentLifePoints -= damages;

        if (currentLifePoints < 0) currentLifePoints = 0;
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

    public void UpdateCanvas()
    {
        lifeText.text = $"{currentLifePoints}/{maxLifePoints}";
        GameManager.instance.uiManager.SetDoorLife(currentLifePoints, maxLifePoints, isDestructionReady);
    }

    private void DestroyTower()
    {
        LevelManager.Instance.GoNextLevel();
        Destroy(gameObject);
    }

    private void SetupSO()
    {
        currentLifePoints = maxLifePoints = doorSO.maxLifePoints;
        limitPercentageToSwitchDestructMode = doorSO.limitPercentageToSwitchDestructMode;
        weaknessDuration = doorSO.weaknessDuration;
        weaknessDamageMultiplyer = doorSO.weaknessDamageMultiplyer;
        hpRegenPerTick = doorSO.hpRegenPerTick;
        regenerationCooldown = doorSO.regenerationCooldown;
        multiplyerPerTowers = doorSO.rengenerationMultiplierPerTower;
    }
}
