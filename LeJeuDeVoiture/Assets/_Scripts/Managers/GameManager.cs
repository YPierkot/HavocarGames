using System.Collections.Generic;
using UnityEngine;
using CarNameSpace;
using ModifierNameSpace;

namespace ManagerNameSpace
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public CarController controller;
        public CarAbilitiesManager abilitiesManager;
        public CarHealthManager healthManager;
        public UIManager uiManager;
        public CameraManager cameraManager;
        public ProwessManager prowessManager;

        [SerializeField] private List<Modifier> gameModifiers;
        [SerializeField] private List<Modifier> itemsModifiers;

        public float gameTimer = 0;
        public int score = 0;

        private void Awake()
        {
            instance = this;
            SetupGame();
        }

        private void SetupGame()
        {
            abilitiesManager.Setup();
            for (int i = 0; i < gameModifiers.Count; i++)
            {
                gameModifiers[i].ApplyModifier();
            }
        }

        /// <summary>
        /// Add and Apply an Item Modifier
        /// </summary>
        public void AddItemModifier(Modifier modifier)
        {
            itemsModifiers.Add(modifier);
            modifier.ApplyModifier();
        }
    }
}
