using UnityEngine;

namespace AbilityNameSpace
{
    public class TestAbility : Ability
    {
        [Header("ABILITY PARAMETERS")] [SerializeField]
        private string message;

        public override void StartAbility()
        {
            base.StartAbility();
            Debug.Log(message);
        }
    }
}
