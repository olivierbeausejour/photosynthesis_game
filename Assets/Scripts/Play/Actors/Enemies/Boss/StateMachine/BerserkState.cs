// Author : Derek Pouliot

using UnityEngine;

namespace Game
{
    public class BerserkState : BaseState
    {
        private readonly BossController boss;
        private SpriteRenderer weakBodyPartRenderer;

        private Vector2 initialVelocity;

        public BerserkState(BossController bossController)
        {
            boss = bossController;
        }

        public override void Enter()
        {
            boss.IsReloading = false;
            boss.DeactivateDashKilling();
            boss.UpdateBodyPartToDestroy();
            boss.ShowBodyPartToDestroy();
            boss.UpdateArmorParts();
            boss.InitJump();
            boss.BossAnimationActuator.IncreaseScale();
            weakBodyPartRenderer = boss.CurrentVulnerableBodyPart.GetComponentInChildren<SpriteRenderer>();
            
            if (boss.IsCritical)
                BecomeReallyAngry();
        }

        private void BecomeReallyAngry()
        {
            var bossSpriteRenderer = boss.GetComponent<SpriteRenderer>();
            bossSpriteRenderer.color = Color.red;
        }

        private void BecomeCalm()
        {
            var bossSpriteRenderer = boss.GetComponent<SpriteRenderer>();
            bossSpriteRenderer.color = boss.InitialColor;
        }

        public override IState Update()
        {
            if (boss.HasRespawned)
            {
                boss.IsActivate = false;
                return new WaitingState(boss);
            }

            if (boss.WeakSpotHasBeenHit)
            {
                if (boss.IsDead)
                {
                    boss.Fall();
                    boss.Kill();
                    return new BaseState();
                }
                
                return new AttackingState(boss);
            }

            if (!boss.IsResting)
                boss.ManageJump();
            
            if (boss.IsFalling)
                boss.Fall();
            
            boss.FlickBossPart(weakBodyPartRenderer);

            return this;
        }

        public override void Leave()
        {
            boss.IsBerserk = false;
            boss.IsFalling = false;
            boss.IsResting = false;
            boss.WeakSpotHasBeenHit = false;
            boss.ActivateDashKilling();
            boss.Fall();
            boss.BossAnimationActuator.ResetScale();
            boss.BossAnimationActuator.ResetAnimationSpeed();
            boss.HasRespawned = false;

            BecomeCalm();
            
            if (weakBodyPartRenderer == null) return;
            boss.ResetBossPartOpacity(weakBodyPartRenderer);
        }
    }
}