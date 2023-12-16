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
    [SerializeField] private int damageToDoOnDieToParentEnemy = 20;
    
    private void Start()
    {
        Spawn();
        lifeText.text = $"{currentHealthPoints} / {maxHealthPoints}";
    }

    public override void Death()
    {
        parentEnemy.OnSentinelDie(damageToDoOnDieToParentEnemy);
        Destroy(gameObject);
        GameManager.instance.prowessManager.TriggerProwessEvent(0.1f,"Sentinel Destroyed !",5); // Speed bonus to player
    }

    public override void CollideWithPlayer()
    {
        if(!car.abilitiesManager.isInBulletMode) return;
        Death();
        //currentHealthPoints -= (int)Math.Floor(car.speed) * car.abilitiesManager.bulletModeSources.Count;
        //lifeText.text = $"{currentHealthPoints} / {maxHealthPoints}";
        //if (currentHealthPoints < 1 ) Death();
    }
}