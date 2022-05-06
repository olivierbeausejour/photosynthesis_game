using System;
using Harmony;
using UnityEngine;

namespace Game
{
    public class AnimationTurretController : MonoBehaviour
    {
        private Animator animator;
        private TurretController turretController;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            turretController = GetComponent<TurretController>();
        }

        private void Update()
        {
            animator.SetBool(R.S.AnimatorParameter.isShooting, turretController.IsShooting);
        }
    }
}