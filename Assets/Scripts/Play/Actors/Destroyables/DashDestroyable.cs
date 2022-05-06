//Author: Olivier Beauséjour

using System;
using Harmony;
using UnityEngine;

namespace Game
{
    public class DashDestroyable : MonoBehaviour, IDashDestroyable
    {
        [Header("Sounds")]
        [SerializeField] private AudioSource destroySound;
        
        private SpriteRenderer[] spriteRenderers;
        private BoxCollider2D[] colliders2D;
        private AudioManager audioManager;
        private PlayerRespawnEventChannel playerRespawnEventChannel;
        private new ParticleSystem particleSystem;
        
        private void Awake()
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            colliders2D = GetComponentsInChildren<BoxCollider2D>();
            audioManager = Finder.AudioManager;
            playerRespawnEventChannel = Finder.PlayerRespawnEventChannel;
            particleSystem = GetComponentInChildren<ParticleSystem>();

            if (destroySound != null) 
                destroySound.clip = audioManager.GetAudioClip(SoundEnum.destroyableWalls);
        }

        private void OnEnable()
        {
            playerRespawnEventChannel.OnPlayerRespawn += ResetComponent;
        }

        private void OnDisable()
        {
            playerRespawnEventChannel.OnPlayerRespawn -= ResetComponent;
        }
        
        private void ResetComponent()
        {
            var layer = gameObject.layer;
            if (layer == LayerMask.NameToLayer(R.S.Layer.Destroyable))
            {
                ReactivateDestroyableWalls();
            }
        }

        private void ReactivateDestroyableWalls()
        {
            foreach (var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.enabled = true;
            }
            foreach (var collider in colliders2D)
            {
                collider.enabled = true;
            }
        }

        public void DestroyByDash()
        {
            var layer = gameObject.layer;
            if (layer == LayerMask.NameToLayer(R.S.Layer.Destroyable))
            {
                if (destroySound != null)  destroySound.Play();
                if (particleSystem != null) particleSystem.Play();
                
                DeactivateDestroyableWall();
            }
            else if(layer == LayerMask.NameToLayer(R.S.Layer.Enemy))
            {
                DeactivateEnemy();
            }
        }

        private void DeactivateEnemy()
        {
           transform.gameObject.GetComponent<BaseEnemyController>().DisableEnemy();
        }

        private void DeactivateDestroyableWall()
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
    }
}