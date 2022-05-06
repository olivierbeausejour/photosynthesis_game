using UnityEngine;

namespace Game
{
    public class SwayActuator : MonoBehaviour
    {
        private const float GRAPPLE_LENGTH_INPUT_MODIFIER_TOLERANCE = 0.01f;
        private const float GRAPPLE_LOCK_LENGTH_TOLERANCE = 0.005f;
        private const float COLLISION_GRAPPLE_LENGTH_MODIFIER_TOLERANCE = 0.001f;

        [Header("Speed parameter")] [SerializeField]
        private float entryVelocityMagnitudeModifier = 1.5f;
    
        [SerializeField] private float maximumSwayVelocityMagnitude = 25f;
        [SerializeField] private float minimumSwayVelocityMagnitude = 15f;
    
        [Header("Maximum length")] 
        [Header("Pendulum parameters")] 
        [Header("___________________")]
        [SerializeField]  private float velocityMagnitudeReductionMaximumLength = 15f;
        [SerializeField] private float velocityMagnitudeAdditionMaximumLength = 10f;
        [SerializeField] private float playerVelocityMagnitudeModifierMaximumLength = 15f;
    
        [Header("Minimum length")] 
        [SerializeField] private float velocityMagnitudeReductionMinimumLength = 25f;
        [SerializeField] private float velocityMagnitudeAdditionMinimumLength = 20f;
        [SerializeField] private float playerVelocityMagnitudeModifierMinimumLength = 30f;
    
        [Header("")] 
        [SerializeField] private AnimationCurve magnitudeReductionCurve;
        [SerializeField] private float animationCurveLimitAngle = 100f;
        [SerializeField] private float middleAngleTreshold = 0.3f;
        [SerializeField] private float middleVelocityTreshold = 0.5f;
        [SerializeField] private float magnitudeFromZero = 300f;
        [SerializeField] private float stationaryGroundedStartVelocityMagnitude = 3f;


        [Header("Collisions parameters")] [Header("___________________")] 
        [SerializeField] private LayerMask collisionMask;

        [Header("Rappel parameter")]  [Header("___________________")]
        [SerializeField] private float rappelSpeed = 50f;

        [Header("Other parameter")] [Header("___________________")] 
        [SerializeField] private float minimumGrappleLength = 2f;

        [SerializeField] private float maximumGrappleLength = 10f;
        [SerializeField] private bool showDebugRay = true;

        private CollisionActuator collisionActuator;
        private GrapplingHookController grapplingHookController;
        private MovementActuator movementActuator;
        private BaseActuator baseActuator;

        private Vector2 playerPosition;
        private Vector2 playerHookPoint;
        private Vector2 grappleHookPoint;
        private Vector2 desiredVelocity;
        private Vector2 rappelAdditiveVelocity;
        private float velocityMagnitude;
        private float swayDirection;
        private float grappleLength;
        private float rappelSmoothTimeVelocity;
        private float angle;
        private bool isLocked;
        public bool IsLocked => isLocked;
        private bool isRappelling;
        public float GrappleLength => grappleLength;
        public float SwayDirection => swayDirection;


        private void Awake()
        {
            collisionActuator = GetComponent<CollisionActuator>();
            grapplingHookController = GetComponent<GrapplingHookController>();
            movementActuator = GetComponent<MovementActuator>();
            baseActuator = GetComponent<BaseActuator>();
        }
    
        private void Start()
        {
            isLocked = false;
        }
    
        public void InitialiseSway(Vector2 rawAxisInput, Vector2 grappleHookPoint, float grappleLength)
        {
            this.grappleHookPoint = grappleHookPoint;
            this.grappleLength = (grappleLength < minimumGrappleLength) ? minimumGrappleLength : grappleLength;
            isLocked = false;
            
            baseActuator.ManageBase(rawAxisInput, false);
        } 
    
