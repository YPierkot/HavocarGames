using UnityEngine;

namespace EnemyNamespace
{
    public class EnemyCollisions : MonoBehaviour
    {
        private Enemy _enemyParent;

        private void Start() => _enemyParent = GetComponent<Enemy>();
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // TODO - A Changer quand il y aura la voiture
                
                _enemyParent.CollideWithPlayer();
                //Debug.Log("CollideWithPlayer");
                /*
                // TODO : C'EST DE LA MERDE, C'EST TEMPORAIRE
                CarController controller = other.GetComponent<CarController>();
                controller.rb.velocity *= controller.speedPercentKeptAtImpact;
                if (controller.rb.velocity.magnitude < controller.damageTakenMaxSpeed)
                {
                    controller.lifePoints -= controller.damagesPerAttacks;
                    controller.fillImage.fillAmount = (float)controller.lifePoints / controller.maxLifePoints;
                    if (controller.lifePoints < 0)
                    {
                        SceneManager.LoadScene("PROTO SCENE");
                    }
                }
                else
                {
                    enemyParent.CollideWithPlayer();
                    GetComponent<Collider>().enabled = false;
                //} */
            }
            
            /*
            if (other.CompareTag("PlayerBonusCollider"))
            {
                agent.EnableRagdoll();
                GetComponent<Collider>().enabled = false;
            }
            */
        }
    }
}