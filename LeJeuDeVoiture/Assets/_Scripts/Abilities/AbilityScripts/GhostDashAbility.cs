using ManagerNameSpace;
using UnityEngine;

namespace AbilityNameSpace
{
    public class GhostDashAbility : Ability
    {
        [Header("Ghost Dash Parameters")] public float dashThroughTime;

        public override void StartAbility()
        {
            base.StartAbility();
            GameManager.instance.controller.abilitiesManager.dash.SetDashThroughWallsTimer(dashThroughTime);
        }

    }
}