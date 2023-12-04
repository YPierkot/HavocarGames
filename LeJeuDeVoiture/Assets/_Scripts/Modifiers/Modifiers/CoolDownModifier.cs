using System.Collections;
using System.Collections.Generic;
using AbilityNameSpace;
using ManagerNameSpace;
using UnityEngine;


namespace ModifierNameSpace
{
    [CreateAssetMenu(fileName = "CoolDownModifier", menuName = "Modifiers/CoolDown", order = 3)]
    public class CoolDownModifier : Modifier
    {
        [Header("MODIFIER PARAMETERS")] 
        [SerializeField] private float coolDownMultiplier;
        [SerializeField] private bool affectAAbility,affectBAbility,affectXAbility,affectYAbility;
        
        public override void ApplyModifier()
        {
            //if(affectAAbility) GameManager.instance.abilitiesManager.aAbility.cooldown *= coolDownMultiplier;
            if(affectBAbility) GameManager.instance.abilitiesManager.bAbility.cooldown *= coolDownMultiplier;
            if(affectXAbility) GameManager.instance.abilitiesManager.xAbility.cooldown *= coolDownMultiplier;
            if(affectYAbility) GameManager.instance.abilitiesManager.yAbility.cooldown *= coolDownMultiplier;
        }
    }   
}
