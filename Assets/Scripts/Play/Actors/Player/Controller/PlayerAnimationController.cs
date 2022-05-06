//Authors:
//Jonathan Mathieu

using System;
using System.Collections;
using Harmony;
using UnityEngine;

namespace Game
{
    public class PlayerAnimationController : MonoBehaviour
    {
        private const float DIRECTION_SWITCH_TOLERANCE = 2f;
        
        private Animator animator;
        private PlayerController playerController;
        private MovementActuator movementActuator;
        private BaseActuator baseActuator;
        private CollisionActuator collisionActuator;
        private DashActuator dashActuator;
        private SwayActuator swayActuator;

        private bool goingLeft;
        
        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            playerController = GetComponentInParent<PlayerController>();
            movementActuator = GetComponentInParent<MovementActuator>();
            baseActuator = GetComponentInParent<BaseActuator>();
            collisionActuator = GetComponentInParent<CollisionActuator>();
            dashActuator = GetComponentInParent<DashActuator>();
            swayActuator = GetComponentInParent<SwayActuator>();
        }

        private void Update()
        {
            SetDirection();

            SetAnimatorValues();
        }

        private void SetDirection()
        {
            if (!playerController.IsSwaying)
            {
                if (movementActuator.CurrentVelocity.x > 0f)
                    transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                else if (movementActuator.CurrentVelocity.x < -0f)
                    transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
            else
            {
                if (!swayActuator.IsLocked)
                {
                    if (movementActuator.CurrentVelocity.x > DIRECTION_SWITCH_TOLERANCE)
                        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    else if (movementActuator.CurrentVelocity.x < -DIRECTION_SWITCH_TOLERANCE)
                        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                }
                else
                {
                    if (swayActuator.SwayDirection > 0f)
                        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    else if (swayActuator.SwayDirection < 0f)
                        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                }
            }
        }

        private void SetAnimatorValues()
        {
            animator.SetBool(R.S.AnimatorParameter.hasJumped, baseActuator.HasJumped);
            animator.SetBool(R.S.AnimatorParameter.grounded, collisionActuator.Collisions.grounded);
            animator.SetBool(R.S.AnimatorParameter.isDashing, playerController.IsDashing());
            animator.SetBool(R.S.AnimatorParameter.isSwaying, playerController.IsSwaying);
            animator.SetBool(R.S.AnimatorParameter.isPlayerAlive, playerController.IsPlayerAlive);
            animator.SetFloat(R.S.AnimatorParameter.velocityX, Mathf.Abs(movementActuator.CurrentVelocity.x));

            if (playerController.IsDashing())
            {
                animator.SetFloat(R.S.AnimatorParameter.velocityY, dashActuator.Direction.y);
            }
        }


        /*private const string IS_LEVEL_FINISHED_ANIMATION = "isLevelFinished";
        private const string IS_LEVEL_FINISHED_ANIMATION_DONE = "EndLevelAnimationDone";
        private Animator animator;
        private LevelCompleteController levelCompleteController;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            levelCompleteController = Finder.LevelCompleteController;
        }

        public void SetPlayerDirection(float direction, GameObject eyes)
        {
            if (direction > 0) SetScaleX(eyes.transform, 1);
            if(direction < 0) SetScaleX(eyes.transform, -1);
        }

        private void SetScaleX(Transform scaleTransform, float x)
        {
            Vector3 scale = scaleTransform.transform.localScale;
            scale.x = x;
            scaleTransform.transform.localScale = scale;
        }
        
        public void PlayEndAnimation()
        {
            animator.SetBool(IS_LEVEL_FINISHED_ANIMATION ,true);
            StartCoroutine(WaitForAnimationToFinish());
        }

        private IEnumerator WaitForAnimationToFinish()
        {
            
            while (isActiveAndEnabled)
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName(IS_LEVEL_FINISHED_ANIMATION_DONE))
                {
                    levelCompleteController.LoadNextLevel();
                    yield break;
                //}
                //yield return null;
            //}
        }*/
    }
}