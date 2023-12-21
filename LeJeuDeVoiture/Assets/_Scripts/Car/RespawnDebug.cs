using CarNameSpace;
using UnityEngine;

namespace DebugSpace {
    public class RespawnDebug : MonoBehaviour {
        [SerializeField] private Transform car = null;
        private PlayerInput inputs = null;

        private void Awake() {
            inputs = new PlayerInput();
            inputs.Enable();
            inputs.Inputs.Respawn.performed += _ => {
                car.transform.position = transform.position;
                car.transform.rotation = transform.rotation;
                car.GetComponent<CarController>().TakeDamage(150);
            };
        }
    }
}
