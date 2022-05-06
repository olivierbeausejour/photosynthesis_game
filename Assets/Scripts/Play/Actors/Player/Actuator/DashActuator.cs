using System;
using System.Collections;
using Harmony;
using UnityEngine;

namespace Game
{
    public class DashActuator : MonoBehaviour
    {
        [SerializeField] private float dashDistance = 3f;
        [SerializeField] [Range(0f, 1f)] private float percentageDistanceDash = 0.8f;
        [SerializeField] private float maxDashDuration = 0.15f;

        private PlayerController playerController;
        private MovementActuator movementActuator;
        private PlayerIsDoneDashingEventChannel playerIsDoneDashingEventChannel;

        private float dashingSpeed;
        private float slowingSpeed;
        private bool isSlowing;
        private bool isDashing;

        private Vector2 velocity;
        private Vector2 direction;

        public float DashDistance => dashDistance;
        
        public Vector2 Velocity => velocity;
        public Vector2 Direction => direction;
        public bool IsDashing => isDashing;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            movementActuator = GetComponent<MovementActuator>();
            playerIsDoneDashingEventChannel = Finder.PlayerIsDoneDashingEventChannel;
        }

        private void Start()
        {
            dashingSpeed = (dashDistance * percentageDistanceDash) / maxDashDuration;
            slowingSpeed = (dashDistance - dashDistance * percentageDistanceDash) / maxDashDuration;
            velocity = Vector2.zero;
        }

        public IEnumerator DashCoroutine(Vector2 direction, float gravity)
        {
            this.direction = direction.normalized;
            playerController.SetHurtboxActive(true);
            
            velocity = Vector2.zero;

            float totalDashingTime = 0f;
            isDashing = true;
            
            while (!isSlowing)
            {
                if (!playerController.isPlayerAlive)
                    break;
                
                if (totalDashingTime > maxDashDuration)
                {
                    isSlowing = true;
                    totalDashingTime = 0;
                }
                else
                {
                    totalDashingTime += Time.deltaTime;
                    velocity = direction * dashingSpeed;
                    movementActuator.Move(velocity);
                }
                
                yield return null;
            }

            while (isSlowing)
            {
                if (!playerController.isPlayerAlive)
                    break;
                
                if(totalDashingTime > maxDashDuration)
                {
                    break;
                }

                velocity = direction * slowingSpeed;
                velocity.y += gravity * Time.deltaTime;
                totalDashingTime += Time.deltaTime;

                movementActuator.Move(velocity);

                yield return null;
            }
            
            isDashing = false;
            isSlowing = false;
            
            playerController.SetHurtboxActive(false);
            playerIsDoneDashingEventChannel.NotifyPlayerIsDoneDashing();
        }
    }
}