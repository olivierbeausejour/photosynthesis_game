using System;
using Harmony;
using UnityEngine;

namespace Game
{
    public class HookableEntityRestart : MonoBehaviour
    {
        private Rigidbody2D hookedEntityRigidbody;
        private PlayerRespawnEventChannel playerRespawnEventChannel;

        private Vector2 initialPosition;
        private void Awake()
        {
            playerRespawnEventChannel = Finder.PlayerRespawnEventChannel;
            initialPosition = transform.position;
            hookedEntityRigidbody = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            playerRespawnEventChannel.OnPlayerRespawn += ResetHookableEntity;
        }

        private void OnDisable()
        {
            playerRespawnEventChannel.OnPlayerRespawn -= ResetHookableEntity;
        }

        private void ResetHookableEntity()
        {
            transform.position = initialPosition;
            hookedEntityRigidbody.bodyType = RigidbodyType2D.Kinematic;
            hookedEntityRigidbody.velocity = Vector2.zero;
        }
    }
}