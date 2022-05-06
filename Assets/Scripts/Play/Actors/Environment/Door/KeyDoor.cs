//Author: Olivier Beauséjour

using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using UnityEngine;

namespace Game
{
    public class KeyDoor : MonoBehaviour
    {
        [Header("Needed keys")]
        [SerializeField] private List<Key> neededKeyHitboxesToUnlock;
        
        [Header("Sounds")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private SoundEnum soundToPlay;
        
        private Sensor sensor;
        private ISensor<PlayerController> playerSensors;
        private SpriteRenderer[] spriteRenderers;
        private BoxCollider2D[] colliders2D;
        private AudioManager audioManager;
        private PlayerRespawnEventChannel playerRespawnEventChannel;
        private PlayerController playerController;

        private static GameController gameController;
        
        private bool hasBeenUnlocked = false;

        public bool CanDoorBeUnlocked =>
            neededKeyHitboxesToUnlock.Count(key => key.HasBeenUsed) >= neededKeyHitboxesToUnlock.Count;

        private void Awake()
        {
            sensor = GetComponent<Sensor>();
            playerController = FindObjectOfType<PlayerController>();
            playerSensors = sensor.For<PlayerController>();
            playerRespawnEventChannel = Finder.PlayerRespawnEventChannel;
            var parent = transform.parent.gameObject;
            spriteRenderers = parent.GetComponentsInChildren<SpriteRenderer>();
            colliders2D = parent.GetComponentsInChildren<BoxCollider2D>();
            audioManager = FindObjectOfType<AudioManager>();

            gameController = Finder.GameController;

            audioSource.clip = audioManager.GetAudioClip(soundToPlay);
        }
        
        private void OnEnable()
        {
            playerSensors.OnSensedObject += UseKeys;
            playerSensors.OnUnsensedObject += RemoveSensedObject;
            playerRespawnEventChannel.OnPlayerRespawn += ResetKeyPosition;
        }

        private void OnDisable()
        {
            playerSensors.OnSensedObject -= UseKeys;
            playerSensors.OnUnsensedObject -= RemoveSensedObject;
            playerRespawnEventChannel.OnPlayerRespawn -= ResetKeyPosition;
        }

        private void Start()
        {
            if (gameController.CurrentPlayerData.KeysUsed.TryGetValue(gameController.CurrentLevelName, out var currentLevelKeysCollected))
            {
                foreach (var key in neededKeyHitboxesToUnlock)
                {
                    if (currentLevelKeysCollected.Contains(key.Parent().name))
                        gameObject.Parent().SetActive(false);
                }
            }
        }

        private void UseKeys(PlayerController player)
        {
            var detainedKeys = player.Inventory.Where(k => k is Key key && neededKeyHitboxesToUnlock.Contains(key));
            
            foreach (var collectibleActuator in detainedKeys)
            {
                var key = (Key) collectibleActuator;
                key.HasBeenUsed = true;
                
                if (!gameController.CurrentPlayerData.KeysUsed.ContainsKey(gameController.CurrentLevelName))
                    gameController.CurrentPlayerData.KeysUsed.Add(gameController.CurrentLevelName, new List<string>());

                gameController.CurrentPlayerData.KeysUsed[gameController.CurrentLevelName].Add(key.Parent().name);
            }
            
            if (CanDoorBeUnlocked) Unlock();
        }

        private void Unlock()
        {
            audioSource.Play();
            hasBeenUnlocked = true;
            DeactivateDoor();
        }

        private void DeactivateDoor()
        {
            foreach (var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.enabled = false;
            }
            
            foreach (var collider in colliders2D)
            {
                collider.enabled = false;
            }
        }

        private void RemoveSensedObject(PlayerController player)
        {
        }
        
        private void ResetKeyPosition()
        {
            if (hasBeenUnlocked) return;
            
            var detainedKeys = playerController.Inventory.Where(k => k is Key key && neededKeyHitboxesToUnlock.Contains(key));
            List<Collectible> keysToDelete = new List<Collectible>();
            foreach (var collectibleActuator in detainedKeys)
            {
                var key = (Key) collectibleActuator;
                key.ResetKey();
                keysToDelete.Add(collectibleActuator);
            }

            foreach (var keyToDelete in keysToDelete )
            {
                playerController.DeleteKey(keyToDelete);   
            }
        }
    }
}