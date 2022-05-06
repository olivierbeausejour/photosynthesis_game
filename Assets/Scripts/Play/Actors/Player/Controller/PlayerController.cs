//Authors:
//Jonathan Mathieu
//Olivier Beauséjour
//Charles Tremblay
//Derek Pouliot
//Anthony Dodier

using System.Collections;
using System.Collections.Generic;
using Harmony;
using UnityEngine;
 
namespace Game
{
    [RequireComponent(typeof(CollisionActuator))]
    [RequireComponent(typeof(PlayerAnimationController))]
    [RequireComponent(typeof(PlayerInputManager))]
    [RequireComponent(typeof(DashActuator))]
    [RequireComponent(typeof(GrapplingHookController))]
    [RequireComponent(typeof(SwayActuator))]
    [RequireComponent(typeof(MovementActuator))]
    public class PlayerController : MonoBehaviour, IDashable, IPullable
    {
        [Header("Sounds")]
        [SerializeField] private SoundEnum deathSound;
        [SerializeField] private SoundEnum dashSound;
        [SerializeField] private SoundEnum jumpSound;
        [SerializeField] private SoundEnum landSound;
        [SerializeField] private SoundEnum grapplingHookSwaySound;
        [SerializeField] private AudioSource deathSoundAudioSource;
        [SerializeField] private AudioSource dashSoundAudioSource;
        [SerializeField] private AudioSource jumpSoundAudioSource;
        [SerializeField] private AudioSource landSoundAudioSource;
        [SerializeField] private AudioSource grapplingHookSwaySoundAudioSource;

        [Header("Other settings")] 
        [SerializeField] private GameObject crosshair;
        [SerializeField] private bool showDebugRay = true;
        [SerializeField] private float timeBeforeRespawn = 0.35f;
        [SerializeField] private float velocityForSwaySound;
        [SerializeField] private float offsetForSwaySound;
        [SerializeField] private float bounceBackForceFromBoss = 30f;
        [SerializeField] private SpriteRenderer firingMechanismSprite;

        [Header("Dash parameters")] [SerializeField]
        private int maximumDashCount = 1;

        [Header("Grappling hook parameters")] [SerializeField]
        private float maximumGrappleCount = 2;
        
        // Common
        private GameController gameController;
        private PlayerInputManager inputManager;
        private CollisionActuator collisionActuator;
        private DashActuator dashActuator;
        private BoxCollider2D hurtboxCollider;
        private GrapplingHookController grapplingHookController;
        private SwayActuator swayActuator;
        private BaseActuator baseActuator;
        private Rope rope;

        private PlayerDeathEventChannel playerDeathEventChannel;
        private PlayerRespawnEventChannel playerRespawnEventChannel;
        private PlayerDashEventChannel playerDashEventChannel;
        private PlayerDeployingGrappleEventChannel playerDeployingGrappleEventChannel;
        private LevelCompleteEventChannel levelCompleteEventChannel;
        private PlatformHasCrushedPlayerEventChannel platformHasCrushedPlayerEventChannel;
        private DashOnBossWeakSpotEventChannel dashOnBossWeakSpotEventChannel;
        
        private MovementActuator movementActuator;

        private SpriteRenderer playerSpriteRenderer;
        private Transform grapplingHookDeploymentPoint;

        private DashDestroyer dashDestroyerSensor;
        private PullDestroyer pullDestroyerSensor;
        
        private AudioManager audioManager;

        public BaseActuator BaseActuator => baseActuator;
        
        public bool isPlayerAlive;
        public bool IsPlayerAlive => isPlayerAlive;

        private BoxCollider2D physicalCollider;
        public BoxCollider2D PhysicalCollider => physicalCollider;

        public Vector3 PlayerPosition => transform.position;

        private Vector2 LastDirection;

        //Inventory
        private List<Collectible> inventory;

        // Movement
        private float gravity;

