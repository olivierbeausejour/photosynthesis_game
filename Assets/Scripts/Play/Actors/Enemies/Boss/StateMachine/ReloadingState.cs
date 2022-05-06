// Author : Derek Pouliot

using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Game
{
    public class ReloadingState : BaseState
    {
        private const float TOTAL_SECONDS_BEFORE_ATTACKING = 10;

        private readonly BossController boss;
        private readonly SpriteRenderer armorPartToPullRenderer;
        private Color armorPartToPullColor;

        private bool canAttackAgain;

        public ReloadingState(BossController boss)
        {
            this.boss = boss;
            armorPartToPullRenderer = boss.CurrentArmorPartToPull.GetComponentInChildren<SpriteRenderer>();
            canAttackAgain = false;
        }

        public override void Enter()
        {
            boss.IsReloading = true;
            boss.StartCoroutine(WaitBeforeAttackingAgain());
        }

        public override void Leave()
        {
            if (armorPartToPullRenderer == null) return;
            boss.ResetBossPartOpacity(armorPartToPullRenderer);
            
            boss.StopAllCoroutines();
            boss.HasRespawned = false;
            boss.IsReloading = false;
        }

        public override IState Update()
        {
            if (boss.HasRespawned)
            {
                boss.IsActivate = false;
                return new WaitingState(boss);
            }
            
            if (canAttackAgain)
                return new AttackingState(boss);
            
            if (boss.IsBerserk) 
                return new BerserkState(boss);
            
            boss.FlickBossPart(armorPartToPullRenderer);
            return this;
        }

        private IEnumerator WaitBeforeAttackingAgain()
        {
            yield return new WaitForSeconds(TOTAL_SECONDS_BEFORE_ATTACKING);
            canAttackAgain = true;
        }
    }
}