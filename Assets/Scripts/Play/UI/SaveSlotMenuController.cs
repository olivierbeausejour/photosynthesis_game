// Author : Derek Pouliot

using System.Collections;
using System.Linq;
using Harmony;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    public class SaveSlotMenuController : MonoBehaviour
    {
        private const string FULL_ON_ADVENTURER_ACHIEVEMENT_NAME = "Full-On Adventurer";
        
        [SerializeField] private SoundEnum selectSaveSound;
        [SerializeField] private SoundEnum pauseUnpauseSound;
        [SerializeField] private SoundEnum selectItemSound;
        [SerializeField] private AudioSource selectSaveSoundAudioSource;
        [SerializeField] private AudioSource pauseUnpauseSoundAudioSource;
        [SerializeField] private AudioSource selectItemSoundAudioSource;
        
        private AudioManager audioManager;
        private EventSystem eventSystem;
        private Button firstSelectedButton;
        private static Canvas saveSlotMenuCanvas;
        private MainMenuController mainMenuController;
        private MenuInputManager menuInputManager;
        private PauseMenuController pauseMenuController;

        private Button[] saveSlotMenuDeleteButtons;
        private Canvas[] saveSlotsCanvas;

        private float timeToPlaySelectSaveSound = 1.5f;

        private void Awake()
        {
            audioManager = Finder.AudioManager;
            eventSystem = GetComponentInChildren<EventSystem>();
            firstSelectedButton = eventSystem.firstSelectedGameObject.GetComponent<Button>();
            menuInputManager = Finder.MenuInputManager;
            mainMenuController = Finder.MainMenuController;
            pauseMenuController = Finder.PauseMenuController;

            saveSlotMenuCanvas = GetComponent<Canvas>();
            saveSlotMenuCanvas.sortingOrder = 1;
            saveSlotMenuCanvas.enabled = false;

            saveSlotMenuDeleteButtons = GetComponentsInChildren<Button>()
                .Where(btn => btn.name.Contains(R.S.Prefab.DeleteSlotButton)).ToArray();

            saveSlotsCanvas = GetComponentsInChildren<Canvas>()
                .Where(canvas => canvas.name.Contains(R.S.Prefab.SlotCanvas)).ToArray();
            
            selectSaveSoundAudioSource.clip = audioManager.GetAudioClip(selectSaveSound);
            pauseUnpauseSoundAudioSource.clip = audioManager.GetAudioClip(pauseUnpauseSound);
            selectItemSoundAudioSource.clip = audioManager.GetAudioClip(selectItemSound);
        }

        private void Start()
        {
            Reload();
        }
        
        private void Update()
        {
            if(menuInputManager.ReturnKey) 
                ReturnToMainMenu();
        }

        private void Reload()
        {
            ManageDeleteSlotButtonsInteractivity();
            UpdateSlotsInformation();
            
            firstSelectedButton.Select();
        }

        private void UpdateSlotsInformation()
        {
            for (int i = 0; i < saveSlotsCanvas.Length; i++)
            {
                int slotId = i + 1;
                Text[] slotTextElements = saveSlotsCanvas[i].GetComponentsInChildren<Text>()
                    .Where(canvas => !canvas.Parent().name.Contains(typeof(Button).Name)).ToArray();

                PlayerData savedPlayerData = null;
                
                if (PlayerSaver.CheckIfFileExists(slotId))
                    savedPlayerData = PlayerSaver.LoadGame(slotId);

                UpdateSlotText(slotId, slotTextElements, savedPlayerData);
            }
        }

        private void UpdateSlotText(int slotId, Text[] slotTextElements, PlayerData savedPlayerData)
        {
            Text slotTitle = slotTextElements.First(text => text.name.Contains(R.S.GameObject.SlotTitle));
            string levelName = savedPlayerData != null 
                ? savedPlayerData.CurrentLevelName.ToSentence() : StringConstants.EMPTY_TEXT;
            slotTitle.text = StringConstants.DEFAULT_SLOT_TITLE + slotId + StringConstants.HYPHEN_TEXT + levelName;
            
            Text playerProgressionText = slotTextElements.First(text => text.name.Contains(R.S.GameObject.ProgressionText));
            float levelProgression = savedPlayerData != null ? CalculateGameProgression(savedPlayerData.CurrentLevelName, slotId) : CalculateGameProgression(null, slotId);
            playerProgressionText.text = StringConstants.DEFAULT_PROGRESSION_TEXT + 
                                         StringConstants.COLON_STRING + 
                                         levelProgression + 
                                         StringConstants.PERCENTAGE_TEXT;
            
            Text filmRollsText = slotTextElements.First(text => text.name.Contains(R.S.GameObject.FilmRollPickedInfo));
            int filmRollsPicked = savedPlayerData != null ? savedPlayerData.TotalFilmStocksPicked : 0;
            filmRollsText.text = filmRollsPicked + StringConstants.SLASH_TEXT + GameConstants.TOTAL_FILM_ROLLS + StringConstants.FILM_ROLLS_TEXT;
        }

        private float CalculateGameProgression(string levelName, int slotId)
        {
            SerializableAchievementData savedAchievements = AchievementSaver.LoadAchievements(slotId);

            if (savedAchievements != null)
            {
                if (savedAchievements.Achievements.First(a => a.Name == FULL_ON_ADVENTURER_ACHIEVEMENT_NAME).Progression > 0)
                    return 100;
            }
            
            switch (levelName)
            {
                case R.S.Scene.Level1:
                    return (0 / (float) GameConstants.NB_LEVELS) * 100;
                case R.S.Scene.Level2:
                    return (1 / (float) GameConstants.NB_LEVELS) * 100;
                case R.S.Scene.Level3:
                    return (2 / (float) GameConstants.NB_LEVELS) * 100;
                case R.S.Scene.Level4:
                    return (3 / (float) GameConstants.NB_LEVELS) * 100;
                case R.S.Scene.Level5:
                    return (4 / (float) GameConstants.NB_LEVELS) * 100;
                default:
                    return 0;
            }
        }

        private void ManageDeleteSlotButtonsInteractivity()
        {
            for (int i = 0; i < saveSlotMenuDeleteButtons.Length; i++)
            {
                saveSlotMenuDeleteButtons[i].interactable = PlayerSaver.CheckIfFileExists(i + 1);
            }
        }

        [UsedImplicitly]
        public void ActivateSaveSlotMenu()
        {
            firstSelectedButton.Select();
            
            saveSlotMenuCanvas.enabled = true;
            saveSlotMenuCanvas.sortingOrder = 1;
        }

        [UsedImplicitly]
        public void StartSlotGame(int slotId)
        {
            selectItemSoundAudioSource.Play();
            GameController.SaveGameId = slotId;
            
            if (PlayerSaver.CheckIfFileExists(slotId))
            {
                PlayerData savedPlayedData = PlayerSaver.LoadGame(slotId);
                Loader.Load(savedPlayedData.CurrentLevelName);
            }
            else
            {
                Loader.Load(R.S.Scene.Level1);
            }
        }

        [UsedImplicitly]
        public void DeleteSlotGame(int slotId)
        {
            selectItemSoundAudioSource.Play();
            GameController.SaveGameId = slotId;
            PlayerSaver.DeleteGameSlot(slotId);
            AchievementSaver.DeleteAchievementData(slotId);

            Reload();
        }

        [UsedImplicitly]
        public void ReturnToMainMenu()
        {
            selectItemSoundAudioSource.Play();
            saveSlotMenuCanvas.enabled = false;
            saveSlotMenuCanvas.sortingOrder = 0;
            
            mainMenuController.ShowMainMenu();
        }
    }
}