        // Dash
        private float currentDashCount;
        private Vector2 LastFreeDashDirection;

        // Grapple
        private float currentGrappleCount;
        public Vector2 GrapplingHookFiringPosition => grapplingHookDeploymentPoint.transform.position;

        // Sway
        private bool isSwaying;
        public bool IsSwaying => isSwaying;

        public bool IsLevelFinished => isLevelFinished;
        private bool isLevelFinished;

        public GrapplingHookController GrapplingHookController => grapplingHookController;

        // Platforms
        private bool isOnMovingVerticalPlatform;


        private bool wasGroundedLastFrame;

        public IReadOnlyList<Collectible> Inventory => inventory;
        
        private void Awake()
        {
            //Components
            inputManager = GetComponent<PlayerInputManager>();
            collisionActuator = GetComponent<CollisionActuator>();
            dashActuator = GetComponent<DashActuator>();
            hurtboxCollider = transform.Find(R.S.GameObject.HurtBox).GetComponent<BoxCollider2D>();
            physicalCollider = transform.Find(R.S.GameObject.Collider).GetComponent<BoxCollider2D>();
            grapplingHookController = GetComponent<GrapplingHookController>();
            swayActuator = GetComponent<SwayActuator>();
            baseActuator = GetComponent<BaseActuator>();
            movementActuator = GetComponent<MovementActuator>();
            rope = GetComponentInChildren<Rope>();
            dashDestroyerSensor = GetComponentInChildren<DashDestroyer>();
            pullDestroyerSensor = GetComponentInChildren<PullDestroyer>();
            
            audioManager = FindObjectOfType<AudioManager>();

            //Sounds setup
            deathSoundAudioSource.clip = audioManager.GetAudioClip(deathSound);
            dashSoundAudioSource.clip = audioManager.GetAudioClip(dashSound);
            jumpSoundAudioSource.clip = audioManager.GetAudioClip(jumpSound);
            landSoundAudioSource.clip = audioManager.GetAudioClip(landSound);
            grapplingHookSwaySoundAudioSource.clip = audioManager.GetAudioClip(grapplingHookSwaySound);

            //Event channels setup
            playerDeathEventChannel = Finder.PlayerDeathEventChannel;
            playerRespawnEventChannel = Finder.PlayerRespawnEventChannel;
            playerDashEventChannel = Finder.PlayerDashEventChannel;
            playerDeployingGrappleEventChannel = Finder.PlayerDeployingGrappleEventChannel;
            levelCompleteEventChannel = Finder.LevelCompleteEventChannel;
            platformHasCrushedPlayerEventChannel = Finder.PlatformHasCrushedPlayerEventChannel;
            dashOnBossWeakSpotEventChannel = Finder.DashOnBossWeakSpotEventChannel;
            
            gameController = Finder.GameController;
            
            grapplingHookDeploymentPoint = GameObject.Find(R.S.GameObject.GrapplingHookDeploymentPoint).transform;
            
            inventory = new List<Collectible>();

            isLevelFinished = false;
            SetHurtboxActive(false);
            
            LastDirection = Vector2.zero;
            LastFreeDashDirection = Vector2.right;
        }

        private void Start()
        {
            isPlayerAlive = true;
            
            // Dash
            currentDashCount = 0;

            // Grappling Hook
            currentGrappleCount = 0;

            if (PlayerSaver.CheckIfFileExists(GameController.SaveGameId))
            {
                FetchLoadedData();
                Respawn();
            }
        }

        private void OnEnable()
        {
            levelCompleteEventChannel.OnLevelComplete += PlayEndAnimation;
            platformHasCrushedPlayerEventChannel.OnPlayerCrushedByPlatform += Kill;
            dashOnBossWeakSpotEventChannel.OnDashOnBossWeakSpot += BounceBackFromBoss;
        }

