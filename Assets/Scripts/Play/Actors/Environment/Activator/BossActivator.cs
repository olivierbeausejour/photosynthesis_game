// Author : Derek Pouliot

using System;
using Harmony;
using UnityEngine;

namespace Game
{
    public class BossActivator: MonoBehaviour, ITriggerEnter
    {
        private BossHasBeenActivatedEventChannel bossHasBeenActivatedEventChannel;
        private PlayerDeathEventChannel playerDeathEventChannel;

        private void Awake()
        {
            bossHasBeenActivatedEventChannel = Finder.BossHasBeenActivatedEventChannel;
            playerDeathEventChannel = Finder.PlayerDeathEventChannel;
        }

        private void OnEnable()
        {
            playerDeathEventChannel.OnPlayerDeath += BecomeActive;
        }

        private void BecomeActive()
        {
            gameObject.Parent().SetActive(true);
        }

        public void OnTriggerDetected(Collider2D other)
        {
            bossHasBeenActivatedEventChannel.NotifyBossHasBeenActivated();
            gameObject.Parent().SetActive(false);
        }
    }
}