        public void ManageSway(Vector2 rawInputAxis, Vector2 playerHookPoint)
        {
            playerPosition = transform.position;
            this.playerHookPoint = playerHookPoint;

            if (!isLocked)
                ManageUnlockedSway(rawInputAxis);
            else
                ManageLockedSway(rawInputAxis);
            
#if UNITY_EDITOR
            if (showDebugRay)
            {
                Debug.DrawLine(playerPosition, grapplingHookController.Information.hookPosition, Color.black);
            }            
#endif
        }
        
        private void ManageUnlockedSway(Vector2 rawAxisInput)
        {
            SetSwayLock();
    
            if (!isLocked)
            {
                baseActuator.ManageBase(rawAxisInput, false,
                    (velocityMagnitude > baseActuator.MovementSpeed && collisionActuator.Collisions.grounded)
                        ? velocityMagnitude
                        : baseActuator.MovementSpeed);
            }
    
            if (CheckIfRopeIntersectWithWorld())
                grapplingHookController.StopGrappleSway();
        }
    
        private void ManageLockedSway(Vector2 rawInputAxis)
        {
            if (CheckIfRopeIntersectWithWorld())
                grapplingHookController.StopGrappleSway();
            
            float currentAngle = Vector2.Angle(Vector2.down, playerPosition - grappleHookPoint);
    
            if (playerPosition.x < grappleHookPoint.x)
            {
                currentAngle = -currentAngle;
            }

            ModifyGrappleLength(rawInputAxis.y);
            ModifyVelocityMagnitude(rawInputAxis, currentAngle);
            GenerateNewVelocity(currentAngle);
            ManageSwayLimits(rawInputAxis.y);

#if UNITY_EDITOR
            Vector2 lastPosition = playerPosition;
#endif
            
            movementActuator.Move(desiredVelocity/Time.deltaTime);

#if UNITY_EDITOR
            if (showDebugRay)
            {
                Debug.DrawLine(lastPosition, playerPosition, Color.green, 10f);
            }
    #endif
        }

        private void SetSwayLock()
        {
            float grappleCurrentLength = Vector2.Distance(playerPosition, grappleHookPoint);
    
            if (grappleCurrentLength > grappleLength + GRAPPLE_LOCK_LENGTH_TOLERANCE)
            {
                isLocked = true;
                Vector2 lockedPosition = grappleHookPoint + (playerPosition - grappleHookPoint).normalized * grappleLength;
                SetSwayStartVelocity();
                playerPosition = lockedPosition;
            }
            else
            {
                isLocked = false;
            }
        }

        private void ModifyGrappleLength(float input)
        {
            if (grappleLength >= maximumGrappleLength && input < 0f ||
                grappleLength <= minimumGrappleLength && input > 0f ||
                collisionActuator.Collisions.left && playerPosition.x < grappleHookPoint.x ||
                collisionActuator.Collisions.right && playerPosition.x > grappleHookPoint.x ||
                collisionActuator.Collisions.above && input > 0f ||
                Mathf.Abs(input) < GRAPPLE_LENGTH_INPUT_MODIFIER_TOLERANCE)
                return;

            grappleLength = Mathf.Clamp(grappleLength -input * rappelSpeed * Time.deltaTime, minimumGrappleLength, maximumGrappleLength);
        }

        private void SetSwayStartVelocity()
        {
            desiredVelocity = movementActuator.CurrentVelocity;
    
            swayDirection = FindPlayerSwayDirection();
    
            velocityMagnitude = FindDesiredVelocityMagnitude();
    
            ManageLockedSway(Vector2.zero);
        }

