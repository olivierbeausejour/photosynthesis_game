//Authors:
//Charles Tremblay

using System.Collections;
using Game;
using Harmony;
using UnityEngine;

namespace Play.Game.Camera
{
    public class CameraController : MonoBehaviour
    {
        private const float POSITION_OFFSET = -10;
        
        [SerializeField] private UnityEngine.Camera mainCamera;
        [SerializeField] private PlayerController target;
        [SerializeField] private float verticalOffset;
        [SerializeField] private float initialLookAheadDstX;
        [SerializeField] private float hookedLookAheadDstX;
        [SerializeField] private float lookSmoothTimeX;
        [SerializeField] private float hookedLookSmoothTimeX;
        [SerializeField] private Vector2 focusAreaSize;
        [SerializeField] private float initialOrthographicSize;
        [SerializeField] private float smoothTimeAfterDash;

        private PlayerDashEventChannel playerDashEventChannel;
        private PlayerIsDoneDashingEventChannel playerIsDoneDashingEventChannel;
        private PlayerRespawnEventChannel playerRespawnEventChannel;
        private GameController gameController;
        private PlayerInputManager playerInputManager;

        private FocusArea focusArea;
        private float lookAheadDstX;
        private float currentLookAheadX;
        private float targetLookAheadX;
        private float lookAheadDirX;
        private float initialLookSmoothTimeX;
        private float smoothLookVelocityX;
        private Vector3 supposedCameraPosition;
        private Vector3 cameraPositionWhenDash;
        private bool shouldCameraMove = true;
        private bool lookAheadStopped;

        private void Awake()
        {
            initialLookSmoothTimeX = lookSmoothTimeX;
            mainCamera.orthographicSize = initialOrthographicSize;
            supposedCameraPosition = transform.position;
            lookAheadDstX = initialLookAheadDstX;
            gameController = Finder.GameController;
            playerDashEventChannel = Finder.PlayerDashEventChannel;
            playerIsDoneDashingEventChannel = Finder.PlayerIsDoneDashingEventChannel;
            playerRespawnEventChannel = Finder.PlayerRespawnEventChannel;
            playerInputManager = Finder.PlayerInputManager;
        }

        private void OnEnable()
        {
            playerDashEventChannel.OnPlayerDash += SetFocusAreaSizeForDash;
            playerIsDoneDashingEventChannel.OnPlayerDoneDashing += ResetCameraAfterDash;
            playerRespawnEventChannel.OnPlayerRespawn += ResetCameraAfterPlayerDeath;
        }

        private void OnDisable()
        {
            playerDashEventChannel.OnPlayerDash -= SetFocusAreaSizeForDash;
            playerIsDoneDashingEventChannel.OnPlayerDoneDashing -= ResetCameraAfterDash;
            playerRespawnEventChannel.OnPlayerRespawn += ResetCameraAfterPlayerDeath;
        }

        private void Start()
        {
            focusArea = new FocusArea(target.PhysicalCollider.bounds, focusAreaSize);
        }

        private void SetFocusAreaSizeForDash()
        {
            cameraPositionWhenDash = transform.position;
            shouldCameraMove = false;
        }

        private void ResetCameraAfterDash()
        {
            if (!shouldCameraMove && transform.position == cameraPositionWhenDash)
                StartCoroutine(ResetCameraAfterDashCoroutine());
        }

        public IEnumerator ResetCameraAfterDashCoroutine()
        {
            Vector3 startPosition = transform.position;
            float lerp = 0.0f;
            float smoothLerp = 0.0f;
            float elapsedTime = 0.0f;
            float finalLerpValue = 1.0f;

            while (lerp < 1.0f && smoothTimeAfterDash > 0.0f)
            {
                lerp = Mathf.Lerp(0.0f, finalLerpValue, elapsedTime / smoothTimeAfterDash);
                smoothLerp = Mathf.SmoothStep(0.0f, finalLerpValue, lerp);
                transform.position = Vector3.Lerp(startPosition, supposedCameraPosition, smoothLerp);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            shouldCameraMove = true;
        }

        private void ResetCameraAfterPlayerDeath()
        {
            if (gameController.CurrentCheckpoint != null)
                mainCamera.orthographicSize = gameController.CurrentCheckpoint.CameraOrthographicSize;
        }

        private void LateUpdate()
        {
            focusArea.Update(target.PhysicalCollider.bounds);

            Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;
            if (focusArea.velocity.x != 0)
            {
                if (target.GrapplingHookController.Information.isGrappleHookedToSurface)
                {
                    lookAheadDstX = hookedLookAheadDstX;
                    lookSmoothTimeX = hookedLookSmoothTimeX;
                }
                else
                {
                    lookAheadDstX = initialLookAheadDstX;
                    lookSmoothTimeX = initialLookSmoothTimeX;
                }

                lookAheadDirX = Mathf.Sign(focusArea.velocity.x);

                if (Mathf.Sign(playerInputManager.MovementAxisInputNormalized.x) == Mathf.Sign(focusArea.velocity.x) &&
                    playerInputManager.MovementAxisInputNormalized.x != 0)
                {
                    lookAheadStopped = false;
                    targetLookAheadX = lookAheadDirX * lookAheadDstX;
                }
                else
                {
                    if (!lookAheadStopped)
                    {
                        lookAheadStopped = true;
                        targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX) / 6f;
                    }
                }
            }

            currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX,
                lookSmoothTimeX);
            focusPosition += Vector2.right * currentLookAheadX;

            if (shouldCameraMove)
            {
                transform.position = (Vector3) focusPosition + Vector3.forward * POSITION_OFFSET;
                supposedCameraPosition = transform.position;
            }
            else
                supposedCameraPosition = (Vector3) focusPosition + Vector3.forward * POSITION_OFFSET;
        }

        //Author: Sebastian Lague
        //Source: https://github.com/SebLague/2DPlatformer-Tutorial
        private class FocusArea : MonoBehaviour
        {
            public Vector2 center;
            public Vector2 velocity;
            public Vector2 size;
            private Bounds bounds;
            private float left, right;
            private float top, bottom;

            public FocusArea(Bounds targetBounds, Vector2 size)
            {
                this.size = size;
                bounds = targetBounds;

                left = bounds.center.x - this.size.x / 2;
                right = bounds.center.x + this.size.x / 2;
                bottom = bounds.min.y;
                top = bounds.min.y + this.size.y;

                velocity = Vector2.zero;
                center = new Vector2((left + right) / 2, (top + bottom) / 2);
            }

            public void Update(Bounds targetBounds)
            {
                float shiftX = 0;

                if (targetBounds.min.x < left)
                {
                    shiftX = targetBounds.min.x - left;
                }
                else if (targetBounds.max.x > right)
                {
                    shiftX = targetBounds.max.x - right;
                }

                left += shiftX;
                right += shiftX;

                float shiftY = 0;

                if (targetBounds.min.y < bottom)
                {
                    shiftY = targetBounds.min.y - bottom;
                }
                else if (targetBounds.max.y > top)
                {
                    shiftY = targetBounds.max.y - top;
                }

                top += shiftY;
                bottom += shiftY;
                center = new Vector2((left + right) / 2, (top + bottom) / 2);
                velocity = new Vector2(shiftX, shiftY);
            }
        }
    }
}