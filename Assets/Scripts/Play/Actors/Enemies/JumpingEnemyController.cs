//Authors:
//Charles Tremblay

using System.Collections;
using System.Linq;
using Harmony;
using UnityEngine;

namespace Game
{
    public class JumpingEnemyController : BaseEnemyController
    {
        [Header("Bounds")]
        [SerializeField] private GameObject leftBound;
        [SerializeField] private GameObject rightBound;

        [Header("Jumping sounds")]
        [SerializeField] private SoundEnum jumpingSound;
        [SerializeField] private AudioSource jumpingSoundAudioSource;

        //TODO is this used somewhere?
        private Hazard hazard;
        private JumpingEnemyAnimatorController jumpingEnemyAnimatorController;
        
        private Vector2 currentPosition;
        private float jumpVelocity;

        private new void Awake()
        {
            audioManager = Finder.AudioManager;
            
            jumpingEnemyAnimatorController = GetComponent<JumpingEnemyAnimatorController>();
            jumpingSoundAudioSource.clip = audioManager.GetAudioClip(jumpingSound);
            base.Awake();
        }
        
        private new void Start()
        {
            
            hazard = GetComponentInChildren<Hazard>();
            currentPosition = transform.position;
            base.Start();
            jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        }
        
        private new void Update()
        {
            if (isPulled)
            {
                leftBound.SetActive(false);
                rightBound.SetActive(false);
            }
            
            currentPosition = transform.position;

            if (currentPosition.x < leftBound.transform.position.x)
            {
                currentDirection = Vector2.right;
            }
                
            
            else if (currentPosition.x > rightBound.transform.position.x)
            {
                currentDirection = Vector2.left;
            }
                
                
            base.Update();
            if (collisionActuator.Collisions.grounded)
            {
                jumpingEnemyAnimatorController.SetIsGrounded(collisionActuator.Collisions.grounded);
            }
        }

        protected override void ManageVerticalMovement()
        {
            if (collisionActuator.Collisions.above) velocity.y = 0f;
            velocity.y += gravity * Time.deltaTime;
        }

        protected override void ManageCollision()
        {
            if (collisionActuator.Collisions.left)
            {
                currentDirection = Vector2.right;
            }
            else if (collisionActuator.Collisions.right)
            {
                currentDirection = Vector2.left;
            }
        }

        private void Jump()
        {
            jumpingSoundAudioSource.Play();
            velocity.y = jumpVelocity;
            jumpingEnemyAnimatorController.WaitForEndFrame();
        }

        protected override void ResetEnemy()
        {
            base.ResetEnemy();
            Jump();
        }

        
    }
}