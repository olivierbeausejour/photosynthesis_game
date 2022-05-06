// Author : Olivier Beauséjour
// Author : Derek Pouliot

using System.Collections.Generic;
using Harmony;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    [Findable(R.S.Tag.GameController)]
    public class GameController : MonoBehaviour
    {
        [Header("Input settings")] 
        [SerializeField] private bool isUsingController;
        
        [Header("Checkpoint")] 
        [SerializeField] private Checkpoint currentCheckpoint;

        [Header("Game Parameters")]
        [SerializeField] [Range(0f, 50f)] private float timeScale = 1f;

        [Header("Sounds")] 
        [SerializeField] private SoundEnum levelThemeSongSound;
        [SerializeField] private AudioSource levelThemeSongSoundAudioSource;
        
        [Header("UI")]
        [SerializeField] private PopupMessage popupMessage;

        private AudioManager audioManager;
        
        private PlayerData currentPlayerData;
        private List<BaseAchievement> achievements;
        private PlayerController playerController;
        
        private PlayerDeathEventChannel playerDeathEventChannel;
        private AchievementUnlockedEventChannel achievementUnlockedEventChannel;
        
        private static int saveGameId;
        
        public Checkpoint CurrentCheckpoint
        {
            get => currentCheckpoint;
            set => currentCheckpoint = value;
        }

        public float TimeScale
        {
            get => timeScale;
            set => timeScale = value;
        }

        public PlayerData CurrentPlayerData => currentPlayerData;

        public bool IsPlayerAlive => playerController.IsPlayerAlive;
        
        public string CurrentLevelName { get; private set; }

        public int CurrentLevelNbDeaths { get; private set; }

        public static int SaveGameId
        {
            get => saveGameId;
            set => saveGameId = value;
        }

        private void Awake()
        {
            isUsingController = false;
            CurrentLevelName = SceneManager.GetActiveScene().name;
            CurrentLevelNbDeaths = 0;

            playerController = FindObjectOfType<PlayerController>();
            playerDeathEventChannel = Finder.PlayerDeathEventChannel;
            achievementUnlockedEventChannel = Finder.AchievementUnlockedEventChannel;
            audioManager = Finder.AudioManager;
        }

        private void OnEnable()
        {
            levelThemeSongSoundAudioSource.clip = audioManager.GetAudioClip(levelThemeSongSound);
            levelThemeSongSoundAudioSource.Play();
            
            var savedPlayerData = LoadPlayerData();
            currentPlayerData = savedPlayerData ?? new PlayerData();
            
            achievements = new List<BaseAchievement>();
            achievements.AddRange(GetComponentsInChildren<BaseAchievement>());
            
            var savedAchievementData = AchievementSaver.LoadAchievements(SaveGameId);

            if (savedAchievementData != null)
            {
                ParseSerializableAchievementData(savedAchievementData);
            }
            
            playerDeathEventChannel.OnPlayerDeath += OnPlayerDeath;
            achievementUnlockedEventChannel.OnAchievementUnlocked += OnAchievementUnlocked;
        }
        
        private void ParseSerializableAchievementData(SerializableAchievementData savedAchievementData)
        {
            foreach (var serializableAchievement in savedAchievementData.Achievements)
            {
                var baseAchievement = achievements.Find(a => a.Name == serializableAchievement.Name);

                baseAchievement.Progression = serializableAchievement.Progression;
            }
        }

        private void OnDisable()
        {
            //TODO don't forget to add credits scene to this if
            if (SceneManager.GetActiveScene().name != R.S.Scene.MainMenu &&
                SceneManager.GetActiveScene().name != R.S.Scene.LoadingScene)
            {
                SavePlayerData();
                SaveAchievementData();
            }

            playerDeathEventChannel.OnPlayerDeath -= OnPlayerDeath;
            achievementUnlockedEventChannel.OnAchievementUnlocked -= OnAchievementUnlocked;
        }

        private void OnPlayerDeath()
        {
            CurrentLevelNbDeaths++;
        }

        private void OnAchievementUnlocked(string achievementName, string achievementDescription)
        {
            popupMessage.ShowPopup(StringConstants.ACHIEVEMENT_UNLOCKED_MESSAGE + " - " + achievementName + " (" + 
                                   achievementDescription + ")", PopupMessage.PopupSoundType.Success, 
                PopupMessage.FADE_TIME, PopupMessage.NORMAL_SHOWING_TIME, PopupMessage.FADE_TIME);
        }

        private void Update()
        {
            Time.timeScale = timeScale;
        }

        public void SetCheckpoint(Checkpoint checkpoint)
        {
            currentCheckpoint = checkpoint;
        }
        
        private void SavePlayerData()
        {
            if (currentPlayerData != null)
            {
                currentPlayerData.CurrentLevelName = CurrentLevelName;
                currentPlayerData.LastCheckpointEncounteredId = currentCheckpoint.CheckpointId;
                PlayerSaver.SaveGame(currentPlayerData, SaveGameId);
            }
        }
        
        private void SaveAchievementData()
        {
            AchievementSaver
                .SaveAchievements(SerializableAchievementData.MakeSerializableAchievementData(achievements),
                    SaveGameId);

            achievements.Clear();
        }
        
        private PlayerData LoadPlayerData()
        {
            return PlayerSaver.LoadGame(SaveGameId);
        }

        public void ResumeGame()
        {
            playerController.enabled = true;
            timeScale = 1;
        }

        public void PauseGame()
        {
            playerController.enabled = false;
            timeScale = 0;
        }
    }
}