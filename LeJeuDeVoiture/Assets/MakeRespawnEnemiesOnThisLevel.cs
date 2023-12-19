using System.Threading.Tasks;
using ManagerNameSpace;
using UnityEngine;

public class MakeRespawnEnemiesOnThisLevel : MonoBehaviour
{
    public float interactionTimer = 5f; // Set your desired timer value
    public Color startColor = Color.green;
    public Color hitColor = Color.red;

    public MeshRenderer rend;
    public bool canInteract = true;

    private void Start()
    { 
        SetColor(startColor);
    }

    private async void InteractWithTimerAsync()
    {
        canInteract = false;
        SetColor(hitColor);

        float currentTimer = interactionTimer;

        while (currentTimer > 0f)
        {
            float lerpValue = Mathf.InverseLerp(0f, interactionTimer, currentTimer);
            Color lerpedColor = Color.Lerp(hitColor, startColor, lerpValue);
            SetColor(lerpedColor);
            await Task.Yield(); 
            Debug.Log(currentTimer);
            currentTimer -= Time.deltaTime;
        }

        canInteract = true;
        SetColor(startColor);

        LevelManager.Instance.ReviveTowers();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!GameManager.instance.controller.abilitiesManager.isInBulletMode) return;

        if (canInteract)
        {
            InteractWithTimerAsync();
        }
    }

    private void SetColor(Color color)
    {
        rend.material.color = color;
    }
}