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
        [SerializeField] private Image xAbilityFill, yAbilityFill, aAbilityFill, bAbilityFill;
        [SerializeField] private TMP_Text xAbilityName, yAbilityName, aAbilityName, bAbilityName;
        [SerializeField] private Image healthJauge;

        
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
        /// Set the Fill Amount of the Player's Health Jauge in the UI
        /// </summary>
        public void SetHealthJauge(float value)
        {
            healthJauge.fillAmount = value;
        }
    }
}
