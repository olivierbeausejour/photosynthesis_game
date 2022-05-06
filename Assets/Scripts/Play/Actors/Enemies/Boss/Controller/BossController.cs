// Author : Derek Pouliot

using System;
using System.Collections;
using Harmony;
using JetBrains.Annotations;
using UnityEngine;

namespace Game
{
    public class BossController : MonoBehaviour
    {
        private const float POSITION_COMPARISON_TOLERANCE = 0.5f;
        private const float TOTAL_SECONDS_BEFORE_DESTROY = 2;

        [SerializeField] private float fallingSpeed = 0.1f;
        [SerializeField] private int initialLifePoints = 5;
        
        [Header("Fading Properties")]
        [SerializeField] private float fadingSpeed = 10f;
        [SerializeField] private int maxFadingOpacity = 1;
        [SerializeField] private int minFadingOpacity = 0;
        
        [Header("Sounds")]
        [SerializeField] private SoundEnum bossHurtSound;
        [SerializeField] private AudioSource bossHurtSoundAudioSource;
        
        [SerializeField] private SoundEnum bossDeathSound;
        [SerializeField] private AudioSource bossDeathSoundAudioSource;
        
        [SerializeField] private SoundEnum bossReloadSound;
        [SerializeField] private AudioSource bossReloadSoundAudioSource;

        private TurretController bossTurret;
        private BaseActuator bossBaseBehaviour;
        private JumpActuator bossJumpActuator;
        private CollisionActuator bossCollisionBehaviour;
        private Hazard bossHazard;
        private BossAnimationActuator bossAnimationActuator;
        private MovementActuator bossMovementActuator;
        private AudioManager audioManager;
            
        private GameObject[] armorParts;
        private GameObject currentArmorPartToPull;
        private GameObject[] weakBodyParts;
        private GameObject[] bodyStimulis;
        private GameObject currentVulnerableBodyPart;
        
        private BossStateMachine bossStateMachine;
        
        private int currentLifePoints;

        private GameObject player;
        private BossLoseArmorPartEventChannel bossLoseArmorPartEventChannel;
        private DashOnBossWeakSpotEventChannel dashOnBossWeakSpotEventChannel;
        private PlayerDeathEventChannel playerDeathEventChannel;
        private BossIsDeadEventChannel bossIsDeadEventChannel;
        private BossHasBeenActivatedEventChannel bossHasBeenActivatedEventChannel;

        private bool isBerserk;
        private bool isFalling;
        private bool isResting;
        private bool isActivate;
        private bool isReloading;
        private bool isDead;
        
        private int numberPartsToPulled;
        private int nbPartsToDestroy;

        private bool isFlicking;
        private Vector2 initialPosition;

        public bool HasRespawned { get; set; }
        
        public BossAnimationActuator BossAnimationActuator => bossAnimationActuator;
        public Color InitialColor { get; private set; }

        public bool IsDead
        {
            get => currentLifePoints <= 0 || isDead;
            private set => isDead = value;
        }

        public bool IsCritical => currentLifePoints == 1;

        public bool IsBerserk
        {
            get => isBerserk;
            set => isBerserk = value;
        }

        public bool IsReloading
        {
            get => isReloading;
            
            set
            {
                isReloading = value;
                
                if (value) bossReloadSoundAudioSource.Play();
                else bossReloadSoundAudioSource.Stop();
            }
        }

        public bool IsActivate 
        {
            get => isActivate;
            set => isActivate = value;
        }

        public bool IsFalling
        {
            get => isFalling;
            set => isFalling = value;
        }
        
        public bool IsResting
        {
            get => isResting;
            set => isResting = value;
        }

        public bool IsMovingLeft => bossMovementActuator.CurrentVelocity.x < 0f || player.transform.position.x < transform.position.x;

        public bool WeakSpotHasBeenHit { get; set; }

        public GameObject CurrentArmorPartToPull => currentArmorPartToPull;
        public GameObject CurrentVulnerableBodyPart => currentVulnerableBodyPart;

