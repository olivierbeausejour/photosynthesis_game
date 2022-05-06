//Author: Olivier Beauséjour

using System;
using Harmony;
using UnityEngine;

namespace Game
{
    public class Collectible : MonoBehaviour
    {
        private Sensor sensor;
        private SpriteRenderer spriteRenderer;
        private Collider2D hitbox;
        protected Vector3 startingPosition;
        
        private ISensor<PlayerController> playerSensors;
        
        protected virtual void Awake()
        {
            sensor = GetComponent<Sensor>();
            spriteRenderer = gameObject.Parent().GetComponentInChildren<SpriteRenderer>();
            hitbox = gameObject.Parent().GetComponentInChildren<Collider2D>();
            
            playerSensors = sensor.For<PlayerController>();
            
            startingPosition = transform.position;
        }
        
        protected void OnEnable()
        {
            playerSensors.OnSensedObject += PickUp;
            playerSensors.OnUnsensedObject += RemoveSensedObject;
        }

        protected void OnDisable()
        {
            playerSensors.OnSensedObject -= PickUp;
            playerSensors.OnUnsensedObject -= RemoveSensedObject;
        }
        
        protected virtual void PickUp(PlayerController player)
        {
            playerSensors.ResetSensor();
            //sensor.ResetSensor();
            spriteRenderer.enabled = false;
            hitbox.enabled = false;
        }
        
        private void RemoveSensedObject(PlayerController player)
        {
        }

        protected void ResetPosition()
        {
            var resetPosition = startingPosition;
            transform.position = resetPosition;
            spriteRenderer.enabled = true;
            hitbox.enabled = true;
        }
    }
}