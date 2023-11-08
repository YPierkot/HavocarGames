using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image xAbilityFill, yAbilityFill, aAbilityFill, bAbilityFill;
    [SerializeField] private TMP_Text xAbilityName, yAbilityName, aAbilityName, bAbilityName;
    [SerializeField] private Image healthJauge;

    public void SetAbilityCooldown(AbilitySocket socket,float value)
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
    
    public void SetAbilityLabel(AbilitySocket socket,string label)
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

    public void SetHealthJauge(float value)
    {
        healthJauge.fillAmount = value;
    }
}
