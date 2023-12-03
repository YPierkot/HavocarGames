using System.Collections;
using System.Collections.Generic;
using AbilityNameSpace;
using ManagerNameSpace;
using UnityEngine;


namespace ModifierNameSpace
{
    [CreateAssetMenu(fileName = "TestModifier", menuName = "Modifiers/Test", order = 2)]
    public class TestModifier : Modifier
    {
        [Header("MODIFIER PARAMETERS")] 
        [SerializeField] private string message;
        public override void ApplyModifier()
        {
            GameManager.instance.abilitiesManager.AbilityActivated += TestMethod;
        }

        public void TestMethod(AbilitySocket socket)
        {
            //Debug.Log(message);
        }
    }   
}
