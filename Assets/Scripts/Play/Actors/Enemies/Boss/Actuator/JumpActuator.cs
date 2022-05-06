// Author : Derek Pouliot

using System;
using Harmony;
using UnityEngine;

namespace Game
{
    public class JumpActuator : MonoBehaviour
    {
        [Header("Jump properties")]
        [SerializeField] private float gravityJumpForce = 9.8f;
        [SerializeField] private float totalDistanceToTravel = 25;
        [SerializeField] private float maxJumpHeight = 8;
        [SerializeField] private float totalSecondsBetweenJumps = 4;
        
        [Header("Sounds")]
        [SerializeField] private SoundEnum jumpSound;
        [SerializeField] private AudioSource jumpSoundAudioSource;

        private AudioManager audioManager;
        
        private float initialJumpAngle;
        private Vector2 initialVelocity;
        
        private float timeElapsedSinceJumpStarted;
        private float totalJumpTime;
        private Vector2 startJumpPosition;
        private int jumpDirection;
        
        private bool isJumping;
        private bool hasReachedMaxHeight;

        public bool IsJumping => isJumping;

        public float TotalSecondsBetweenJumps => totalSecondsBetweenJumps;
        public bool HasCompletedJump => timeElapsedSinceJumpStarted >= totalJumpTime;
        public bool HasReachedMaxJumpHeight => hasReachedMaxHeight;

        private void Awake()
        {
            audioManager = Finder.AudioManager;
            jumpSoundAudioSource.clip = audioManager.GetAudioClip(jumpSound);
            
            isJumping = false;
            timeElapsedSinceJumpStarted = 0;
        }

        public void InitJump()
        {
            jumpSoundAudioSource.Play();
            
            startJumpPosition = transform.position;
            isJumping = true;
            hasReachedMaxHeight = false;
            timeElapsedSinceJumpStarted = 0;
        }

        private void Start()
        {
            CalculateJumpPhysics();
        }

        public void StopJump()
        {
            isJumping = false;
        }

        public void JumpTowardsPlayer()
        {
            var boss = transform;
            
            timeElapsedSinceJumpStarted += Time.deltaTime;
            var newBossPosition = boss.position;
            
            // Source : https://en.wikipedia.org/wiki/Projectile_motion - Displacement
            newBossPosition.x = startJumpPosition.x + initialVelocity.x * timeElapsedSinceJumpStarted * jumpDirection;
            newBossPosition.y = startJumpPosition.y + initialVelocity.y * timeElapsedSinceJumpStarted - (0.5f * gravityJumpForce * (float) Math.Pow(timeElapsedSinceJumpStarted, 2));

            boss.position = newBossPosition;
        }

        public void CheckIfHasReachedMaxHeight()
        {
            if (transform.position.y >= maxJumpHeight + startJumpPosition.y)
                hasReachedMaxHeight = true;
        }

        private void CalculateJumpPhysics()
        {
            // Source : https://en.wikipedia.org/wiki/Projectile_motion - Relation between horizontal range and maximum height
            initialJumpAngle = Mathf.Rad2Deg * Mathf.Atan((4f * maxJumpHeight) / totalDistanceToTravel);

            // Source : https://en.wikipedia.org/wiki/Projectile_motion - Maximum distance of projectile
            var jumpSpeed = Mathf.Sqrt((gravityJumpForce * totalDistanceToTravel) / Mathf.Sin((initialJumpAngle / 0.5f) * Mathf.Deg2Rad));
            
            initialVelocity = new Vector2
            (
                (float) Math.Cos(initialJumpAngle * Mathf.Deg2Rad) * jumpSpeed,
                (float) Math.Sin(Mathf.Deg2Rad * initialJumpAngle) * jumpSpeed
            );

            totalJumpTime = (2f * initialVelocity.y) / gravityJumpForce;
        }

        public void InitJumpDirection(Vector2 playerPosition)
        {
            jumpDirection = playerPosition.x < transform.position.x ? -1 : 1;
        }
    }
}