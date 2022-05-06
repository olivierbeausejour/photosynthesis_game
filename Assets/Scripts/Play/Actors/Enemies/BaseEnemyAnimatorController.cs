//Author:Anthony Dodier
using System.Collections;
using System.Collections.Generic;
using Harmony;
using UnityEngine;

namespace Game
{
    public class BaseEnemyAnimatorController : MonoBehaviour
    {
        private Animator animator;
        private BaseEnemyController baseEnemyController;


        private void Awake()
        {
            animator = GetComponent<Animator>();
            baseEnemyController = GetComponent<BaseEnemyController>();
            SetScaleX(transform, -1);
        }

        void Update()
        {
            SetDirection();
            SetDeadAnimation();
        }
        private void SetDeadAnimation()
        {
            animator.SetBool(R.S.AnimatorParameter.isDead, !baseEnemyController.IsEnemyActive);
        }

        private void SetDirection()
        {
            if (baseEnemyController.currentDirection == Vector2.right)
                SetScaleX(transform, -1);
            else if (baseEnemyController.currentDirection == Vector2.left)
                SetScaleX(transform, 1);
        }

        protected void SetScaleX(Transform scaleTransform, float x)
        {
            Vector3 scale = scaleTransform.transform.localScale;
            scale.x = x;
            scaleTransform.transform.localScale = scale;
        }
    }

}

