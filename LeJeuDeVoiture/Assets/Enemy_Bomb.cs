using UnityEngine;
using UnityEngine.UI;

public class Enemy_Bomb : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private Image _image;
    private float timer;
    private float castingTime;
    private float damagesToApply;
    private bool isDone;
    
    public void Setup(float castingTime, float damagesToApply)
    {
        this.castingTime = castingTime;
        this.damagesToApply = damagesToApply;
        _image.fillAmount = 0;
    }
    
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > castingTime)
        {
            if (!isDone)
            {
                Collider[] results = new Collider[1];
                var size = Physics.OverlapSphereNonAlloc(transform.position, 2f, results, playerLayerMask);
                if (results[0] )
                {
                    
                }
                Destroy(gameObject);
            }
        }
        else
        {
            _image.fillAmount = timer / castingTime;
            _image.color = Color.Lerp(Color.white, Color.red,timer / castingTime);
        }
    }
}
