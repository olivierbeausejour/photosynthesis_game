//Author: Olivier Beauséjour

using System;
using Harmony;
using UnityEngine;

namespace Game
{
    public class LaserActuator : Hazard
    {
        [Header("Laser properties")] 
        [SerializeField] private float angle;
        [SerializeField] private LayerMask collisionMask;
        [SerializeField] private float maxDistance = 500;

        private Vector2 endPoint;

        public Vector2 EndPoint => endPoint;

        private RaycastHit2D hit;

        public RaycastHit2D Hit => hit;

        public float Angle
        {
            get => angle;
            set => angle = value;
        }

        protected void Update()
        {
            sensor.Collider.transform.position = transform.parent.position;
            if (angle > 360 || angle < 0) angle %= 360;
            
            hit = Physics2D.Raycast(transform.position, 
                (Quaternion.Euler(0,0,angle) * Vector2.right), maxDistance, collisionMask);

            if (hit) sensor.Collider.transform.position = endPoint = hit.point;
            else
                sensor.Collider.transform.position = endPoint =
                    transform.position + Quaternion.Euler(0, 0, angle) * Vector2.right * maxDistance;
            
            
        }
    }
}