using CarNameSpace;
using UnityEngine;

public class EnvironmentInteraction : MonoBehaviour
{
    public virtual void Interact(CarController player)
    {
        // ...
    }
    
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CarController player = other.GetComponent<CarController>();
            if (player != null)
            {
                Interact(player);
            }
        }
    }
}