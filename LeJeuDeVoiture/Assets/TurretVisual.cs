
using System;
using System.Threading.Tasks;
using ManagerNameSpace;
using UnityEngine;

public class TurretVisual : MonoBehaviour
{
    [SerializeField] private Rigidbody[] debrisParts;
    [SerializeField] private float[] debrisPartsVelocity;
    public Vector3 velocity;
    public Light light;
    public ParticleSystem sparks,canonsparks;
    public float damageValue;
    public Material glassMat;
    public MeshRenderer glassRenderer;
    [SerializeField] private Transform[] shakingParts;
    [SerializeField] private Vector3[] shakingPartsPos;
    public Animation shootAnim;
    public Transform turretBase;

    private void Start()
    {
        glassMat = new Material(glassMat);
        glassRenderer.material = glassMat;
        for (int i = 0; i < shakingParts.Length; i++)
        {
            shakingPartsPos[i] = shakingParts[i].position;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G)) DestroyTurret();
        if (Input.GetKeyDown(KeyCode.S)) Shoot();
        
        glassMat.SetFloat("_CrackAmount",damageValue);

        for (int i = 0; i < shakingParts.Length; i++)
        {
            shakingParts[i].position = shakingPartsPos[i] +
                                       new Vector3(Mathf.Sin(Time.time * 50 * damageValue + i) * 0.1f * damageValue, 0,
                                           Mathf.Sin(Time.time * 70 * damageValue + i) * 0.1f * damageValue);
        }
    }
    

    [ContextMenu("DestroyTurret")]
    public async void DestroyTurret()
    {
        for (int i = 0; i < debrisParts.Length; i++)
        {
            debrisParts[i].isKinematic = false;
            debrisParts[i].velocity = velocity * debrisPartsVelocity[i];
            debrisParts[i].transform.parent = null;
        }
        Pooler.instance.SpawnTemporaryInstance(Key.FX_FluidSplash, new Vector3(transform.position.x, 3.4f, transform.position.z), 
            Quaternion.LookRotation(velocity),15);
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        sparks.Play();
        
        float lightRange = light.range;
        float x = 1;
        while (x > 0)
        {
            x -= Time.deltaTime;
            light.range = x * lightRange;
            await Task.Yield();
        }

        await Task.Delay(2500);
        x = 1;
        while (x > 0.8f)
        {
            x -= Time.deltaTime * 0.1f;
            for (int i = 0; i < debrisParts.Length; i++)
            {
                debrisParts[i].transform.localScale = Vector3.Lerp(Vector3.zero, debrisParts[i].transform.localScale,x);
            }
            await Task.Yield();
        }
    }

    public void SetDamagedValue(float damageValue)
    {
        
    }

    public void Shoot()
    {
        shootAnim.Play();
        canonsparks.Play();
    }
    
    public void LookDirection(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction = new Vector3(direction.x, 0, direction.z).normalized;
        float angle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
        turretBase.localRotation = Quaternion.Euler(-90,0,angle + 90);
    }
}
