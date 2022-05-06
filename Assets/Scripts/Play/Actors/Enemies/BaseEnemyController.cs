﻿//Authors:
//Anthony Dodier
//Charles Tremblay

using Game;
using Harmony;
using UnityEngine;

public class BaseEnemyController : MonoBehaviour
{
    [Header("Movement parameters")] 
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float accelerationAirborne = 0.1f;
    [SerializeField] private float accelerationGrounded = 0.05f;
    [SerializeField] private float accelerationDampingClamp = 0.001f;
    [SerializeField] protected float jumpHeight = 2f;
    [SerializeField] protected float timeToJumpApex = 0.2f;
    [SerializeField] private float minimumDynamicRigidbodySpeed = 3f;
    
    [Header("Sounds")] 
    [SerializeField] protected SoundEnum deathSound;
    [SerializeField] protected SoundEnum idleSound;
    [SerializeField] protected AudioSource deathSoundAudioSource;
    [SerializeField] protected AudioSource idleSoundAudioSource;
    [SerializeField] protected int idleSoundFrequency;
    
    protected CollisionActuator collisionActuator;
    protected MovementActuator movementActuator;
    private Rigidbody2D rigidbody2D;
    private EnemyIsDoneBeingPulledEventChannel enemyIsDoneBeingPulledEventChannel;
    private PlayerRespawnEventChannel playerRespawnEventChannel;
    private Vector3 initialPosition;
    private SpriteRenderer[] spriteRenderers;
    private Collider2D[] colliders2D;
    private Hazard hazardSensor;
    private ParticleSystem particleSystem;

    // Common
    protected Vector2 velocity;
    public Vector2 currentDirection;
    private Vector2 startDirection;
    private float nbUpdates = 0f;

    // Movement
    private float velocitySmoothingX;
    private float horizontalVelocity;
    protected float gravity;
    protected bool isEnemyActive;
    
    //Sounds
    protected AudioManager audioManager;

    private bool isHooked = false;
    public bool isPulled = false;

    public bool IsHooked
    {
        get => isHooked;
        set => isHooked = value;
    }

    public bool IsPulled
    {
        get => isPulled;
        set => isPulled = value;
    }

    public bool IsEnemyActive => isEnemyActive;

    protected virtual void Awake()
    {
        collisionActuator = GetComponent<CollisionActuator>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        colliders2D = GetComponentsInChildren<Collider2D>();
        hazardSensor = GetComponentInChildren<Hazard>();
        particleSystem = GetComponentInChildren<ParticleSystem>();
        audioManager = Finder.AudioManager;
        
        deathSoundAudioSource.clip = audioManager.GetAudioClip(deathSound);
        idleSoundAudioSource.clip = audioManager.GetAudioClip(idleSound);

        enemyIsDoneBeingPulledEventChannel = Finder.EnemyIsDoneBeingPulledEventChannel;
        playerRespawnEventChannel = Finder.PlayerRespawnEventChannel;
        
        initialPosition = transform.position;
        startDirection = Vector2.right;
        currentDirection = startDirection;
        isEnemyActive = true;
    }

    protected virtual void OnEnable()
    {
        playerRespawnEventChannel.OnPlayerRespawn += ResetEnemy;
    }
    protected virtual void OnDisable()
    {
        playerRespawnEventChannel.OnPlayerRespawn -= ResetEnemy;
    }

    protected virtual void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        collisionActuator = GetComponent<CollisionActuator>();
        movementActuator = GetComponent<MovementActuator>();
        gravity = -(2f * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
    }
    
    protected virtual void Update()
    {
        if (!isEnemyActive) return;
        if (isHooked) return;
        
        if (rigidbody2D.velocity.magnitude <= minimumDynamicRigidbodySpeed)
        {
            rigidbody2D.velocity = Vector2.zero;
            rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

            if (isPulled)
            {
                isPulled = false;
                enemyIsDoneBeingPulledEventChannel.NotifyEnemyDoneBeingPulled();
            }
        }
        
        ManageHorizontalMovement();
        ManageVerticalMovement();
        movementActuator.Move(velocity);
        
        var mustPlayIdleSound = nbUpdates * Time.deltaTime > 0 && nbUpdates % idleSoundFrequency == 0;
        if (mustPlayIdleSound) idleSoundAudioSource.Play();
        
        nbUpdates++;

    }

    protected virtual void ManageVerticalMovement()
    {
        if (collisionActuator.Collisions.below || collisionActuator.Collisions.above) velocity.y = 0f;
        velocity.y += gravity * Time.deltaTime;
    }

    private void ManageHorizontalMovement()
    {
        ManageCollision();

        float targetVelocityX = currentDirection.x * movementSpeed;
        horizontalVelocity = Mathf.SmoothDamp(horizontalVelocity, targetVelocityX, ref velocitySmoothingX,
            (collisionActuator.Collisions.grounded) ? accelerationGrounded : accelerationAirborne);
        if (horizontalVelocity > -accelerationDampingClamp && horizontalVelocity < accelerationDampingClamp)
            horizontalVelocity = 0f;

        velocity.x = horizontalVelocity;
    }

    protected virtual void ManageCollision()
    {
        if (collisionActuator.Collisions.left)
        {
            currentDirection = Vector2.right;
        }
        else if (collisionActuator.Collisions.right)
        {
            currentDirection = Vector2.left;
        }
        else if (collisionActuator.Collisions.belowRight)
        {
            currentDirection = Vector2.left;
        }
        else if (collisionActuator.Collisions.belowLeft)
        {
            currentDirection = Vector2.right;
        }
    }

    protected virtual void ResetEnemy()
    {
        hazardSensor.ResetSensor();
        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.enabled = true;
        }
        foreach (var collider in colliders2D)
        {
            collider.enabled = true;
        }
        transform.position = initialPosition;
        currentDirection = startDirection;
        isEnemyActive = true;
    }

    public void DisableEnemy()
    {
        deathSoundAudioSource.Play();
        particleSystem.Play();
        
        isEnemyActive = false;
        hazardSensor.ResetSensor();
        foreach (var collider in colliders2D)
        {
            collider.enabled = false;
        }
        rigidbody2D.bodyType = RigidbodyType2D.Static;
    }

    private void DisableSprite()
    {
        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.enabled = false;
        }
    }
}