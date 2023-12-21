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
    [SerializeField] private ParticleSystem puddleFx;
    
    private void Start()
    {
        Spawn();
        UpdateCanvas();
    }

    protected override void OnDie()
    {
        base.OnDie();
        Destroy(gameObject);
        Pooler.instance.SpawnTemporaryInstance(Key.FX_Puddle, new Vector3(transform.position.x, 0.4f, transform.position.z), Quaternion.identity,10);
    }

    public override void Death()
    {
        parentEnemy.OnSentinelDie(damageToDoOnDieToParentEnemy);
        OnDie();
    }

    // public override void CollideWithPlayer()
    // {
    //     if(!car.abilitiesManager.isInBulletMode) return;
    //     Kill();
    //     //Death();
    //     //currentHealthPoints -= (int)Math.Floor(car.speed) * car.abilitiesManager.bulletModeSources.Count;
    //     //lifeText.text = $"{currentHealthPoints} / {maxHealthPoints}";
    //     //if (currentHealthPoints < 1 ) Death();
    // }

    public void TakeDamage(int damages) => Kill();
    public void Kill() => Death();
}