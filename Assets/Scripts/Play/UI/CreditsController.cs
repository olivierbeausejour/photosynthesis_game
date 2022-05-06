//Authors:
//Charles Tremblay

using Harmony;
using UnityEngine;

namespace Game
{
    public class CreditsController : MonoBehaviour
    {
        [Header("Sounds")] 
        [SerializeField] private SoundEnum creditsThemeSong;
        [SerializeField] private AudioSource creditsThemeSongAudioSource;

        private AudioManager audioManager;
        private GamepadManager gamepadManager;

        private void Awake()
        {
            audioManager = Finder.AudioManager;
            gamepadManager = GetComponent<GamepadManager>();
        }
        
        private void OnEnable()
        {
            creditsThemeSongAudioSource.clip = audioManager.GetAudioClip(creditsThemeSong);
            creditsThemeSongAudioSource.Play();
        }
        
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape) || gamepadManager.GetButtonDown(GamepadManager.Button.B))
                LoadMenu();
        }

        public void LoadMenu()
        {
            Loader.Load(R.S.Scene.MainMenu);
        }
    }
}