// Author: Jonathan Mathieu

using System;
using Harmony;
using UnityEngine;

namespace Game
{
    public class LowGravityController : MonoBehaviour, ITriggerEnter, ITriggerExit
    {
        [SerializeField] [Range(0f, 1f)] private float lowGravityTimeScaleValue = 0.5f;
        
        private GameController gameController;
        private PlayerDeathEventChannel playerDeathEventChannel;

        private void Awake()
        {
            gameController = Finder.GameController;
            playerDeathEventChannel = Finder.PlayerDeathEventChannel;
        }
        
        private void OnEnable()
        {
            playerDeathEventChannel.OnPlayerDeath += OnPlayerDeath;
        }

        private void OnDisable()
        {
            playerDeathEventChannel.OnPlayerDeath -= OnPlayerDeath;
        }

        private void OnPlayerDeath()
        {
            gameController.TimeScale = 1f;
        }

        public void OnTriggerDetected(Collider2D other)
        {
            gameController.TimeScale = lowGravityTimeScaleValue;
        }

        public void OnTriggerExitDetected(Collider2D other)
        {
            gameController.TimeScale = 1f;
        }
    }
}