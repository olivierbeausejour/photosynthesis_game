//Authors:
//Benjamin Lemelin
//Olivier Beauséjour

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Harmony;
using UnityEngine;

namespace Game
{
    public interface ISensor<out T>
    {
        event SensorEventHandler<T> OnSensedObject;
        event SensorEventHandler<T> OnUnsensedObject;

        IReadOnlyList<T> SensedObjects { get; }

        void ResetSensor();
    }

    public sealed class Sensor : MonoBehaviour, ISensor<GameObject>
    {
        private new Collider2D collider;

        private Transform parentTransform;
        private readonly List<GameObject> sensedObjects;
        private ulong dirtyFlag;

        public event SensorEventHandler<GameObject> OnSensedObject;
        public event SensorEventHandler<GameObject> OnUnsensedObject;

        public Collider2D Collider => collider;

        public IReadOnlyList<GameObject> SensedObjects => sensedObjects;
        public ulong DirtyFlag => dirtyFlag;
        
        public Sensor()
        {
            sensedObjects = new List<GameObject>();
            dirtyFlag = ulong.MinValue;
        }

        protected void Awake()
        {
            collider = GetComponent<Collider2D>();
            
            parentTransform = transform.parent;
            SetSensorLayer(R.S.Layer.Sensor);
        }

        protected void OnEnable()
        {
            collider.enabled = true;
            collider.isTrigger = true;
        }

        protected void OnDisable()
        {
            collider.enabled = false;
            collider.isTrigger = false;
            ClearSensedObjects();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var sensedObject = IsStimuliInChildren(other) ? other.gameObject : other.gameObject.Parent();

            if (sensedObject == null) return;
            
            var stimuli = sensedObject.GetComponentInChildren<Stimuli>();

            if (stimuli == null || stimuli.PrefabToAffect != sensedObject) return;
            stimuli.OnDestroyed += RemoveSensedObject;
            AddSensedObject(stimuli.PrefabToAffect);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var sensedObject = IsStimuliInChildren(other) ? other.gameObject : other.gameObject.Parent();

            if (sensedObject == null) return;

            var stimuli = sensedObject.GetComponentInChildren<Stimuli>();

            if (stimuli == null || stimuli.PrefabToAffect != sensedObject) return;
            
            stimuli.OnDestroyed -= RemoveSensedObject;
            RemoveSensedObject(stimuli.PrefabToAffect);
        }

        private bool IsStimuliInChildren(Collider2D other)
        {
            return other.gameObject.GetComponentInChildren<Stimuli>() != null;
        }

        private void AddSensedObject(GameObject otherObject)
        {
            if (!sensedObjects.Contains(otherObject))
            {
                sensedObjects.Add(otherObject);
                dirtyFlag++;
                NotifySensedObject(otherObject);
            }
        }

        private void RemoveSensedObject(GameObject otherObject)
        {
            if (sensedObjects.Contains(otherObject))
            {
                sensedObjects.Remove(otherObject);
                dirtyFlag++;
                NotifyUnsensedObject(otherObject);
            }
        }

        public ISensor<T> For<T>()
        {
            return new Sensor<T>(this);
        }

        public void SetSensorLayer(string layerName)
        {
            gameObject.layer = LayerMask.NameToLayer(layerName);
        }

        private void ClearSensedObjects()
        {
            sensedObjects.Clear();
            dirtyFlag++;
        }

        private bool IsSelf(Transform otherParentTransform)
        {
            return parentTransform == otherParentTransform;
        }

        private void NotifySensedObject(GameObject otherObject)
        {
            if (OnSensedObject != null) OnSensedObject(otherObject);
        }

        private void NotifyUnsensedObject(GameObject otherObject)
        {
            if (OnUnsensedObject != null) OnUnsensedObject(otherObject);
        }
        
        public void ResetSensor()
        {
            ClearSensedObjects();
        }
    }

    [SuppressMessage("ReSharper", "DelegateSubtraction")]
    public sealed class Sensor<T> : ISensor<T>
    {
        private readonly Sensor sensor;
        private SensorEventHandler<T> onSensedObject;
        private SensorEventHandler<T> onUnsensedObject;

        private readonly List<T> sensedObjects;
        private ulong dirtyFlag;

        public IReadOnlyList<T> SensedObjects
        {
            get
            {
                if (IsDirty()) UpdateSensor();

                return sensedObjects;
            }
        }

        public event SensorEventHandler<T> OnSensedObject
        {
            add
            {
                if (onSensedObject == null || onSensedObject.GetInvocationList().Length == 0)
                    sensor.OnSensedObject += OnSensedObjectInternal;
                onSensedObject += value;
            }
            remove
            {
                if (onSensedObject != null && onSensedObject.GetInvocationList().Length == 1)
                    sensor.OnSensedObject -= OnSensedObjectInternal;
                onSensedObject -= value;
            }
        }

        public event SensorEventHandler<T> OnUnsensedObject
        {
            add
            {
                if (onUnsensedObject == null || onUnsensedObject.GetInvocationList().Length == 0)
                    sensor.OnUnsensedObject += OnUnsensedObjectInternal;
                onUnsensedObject += value;
            }
            remove
            {
                if (onUnsensedObject != null && onUnsensedObject.GetInvocationList().Length == 1)
                    sensor.OnUnsensedObject -= OnUnsensedObjectInternal;
                onUnsensedObject -= value;
            }
        }

        public Sensor(Sensor sensor)
        {
            this.sensor = sensor;
            sensedObjects = new List<T>();
            dirtyFlag = sensor.DirtyFlag;

            UpdateSensor();
        }

        private bool IsDirty()
        {
            return sensor.DirtyFlag != dirtyFlag;
        }

        private void UpdateSensor()
        {
            sensedObjects.Clear();

            foreach (var otherObject in sensor.SensedObjects)
            {
                var otherComponent = otherObject.GetComponentInChildren<T>();
                if (otherComponent != null) sensedObjects.Add(otherComponent);
            }

            dirtyFlag = sensor.DirtyFlag;
        }

        private void OnSensedObjectInternal(GameObject otherObject)
        {
            var otherComponent = otherObject.GetComponentInChildren<T>();
            if (otherComponent != null && !sensedObjects.Contains(otherComponent))
            {
                sensedObjects.Add(otherComponent);
                NotifySensedObject(otherComponent);
            }

            dirtyFlag = sensor.DirtyFlag;
        }

        private void OnUnsensedObjectInternal(GameObject otherObject)
        {
            var otherComponent = otherObject.GetComponentInChildren<T>();
            if (otherComponent != null && sensedObjects.Contains(otherComponent))
            {
                sensedObjects.Remove(otherComponent);
                NotifyUnsensedObject(otherComponent);
            }

            dirtyFlag = sensor.DirtyFlag;
        }

        private void ClearSensedObjects()
        {
            sensedObjects.Clear();
            
            sensor.ResetSensor();
            dirtyFlag = sensor.DirtyFlag;
        }

        private void NotifySensedObject(T otherObject)
        {
            if (onSensedObject != null) onSensedObject(otherObject);
        }

        private void NotifyUnsensedObject(T otherObject)
        {
            if (onUnsensedObject != null) onUnsensedObject(otherObject);
        }
        
        public void ResetSensor()
        {
            ClearSensedObjects();
        }
    }

    public delegate void SensorEventHandler<in T>(T otherObject);
}