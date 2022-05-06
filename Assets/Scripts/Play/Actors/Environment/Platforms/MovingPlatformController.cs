// Authors :
// Derek Pouliot
// Olivier Beauséjour
// Charles Tremblay

using System;
using System.Collections;
using Harmony;
using UnityEngine;

namespace Game
{
    public class MovingPlatformController : MonoBehaviour
    {
        [Header("Movement points")]
        [SerializeField] private Transform startPosition;
        [SerializeField] private Transform endPosition;
        
        [Header("Moving properties")]
        [Range(0f, 40.0f)] [SerializeField] private float smoothTime = 3f;
        [SerializeField] private float directionSwapTreshold = 0.01f;
        [SerializeField] private float maxSpeed = 3f;
        [SerializeField] private float crushingThreshold = 0.02f;

        [Header("Acting properties")] 
        [SerializeField] private bool oneWayOnly;
        [SerializeField] private bool startMovingWhenPlayerSensed;

        [Header("Sounds")] 
        [SerializeField] private SoundEnum movingSound;
        [SerializeField] private float secondsBetweenEachSound = 2f;

        private BoxCollider2D platformBoxCollider;
        private Sensor sensor;
        
        private ISensor<PlayerController> playerSensors;
        private bool canMove;

        private Vector2 targetPosition;
        private Vector2 platformVelocity;
        private Vector2 currentVelocity;
        private AudioManager audioManager;
        private AudioSource audioSource;

        private bool atEndPosition = false;

        private PlayerRespawnEventChannel playerRespawnEventChannel;

        private void Awake()
        {
            platformBoxCollider = GetComponent<BoxCollider2D>();
            sensor = GetComponentInChildren<Sensor>();

            playerSensors = sensor.For<PlayerController>();
            audioManager = FindObjectOfType<AudioManager>();
            audioSource = GetComponentInChildren<AudioSource>();
            
            audioSource.clip = audioManager.GetAudioClip(movingSound);
            transform.position = startPosition.position;
            targetPosition = endPosition.position;
            if (!startMovingWhenPlayerSensed) canMove = true;
            StartCoroutine(PlaySound());

            playerRespawnEventChannel = Finder.PlayerRespawnEventChannel;
        }
        
        private void OnEnable()
        {
            playerSensors.OnSensedObject += UnlockPlatform;
            playerSensors.OnUnsensedObject += RemoveSensedObject;
            playerRespawnEventChannel.OnPlayerRespawn += ResetPlatform;
        }

        private void OnDisable()
        {
            playerSensors.OnSensedObject -= UnlockPlatform;
            playerSensors.OnUnsensedObject -= RemoveSensedObject;
            playerRespawnEventChannel.OnPlayerRespawn -= ResetPlatform;
        }

        private void UnlockPlatform(PlayerController player)
        {
            if (startMovingWhenPlayerSensed && !canMove) canMove = true;
        }

        private void RemoveSensedObject(PlayerController player)
        {
            // Empty on purpose.
            // Necessary to be there. Otherwise, there are sensor bugs appearing.
        }

        private void Update()
        {
            if (!canMove) return;
            
            Vector2 currentPosition = transform.position;
            
            transform.position = Vector2.SmoothDamp(currentPosition, targetPosition, ref currentVelocity, smoothTime, maxSpeed);

            if (!(Mathf.Abs(targetPosition.x - transform.position.x) < directionSwapTreshold) ||
                !(Mathf.Abs(targetPosition.y - transform.position.y) < directionSwapTreshold)) return;

            if (oneWayOnly) return;
            
            atEndPosition = !atEndPosition;
            ChangePlatformDirection();
        }

        private void ChangePlatformDirection()
        {
            targetPosition = atEndPosition ? startPosition.position : endPosition.position;
        }

        private IEnumerator PlaySound()
        {
            while (isActiveAndEnabled)
            {
                audioSource.Play();
                yield return new WaitForSeconds(secondsBetweenEachSound);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(startPosition.position, endPosition.position);
        }
        
        public void DetachPlayerFromPlatform(Transform playerTransform)
        {
            playerTransform.parent = null;
        }

        private bool IsPlayerVerticallyConfinedWithPlatform(Collider2D playerCollider, CollisionActuator playerCollisionActuator)
        {
            Vector2 playerSize = playerCollider.bounds.size;
            Vector2 platformSize = platformBoxCollider.bounds.size;
            
            var currentPosition = transform.position;
            var playerPosition = playerCollider.transform.position;

            return Math.Abs((currentPosition.y - platformSize.y / 2) - (playerPosition.y + playerSize.y)) <= crushingThreshold &&
                   playerPosition.y < currentPosition.y - platformSize.y / 2 &&
                   playerCollisionActuator.Collisions.below;
        }
        
        public bool IsCrushingPlayerVertically(Collider2D playerCollider)
        {
            var player = playerCollider.Parent().gameObject;
            var playerCollisionActuator = player.GetComponent<CollisionActuator>();

            return IsPlayerVerticallyConfinedWithPlatform(playerCollider, playerCollisionActuator);
        }

        private void ResetPlatform()
        {
            if(startMovingWhenPlayerSensed)
                canMove = false;
            transform.position = startPosition.position;
            currentVelocity = Vector2.zero;
            targetPosition = endPosition.position;
        }
    }
}