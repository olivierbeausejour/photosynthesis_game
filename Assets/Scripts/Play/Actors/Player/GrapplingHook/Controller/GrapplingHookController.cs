//Authors: 
//Jonathan Mathieu
//Charles Tremblay

using System;
using System.Collections;
using Harmony;
using UnityEngine;

namespace Game
{
    public class GrapplingHookController : MonoBehaviour
    {
        [Header("Firing Hook Parameters")]
        [SerializeField] private float timeToReachMaximumGrappleLength = 0.3f;
        [SerializeField] private float maximumGrappleLength = 10f;

        [Header("Rope parameters")]
        [SerializeField] private Rope rope;
        [SerializeField] private GameObject ropeEndPoint;

        [Header("Sounds")] 
        [SerializeField]
        
        private SoundEnum grapplingHookFiringSound;
        [SerializeField] private SoundEnum grapplingHookImpactSound;
        [SerializeField] private SoundEnum grapplingHookRetractSound;
        [SerializeField] private AudioSource grapplingHookFiringSoundAudioSource;
        [SerializeField] private AudioSource grapplingHookImpactSoundAudioSource;
        [SerializeField] private AudioSource grapplingHookRetractSoundAudioSource;

        [Header("Others")]
        [SerializeField] private LayerMask ropeLayerMask;
        [SerializeField] private bool showDebugLines;
        
        private AudioManager audioManager;
        private PlayerInputManager inputManager;
        private PlayerController playerController;        
        private HookedToHookableEntityActuator hookedToHookableEntityBehaviour;
        private EnemyIsDoneBeingPulledEventChannel enemyIsDoneBeingPulledEventChannel;
        private GrapplingHookInformation information;

        private bool isGrappleRetracting;
        private Vector2 hookLocationOnEntity;

        public GrapplingHookInformation Information => information;
        public float MaximumGrappleLength => maximumGrappleLength;

        private void Awake()
        {
            audioManager = FindObjectOfType<AudioManager>();
            inputManager = GetComponent<PlayerInputManager>();
            playerController = GetComponent<PlayerController>();
            hookedToHookableEntityBehaviour = GetComponent<HookedToHookableEntityActuator>();
            
            enemyIsDoneBeingPulledEventChannel = Finder.EnemyIsDoneBeingPulledEventChannel;
            
            grapplingHookFiringSoundAudioSource.clip = audioManager.GetAudioClip(grapplingHookFiringSound);
            grapplingHookImpactSoundAudioSource.clip = audioManager.GetAudioClip(grapplingHookImpactSound);
            grapplingHookRetractSoundAudioSource.clip = audioManager.GetAudioClip(grapplingHookRetractSound);
        }

        private void OnEnable()
        {
            enemyIsDoneBeingPulledEventChannel.OnEnemyDoneBeingPulled += StartResetRoutine;
        }

        private void OnDisable()
        {
            enemyIsDoneBeingPulledEventChannel.OnEnemyDoneBeingPulled -= StartResetRoutine;
        }

        private void Start()
        {
            information.isGrappleHookedToSurface = false;
            information.isGrappleHookedToEntity = false;
            information.isGrappleActive = false;
            information.isGrapplePulling = false;
            isGrappleRetracting = false;
        }

        private void Update()
        {
            if(!information.isGrappleActive)
                rope.SetRopeLength(information.currentGrappleLength);

            if (information.isGrappleHookedToSurface)
            {
                ropeEndPoint.transform.position = information.hookPosition;
            }
            else if (information.isGrappleHookedToEntity)
            {
                ropeEndPoint.transform.position = (Vector2)information.hookedGameObject.transform.position - hookLocationOnEntity;
                information.hookPosition = ropeEndPoint.transform.position;
            }
            else if(!information.isGrappleActive)
                ropeEndPoint.transform.position = playerController.GrapplingHookFiringPosition;
        }

        public void FireGrapplingHook(Vector2 hookDirection)
        {
            if(information.isGrappleActive) return;

            rope.SetActive(true);
            rope.SetRopeLength(0f);
            information.isGrappleActive = true;
            information.currentGrappleLength = 0f;

            RaycastHit2D ropeHitPoint = Physics2D.Raycast(playerController.GrapplingHookFiringPosition, hookDirection,
                maximumGrappleLength, ropeLayerMask);

            if (ropeHitPoint)
            {
                information.hookPosition = ropeHitPoint.point;
                Debug.DrawLine(transform.position,ropeHitPoint.point,Color.red,10f);
            }
               
            else
                information.hookPosition = playerController.GrapplingHookFiringPosition +
                                           (hookDirection.normalized * maximumGrappleLength);
            Debug.DrawLine(transform.position,information.hookPosition,Color.blue,10f);
            
            StartCoroutine(DeployGrappleCoroutine());
        }

        private IEnumerator DeployGrappleCoroutine()
        {
            grapplingHookFiringSoundAudioSource.Play();
            float timePassed = 0f;

            while (isActiveAndEnabled)
            {
                timePassed += Time.deltaTime;

                information.currentGrappleLength = Mathf.Lerp(0f, maximumGrappleLength,
                    timePassed / timeToReachMaximumGrappleLength);

                ropeEndPoint.transform.position = playerController.GrapplingHookFiringPosition +
                                                   (information.hookPosition -
                                                    playerController.GrapplingHookFiringPosition).normalized *
                                                   information.currentGrappleLength;

                rope.SetRopeLength(information.currentGrappleLength);

                RaycastHit2D grappleHit = CheckForGrappleHit();
                if (grappleHit)
                {
                    grapplingHookImpactSoundAudioSource.Play();
                    ManageGrappleHit(grappleHit);
                    yield break;
                }
                
                if (timePassed >= timeToReachMaximumGrappleLength || inputManager.FireKeyUp)
                {
                    StartCoroutine(RetractGrappleCoroutine());
                    
                    yield break;
                }

                yield return null;
            }
        }