        private void OnDisable()
        {
            levelCompleteEventChannel.OnLevelComplete -= PlayEndAnimation;
            platformHasCrushedPlayerEventChannel.OnPlayerCrushedByPlatform -= Kill;
            dashOnBossWeakSpotEventChannel.OnDashOnBossWeakSpot -= BounceBackFromBoss;
        }

        private void PlayEndAnimation(string levelCompleteName, int nbDeathsOnCurrentLevel)
        {
            // Empty on purpose.
        }

        private void Update()
        {
            if(!isPlayerAlive || isLevelFinished)
                return;
            
            // GrapplingHook
            ManageGrapplingHook();
            
            // Velocity
            ManageVelocity();
        }

        /* ----------- Velocity ---------------*/
        private void ManageVelocity()
        {
            ManageDash();
            ManageSway();
            ManageBase();
        }

        /* ----------- Base ----------- */
        private void ManageBase()
        {
            if(grapplingHookController.Information.isGrappleHookedToSurface || dashActuator.IsDashing)
                return;
            
            if(inputManager.JumpKeyDown && baseActuator.Grounded)
                jumpSoundAudioSource.Play();
            
            if(baseActuator.Grounded && !wasGroundedLastFrame)
                landSoundAudioSource.Play();

            wasGroundedLastFrame = baseActuator.Grounded;
            
            baseActuator.ManageBase(inputManager.MovementAxisInput, inputManager.JumpKeyDown);
            if(inputManager.MovementAxisInput != Vector2.zero)
                LastDirection = inputManager.MovementAxisInput;
        }

        /* -----------  Dash ---------------*/
        private void ManageDash()
        {
            if (grapplingHookController.Information.isGrappleHookedToSurface || grapplingHookController.Information.isGrappleHookedToEntity)
                return;

            if (collisionActuator.Collisions.grounded) currentDashCount = 0;

            if (baseActuator.HasBounced)
            {
                currentDashCount = 0;
                baseActuator.HasBounced = false;
            }

            if (inputManager.DashKeyDown && currentDashCount < maximumDashCount &&
                !dashActuator.IsDashing)
            {
                Dash();
            }
        }

        private void Dash()
        {
            Vector2 dashDirection;
            baseActuator.Grounded = false;
            dashSoundAudioSource.Play();
            currentDashCount++;
            if(gameController.CurrentPlayerData != null) gameController.CurrentPlayerData.NbTotalDashes++;
            playerDashEventChannel.NotifyPlayerDash();
            
            dashDirection = inputManager.DashCardinalDirection == Vector2.zero ? LastDirection : inputManager.DashCardinalDirection;
            if(inputManager.DashFreeDirection != Vector2.zero)
                LastFreeDashDirection = inputManager.DashFreeDirection;


            StartCoroutine(dashActuator.DashCoroutine(inputManager.IsDashMouseDirection ? LastFreeDashDirection : dashDirection, baseActuator.Gravity));
            baseActuator.Velocity = Vector2.zero;
        }

        /* ----------- Grappling Hook ---------------*/
        private void ManageGrapplingHook()
        {
            if (dashActuator.IsDashing && grapplingHookController.Information.isGrappleActive)
                return;

            if (collisionActuator.Collisions.grounded) currentGrappleCount = 0;

            if (inputManager.FireKeyDown && currentGrappleCount < maximumGrappleCount)
            {
                currentGrappleCount++;
                grapplingHookController.FireGrapplingHook(
                    (crosshair.transform.position - grapplingHookDeploymentPoint.position).normalized);
                if(gameController.CurrentPlayerData != null) gameController.CurrentPlayerData.NbTotalGrapplingHookDeployments++;
                playerDeployingGrappleEventChannel.NotifyPlayerDeployingGrapple();
            }
        }

        public void InitializeSway(Vector2 hitPoint, float grappleLength)
        {
            swayActuator.InitialiseSway(inputManager.MovementAxisInput, hitPoint, grappleLength);
        }

