// Author: Olivier Beauséjour

using Harmony;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    public class OptionsMenuController : MonoBehaviour
    {
        public const string MASTER_VOLUME = "masterVolume";
        public const string SFX_VOLUME = "sfxVolume";
        public const string MUSIC_VOLUME = "musicVolume";
        
        [Header("Audio mixer")] 
        [SerializeField] private AudioMixer audioMixer;
        
        [Header("Sounds")]
        [SerializeField] private SoundEnum selectItemSound;
        [SerializeField] private AudioSource selectItemSoundAudioSource;
        
        [Header("Sound settings")]
        [SerializeField] private float audioMultiplier = 20;

        private MainMenuController mainMenuController;
        private PauseMenuController pauseMenuController;
        private MenuInputManager menuInputManager;
        private AudioManager audioManager;
        private EventSystem eventSystem;
        private Slider firstSelectedSlider;
        private static Canvas optionsMenuCanvas;

        private void Awake()
        {
            audioManager = Finder.AudioManager;
            eventSystem = GetComponentInChildren<EventSystem>();
            optionsMenuCanvas = GetComponent<Canvas>();
            mainMenuController = Finder.MainMenuController;
            pauseMenuController = Finder.PauseMenuController;
            menuInputManager = Finder.MenuInputManager;
            
            selectItemSoundAudioSource.clip = audioManager.GetAudioClip(selectItemSound);

            firstSelectedSlider = eventSystem.firstSelectedGameObject.GetComponent<Slider>();

            optionsMenuCanvas.enabled = false;
            optionsMenuCanvas.sortingOrder = 0;
        }

        private void Update()
        {
            if(menuInputManager.ReturnKey)
                ReturnToMainMenu();
        }

        [UsedImplicitly]
        public void ActivateOptionsMenu()
        {
            optionsMenuCanvas.enabled = true;
            optionsMenuCanvas.sortingOrder = 1;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            firstSelectedSlider.Select();
        }
        
        [UsedImplicitly]
        public void ReturnToMainMenu()
        {
            selectItemSoundAudioSource.Play();
            
            optionsMenuCanvas.enabled = false;
            optionsMenuCanvas.sortingOrder = 0;
            
            if(menuInputManager.isInMainMenu)
                mainMenuController.ShowMainMenu();
            else if(menuInputManager.isInPauseMenu)
                pauseMenuController.ShowPauseMenu();
        }
        
        [UsedImplicitly]
        public void SetMasterVolume(float value)
        {
            audioMixer.SetFloat(MASTER_VOLUME, Mathf.Log10(value) * audioMultiplier);
        }
        
        [UsedImplicitly]
        public void SetSfxVolume(float value)
        {
            audioMixer.SetFloat(SFX_VOLUME, Mathf.Log10(value) * audioMultiplier);
        }
        
        [UsedImplicitly]
        public void SetMusicVolume(float value)
        {
            audioMixer.SetFloat(MUSIC_VOLUME, Mathf.Log10(value) * audioMultiplier);
        }
    }
}