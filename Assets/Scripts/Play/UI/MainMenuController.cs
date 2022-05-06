// Authors:
// Anthony Dodier
// Olivier Beauséjour
// Charles Tremblay

using Harmony;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    [Findable(R.S.Tag.MainMenu)]
    public class MainMenuController : MonoBehaviour
    {
        [Header("Sounds")]
        [SerializeField] private SoundEnum selectItemSound;
        [SerializeField] private SoundEnum mainMenuThemeSongSound;
        [SerializeField] private AudioSource selectItemSoundAudioSource;
        [SerializeField] private AudioSource mainMenuThemeSongSoundAudioSource;

        private AudioManager audioManager;
        private MenuInputManager inputManager;
        private EventSystem eventSystem;
        private Button firstSelectedButton;
        private Canvas mainMenuCanvas;

        private static Button CurrentSelectedButton =>
            EventSystem.current.currentSelectedGameObject?.GetComponent<Button>();

        private void Awake()
        {
            audioManager = Finder.AudioManager;
            mainMenuCanvas = GetComponent<Canvas>();
            inputManager = GetComponent<MenuInputManager>();
            eventSystem = GetComponentInChildren<EventSystem>();

            firstSelectedButton = eventSystem.firstSelectedGameObject.GetComponent<Button>();
            
            selectItemSoundAudioSource.clip = audioManager.GetAudioClip(selectItemSound);
            mainMenuThemeSongSoundAudioSource.clip = audioManager.GetAudioClip(mainMenuThemeSongSound);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void Start()
        {
            ShowMainMenu();
            mainMenuThemeSongSoundAudioSource.Play();
        }
        
        // Author : Derek Pouliot
        [UsedImplicitly]
        public void ShowMainMenu()
        {
            inputManager.isInMainMenu = true;
            inputManager.isInPauseMenu = false;
            mainMenuCanvas.enabled = true;
            mainMenuCanvas.sortingOrder = 1;
            
            firstSelectedButton.Select();
        }

        // Author : Derek Pouliot
        [UsedImplicitly]
        public void HideMainMenu()
        {
            selectItemSoundAudioSource.Play();
            mainMenuCanvas.enabled = false;
            mainMenuCanvas.sortingOrder = 0;
        }

        public void Exit()
        {
            selectItemSoundAudioSource.Play();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            ApplicationExtensions.Quit();
#endif
        }
    }
}