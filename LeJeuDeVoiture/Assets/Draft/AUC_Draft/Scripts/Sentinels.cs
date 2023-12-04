using System;
using EnemyNamespace;
using ManagerNameSpace;
using TMPro;
using UnityEngine;

public class Sentinels : Enemy
{
    [Space]
    [Header("Sentinel")]
    public Enemy parentEnemy;
    public TextMeshProUGUI lifeText;
    
    private void Start()
    {
        Spawn();
        lifeText.text = $"{currentHealthPoints} / {maxHealthPoints}";
    }

    public override void Death()
    {
        parentEnemy.OnSentinelDie(maxHealthPoints);
        Debug.Log(maxHealthPoints);
        Destroy(gameObject);
        GameManager.instance.prowessManager.TriggerProwessEvent(0.1f,"Sentinel Destroyed !",5);
    }

    public override void CollideWithPlayer()
    {
        if(!car.abilitiesManager.isInBulletMode) return;
        currentHealthPoints -= (int)Math.Floor(car.speed);
        lifeText.text = $"{currentHealthPoints} / {maxHealthPoints}";
        if (currentHealthPoints < 1 ) Death();
    }
}