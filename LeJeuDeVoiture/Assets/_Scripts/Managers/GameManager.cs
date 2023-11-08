using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Car;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public CarController controller;
    public CarAbilitiesManager abilitiesManager;
    public CarHealthManager healthManager;
    public UIManager uiManager;
    public CameraManager cameraManager;

    public float gameTimer = 0;
    public int score = 0;

    private void Awake()
    {
        instance = this;
    }

    public void SetupGame()
    {
        abilitiesManager.Setup();
    }
}
