//Author: Olivier Beauséjour

using System;
using System.Collections.Generic;
using Harmony;
using UnityEngine;

namespace Game
{
    public class FilmRoll : Collectible
    {
        [Header("Sounds")]
        [SerializeField] private AudioSource pickUpSoundAudioSource;
        [SerializeField] private SoundEnum pickUpSound;
        
        private static GameController gameController;
        private AudioManager audioManager;

        private int filmRollId;
        private static int currentFilmRollId;
        public int FilmRollId => filmRollId;

        protected new void Awake()
        {
            gameController = Finder.GameController;
            audioManager = Finder.AudioManager;
            filmRollId = currentFilmRollId++;

            pickUpSoundAudioSource.clip = audioManager.GetAudioClip(pickUpSound);
            
            base.Awake();
        }

        private void Start()
        {
            if (gameController.CurrentPlayerData.FilmRollsCollected.TryGetValue(gameController.CurrentLevelName, out var currentLevelFilmRollsIdCollected))
            {
                if (currentLevelFilmRollsIdCollected.Contains(gameObject.Parent().name))
                    gameObject.Parent().SetActive(false);
            }
        }

        protected override void PickUp(PlayerController player)
        {
            gameController.CurrentPlayerData.TotalFilmStocksPicked++;
            
            if (!gameController.CurrentPlayerData.FilmRollsCollected.ContainsKey(gameController.CurrentLevelName))
                gameController.CurrentPlayerData.FilmRollsCollected.Add(gameController.CurrentLevelName, new List<string>());

            gameController.CurrentPlayerData.FilmRollsCollected[gameController.CurrentLevelName].Add(gameObject.Parent().name);
            
            audioManager.Play(pickUpSound,transform);
            base.PickUp(player);
        }
    }
}