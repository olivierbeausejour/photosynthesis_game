//Authors:
//Jonathan Mathieu

using System.Configuration;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(CollisionActuator))]
    public class MovementActuator : MonoBehaviour
    {
        [SerializeField] private bool ShowPlayerMovementDebugRay = true;
        
        private CollisionActuator collisionActuator;
        private Vector2 currentVelocity;

        public Vector2 CurrentVelocity
        {
            get => currentVelocity;
            set => currentVelocity = value;
        }

        private void Awake()
        {
            collisionActuator = GetComponent<CollisionActuator>();
        }

        public void Move(Vector2 velocity)
        {
            currentVelocity = velocity;
            
#if UNITY_EDITOR
            Vector2 lastPosition = transform.position;
#endif
        
            transform.Translate(collisionActuator.LimitVelocity(currentVelocity * Time.deltaTime));
        
#if UNITY_EDITOR
            if (ShowPlayerMovementDebugRay)
            {
                Debug.DrawLine(lastPosition, transform.position, Color.cyan, 5f);
            }
#endif
            
            
            // Synchronize collider transform manually or else it can be late from the physics internal update.
            Physics2D.SyncTransforms();
        }

        public void DashingMove(Vector2 velocity)
        {
            currentVelocity = velocity;
            
#if UNITY_EDITOR
            Vector2 lastPosition = transform.position;
#endif
        
            transform.Translate(collisionActuator.LimitVelocity(currentVelocity * Time.deltaTime));
        
#if UNITY_EDITOR
            if (ShowPlayerMovementDebugRay)
            {
                Debug.DrawLine(lastPosition, transform.position, Color.cyan, 5f);
            }
#endif
            
            
            // Synchronize collider transform manually or else it can be late from the physics internal update.
            Physics2D.SyncTransforms();
        }
    }
}