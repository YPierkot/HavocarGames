using ManagerNameSpace;
using UnityEngine;

namespace CarNameSpace
{
    public class CarHealthManager : MonoBehaviour , IDamageable
    {
        [SerializeField] private int maxLifePoints = 100;
        private int lifePoints = 100;

        public void TakeDamage(int damages)
        {
            lifePoints -= damages;
            GameManager.instance.uiManager.SetHealthJauge((float) lifePoints / maxLifePoints);
            if(lifePoints <= 0) Kill();
        }
        
        public void Kill()
        {
            lifePoints = 0;
            // TODO : DEFINIR LA FONCTION DE MORT DU JOUEUR
        }
    }   
}
