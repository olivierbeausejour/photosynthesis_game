//Authors:
//Anthony Dodier

using System;
using Harmony;
using UnityEngine;

namespace Game
{
    [Findable(R.S.Tag.LevelCompleteController)]
    public class LevelCompleteController : MonoBehaviour, ITriggerEnter
    {
        [SerializeField] private String nextSceneName;

        private GameController gameController;
        private LevelCompleteEventChannel levelCompleteEventChannel;
        private GameCompleteEventChannel gameCompleteEventChannel;

        private bool hasBeenTriggered;
        private BoxCollider2D collider;

        private void Awake()
        {
            levelCompleteEventChannel = Finder.LevelCompleteEventChannel;
            gameCompleteEventChannel = Finder.GameCompleteEventChannel;
            
            gameController = Finder.GameController;
            collider = GetComponent<BoxCollider2D>();
        }

        public void OnTriggerDetected(Collider2D other)
        {
            hasBeenTriggered = true;
            gameController.CurrentCheckpoint.CheckpointId = 0;

            levelCompleteEventChannel.NotifyLevelComplete(gameController.CurrentLevelName,
                gameController.CurrentLevelNbDeaths);
            if (gameController.CurrentLevelName == R.S.Scene.Level5)
                gameCompleteEventChannel.NotifyGameComplete(gameController.CurrentPlayerData.NbTotalDeaths);

            LoadNextLevel();
        }

        public void LoadNextLevel()
        {
            if (hasBeenTriggered)
            {
                Loader.Load(nextSceneName);
            }
        }
    }
}