using System;
using System.Threading.Tasks;
using CarNameSpace;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BulletBill : MonoBehaviour
{
    public GameObject exploPS;
    // [SerializeField] private float bulletSpeed;
    // [SerializeField] private float rotateSpeed;
    // [SerializeField] private Rigidbody rb;
    // [SerializeField] private CarController player;
    //
    // public void Setup(CarController player)
    // {
    //     this.player = player;
    // }
    //
    // private void Update()
    // {
    //     //transform.LookAt(player.transform.position);
    //     //var dir = (player.transform.position - transform.position).normalized;
    //     //rb.velocity = dir * (bulletSpeed * Time.deltaTime);
    //     
    //     transform.LookAt(Vector3.Lerp(transform.forward, player.transform.position, rotateSpeed * 5 * Time.deltaTime));
    //     rb.AddForce(transform.forward * (Time.deltaTime * bulletSpeed));
    // }
    //
    // private async void OnCollisionEnter(Collision other)
    // {
    //     if (other.gameObject.CompareTag("Wall"))
    //     {
    //         Destroy(gameObject);
    //     }
    //
    //     if (other.gameObject.CompareTag("Player"))
    //     {
    //         if (other.gameObject.GetComponent<CarController>().enabled)
    //         {
    //             Destroy(gameObject);
    //             if(player.abilitiesManager.isShielded) return;
    //             other.gameObject.GetComponent<CarController>().enabled = false;
    //             await Task.Delay(4000);
    //             SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    //         }
    //     }
    // }

    private void OnDestroy()
    {
        Instantiate(exploPS, transform.position, Quaternion.identity);
    }
}