//Authors:
//Anthony Dodier
//Charles Tremblay
//Olivier Beauséjour

using System.Collections.Generic;
using System.Linq;
using Harmony;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    [Findable(R.S.Tag.MainMenu)]
    public class PauseMenuController : MonoBehaviour
    {
        [Header("Buttons")] 

        [Header("Sounds")]
        [SerializeField] private SoundEnum pauseUnpauseSound;
        [SerializeField] private SoundEnum hoverItemSound;
        [SerializeField] private SoundEnum selectItemSound;
        [SerializeField] private AudioSource pauseUnpauseSoundAudioSource;
        [SerializeField] private AudioSource hoverItemSoundAudioSource;
        [SerializeField] private AudioSource selectItemSoundAudioSource;

        private AudioManager audioManager;
        private GameController gameController;
        private List<Button> buttons;
        private EventSystem eventSystem;

        private ControlsMenuController controlsMenuController;
        private OptionsMenuController optionsMenuController;
        private AchievementsMenuController achievementsMenuController;

        private MenuInputManager inputManager;
        private PauseMenuController pauseMenuController;
        private Canvas pauseMenuCanvas;

        private bool isPaused;
        private Button firstSelectedButton;

        private void Awake()
        {
            audioManager = Finder.AudioManager;
            gameController = Finder.GameController;
            buttons = GetComponentsInChildren<Button>().ToList();
            eventSystem = GetComponentInChildren<EventSystem>();
            pauseMenuController = Finder.PauseMenuController;

            controlsMenuController = this.GetComponentInSiblings<ControlsMenuController>();
            optionsMenuController = this.GetComponentInSiblings<OptionsMenuController>();
            achievementsMenuController = this.GetComponentInSiblings<AchievementsMenuController>();
            
            controlsMenuController.gameObject.SetActive(false);
            optionsMenuController.gameObject.SetActive(false);
            achievementsMenuController.gameObject.SetActive(false);
            
            pauseMenuCanvas = GetComponent<Canvas>();
            inputManager = GetComponent<MenuInputManager>();
            
            pauseUnpauseSoundAudioSource.clip = audioManager.GetAudioClip(pauseUnpauseSound);
            hoverItemSoundAudioSource.clip = audioManager.GetAudioClip(hoverItemSound);
            selectItemSoundAudioSource.clip = audioManager.GetAudioClip(selectItemSound);

            firstSelectedButton = eventSystem.firstSelectedGameObject.GetComponent<Button>();
            SetEnableButtons(false);
            
            isPaused = false;
            pauseMenuCanvas.enabled = false;
        }

        private void Update()
        {
            if (!isPaused)
            {
                if (inputManager.ExitKeyDown)
                {
                    ShowPauseMenu();
                    Pause();
                }
            }
            
            else
            {
                if ((inputManager.ExitKeyDown || inputManager.ReturnKey) && pauseMenuCanvas.enabled)
                {
                    HidePauseMenu();
                    Resume();
                }
            }
        }
        
        [UsedImplicitly]
        public void ShowPauseMenu()
        {
            inputManager.isInMainMenu = false;
            inputManager.isInPauseMenu = true;
            pauseUnpauseSoundAudioSource.Play();
            
            controlsMenuController.gameObject.SetActive(false);
            optionsMenuController.gameObject.SetActive(false);
            achievementsMenuController.gameObject.SetActive(false);
            
            pauseMenuCanvas.enabled = true;
            pauseMenuCanvas.sortingOrder = 1;
        }
        
        [UsedImplicitly]
        public void EnableOptionsMenu()
        {
            optionsMenuController.gameObject.SetActive(true);
        }
        
        [UsedImplicitly]
        public void EnableControlsMenu()
        {
            controlsMenuController.gameObject.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        
        [UsedImplicitly]
        public void EnableAchievementsMenu()
        {
            achievementsMenuController.gameObject.SetActive(true);
        }
        
        [UsedImplicitly]
        public void HidePauseMenu()
        {
            pauseMenuCanvas.enabled = false;
            pauseMenuCanvas.sortingOrder = 0;
            
            selectItemSoundAudioSource.Play();
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
        
        [UsedImplicitly]
        public void ReturnToMainMenu()
        {
            selectItemSoundAudioSource.Play();
            Loader.Load(R.S.Scene.MainMenu);
        }

        public void Resume()
        {
            isPaused = false;
            inputManager.CanUseMenuControls = false;
            gameController.ResumeGame();

            SetEnableButtons(false);
            HidePauseMenu();
            
            selectItemSoundAudioSource.Play();
        }

        public void Pause()
        {
            isPaused = true;
            inputManager.CanUseMenuControls = true;
            gameController.PauseGame();
            
            SetEnableButtons(true);
            pauseMenuCanvas.enabled = true;

            firstSelectedButton.Select();
        }
        
        private void SetEnableButtons(bool value)
        {
            foreach (var button in buttons) button.enabled = value;
        }
    }
}