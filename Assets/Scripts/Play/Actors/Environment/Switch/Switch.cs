//Authors:
//Olivier Beauséjour
//Charles Tremblay

using System;
using System.Collections.Generic;
using Harmony;
using UnityEngine;

namespace Game
{
    public class Switch : MonoBehaviour
    {
        [Header("Visuals")]
        [SerializeField] private SpriteRenderer switchSpriteRenderer;
        [SerializeField] private Sprite offSprite;
        [SerializeField] private Sprite onSprite;

        [Header("Sounds")] 
        [SerializeField] private AudioSource soundToPlayAudioSource;
        [SerializeField] private SoundEnum soundToPlay;

        private Sensor sensor;
        private ISensor<PlayerController> playerSensors;
        private AudioManager audioManager;
        
        private bool switched;
        private GameController gameController;

        public event SwitchEventHandler OnSwitched;

        public bool Switched
        {
            get => switched;
            
            private set
            {
                switched = value;
                NotifySwitched();
            }
        }

        private void Awake()
        {
            sensor = GetComponent<Sensor>();
            switchSpriteRenderer.sprite = offSprite;
            playerSensors = sensor.For<PlayerController>();
            audioManager = Finder.AudioManager;

            soundToPlayAudioSource.clip = audioManager.GetAudioClip(soundToPlay);

            gameController = Finder.GameController;
        }

        private void OnEnable()
        {
            playerSensors.OnSensedObject += ToggleSwitch;
            playerSensors.OnUnsensedObject += RemoveSensedObject;
        }

        private void OnDisable()
        {
            playerSensors.OnSensedObject -= ToggleSwitch;
            playerSensors.OnUnsensedObject -= RemoveSensedObject;
        }

        private void Start()
        {
            if (gameController.CurrentPlayerData.SwitchesUsed.TryGetValue(gameController.CurrentLevelName, out var currentLevelSwitchesUsed))
            {
                if (currentLevelSwitchesUsed.Contains(gameObject.Parent().name))
                    switched = true;
            }
            
            UpdateSwitchVisual();
        }

        private void UpdateSwitchVisual()
        {
            if (!Switched)
                switchSpriteRenderer.sprite = offSprite;
            else
                switchSpriteRenderer.sprite = onSprite;
        }

        private void ToggleSwitch(PlayerController player)
        {
            soundToPlayAudioSource.Play();
            Switched = !Switched;

            UpdateSwitchVisual();
            
            if (!gameController.CurrentPlayerData.SwitchesUsed.ContainsKey(gameController.CurrentLevelName))
                gameController.CurrentPlayerData.SwitchesUsed.Add(gameController.CurrentLevelName, new List<string>());

            if (Switched)
                gameController.CurrentPlayerData.SwitchesUsed[gameController.CurrentLevelName].Add(gameObject.Parent().name);
            else
                gameController.CurrentPlayerData.SwitchesUsed[gameController.CurrentLevelName].Remove(gameObject.Parent().name);
        }

        private void RemoveSensedObject(PlayerController player)
        {
        }

        public void NotifySwitched()
        {
            if (OnSwitched != null)
            {
                soundToPlayAudioSource.Play();
                OnSwitched();
            }
        }
        
        public delegate void SwitchEventHandler();
    }
}