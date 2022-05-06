//Authors:
//Olivier Beauséjour

using System;
using System.Collections;
using Harmony;
using UnityEngine;

namespace Game
{
    public class ProjectileController : MonoBehaviour
    {
        [Header("Movement parameters")]
        [SerializeField] private float movementSpeed = 10f;
        [SerializeField] private Vector2 direction;
        [SerializeField] private LayerMask collisionMask;
        [SerializeField] private float timeToDisableInSeconds = 2f;

        private SpriteRenderer projectileSpriteRenderer;

        private PlayerRespawnEventChannel playerRespawnEventChannel;

        private Vector3 velocity;

        private void Awake()
        {
            playerRespawnEventChannel = Finder.PlayerRespawnEventChannel;
            projectileSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void OnEnable()
        {
            playerRespawnEventChannel.OnPlayerRespawn += DeactivateProjectile;
            
            StartCoroutine(SetDisableAfterTime());
        }

        private void OnDisable()
        {
            playerRespawnEventChannel.OnPlayerRespawn -= DeactivateProjectile;
        }
        
        private void DeactivateProjectile()
        {
            gameObject.SetActive(false);
        }

        private void Start()
        {
            enabled = true;
        }

        private IEnumerator SetDisableAfterTime()
        {
            yield return new WaitForSeconds(timeToDisableInSeconds);

            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (ShouldCollideWithObjectLayer(other))
                gameObject.SetActive(false);
        }

        private bool ShouldCollideWithObjectLayer(Collider2D other)
        {
            return collisionMask == (collisionMask | (1 << other.gameObject.layer));
        }

        private void Update()
        {
            ManageVelocity();
            ManageMovement();
            ManageHorizontalDirection();
        }

        private void ManageHorizontalDirection()
        {
            if (velocity.x < 0)
                projectileSpriteRenderer.flipX = true;
            else
                projectileSpriteRenderer.flipX = false;
        }

        private void ManageVelocity()
        {
            velocity = movementSpeed * direction.normalized;
        }
        
        private void ManageMovement()
        {
            transform.Translate(velocity * Time.deltaTime);
        }
    }
}