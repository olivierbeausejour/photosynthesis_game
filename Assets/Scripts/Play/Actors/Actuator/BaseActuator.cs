using Harmony;
using UnityEngine;

namespace Game
{
    public class BaseActuator : MonoBehaviour
    {
        [Header("Movement parameters")] [SerializeField]
        private const float movementSpeed = 5f;

        [SerializeField] private float gravity = -30.0f;

        [Header("Resistance parameters")] [SerializeField]
        private float playerControlsAirborneResistance = 0.1f;

        [SerializeField] private float playerControlsGroundedResistance = 0.05f;
        [SerializeField] private float airResistance = 0.05f;
        [SerializeField] [Range(0f, 1f)] private float resistanceRatioFromSpeedInAir = 0.5f;
        [SerializeField] private float accelerationDampingClamp = 0.001f;

        [Header("Jump parameters")] [SerializeField]
        private float coyoteTime = 0.1f;

        [SerializeField] private int maximumJumpCount = 2;
        [SerializeField] private float jumpHeight = 2f;
        [SerializeField] private float timeToJumpApex = 0.2f;

        private CollisionActuator collisionActuator;
        private MovementActuator movementActuator;
        private ActorBouncedEventChannel actorBouncedEventChannel;

        public CollisionActuator CollisionActuator => collisionActuator;

        // Common
        private Vector2 velocity;
        private bool isReachingForJumpApex;
        private bool grounded;
        private bool hasBounced = false;

        public bool Grounded
        {
            get => grounded;
            set => grounded = value;
        }

        public bool HasBounced
        {
            get => hasBounced;
            set => hasBounced = value;
        }
        
        public Vector2 Velocity
        {
            get => velocity;
            set => velocity = value;
        }
        
        // Jump
        private bool hasJumped;
        public bool HasJumped => hasJumped;

        // Movement
        public float velocitySmoothingX;
        private float horizontalVelocity;

        // Jump
        private float currentCoyoteTime;
        private float jumpVelocity;
        private float jumpGravity;

        public float Gravity => gravity;
        public float MovementSpeed => movementSpeed;


        
        private void Awake()
        {
            collisionActuator = GetComponent<CollisionActuator>();
            movementActuator = GetComponent<MovementActuator>();
            actorBouncedEventChannel = Finder.ActorBouncedEventChannel;
        }

        private void OnEnable()
        {
            actorBouncedEventChannel.OnActorBounced += SetHasBouncedIsTrue;
        }

        private void OnDisable()
        {
            actorBouncedEventChannel.OnActorBounced -= SetHasBouncedIsTrue;
        }

        private void Start()
        {
            // Common
            velocity = Vector2.zero;
            isReachingForJumpApex = false;
            grounded = false;
            
            // Jump
            /* Gravity formula to find the gravity needed to reach the jump apex in a determined distance */
            jumpGravity = -(2f * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            jumpVelocity = Mathf.Abs(jumpGravity) * timeToJumpApex;
        }

        private void SetHasBouncedIsTrue()
        {
            hasBounced = true;
        }

        public void ManageBase(Vector2 rawInputAxis, bool jumpKeyDown, float speed = movementSpeed)
        {
            velocity = movementActuator.CurrentVelocity;
            GroundCheck();
            ManageHorizontalMovement(rawInputAxis, speed);
            ManageVerticalMovement();
            ManageJump(jumpKeyDown);
            movementActuator.Move(velocity);
        }

        public void ManageExternalForce(Vector2 newVelocity)
        {
            velocity = newVelocity;
            movementActuator.Move(velocity);
        }

        /// <summary>
        /// Manages the effect of the movement resistance depending on where the player is in the game(Moving fast in the air, moving slow in the air, moving on the ground.
        /// Takes the player inputs and transforms it into a horizontal velocity for the character.
        /// </summary>
        /// <param name="rawAxisInput">Should always be -1, 0 or 1 for the method to work properly</param>
        private void ManageHorizontalMovement(Vector2 rawAxisInput, float speed)
        {
            if (collisionActuator.Collisions.left || collisionActuator.Collisions.right)
            {
                velocity.x = 0f;
                return;
            }
            
            if (Grounded)
            {
                velocity.x = GetVelocityFromPlayerSmoothedInput(
                    velocity.x,
                    rawAxisInput.x * speed, 
                    playerControlsGroundedResistance);
            }
            else
            {
                if (rawAxisInput != Vector2.zero)
                {
                    if (Mathf.Sign(rawAxisInput.x) == Mathf.Sign(velocity.x))
                    {
                        if (velocity.x > speed || velocity.x < -speed)
                        {
                            velocity.x -= airResistance * Mathf.Sign(velocity.x) * Time.deltaTime;
                        }
                        else
                        {
                            velocity.x = GetVelocityFromPlayerSmoothedInput(velocity.x, rawAxisInput.x * speed,
                                playerControlsAirborneResistance);
                        }
                    }
                    else
                    {
                        velocity.x = GetVelocityFromPlayerSmoothedInput(velocity.x, rawAxisInput.x * speed,
                            playerControlsAirborneResistance);
                    }
                }
                else
                {
                    if (velocity.x > speed * resistanceRatioFromSpeedInAir ||
                        velocity.x < -speed * resistanceRatioFromSpeedInAir)
                    {
                        velocity.x -= airResistance * Mathf.Sign(velocity.x) * Time.deltaTime;
                    }
                    else
                    {
                        velocity.x = GetVelocityFromPlayerSmoothedInput(velocity.x, rawAxisInput.x * speed,
                            playerControlsAirborneResistance);
                    }
                }
            }
        }

        private float GetVelocityFromPlayerSmoothedInput(float velocityX, float targetVelocity, float smoothTime)
        {
            float horizontalVelocity = velocityX;
            horizontalVelocity =
                Mathf.SmoothDamp(horizontalVelocity, targetVelocity, ref velocitySmoothingX, smoothTime);

            if (horizontalVelocity > -accelerationDampingClamp && horizontalVelocity < accelerationDampingClamp)
                horizontalVelocity = 0f;

            return horizontalVelocity;
        }

        private void ManageVerticalMovement()
        {
            if (collisionActuator.Collisions.below || collisionActuator.Collisions.above) velocity.y = 0f;

            if (isReachingForJumpApex)
            {
                if (velocity.y <= 0f) isReachingForJumpApex = false;
                velocity.y += jumpGravity * Time.deltaTime;
            }
            else
                velocity.y += gravity * Time.deltaTime;
        }

        public void ManageJump(bool jumpKeyDown)
        {
            if (Grounded && jumpKeyDown)
            {
                hasJumped = true;
                isReachingForJumpApex = true;
                velocity.y = jumpVelocity;
            }
            else
            {
                hasJumped = false;
            }
        }

        private void GroundCheck()
        {
            if (collisionActuator.Collisions.grounded) grounded = true;

            if (!collisionActuator.Collisions.grounded && Grounded)
            {
                currentCoyoteTime += Time.deltaTime;
            }

            if (currentCoyoteTime >= coyoteTime)
            {
                currentCoyoteTime = 0f;
                grounded = false;
            }
        }
    }
}