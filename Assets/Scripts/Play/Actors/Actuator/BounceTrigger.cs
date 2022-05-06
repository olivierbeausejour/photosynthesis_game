using System;
using Harmony;
using UnityEngine;

namespace Game
{
    public class BounceTrigger : MonoBehaviour
    {
        [SerializeField] [Range(10f, 30f)] private float bounceForce;
        
        private Sensor sensor;
        private ActorBouncedEventChannel actorBouncedEventChannel;
        
        private ISensor<BaseActuator> baseActuatorSensors;

        private void Awake()
        {
            sensor = GetComponent<Sensor>();
            actorBouncedEventChannel = Finder.ActorBouncedEventChannel;
            baseActuatorSensors = sensor.For<BaseActuator>();
        }

        private void OnEnable()
        {
            baseActuatorSensors.OnSensedObject += BouncePlayer;
            baseActuatorSensors.OnUnsensedObject += RemoveSensedObject;
        }

        private void OnDisable()
        {
            baseActuatorSensors.OnSensedObject -= BouncePlayer;
            baseActuatorSensors.OnUnsensedObject -= RemoveSensedObject;
        }

        private void BouncePlayer(BaseActuator baseObject)
        {
            actorBouncedEventChannel.NotifyActorBounced();
            baseObject.ManageExternalForce(-baseObject.Velocity.normalized * bounceForce);
        }

        private void RemoveSensedObject(BaseActuator baseObject)
        {
        }
    }
}