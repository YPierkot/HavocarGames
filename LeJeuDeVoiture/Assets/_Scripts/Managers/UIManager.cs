using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AbilityNameSpace;

namespace ManagerNameSpace
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Image xAbilityDuration, yAbilityDuration, aAbilityDuration, bAbilityDuration;
        [SerializeField] private Image xAbilityFill, yAbilityFill, aAbilityFill, bAbilityFill;
        [SerializeField] private TMP_Text xAbilityName, yAbilityName, aAbilityName, bAbilityName;
        [SerializeField] private Image rageJaugeFill;
        [SerializeField] private Image[] energySegments;
        [SerializeField] private Color rageJaugeMainColor, rageJaugeUsedColor;
        
        /// <summary>
        /// Set the Fill Amount on an Ability Socket
        /// </summary>
        public void SetAbilityCooldown(AbilitySocket socket, float value)
        {
            switch (socket)
            {
                case AbilitySocket.ABILITY_A:
                    aAbilityFill.fillAmount = value;
                    break;
                case AbilitySocket.ABILITY_B:
                    bAbilityFill.fillAmount = value;
                    break;
                case AbilitySocket.ABILITY_X:
                    xAbilityFill.fillAmount = value;
                    break;
                case AbilitySocket.ABILITY_Y:
                    yAbilityFill.fillAmount = value;
                    break;
            }
        } 
        
        
        /// <summary>
        /// Set the Fill Amount on the Ability Duration Socket
        /// </summary>
        public void SetAbilityDuration(AbilitySocket socket, float value)
        {
            switch (socket)
            {
                case AbilitySocket.ABILITY_A:
                    aAbilityDuration.fillAmount = value;
                    break;
                case AbilitySocket.ABILITY_B:
                    bAbilityDuration.fillAmount = value;
                    break;
                case AbilitySocket.ABILITY_X:
                    xAbilityDuration.fillAmount = value;
                    break;
                case AbilitySocket.ABILITY_Y:
                    yAbilityDuration.fillAmount = value;
                    break;
            }
        }

        /// <summary>
        /// Set the label ( name ) of an Ability Socket in the UI
        /// </summary>
        public void SetAbilityLabel(AbilitySocket socket, string label)
        {
            switch (socket)
            {
                case AbilitySocket.ABILITY_A:
                    aAbilityName.text = label;
                    break;
                case AbilitySocket.ABILITY_B:
                    bAbilityName.text = label;
                    break;
                case AbilitySocket.ABILITY_X:
                    xAbilityName.text = label;
                    break;
                case AbilitySocket.ABILITY_Y:
                    yAbilityName.text = label;
                    break;
            }
        }


        /// <summary>
        /// Set the Fill Amount of the Player's Energy Jauge in the UI
        /// </summary>
        public void SetEnergyJauge(float value)
        {
            for (int i = 0; i < energySegments.Length; i++)
            {
                energySegments[i].fillAmount = Mathf.Clamp01(value - i);
            }
        }
        
        /// <summary>
        /// Set the Fill Amount of the Player's Rage Jauge in the UI
        /// </summary>
        public void SetRageJauge(float value,bool used)
        {
            rageJaugeFill.fillAmount = Mathf.Lerp(rageJaugeFill.fillAmount,Mathf.Clamp01(value),Time.deltaTime*5);
            if(used) rageJaugeFill.color = Color.Lerp(rageJaugeFill.color,rageJaugeUsedColor,Time.deltaTime*5);
            else rageJaugeFill.color = Color.Lerp(rageJaugeFill.color,rageJaugeMainColor,Time.deltaTime*5);
        }
    }
}
