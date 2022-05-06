// Author : Derek Pouliot

using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Game
{
    public class AttackingState : BaseState
    {
        private const float TOTAL_SECONDS_BEFORE_RELOADING = 5;
            
        private readonly BossController boss;

        public AttackingState(BossController bossController)
        {
            boss = bossController;
            boss.IsReloading = false;
        }

        public override void Enter()
        {
            boss.EnableShooting();
            boss.StartCoroutine(WaitBeforeReloading());
        }

        public override IState Update()
        {
            if (boss.HasRespawned)
            {
                boss.IsActivate = false;
                return new WaitingState(boss);
            }
            
            if (boss.IsReloading)
                return new ReloadingState(boss);

            boss.MoveTowardsPlayer(false);

            return this;
        }

        public override void Leave()
        {
            boss.StopAllCoroutines();
            boss.DisableShooting();
            boss.IsReloading = true;
        }

        private IEnumerator WaitBeforeReloading()
        {
            yield return new WaitForSeconds(TOTAL_SECONDS_BEFORE_RELOADING);
            boss.IsReloading = true;
        }
    }
}