// Author : Derek Pouliot

using System;
using Harmony;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(Animator))]
    public class BossAnimationActuator : MonoBehaviour
    {
        private const string IDLE_STATE_NAME = "Idle";
        private const string FALLING_STATE_NAME = "Falling";
        
        [Header("Skin Widths Parameters")]
        [SerializeField] private float fallingSkinWidth = 0.9f;
        [SerializeField] private float normalSkinWidth = 0.1f;

        [Header("Animation Speed Parameters")] 
        [SerializeField] private float fallingAnimationSpeed = 4;
        [SerializeField] private float normalAnimationSpeed = 1;
        
        [Header("Berseck Parameters")] 
        [SerializeField] private float berseckScale = 1.4f;

        private Animator bossAnimator;
        private BossController bossController;
        private JumpActuator bossJumpActuator;
        private TurretController bossTurretController;
        private CollisionActuator bossCollisionActuator;

        private BossStateMachine bossStateMachine;
        private float bossInitialScale;

        private float CurrentHorizontalScale => Math.Abs(transform.localScale.x);
        private bool BossTurretIsFlipped => bossController.transform.localScale.x < 0;

        private void Awake()
        {
            bossAnimator = GetComponent<Animator>();
            bossController = GetComponent<BossController>();
            bossJumpActuator = GetComponent<JumpActuator>();
            bossTurretController = GetComponentInChildren<TurretController>();
            bossCollisionActuator = GetComponent<CollisionActuator>();

            bossInitialScale = transform.localScale.x;
        }

        public void LinkToStateMachine(BossStateMachine stateMachine)
        {
            bossStateMachine = stateMachine;
        }

        private void Update()
        {
            UpdateBossAnimationParameters();
            ChangeHorizontalDirection();
            
            bossCollisionActuator.SkinWidth = bossAnimator.GetCurrentAnimatorStateInfo(0).IsName(FALLING_STATE_NAME) ? fallingSkinWidth: normalSkinWidth;
        }

        private void UpdateBossAnimationParameters()
        {
            bossAnimator.SetBool(R.S.AnimatorParameter.IsActivate, bossController.IsActivate);
            bossAnimator.SetBool(R.S.AnimatorParameter.IsDead, bossController.IsDead);
            bossAnimator.SetBool(R.S.AnimatorParameter.IsResting, bossController.IsResting);
            bossAnimator.SetBool(R.S.AnimatorParameter.IsJumping, bossJumpActuator.IsJumping);
            bossAnimator.SetBool(R.S.AnimatorParameter.IsFalling, bossJumpActuator.HasReachedMaxJumpHeight || bossController.IsFalling);
            bossAnimator.SetBool(R.S.AnimatorParameter.IsBerseck, bossStateMachine.CurrentState.GetType() == typeof(BerserkState));
            bossAnimator.SetBool(R.S.AnimatorParameter.IsReloading, bossStateMachine.CurrentState.GetType() == typeof(ReloadingState));
        }

        private void ChangeHorizontalDirection()
        {
            if (bossAnimator.GetCurrentAnimatorStateInfo(0).IsName(IDLE_STATE_NAME))
            {
                if (transform.localScale.x < 0)
                    SetHorizontalScale(CurrentHorizontalScale);
            }
            else if (!bossAnimator.GetCurrentAnimatorStateInfo(0).IsName(FALLING_STATE_NAME))
            {
                if (bossController.IsMovingLeft)
                {
                    SetHorizontalScale(-CurrentHorizontalScale);
                }
                else
                {
                    SetHorizontalScale(CurrentHorizontalScale);
                }
            }

            ResetTurretDirection();
        }

        private void ResetTurretDirection()
        {
            var bossTurretTransform = bossTurretController.transform;
            var bossTurretLocalScale = bossTurretTransform.localScale;
            
            if (BossTurretIsFlipped)
            {
                bossTurretLocalScale.x = -1;
                bossTurretTransform.localScale = bossTurretLocalScale;
            }
            else
            {
                bossTurretLocalScale.x = 1;
                bossTurretTransform.localScale = bossTurretLocalScale;
            }
        }

        private void SetHorizontalScale(float horizontalScale)
        {
            var bossTransform = transform;
            Vector3 bossScale = bossTransform.localScale;
            
            bossScale.x = horizontalScale;
            bossTransform.localScale = bossScale;
        }

        public void ResetAnimationSpeed()
        {
            bossAnimator.speed = normalAnimationSpeed;
        }

        public void IncreaseAnimationSpeed()
        {
            bossAnimator.speed = fallingAnimationSpeed;
        }
        
        public void IncreaseScale()
        {
            SetScale(berseckScale);
        }
        
        public void ResetScale()
        {
            SetScale(bossInitialScale);
        }

        private void SetScale(float newBossScale)
        {
            var bossTransform = transform;
            Vector3 bossScale = bossTransform.localScale;

            bossScale.x = newBossScale;
            bossScale.y = newBossScale;
            
            bossTransform.localScale = bossScale;
        }
    }
}