        /* ----------- Sway ---------------*/
        private void ManageSway()
        {
            isSwaying = false;
            
            if (!grapplingHookController.Information.isGrappleHookedToSurface || dashActuator.IsDashing)
                return;

            currentDashCount = 0;

            if (!inputManager.FireKeyUp)
            {
                isSwaying = true;
                swayActuator.ManageSway(inputManager.MovementAxisInput, grapplingHookDeploymentPoint.position);
                rope.SetRopeLength(swayActuator.GrappleLength);
                if (transform.position.x <= grapplingHookController.Information.hookPosition.x + offsetForSwaySound &&
                    transform.position.x >= grapplingHookController.Information.hookPosition.x - offsetForSwaySound &&
                    (movementActuator.CurrentVelocity.x > velocityForSwaySound ||
                     movementActuator.CurrentVelocity.x < -velocityForSwaySound))
                {
                    grapplingHookSwaySoundAudioSource.Play();
                }
            }
            else
            {
                isSwaying = false;
                grapplingHookController.StopGrappleSway();
            }
        }

        public void Kill()
        {
            if (!isPlayerAlive) return;

            firingMechanismSprite.enabled = false;
            deathSoundAudioSource.Play();
            isPlayerAlive = false;
            if(gameController.CurrentPlayerData != null) gameController.CurrentPlayerData.NbTotalDeaths++;
            movementActuator.CurrentVelocity = Vector2.zero;
            rope.SetActive(false);
            isSwaying = false;
            playerDeathEventChannel.NotifyPlayerDeath();
            StartCoroutine(WaitBeforeRespawn());
            dashDestroyerSensor.ResetSensor();
            pullDestroyerSensor.ResetSensor();
        }

        private IEnumerator WaitBeforeRespawn()
        {
            while (isActiveAndEnabled)
            {
                yield return new WaitForSeconds(timeBeforeRespawn);
                Respawn();
                yield break;
            }
        }

        public bool IsDashing()
        {
            return dashActuator.IsDashing;
        }

        private void MoveToCurrentCheckpoint()
        {
            transform.position = gameController.CurrentCheckpoint.transform.position;
        }

        private void Respawn()
        {
            firingMechanismSprite.enabled = true;
            isPlayerAlive = true;
            if (gameController.CurrentCheckpoint != null)
                MoveToCurrentCheckpoint();
            grapplingHookController.ResetGrapplingHook();
            transform.parent = null;
            playerRespawnEventChannel.NotifyPlayerRespawn();
        }

        // Author : Derek Pouliot
        private void FetchLoadedData()
        {
            PlayerData savedPlayerData = gameController.CurrentPlayerData;

            GameObject[] allCheckpoints = GameObject.FindGameObjectsWithTag(R.S.Tag.Checkpoint);

            foreach (var checkpoint in allCheckpoints)
            {
                Checkpoint fetchedCheckpoint = checkpoint.GetComponentInChildren<Checkpoint>();
                if (savedPlayerData != null && fetchedCheckpoint.CheckpointId == savedPlayerData.LastCheckpointEncounteredId)
                {
                    gameController.CurrentCheckpoint = fetchedCheckpoint;
                    break;
                }
            }
        }

        public bool IsPulling()
        {
            return grapplingHookController.Information.isGrapplePulling;
        }

        public void PickUpCollectible(Collectible collectible)
        {
            if (!inventory.Contains(collectible)) inventory.Add(collectible);
        }

        public void SetHurtboxActive(bool value)
        {
            hurtboxCollider.gameObject.SetActive(value);
        }

        public void DeleteKey(Collectible key)
        {
            if (inventory.Contains(key))
            {
                inventory.Remove(key);
            }
        }

        // Author : Derek Pouliot
        private void BounceBackFromBoss()
        {
            baseActuator.ManageExternalForce(-movementActuator.CurrentVelocity * bounceBackForceFromBoss);
        }
    }
}