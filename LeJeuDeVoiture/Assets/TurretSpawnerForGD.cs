using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EnemyNamespace
{
    public class TurretSpawnerForGD : MonoBehaviour
    {
        [SerializeField] private List<Transform> pos;
        [SerializeField] private CombatState currentState = CombatState.Start;
        [SerializeField] private List<Transform> turretAlive = new();
        [SerializeField] private GameObject turretPrefab;

        private void Update()
        {
            if (turretAlive.Count == 0)
            {
                switch (currentState)
                {
                    case CombatState.Start:
                        SwitchState(CombatState.OneTurret);
                        break;
                    case CombatState.OneTurret:
                        SwitchState(CombatState.TwoTurrets);
                        break;
                    case CombatState.TwoTurrets:
                        SwitchState(CombatState.FourTurrets);
                        break;
                    case CombatState.FourTurrets:
                        SwitchState(CombatState.SixTurrets);
                        break;
                    case CombatState.SixTurrets:
                        SwitchState(CombatState.SixTurrets);
                        break;
                }
            }

            for (int i = 0; i < turretAlive.Count; i++)
            {
                if (turretAlive[i] == null)
                {
                    turretAlive.RemoveAt(i);
                }
            }
        }

        private async void SwitchState(CombatState state)
        {
            switch (state)
            {
                case CombatState.Start:
                    SwitchState(CombatState.OneTurret);
                    break;
                case CombatState.OneTurret:
                    await SpawnTurret(1);
                    break;
                case CombatState.TwoTurrets:
                    await SpawnTurret(2);
                    break;
                case CombatState.FourTurrets:
                    await SpawnTurret(4);
                    break;
                case CombatState.SixTurrets:
                    await SpawnTurret(6);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            currentState = state;
        }

        async Task SpawnTurret(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 randPos = pos[Random.Range(0, pos.Count)].position + new Vector3(0, 0.2f, 0);
                var turret = Instantiate(turretPrefab, randPos, Quaternion.identity);
                turretAlive.Add(turret.transform);
            }

            await Task.Yield();
        }

        private enum CombatState
        {
            Start,
            OneTurret,
            TwoTurrets,
            FourTurrets,
            SixTurrets
        }
    }
}