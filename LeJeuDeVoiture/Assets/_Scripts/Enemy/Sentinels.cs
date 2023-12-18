using EnemyNamespace;
using ManagerNameSpace;
using TMPro;
using UnityEngine;

public class Sentinels : Enemy, IDamageable
{
    [Space]
    [Header("Sentinel")]
    public Enemy parentEnemy;
    public TextMeshProUGUI lifeText;
    [SerializeField] private int damageToDoOnDieToParentEnemy = 20;
    
    private void Start()
    {
        Spawn();
        UpdateCanvas();
    }

    public override void Death()
    {
        parentEnemy.OnSentinelDie(damageToDoOnDieToParentEnemy);
        GameManager.instance.prowessManager.TriggerProwessEvent(0.1f,"Sentinel Destroyed !",5); // Speed bonus to player
        Destroy(gameObject);
    }

    public override void CollideWithPlayer()
    {
        if(!car.abilitiesManager.isInBulletMode) return;
        Kill();
        //Death();
        //currentHealthPoints -= (int)Math.Floor(car.speed) * car.abilitiesManager.bulletModeSources.Count;
        //lifeText.text = $"{currentHealthPoints} / {maxHealthPoints}";
        //if (currentHealthPoints < 1 ) Death();
    }

    public void TakeDamage(int damages) => Kill();
    public void Kill() => Death();
}