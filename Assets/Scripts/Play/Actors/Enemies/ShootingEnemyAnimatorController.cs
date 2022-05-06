//Author:Anthony Dodier
using System;
using System.Collections;
using System.Collections.Generic;
using Harmony;
using UnityEngine;

namespace Game
{
    public class ShootingEnemyAnimatorController : MonoBehaviour
    {
        private Animator animator;
        private ShootingEnemyController shootingEnemyController;
        private TurretController turretController;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            shootingEnemyController = GetComponent<ShootingEnemyController>();
            turretController = GetComponentInChildren<TurretController>();
        }
        private void Update()
        {
            SetDirection();
            SetInViewAnimation();
            SetShootingAnimation();
            SetDeadAnimation();
        }

        private void SetDeadAnimation()
        {
            animator.SetBool(R.S.AnimatorParameter.isDead, !shootingEnemyController.IsEnemyActive);
        }

        private void SetDirection()
        {
            if (!turretController.IsInView)
            {
                if(shootingEnemyController.currentDirection == Vector2.right)
                    SetScaleX(transform,-1);
                else if(shootingEnemyController.currentDirection == Vector2.left)
                    SetScaleX(transform,1);
            }
            else
            {
                if(turretController.ShootRight)
                    SetScaleX(transform,-1);
                else if(!turretController.ShootRight)
                    SetScaleX(transform,1);
            }
           
            
        }
        
        protected void SetScaleX(Transform scaleTransform, float x)
        {
            Vector3 scale = scaleTransform.transform.localScale;
            scale.x = x;
            scaleTransform.transform.localScale = scale;
        }
        
        private void SetShootingAnimation()
        {
            animator.SetBool(R.S.AnimatorParameter.isShooting,turretController.IsShooting);
        }

        private void SetInViewAnimation()
        {
            animator.SetBool(R.S.AnimatorParameter.isInView,turretController.IsInView);
        }
    }

}

