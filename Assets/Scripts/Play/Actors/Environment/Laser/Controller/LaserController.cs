//Author: Olivier Beauséjour

using System.Collections;
using UnityEngine;

namespace Game
{
    public class LaserController : MonoBehaviour
    {
        [Header("General properties")] 
        [SerializeField] private float firstFacingAngle;
        
        [Header("Flicking properties")] 
        [SerializeField] private bool shouldFlick;
        [SerializeField] private float maximumTimeActive;
        [SerializeField] private float maximumTimeInactive;
        
        [Header("Rotating properties")] 
        [SerializeField] private bool shouldRotate;
        [SerializeField] private float nbAnglesPerSecond;
        [SerializeField] private bool clockwise;

        private LaserActuator laserActuator;
        private LineRenderer lineRenderer;

        private bool isActive;

        private void Awake()
        {
            laserActuator = GetComponentInChildren<LaserActuator>();
            lineRenderer = GetComponent<LineRenderer>();
        }

        private void Start()
        {
            isActive = true;
            laserActuator.Angle = firstFacingAngle;

            if (shouldFlick) StartCoroutine(Flick());
        }

        private void Update()
        {
            if (isActive) UpdateRopeVisuals();
            if (shouldRotate) ManageRotation();
        }

        private IEnumerator Flick()
        {
            while (isActiveAndEnabled)
            {
                yield return new WaitForSeconds(maximumTimeActive);
                DeactivateLaser();
                yield return new WaitForSeconds(maximumTimeInactive);
                ActivateLaser();
            }
        }

        private void UpdateRopeVisuals()
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, laserActuator.EndPoint);
        }

        private void ManageRotation()
        {
            float anglesToAdd = nbAnglesPerSecond * Time.deltaTime;
            laserActuator.Angle += anglesToAdd * (clockwise ? -1 : 1);
        }

        private void ActivateLaser()
        {
            isActive = true;
            laserActuator.enabled = true;
            lineRenderer.enabled = true;
        }
        
        private void DeactivateLaser()
        {
            isActive = false;
            laserActuator.enabled = false;
            lineRenderer.enabled = false;
        }
    }
}