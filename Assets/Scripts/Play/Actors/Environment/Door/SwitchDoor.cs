//Author: Olivier Beauséjour

using System.Collections.Generic;
using System.Linq;
using Harmony;
using UnityEngine;

namespace Game
{
    public class SwitchDoor : MonoBehaviour
    {
        [Header("Needed switches")]
        [SerializeField] private List<Switch> neededSwitchHitboxesToUnlock;
        
        [Header("Visuals")]
        [SerializeField] private GameObject closedVisualsGameObject;
        [SerializeField] private GameObject openedVisualsGameObject;
        
        [Header("Sounds")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private SoundEnum soundToPlay;
        
        private Collider2D physicalCollider;
        private GameController gameController;
        private AudioManager audioManager;

        public bool AllSwitchesActivated =>
            neededSwitchHitboxesToUnlock.Count(s => s.Switched) == neededSwitchHitboxesToUnlock.Count();

        private void Awake()
        {
            closedVisualsGameObject.SetActive(true);
            openedVisualsGameObject.SetActive(false);
            physicalCollider = gameObject.Parent().GetComponentInChildren<Collider2D>();

            gameController = Finder.GameController;

            audioManager = Finder.AudioManager;
            audioSource.clip = audioManager.GetAudioClip(soundToPlay);
        }

        private void OnEnable()
        {
            foreach (var switchActuator in neededSwitchHitboxesToUnlock)
            {
                switchActuator.OnSwitched += OnSwitchSwitched;
            }
        }

        private void OnDisable()
        {
            foreach (var switchActuator in neededSwitchHitboxesToUnlock)
            {
                switchActuator.OnSwitched -= OnSwitchSwitched;
            }
        }

        private void Start()
        {
            if (gameController.CurrentPlayerData.SwitchesUsed.TryGetValue(gameController.CurrentLevelName, out var currentLevelSwitchesActivated))
            {
                if (currentLevelSwitchesActivated.Count == neededSwitchHitboxesToUnlock.Count)
                    Unlock();
                else 
                    Lock();
            }
        }

        private void OnSwitchSwitched()
        {
            audioSource.Play();
            
            if (AllSwitchesActivated) Unlock();
            else Lock();
        }

        private void Unlock()
        {
            closedVisualsGameObject.SetActive(false);
            openedVisualsGameObject.SetActive(true);
            physicalCollider.enabled = false;
        }

        private void Lock()
        {
            closedVisualsGameObject.SetActive(true);
            openedVisualsGameObject.SetActive(false);
            physicalCollider.enabled = true;
        }
    }
}