        private float FindPlayerSwayDirection()
        {
            float direction = 0;
    
            Vector2 linearFunctionPoint1 = grappleHookPoint;
            Vector2 linearFunctionPoint2 = grappleHookPoint + (playerPosition - grappleHookPoint);
    
            Vector2 playerVelocityEndPoint = playerPosition + desiredVelocity;
    
            // Linear Function using point 1 and point 2 to know if the velocity end point will be above or below the line 
            // to determine the direction of the sway...
            // y = ax+b
            // y - b = ax
            // x = (y-b) / a
            // then evaluate X with playerVelocityEndPoint to know if the vector is on the left, right or on the line
    
            float a = 0f;
    
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (linearFunctionPoint2.x != linearFunctionPoint1.x)
            {
                a = (linearFunctionPoint2.y - linearFunctionPoint1.y) / (linearFunctionPoint2.x - linearFunctionPoint1.x);
            }
    
            float b = linearFunctionPoint1.y - (a * linearFunctionPoint1.x);
    
            float xEvaluation = (playerVelocityEndPoint.y - b) / a;

            if (playerVelocityEndPoint.x < xEvaluation)
                direction = (playerPosition.y <= grappleHookPoint.y) ? -1 : 1;
            else if (playerVelocityEndPoint.x > xEvaluation)
                direction = (playerPosition.y <= grappleHookPoint.y) ? 1 : -1;
            else
                direction = 0;
    
            return direction;
        }

        private float FindDesiredVelocityMagnitude()
        {
            Vector2 directionFromGrappleHookPointToPlayer = (playerPosition - grappleHookPoint);
    
            float playerVelocityNormalAngleInRadius = Vector2.Angle(directionFromGrappleHookPointToPlayer,
                movementActuator.CurrentVelocity);
    
            float velocityModifierRatio = playerVelocityNormalAngleInRadius / 90f;

            if (collisionActuator.Collisions.grounded && desiredVelocity.magnitude < baseActuator.MovementSpeed)
                return stationaryGroundedStartVelocityMagnitude;
    
            return (desiredVelocity.magnitude * velocityModifierRatio * entryVelocityMagnitudeModifier >
                    maximumSwayVelocityMagnitude)
                ? maximumSwayVelocityMagnitude
                : desiredVelocity.magnitude * velocityModifierRatio * entryVelocityMagnitudeModifier;
        }

        private void ModifyVelocityMagnitude(Vector2 rawAxisInput, float currentAngle)
        {
            float grappleLengthMagnitudeRatio =
                (grappleLength - minimumGrappleLength) / (maximumGrappleLength - minimumGrappleLength);
    
            float amountToModify = 0f;
    
            if (Mathf.Abs(velocityMagnitude) < middleVelocityTreshold * Time.deltaTime && Mathf.Abs(currentAngle) < middleAngleTreshold * Time.deltaTime)
            {
                velocityMagnitude = 0f;
    
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (rawAxisInput.x != 0f)
                {
                    velocityMagnitude = magnitudeFromZero * Time.deltaTime;
                    swayDirection = (rawAxisInput.x > 0f) ? 1f : -1f;
                }
    
                return;
            }
    
            // Climbing
            if (swayDirection > 0f && playerPosition.x >= grappleHookPoint.x ||
                swayDirection <= 0f && playerPosition.x < grappleHookPoint.x)
            {
                amountToModify = -magnitudeReductionCurve.Evaluate(Mathf.Abs(currentAngle) / animationCurveLimitAngle) *
                                 Mathf.Lerp(
                                     velocityMagnitudeReductionMinimumLength, velocityMagnitudeReductionMaximumLength,
                                     grappleLengthMagnitudeRatio);
    
                // ReSharper disable 3 CompareOfFloatsByEqualityOperator
                if (rawAxisInput.x != 0f && swayDirection != 0f && Mathf.Sign(rawAxisInput.x) != Mathf.Sign(swayDirection))
                {
                    amountToModify -= Mathf.Lerp(playerVelocityMagnitudeModifierMinimumLength,
                        playerVelocityMagnitudeModifierMaximumLength, grappleLengthMagnitudeRatio);
                }
            }
            // Descending
            else if (swayDirection > 0f && playerPosition.x < grappleHookPoint.x ||
                     swayDirection <= 0f && playerPosition.x >= grappleHookPoint.x)
            {
                amountToModify = Mathf.Lerp(velocityMagnitudeAdditionMinimumLength, velocityMagnitudeAdditionMaximumLength,
                    grappleLengthMagnitudeRatio);
    
                // ReSharper disable 3 CompareOfFloatsByEqualityOperator
                if (rawAxisInput.x != 0f && swayDirection != 0f && Mathf.Sign(rawAxisInput.x) == Mathf.Sign(swayDirection))
                {
                    amountToModify += Mathf.Lerp(playerVelocityMagnitudeModifierMinimumLength,
                        playerVelocityMagnitudeModifierMaximumLength, grappleLengthMagnitudeRatio);
                }
            }
            
            amountToModify *= Time.deltaTime;
    
            if (velocityMagnitude + amountToModify <= 0)
            {
                velocityMagnitude = 0;
                desiredVelocity = -desiredVelocity;
                swayDirection = -swayDirection;
            }
            else
            {
                float magnitudeSpeed = Mathf.Lerp(minimumSwayVelocityMagnitude, maximumSwayVelocityMagnitude,
                    grappleLengthMagnitudeRatio);
                if (velocityMagnitude + amountToModify > magnitudeSpeed)
                    velocityMagnitude = magnitudeSpeed;
    
                velocityMagnitude += amountToModify;
            }
        }

