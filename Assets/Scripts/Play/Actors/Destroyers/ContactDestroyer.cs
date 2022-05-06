//Author: Olivier Beauséjour

using UnityEngine;

namespace Game
{
    public class ContactDestroyer : MonoBehaviour
    {
        private Sensor sensor;
        private ISensor<IContactDestroyable> contactDestroyableSensors;
        
        private void Awake()
        {
            sensor = GetComponent<Sensor>();
            
            contactDestroyableSensors = sensor.For<IContactDestroyable>();
        }

        private void OnEnable()
        {
            contactDestroyableSensors.OnSensedObject += DestroyDestroyableSensor;
            contactDestroyableSensors.OnUnsensedObject += RemoveSensedObject;
        }

        private void OnDisable()
        {
            contactDestroyableSensors.OnSensedObject -= DestroyDestroyableSensor;
            contactDestroyableSensors.OnUnsensedObject -= RemoveSensedObject;
        }

        private void DestroyDestroyableSensor(IContactDestroyable contactDestroyable)
        {
            contactDestroyable.DestroyByContact();
        }
        
        private void RemoveSensedObject(IContactDestroyable contactDestroyable)
        {
        }
    }
}