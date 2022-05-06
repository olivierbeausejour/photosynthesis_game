//Authors:
//Charles Tremblay

using Harmony;
using UnityEngine;

namespace Game
{
    public class TransitionZoneController : MonoBehaviour
    {
        [SerializeField] private float originalOrthographicSize;
        [SerializeField] private float targetOrthographicSize;
        [SerializeField] private Transform bound1;
        [SerializeField] private float bound1Radius;
        [SerializeField] private Transform bound2;
        [SerializeField] private float bound2Radius;
        [SerializeField] private RectTransform zone;
        [SerializeField] private float smoothTime;

        private PlayerController playerController;
        private Camera mainCamera;

        private Vector2 playerPosition;
        private float newSize;
        private float originalAndNewSizeDiff;
        private float playerProgressionThroughZone;
        private float distanceToTravel;
        private float refCameraSmoothingVelocity;

        private void Awake()
        {
            playerController = FindObjectOfType<PlayerController>();
            mainCamera = GameObject.FindGameObjectWithTag(R.S.Tag.MainCamera).GetComponent<Camera>();
            
            playerPosition = playerController.PlayerPosition;
            distanceToTravel = Mathf.Abs(Vector2.Distance(bound1.position, bound2.position));
            originalAndNewSizeDiff = Mathf.Abs(originalOrthographicSize - targetOrthographicSize);
        }

        private void Update()
        {
            float temp;
            playerPosition = playerController.PlayerPosition;

            if (!CheckIfCameraMustResize())
                return;

            playerProgressionThroughZone = Mathf.Abs(Vector3.Distance(playerPosition, bound1.position) / distanceToTravel);

            if (targetOrthographicSize < originalOrthographicSize)
            {
                temp = originalOrthographicSize - (originalAndNewSizeDiff * playerProgressionThroughZone);
                if(temp >= targetOrthographicSize)
                    newSize = temp;
            }
            else
            {
                temp = originalOrthographicSize + (originalAndNewSizeDiff * playerProgressionThroughZone);
                if(temp <= targetOrthographicSize)
                    newSize = temp;
            }
            
            mainCamera.orthographicSize = Mathf.SmoothDamp(mainCamera.orthographicSize, newSize, ref refCameraSmoothingVelocity, smoothTime);
        }

        private bool CheckIfCameraMustResize()
        {
            return RectTransformUtility.RectangleContainsScreenPoint(zone, playerPosition) && 
                   Vector2.Distance(playerPosition, bound1.position) >= bound1Radius &&
                   Vector2.Distance(playerPosition, bound2.position) >= bound2Radius;
        }
    }
}