        private void Awake()
        {
            bossTurret = transform.Find(R.S.GameObject.Turret).GetComponent<TurretController>();
            bossBaseBehaviour = GetComponent<BaseActuator>();
            bossJumpActuator = GetComponent<JumpActuator>();
            bossCollisionBehaviour = GetComponent<CollisionActuator>();
            bossHazard = GetComponentInChildren<Hazard>();
            bossAnimationActuator = GetComponent<BossAnimationActuator>();
            bossMovementActuator = GetComponent<MovementActuator>();

            player = GameObject.FindGameObjectWithTag(R.S.Tag.Player); // We all agreed that only the player has the tag "Player".
            bossLoseArmorPartEventChannel = Finder.BossLoseArmorPartEventChannel;
            dashOnBossWeakSpotEventChannel = Finder.DashOnBossWeakSpotEventChannel;
            playerDeathEventChannel = Finder.PlayerDeathEventChannel;
            bossIsDeadEventChannel = Finder.BossIsDeadEventChannel;
            bossHasBeenActivatedEventChannel = Finder.BossHasBeenActivatedEventChannel;
            audioManager = Finder.AudioManager;

            bossHurtSoundAudioSource.clip = audioManager.GetAudioClip(bossHurtSound);
            bossDeathSoundAudioSource.clip = audioManager.GetAudioClip(bossDeathSound);
            bossReloadSoundAudioSource.clip = audioManager.GetAudioClip(bossReloadSound);

            DisableShooting();

            currentLifePoints = initialLifePoints;   
            bossStateMachine = new BossStateMachine(new WaitingState(this));

            numberPartsToPulled = initialLifePoints;
            
            isFalling = false;
            isResting = false;
            isActivate = false;
            HasRespawned = false;
            
            InitArmorParts();
            InitWeakBodyParts();

            initialPosition = transform.position;
            isFlicking = false;

            InitialColor = GetComponent<SpriteRenderer>().color;
        }

        private void Start()
        {
            bossAnimationActuator.LinkToStateMachine(bossStateMachine);
        }

        private void HideArmorParts()
        {
            foreach (var armorPart in armorParts)
            {
                armorPart.SetActive(false);
            }
        }

        private void InitArmorParts()
        {
            var bossArmor = transform.Find(R.S.GameObject.Armor).gameObject;
            armorParts = bossArmor.Children();

            foreach (var armorPart in armorParts)
            {
                armorPart.SetActive(true);
            }
            
            currentArmorPartToPull = armorParts[numberPartsToPulled - 1];
        }

        public void UpdateArmorParts()
        {
            if (!IsCritical)
                currentArmorPartToPull = armorParts[(--numberPartsToPulled) - 1];
        }

        public void ShowBodyPartToDestroy()
        {
            currentVulnerableBodyPart.GetComponent<SpriteRenderer>().enabled = true;
        }

        private void WeakenAttackedBodyPart()
        {
            weakBodyParts[numberPartsToPulled].SetActive(false);
            bodyStimulis[numberPartsToPulled].GetComponent<DashDestroyable>().enabled = false;
        }
        
        public void UpdateBodyPartToDestroy()
        {
            currentVulnerableBodyPart = weakBodyParts[numberPartsToPulled - 1];
            bodyStimulis[numberPartsToPulled - 1].GetComponent<DashDestroyable>().enabled = true;
        }

        private void InitWeakBodyParts()
        {
            var bossBodyStimulis = transform.Find(R.S.GameObject.Stimulis).gameObject;
            bodyStimulis = bossBodyStimulis.Children();

            weakBodyParts = transform.Find(R.S.GameObject.Visuals).Find(R.S.GameObject.Body).Children();
            
            foreach (var weakBodyPart in weakBodyParts)
            {
                weakBodyPart.SetActive(true);
            }

            foreach (var bodyPartStimuli in bodyStimulis)
            {
                bodyPartStimuli.GetComponent<DashDestroyable>().enabled = false;
                bodyPartStimuli.GetComponentInChildren<Stimuli>().enabled = false;
            }
            
            HideWeakBodyParts();
            currentVulnerableBodyPart = null;
        }

