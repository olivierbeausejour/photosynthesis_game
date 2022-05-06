//Authors:
//Charles Tremblay
//Olivier Beauséjour

using Harmony;
using UnityEngine;

namespace Game
{
    public class PullDestroyer : MonoBehaviour
    {
        private Sensor sensor;
        private HookedToHookableEntityActuator hookedToHookableEntityActuator;

        private ISensor<IPullDestroyable> pullDestroyableSensors;

        private void Awake()
        {
            sensor = GetComponent<Sensor>();
            hookedToHookableEntityActuator = GetComponentInParent<HookedToHookableEntityActuator>();

            pullDestroyableSensors = sensor.For<IPullDestroyable>();
            
            sensor.SetSensorLayer(R.S.Layer.Player);
        }

        private void OnEnable()
        {
            pullDestroyableSensors.OnSensedObject += DestroyDestroyableSensor;
            pullDestroyableSensors.OnUnsensedObject += RemoveSensedObject;
        }

        private void OnDisable()
        {
            pullDestroyableSensors.OnSensedObject -= DestroyDestroyableSensor;
            pullDestroyableSensors.OnUnsensedObject -= RemoveSensedObject;
        }

        private void DestroyDestroyableSensor(IPullDestroyable pulledDestroyable)
        { 
            if (hookedToHookableEntityActuator.HookedEntityRigidbody == null) return;

            var currentPulledIDestroyable =
                hookedToHookableEntityActuator.HookedEntityRigidbody.GetComponent<IPullDestroyable>();
            
            if (currentPulledIDestroyable != null && currentPulledIDestroyable == pulledDestroyable && 
                hookedToHookableEntityActuator.GrapplingHookController.Information.isGrapplePulling) 
                pulledDestroyable.DestroyByPull();
        }

        private void RemoveSensedObject(IPullDestroyable pulledDestroyable)
        {
        }
        
        public void ResetSensor()
        {
            pullDestroyableSensors.ResetSensor();
        }
    }
}