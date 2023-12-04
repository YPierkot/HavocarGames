using System.Threading.Tasks;
using CarNameSpace;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BulletBill : MonoBehaviour
{
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CarController player;
    [SerializeField] private float speed = 0.5f;
    private Vector3 currentPosLook = Vector3.zero;
    
    public void Setup(CarController player)
    {
        this.player = player;
    }

    private void Update()
    {
        var dir = (player.transform.position - transform.position).normalized;
        rb.velocity = Vector3.Lerp(rb.velocity, dir * bulletSpeed, speed * Time.deltaTime);
        transform.LookAt(rb.velocity);
    }

    private async void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }

        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<CarController>().enabled)
            {
                Destroy(gameObject);
                if(player.abilitiesManager.isShielded) return;
                other.gameObject.GetComponent<CarController>().enabled = false;
                await Task.Delay(4000);
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}