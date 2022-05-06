//Authors:
//Olivier Beauséjour
//Charles Tremblay

using Harmony;
using UnityEngine;

namespace Game
{
    public class Hazard : MonoBehaviour
    {
        [SerializeField] private bool isNoCollideHazard;
        
        [SerializeField] private bool killDashingTargets;
        [SerializeField] private bool killPullingTargets;

        protected Sensor sensor;
        private ISensor<IHurtable> hurtableSensors;

        public bool KillDashingTargets
        {
            set => killDashingTargets = value;
        }

        private EnemyIsDoneBeingPulledEventChannel enemyIsDoneBeingPulledEventChannel;
        
        protected void Awake()
        {
            sensor = GetComponent<Sensor>();
            hurtableSensors = sensor.For<IHurtable>();
            enemyIsDoneBeingPulledEventChannel = Finder.EnemyIsDoneBeingPulledEventChannel;
            
            sensor.SetSensorLayer(isNoCollideHazard ? R.S.Layer.NoCollideHazard : R.S.Layer.Hazard);
        }

        protected void OnEnable()
        {
            hurtableSensors.OnSensedObject += KillHurtableSensors;
            hurtableSensors.OnUnsensedObject += RemoveSensedObject;
        }

        protected void OnDisable()
        {
            hurtableSensors.OnSensedObject -= KillHurtableSensors;
            hurtableSensors.OnUnsensedObject -= RemoveSensedObject;
        }

        private void KillHurtableSensors(IHurtable hurtable)
        {
            var isHurtableDashing = hurtable is IDashable dashable && dashable.IsDashing();
            var isHurtablePulling = hurtable is IPullable pullable && pullable.IsPulling();

            if(killDashingTargets || killPullingTargets) hurtable.Kill();
            else if (!killDashingTargets && !isHurtableDashing && !isHurtablePulling) hurtable.Kill();
            else if(!killPullingTargets && !isHurtablePulling && !isHurtableDashing) hurtable.Kill();
        }
        
        private void RemoveSensedObject(IHurtable hurtable)
        {
        }

        public void ResetSensor()
        {
            hurtableSensors.ResetSensor();
        }
    }
}