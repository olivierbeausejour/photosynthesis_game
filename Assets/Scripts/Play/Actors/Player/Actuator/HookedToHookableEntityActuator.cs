//Authors:
//Charles Tremblay

using Harmony;
using UnityEngine;

namespace Game
{
    public class HookedToHookableEntityActuator : MonoBehaviour
    {
        [SerializeField] private float forceToAddToHookedEntity = 200f;
        [SerializeField] private int minDistanceToUnhook = 6;
        
        private GrapplingHookController grapplingHookController;
        private PlayerInputManager playerInputManager;
        private Rigidbody2D hookedEntityRigidbody;
        private BaseEnemyController enemyController;
        private PlayerController playerController;
        
        private Vector2 currentPosition;
        private Vector2 hookedEntityPosition;
        private Vector2 forceToApplyToHookedEntity;

        private BossLoseArmorPartEventChannel bossLoseArmorPartEventChannel;
        
        public Rigidbody2D HookedEntityRigidbody => hookedEntityRigidbody;
        
        public GrapplingHookController GrapplingHookController => grapplingHookController;

        private void Awake()
        {
            grapplingHookController = GetComponent<GrapplingHookController>();
            playerInputManager = GetComponent<PlayerInputManager>();
            playerController = GetComponent<PlayerController>();
            
            bossLoseArmorPartEventChannel = Finder.BossLoseArmorPartEventChannel;
            
            currentPosition = transform.position;
            hookedEntityPosition = Vector2.zero;
        }

        private void Update()
        {
            currentPosition = transform.position;
            
            if (hookedEntityRigidbody != null)
            {
                UpdateAttributes();
                ManageInputs();
            }
        }

        private void UpdateAttributes()
        {
            hookedEntityPosition = hookedEntityRigidbody.transform.position;
        }

        private void ManageInputs()
        {
            if (playerInputManager.PullKey && !playerController.IsDashing())
            {
                PullEntity();
            }
            else if (CheckIfHasToRetract())
            {
                StopPullingEntity();
            }
        }

        private bool CheckIfHasToRetract()
        {
            return playerInputManager.PullKeyUp || playerInputManager.FireKeyUp ||
                   Vector2.Distance(currentPosition, hookedEntityPosition) >
                   grapplingHookController.MaximumGrappleLength ||
                   Vector2.Distance(currentPosition, hookedEntityPosition) <
                   grapplingHookController.MaximumGrappleLength / minDistanceToUnhook;
        }

        private void PullEntity()
        {
            playerController.SetHurtboxActive(true);
            
            if (enemyController != null) 
                enemyController.IsPulled = true;
            else
                hookedEntityRigidbody.bodyType = RigidbodyType2D.Dynamic;
            
            grapplingHookController.SetIsGrapplePulling(true);

            forceToApplyToHookedEntity = (currentPosition - hookedEntityPosition).normalized;
            
            if(forceToApplyToHookedEntity.y < 0 && enemyController == null)
                hookedEntityRigidbody.AddForce(forceToApplyToHookedEntity * forceToAddToHookedEntity);
            else if(enemyController != null)
                hookedEntityRigidbody.AddForce(forceToApplyToHookedEntity * forceToAddToHookedEntity);
        }

        private void StopPullingEntity()
        {
            playerController.SetHurtboxActive(false);

            if (hookedEntityRigidbody.gameObject.layer == LayerMask.NameToLayer(R.S.Layer.ArmorPiece))
            {
                hookedEntityRigidbody.gameObject.SetActive(false);
                bossLoseArmorPartEventChannel.NotifyBossLoseArmorPart();
            }

            if (enemyController != null)
            {
                enemyController.isPulled = false;
                enemyController.IsHooked = false;
            }
            grapplingHookController.SetIsGrapplePulling(false);
            hookedEntityRigidbody = null;
            enemyController = null;
            grapplingHookController.RetractGrapple();
        }

        public void HookToEntity(RaycastHit2D hit)
        {
            hookedEntityRigidbody = hit.collider.attachedRigidbody;
            enemyController = hookedEntityRigidbody.transform.GetComponent<BaseEnemyController>();
            if(enemyController != null) hookedEntityRigidbody.bodyType = RigidbodyType2D.Dynamic;
            
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer(R.S.Layer.Enemy))
                enemyController.IsHooked = true;
        }
    }
}