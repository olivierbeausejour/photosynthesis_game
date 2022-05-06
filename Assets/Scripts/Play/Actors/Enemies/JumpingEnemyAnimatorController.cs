//Author:Anthony Dodier
using System.Collections;
using System.Collections.Generic;
using Harmony;
using UnityEngine;

namespace Game
{
    public class JumpingEnemyAnimatorController : MonoBehaviour
    {
        private Animator animator;
        private JumpingEnemyController jumpingEnemyController;


        private void Awake()
        {
            animator = GetComponent<Animator>();
            jumpingEnemyController = GetComponent<JumpingEnemyController>();
            SetScaleX(transform, -1);
        }

        void Update()
        {
            SetDirection();
            SetDeadAnimation();
        }

        private void SetDeadAnimation()
        {
            animator.SetBool(R.S.AnimatorParameter.isDead, !jumpingEnemyController.IsEnemyActive);
        }

        private void SetDirection()
        {
            if (jumpingEnemyController.currentDirection == Vector2.right)
                SetScaleX(transform, -1);
            else if (jumpingEnemyController.currentDirection == Vector2.left)
                SetScaleX(transform, 1);
        }
        public void SetIsGrounded(bool collisionsGrounded)
        {
            animator.SetBool(R.S.AnimatorParameter.isGrounded, collisionsGrounded);
        }

        protected void SetScaleX(Transform scaleTransform, float x)
        {
            Vector3 scale = scaleTransform.transform.localScale;
            scale.x = x;
            scaleTransform.transform.localScale = scale;
        }
        
        private IEnumerator EndFrameCoroutine()
        {
            yield return new WaitForEndOfFrame();
            animator.SetBool(R.S.AnimatorParameter.isGrounded, false);
        }

        public void WaitForEndFrame()
        {
            StartCoroutine(EndFrameCoroutine());
        }
    }

}