        private IEnumerator RetractGrappleCoroutine()
        {
            grapplingHookRetractSoundAudioSource.Play();
            float timePassed = 0f;
            float grappleLength = information.currentGrappleLength;
            float timeToReachZero = timeToReachMaximumGrappleLength * (information.currentGrappleLength / maximumGrappleLength);

            while (isActiveAndEnabled)
            {
                timePassed += Time.deltaTime;

                information.currentGrappleLength = Mathf.Lerp(grappleLength, 0f, timePassed / timeToReachZero);

                ropeEndPoint.transform.position = playerController.GrapplingHookFiringPosition +
                                                   (information.hookPosition -
                                                    playerController.GrapplingHookFiringPosition).normalized *
                                                   information.currentGrappleLength;

                rope.SetRopeLength(information.currentGrappleLength);
                
                if (timePassed >= timeToReachZero)
                {
                    information.isGrappleActive = false;
                    isGrappleRetracting = false;
                    rope.SetActive(false);
                    
                    yield break;
                }
                isGrappleRetracting = false;

                yield return null;
            }
        }

        private RaycastHit2D CheckForGrappleHit()
        {
#if UNITY_EDITOR
            if (showDebugLines)
            {
                Debug.DrawRay(playerController.GrapplingHookFiringPosition,
                    (information.hookPosition - playerController.GrapplingHookFiringPosition).normalized *
                    information.currentGrappleLength, Color.white);
            }
#endif
            return Physics2D.Raycast(playerController.GrapplingHookFiringPosition, information.hookPosition - playerController.GrapplingHookFiringPosition,
                information.currentGrappleLength, ropeLayerMask);
        }

        private void ManageGrappleHit(RaycastHit2D grappleHit)
        {
            int hitLayer = grappleHit.collider.gameObject.layer;
            information.hookPosition = grappleHit.point;
            
            if (hitLayer == LayerMask.NameToLayer(R.S.Layer.Hookable))
            {
                information.isGrappleHookedToSurface = true;
                playerController.InitializeSway(grappleHit.point, Vector2.Distance(transform.position, grappleHit.point));
                SetHookHitInformation(grappleHit);
            }
            else if (hitLayer == LayerMask.NameToLayer(R.S.Layer.Enemy) ||
                     hitLayer == LayerMask.NameToLayer(R.S.Layer.HookableEntity))
            {
                information.isGrappleHookedToEntity = true;
                hookedToHookableEntityBehaviour.HookToEntity(grappleHit);
                SetHookHitInformation(grappleHit);
                hookLocationOnEntity = (Vector2)information.hookedGameObject.transform.position - grappleHit.point;
            }
            else if (hitLayer == LayerMask.NameToLayer(R.S.Layer.ArmorPiece) &&
                     GameObject.FindWithTag(R.S.Tag.Boss).GetComponent<BossController>().IsReloading &&
                     GameObject.FindWithTag(R.S.Tag.Boss).GetComponent<BossController>().CurrentArmorPartToPull == grappleHit.transform.gameObject) 
            {
                information.isGrappleHookedToEntity = true;
                hookedToHookableEntityBehaviour.HookToEntity(grappleHit);
                SetHookHitInformation(grappleHit);
            }
            else
            {
                RetractGrapple();
            }
        }
        
        public void RetractGrapple()
        {
            if(!information.isGrappleActive && isGrappleRetracting)
                return;
            
            isGrappleRetracting = true;
            information.isGrappleHookedToEntity = false;
            information.isGrappleHookedToSurface = false;
            
            StartCoroutine(RetractGrappleCoroutine());
        }

        private void SetHookHitInformation(RaycastHit2D hit)
        {
            information.hookedGameObject = hit.collider.gameObject;
        }

        private void ResetGrapplingHookAfterPull()
        {
            
            RetractGrapple();
            information.isGrapplePulling = false;
            information.isGrappleHookedToEntity = false;
            information.hookedGameObject = null;
        }

        private void StartResetRoutine()
        {
            StartCoroutine(WaitBeforeReset());
        }

        private IEnumerator WaitBeforeReset()
        {
            yield return new WaitForFixedUpdate();
            ResetGrapplingHookAfterPull();
        }

        public void StopGrappleSway()
        {
            information.isGrappleHookedToSurface = false;
            
            RetractGrapple();
        }

        public void StopHooking()
        {
            information.isGrappleHookedToSurface = false;
            information.isGrappleHookedToEntity = false;
        }

        public void ResetGrapplingHook()
        {
            StopHooking();
            information.isGrappleActive = false;
        }

        public void SetIsGrapplePulling(bool value)
        {
            information.isGrapplePulling = value;
        }

        public struct GrapplingHookInformation
        {
            public bool isGrappleHookedToSurface;
            public bool isGrappleHookedToEntity;
            public bool isGrappleActive;
            public bool isGrapplePulling;
            public float currentGrappleLength;
            public GameObject hookedGameObject;
            public Vector2 hookPosition;
        }
    }
}
