using ManagerNameSpace;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_Bomb : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private Image _image;
    private float timer;
    private float castingTime;
    private float damagesToApply;
    private float size;
    private bool isDone;
    private Vector3 v0, v1, v2, v3;
    private BulletBill projectileBomb;

    public void Setup(float castingTime, float damagesToApply, float explosionSize, Vector3 launchPos, BulletBill i)
    {
        this.castingTime = castingTime;
        this.damagesToApply = damagesToApply;
        this.size = explosionSize;
        _image.fillAmount = 0;
        RectTransform rt = _image.GetComponent (typeof (RectTransform)) as RectTransform;
        rt.sizeDelta = new Vector2 (explosionSize, explosionSize);
        v0 = launchPos;
        v1 = v0 + new Vector3(0, 2, 0);
        v3 = transform.position;
        v2 = v3 + new Vector3(0, 3, 0);
        projectileBomb = i;
    }
    
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > castingTime)
        {
            if (!isDone)
            {
                isDone = true;
                Collider[] results = new Collider[1];
                var size = Physics.OverlapSphereNonAlloc(transform.position, this.size - 0.5f, results, playerLayerMask);
                if (size > 0) GameManager.instance.controller.SubtractMaxSpeed(damagesToApply);
                Destroy(gameObject);
                Destroy(projectileBomb.gameObject);
            }
        }
        else
        {
            _image.fillAmount = timer / castingTime;
            _image.color = Color.Lerp(Color.white, Color.red,timer / castingTime);
            Vector3 pos = Ex.CubicBeziersCurve(v0, v1, v2, v3, timer / castingTime);
            projectileBomb.transform.position = pos;
        }
    }
}
