//Author: Olivier Beauséjour

using System;
using System.Collections.Generic;
using Harmony;
using UnityEngine;

namespace Game
{
    public class Key : Collectible
    {
        [Header("Sounds")]
        [SerializeField] private SoundEnum soundToPlay;
        [SerializeField] private AudioSource soundToPlayAudioSource;
        
        private static GameController gameController;
        private AudioManager audioManager;
        
        public bool HasBeenUsed { get; set; }

        protected override void Awake()
        {
            base.Awake();

            gameController = Finder.GameController;
            audioManager = Finder.AudioManager;
            soundToPlayAudioSource.clip = audioManager.GetAudioClip(soundToPlay);
        }
        
        private void Start()
        {
            if (gameController.CurrentPlayerData.KeysUsed.TryGetValue(gameController.CurrentLevelName, out var currentLevelKeysCollected))
            {
                if (currentLevelKeysCollected.Contains(gameObject.Parent().name))
                    gameObject.Parent().SetActive(false);
            }
        }

        protected override void PickUp(PlayerController player)
        {
            FindObjectOfType<AudioManager>().Play(soundToPlay,transform);
            player.PickUpCollectible(this);
            base.PickUp(player);
        }

        public void ResetKey()
        {
            HasBeenUsed = false;
            ResetPosition();
        }
    }
}