        private void HideWeakBodyParts()
        {
            foreach (var weakBodyPart in weakBodyParts)
            {
                weakBodyPart.GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        private void OnEnable()
        {
            bossLoseArmorPartEventChannel.OnBossLoseArmorPart += BecomeBerserk;
            dashOnBossWeakSpotEventChannel.OnDashOnBossWeakSpot += Weaken;
            playerDeathEventChannel.OnPlayerDeath += Respawn;
            bossHasBeenActivatedEventChannel.OnBossActivated += StartToAttack;
        }

        private void OnDisable()
        {
            bossLoseArmorPartEventChannel.OnBossLoseArmorPart -= BecomeBerserk;
            dashOnBossWeakSpotEventChannel.OnDashOnBossWeakSpot -= Weaken;
            playerDeathEventChannel.OnPlayerDeath -= Respawn;
            bossHasBeenActivatedEventChannel.OnBossActivated -= StartToAttack;
        }

        private void Update()
        {
            bossStateMachine.Update();
        }

        private void StartToAttack()
        {
            isActivate = true;
        }

        private void Respawn()
        {
            if (isActivate)
            {
                numberPartsToPulled = initialLifePoints;
                currentLifePoints = initialLifePoints;
                bossAnimationActuator.ResetAnimationSpeed();

                ResetPosition();
                InitArmorParts();
                InitWeakBodyParts();
                ResetBehaviour();
                ResetProjectiles();

                ActivateDashKilling();
                EnableShooting();
                StopAllCoroutines();

                HasRespawned = true;
            }
        }

        private void ResetProjectiles()
        {
            var bossProjectiles = FindObjectsOfType<ProjectileController>();

            foreach (var projectile in bossProjectiles)
            {
                projectile.gameObject.SetActive(false);
            }
        }

        private void ResetBehaviour()
        {
            isBerserk = false;
            isFalling = false;
            IsReloading = false;
            isResting = false;
        }

        private void ResetPosition()
        {
            transform.position = initialPosition;
        }

        public void EnableShooting()
        {
            bossTurret.CanShoot = true;
        }

        public void DisableShooting()
        {
            bossTurret.CanShoot = false;
        }

        private void BecomeBerserk()
        {
            isBerserk = true;
        }

        private void LoseLifePoints()
        {
            currentLifePoints--;
        }
        
        private void Weaken()
        {
            LoseLifePoints();
            WeakenAttackedBodyPart();
            WeakSpotHasBeenHit = true;
            
            bossHurtSoundAudioSource.Play();
        }

        public void MoveTowardsPlayer(bool jumpWhileMoving)
        {
            var bossVelocity = (player.transform.position - transform.position).normalized;
            bossBaseBehaviour.ManageBase(bossVelocity, jumpWhileMoving);
        }

        public void InitJump()
        {
            var playerPosition = player.transform.position;

            bossJumpActuator.InitJump();
            bossJumpActuator.InitJumpDirection(playerPosition);

            isResting = false;
        }

        public void ManageJump()
        {
            bossJumpActuator.CheckIfHasReachedMaxHeight();
            
            if (bossJumpActuator.IsJumping)
            {
                bossJumpActuator.JumpTowardsPlayer();
            }

            if (IsBeingOverPlayer())
            {
                bossAnimationActuator.IncreaseAnimationSpeed();
                isFalling = true;
                bossJumpActuator.StopJump();
            }

            if (bossJumpActuator.HasCompletedJump)
                RestartJump();
            
            if (bossCollisionBehaviour.Collisions.left || bossCollisionBehaviour.Collisions.right)
                Fall();
        }

        private void RestartJump()
        {
            bossAnimationActuator.ResetAnimationSpeed();
            bossJumpActuator.StopJump();
            isResting = true;
            StartCoroutine(WaitBeforeNewJump());
        }

        private IEnumerator WaitBeforeNewJump()
        {
            yield return new WaitForSeconds(bossJumpActuator.TotalSecondsBetweenJumps);
            InitJump();
        }

        private bool IsBeingOverPlayer()
        {
            return Math.Abs(player.transform.position.x - transform.position.x) < POSITION_COMPARISON_TOLERANCE;
        }

        public void Fall()
        {
            bossBaseBehaviour.ManageBase(Vector2.down * fallingSpeed, false);
            
            if (bossCollisionBehaviour.Collisions.grounded)
            {
                isFalling = false;
                RestartJump();
            }
        }

        public void DeactivateDashKilling()
        {
            bossHazard.KillDashingTargets = false;
        }
        
        public void ActivateDashKilling()
        {
            bossHazard.KillDashingTargets = true;
        }

        public void Kill()
        {
            IsDead = true;
            HideWeakBodyParts();
            HideArmorParts();
            ResetProjectiles();
            bossDeathSoundAudioSource.Play();
            
            bossIsDeadEventChannel.NotifyBossIsDead();
            StartCoroutine(WaitBeforeDestroy());
        }

        private IEnumerator WaitBeforeDestroy()
        {
            yield return new WaitForSeconds(TOTAL_SECONDS_BEFORE_DESTROY);
            Destroy(gameObject);
        }

        public void FlickBossPart(SpriteRenderer bossPartRenderer)
        {
            CheckIfFlicking(bossPartRenderer);
            Flick(bossPartRenderer);
        }

        public void ResetBossPartOpacity(SpriteRenderer bossPartRenderer)
        {
            var armorPartToPullColor = bossPartRenderer.color;
            armorPartToPullColor.a = maxFadingOpacity;
            bossPartRenderer.color = armorPartToPullColor;
        }
        
        private void CheckIfFlicking(SpriteRenderer bossPartRenderer)
        {
            if (bossPartRenderer.color.a >= maxFadingOpacity)
                isFlicking = true;
            else if (bossPartRenderer.color.a <= minFadingOpacity)
                isFlicking = false;
        }

        private void Flick(SpriteRenderer bossPartRenderer)
        {
            var armorPartToPullColor = bossPartRenderer.color;

            if (isFlicking)
                armorPartToPullColor.a -= Time.deltaTime * fadingSpeed;
            else
                armorPartToPullColor.a += Time.deltaTime * fadingSpeed;

            bossPartRenderer.color = armorPartToPullColor;
        } 
    }
}