        private void GenerateNewVelocity(float currentAngle)
        {
            if (grappleLength > maximumGrappleLength) grappleLength = maximumGrappleLength;
            else if (grappleLength < minimumGrappleLength) grappleLength = minimumGrappleLength;
            
            float additiveAngle = Mathf.Asin(velocityMagnitude * Time.deltaTime / 2 / grappleLength) * 2f *
                                  Mathf.Rad2Deg;

            angle = currentAngle + additiveAngle * swayDirection;
            
            Vector2 desiredPosition = grappleHookPoint +
                                      new Vector2(
                                          -Mathf.Sin(-angle * Mathf.Deg2Rad),
                                          -Mathf.Cos(-angle * Mathf.Deg2Rad)) *
                                      grappleLength;
            
            desiredVelocity = (desiredPosition - playerPosition);
            Debug.DrawRay(playerPosition, desiredVelocity * Time.deltaTime, Color.red, 10f);
        }

        private bool CheckIfRopeIntersectWithWorld()
        {
            RaycastHit2D hit = Physics2D.Linecast(grappleHookPoint, playerHookPoint, collisionMask);
            
            Debug.DrawLine(grappleHookPoint, playerHookPoint, Color.white);
    
            if (hit)
            {
                return true;
            }
    
            return false;
        }
    
        private void ManageSwayLimits(float inputY)
        {
            Vector2 limitedVelocity = collisionActuator.LimitVelocity(desiredVelocity * Time.deltaTime);
    
            if (limitedVelocity != desiredVelocity * Time.deltaTime)
            {
                if (collisionActuator.Collisions.above)
                {
                    velocityMagnitude = 0f;
                }

                if (collisionActuator.Collisions.right && swayDirection > 0f)
                {
                    velocityMagnitude = 0f;

                    if (grappleHookPoint.x < playerPosition.x)
                    {
                        swayDirection = -swayDirection;
                    }
                    else
                    {
                        if (Mathf.Abs(inputY) < COLLISION_GRAPPLE_LENGTH_MODIFIER_TOLERANCE)
                        {
                            desiredVelocity = Vector2.zero;
                        }
                    }
                }
                else if (collisionActuator.Collisions.left && swayDirection < 0f)
                {
                    velocityMagnitude = 0f;

                    if (grappleHookPoint.x > playerPosition.x)
                    {
                        swayDirection = -swayDirection;

                    }
                    else
                    {
                        if (Mathf.Abs(inputY) < COLLISION_GRAPPLE_LENGTH_MODIFIER_TOLERANCE)
                        {
                            desiredVelocity = Vector2.zero;
                        }
                    }
                }

                if (collisionActuator.Collisions.grounded)
                {
                    isLocked = false;
                }
            }
        }
    }
}
