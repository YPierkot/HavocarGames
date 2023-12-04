using System.Threading.Tasks;
using CarNameSpace;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BulletBill : MonoBehaviour
{
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CarController player;

    public void Setup(CarController player)
    {
        this.player = player;
    }

    private void Update()
    {
        transform.LookAt(player.transform.position);
        var dir = (player.transform.position - transform.position).normalized;
        rb.velocity = dir * (bulletSpeed * Time.deltaTime);
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