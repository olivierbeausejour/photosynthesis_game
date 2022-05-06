//Author: Olivier Beauséjour

using Harmony;
using UnityEngine;

namespace Game
{
    public class DashDestroyer : MonoBehaviour
    {
        private Sensor sensor;
        private DashActuator dashActuator;

        private DashOnBossWeakSpotEventChannel dashOnBossWeakSpotEventChannel;
        
        private ISensor<IDashDestroyable> dashDestroyableSensors;
        
        private void Awake()
        {
            sensor = GetComponent<Sensor>();
            dashActuator = GetComponentInParent<DashActuator>();

            dashOnBossWeakSpotEventChannel = Finder.DashOnBossWeakSpotEventChannel;
            
            dashDestroyableSensors = sensor.For<IDashDestroyable>();
            
            sensor.SetSensorLayer(R.S.Layer.Player);
        }

        private void OnEnable()
        {
            dashDestroyableSensors.OnSensedObject += DestroyDestroyableSensor;
            dashDestroyableSensors.OnUnsensedObject += RemoveSensedObject;
        }

        private void OnDisable()
        {
            dashDestroyableSensors.OnSensedObject -= DestroyDestroyableSensor;
            dashDestroyableSensors.OnUnsensedObject -= RemoveSensedObject;
        }

        private void DestroyDestroyableSensor(IDashDestroyable dashDestroyable)
        {
            if(dashDestroyable is DashDestroyable destroyable && destroyable.gameObject.layer == LayerMask.NameToLayer(R.S.Layer.BossBodyPart) && destroyable.enabled)
                dashOnBossWeakSpotEventChannel.NotifyDashOnBossWeakSpot();
            
            if (dashActuator.IsDashing) dashDestroyable.DestroyByDash();
        }
        
        private void RemoveSensedObject(IDashDestroyable dashDestroyable)
        {
        }

        public void ResetSensor()
        {
            dashDestroyableSensors.ResetSensor();
        }
    }
}