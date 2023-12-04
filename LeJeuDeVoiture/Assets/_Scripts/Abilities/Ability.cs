using ManagerNameSpace;
using UnityEngine;

namespace AbilityNameSpace
{
    public abstract class Ability : MonoBehaviour
    {
        [Header("MAIN PARAMETERS")] public float cooldown = 10;
        public string abilityName;
        [TextArea] public string description;
        public AbilitySocket socket;
        [HideInInspector] public float cooldownTimer = 0;
        [HideInInspector] public bool activable = true;
        public int energyCost;

        /// <summary>
        /// Used Once At the Start of the start of the game, Setup the ability objects and essentials
        /// </summary>
        public virtual void SetupAbility(AbilitySocket currentSocket)
        {
            activable = true;
            cooldownTimer = 0;
            socket = currentSocket;
            GameManager.instance.uiManager.SetAbilityLabel(socket, abilityName);
        }

        /// <summary>
        /// Apply the effect of the ability and start CoolDown
        /// </summary>
        public virtual void StartAbility()
        {
            activable = false;
            cooldownTimer = cooldown;
            
        }

        public virtual void UpdateAbility()
        {
            CoolDown();
        }

        private void CoolDown()
        {
            if (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
                GameManager.instance.uiManager.SetAbilityCooldown(socket, 1 - cooldownTimer / cooldown);
            }
            else
            {
                activable = true;
                GameManager.instance.uiManager.SetAbilityCooldown(socket, 1);
            }
        }
    }

    public enum AbilitySocket
    {
        ABILITY_X,
        ABILITY_Y,
        ABILITY_A,
        ABILITY_B
    }
}
