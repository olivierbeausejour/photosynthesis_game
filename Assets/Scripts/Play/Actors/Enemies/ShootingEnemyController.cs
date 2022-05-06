//Authors:
//Anthony Dodier
using System;
using Game;
using Harmony;
using UnityEngine;

namespace Game
{
    public class ShootingEnemyController : BaseEnemyController
    {
        [SerializeField] private SoundEnum shootingSound;
        [SerializeField] private AudioSource shootingSoundAudioSource;

        private TurretController turretController;
        private new void Awake()
        {
            audioManager = Finder.AudioManager;
            
            shootingSoundAudioSource.clip = audioManager.GetAudioClip(shootingSound);
            turretController = GetComponentInChildren<TurretController>();
            base.Awake();
        }

        private new void OnEnable()
        {
            base.OnEnable();
        }

        private new void OnDisable()
        {
            base.OnDisable();
        }

        private new void Update()
        {
            if (isEnemyActive)
            {
                if (!turretController.IsInView)
                {
                    base.Update();
                }
                else
                {
                    velocity = Vector2.zero;
                    base.ManageVerticalMovement();
                    movementActuator.Move(velocity);
                }
            }
            else
            {
                turretController.IsInView = false;
            }
            
        }

        private void FireTurret()
        {
            turretController.Fire();
            shootingSoundAudioSource.Play();
        }
    }
}