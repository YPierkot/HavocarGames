using ManagerNameSpace;
using UnityEngine;

public class MakeRespawnEnemiesOnThisLevel : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if(!GameManager.instance.controller.abilitiesManager.isInBulletMode) return;
        
        LevelManager.Instance.ReviveTowers();
        Destroy(gameObject);
    }
}
