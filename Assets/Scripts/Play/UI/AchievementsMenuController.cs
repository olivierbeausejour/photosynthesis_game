// Author: Olivier Beauséjour

using System.Collections.Generic;
using Harmony;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    public class AchievementsMenuController : MonoBehaviour
    {
        [Header("Base UI values")] 
        [SerializeField] private int minSaveSlotId = 1;
        [SerializeField] private int maxSaveSlotId = 3;
        [SerializeField] private int defaultSaveSlotId = 1;
        
        [Header("Save slot ID")] 
        [SerializeField] private Text saveSlotIdText;
        
        [Header("Achievements list")]
        [SerializeField] private GameObject achievementSlotPrefab;
        [SerializeField] private Transform achievementsContentList;
        
        [Header("Sounds")]
        [SerializeField] private SoundEnum selectItemSound;
        [SerializeField] private AudioSource selectItemSoundAudioSource;

        private MainMenuController mainMenuController;
        private PauseMenuController pauseMenuController;
        private AudioManager audioManager;
        private MenuInputManager menuInputManager;
        private EventSystem eventSystem;
        private Button firstSelectedButton;
        private static Canvas achievementsMenuCanvas;
        
        private List<AchievementSlot> achievementSlots;

        private int selectedSaveSlotId;

        public int SelectedSaveSlotId
        {
            get => selectedSaveSlotId;
            set
            {
                if (value < minSaveSlotId || value > maxSaveSlotId) return;
                selectedSaveSlotId = value;
            }
        }

        private void Awake()
        {
            audioManager = Finder.AudioManager;
            eventSystem = GetComponentInChildren<EventSystem>();
            achievementsMenuCanvas = GetComponent<Canvas>();
            menuInputManager = Finder.MenuInputManager;
            mainMenuController = Finder.MainMenuController;
            pauseMenuController = Finder.PauseMenuController;

            firstSelectedButton = eventSystem.firstSelectedGameObject.GetComponent<Button>();
            
            selectItemSoundAudioSource.clip = audioManager.GetAudioClip(selectItemSound);

            achievementSlots = new List<AchievementSlot>();
            saveSlotIdText.text = defaultSaveSlotId.ToString();

            selectedSaveSlotId = defaultSaveSlotId;
            LoadAchievements();
            
            achievementsMenuCanvas.enabled = false;
            achievementsMenuCanvas.sortingOrder = 0;
        }

        private void Update()
        {
            if(menuInputManager.ReturnKey)
                ReturnToMainMenu();
        }

        [UsedImplicitly]
        public void ActivateAchievementsMenu()
        {
            achievementsMenuCanvas.enabled = true;
            achievementsMenuCanvas.sortingOrder = 1;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            firstSelectedButton.Select();
        }
        
        [UsedImplicitly]
        public void ReturnToMainMenu()
        {
            selectItemSoundAudioSource.Play();
            
            achievementsMenuCanvas.enabled = false;
            achievementsMenuCanvas.sortingOrder = 0;
            
            if(menuInputManager.isInMainMenu)
                mainMenuController.ShowMainMenu();
            else if(menuInputManager.isInPauseMenu)
                pauseMenuController.ShowPauseMenu();
        }

        private void LoadAchievements()
        {
            foreach (var slot in achievementSlots) Destroy(slot.gameObject);
            achievementSlots.Clear();
            
            var achievementData = AchievementSaver.LoadAchievements(selectedSaveSlotId);
               
            if (achievementData != null)
            {
                foreach (var achievement in achievementData.Achievements)
                {
                    var achievementSlot = 
                        Instantiate(achievementSlotPrefab, achievementsContentList).GetComponent<AchievementSlot>();
                    
                    achievementSlot.SetAchievementName(achievement.Name);
                    achievementSlot.SetAchievementDescription(achievement.Description);
                    achievementSlot.SetAchievementProgression(achievement.Progression, achievement.GoalValue);
                    
                    achievementSlots.Add(achievementSlot);
                }
            }
        }
        
        [UsedImplicitly]
        public void UpdateSaveSlotId(int incrementation)
        {
            selectItemSoundAudioSource.Play();
            
            SelectedSaveSlotId += incrementation;
            saveSlotIdText.text = selectedSaveSlotId.ToString();
            LoadAchievements();
        }
    }
}