using System;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(MovementActuator))]
    public class EnemyControllerTest : MonoBehaviour
    {
        //TODO Dans l'ennemyController, mettre bool ishooked et ensuite si a hit le player, die, ou si distance traveled == something
        [SerializeField] private float gravity;

        private CollisionActuator collisionActuator;
        private MovementActuator movementActuator;

        private Vector2 velocity;
        private bool hasBeenPulled = false;

        public bool HasBeenPulled
        {
            get => hasBeenPulled;
            set => hasBeenPulled = value;
        }

        public Vector2 Velocity
        {
            get => velocity;
            set => velocity = value;
        }

        private void Awake()
        {
            collisionActuator = GetComponent<CollisionActuator>();
            movementActuator = GetComponent<MovementActuator>();

            velocity = Vector2.zero;
        }

        private void Update()
        {
            if (hasBeenPulled) return;

            ManageHorizontalMovement();
            ManageVerticalMovement();
            movementActuator.Move(velocity);
        }

        private void ManageHorizontalMovement()
        {
        }

        private void ManageVerticalMovement()
        {
            if (collisionActuator.Collisions.below || collisionActuator.Collisions.above) velocity.y = 0f;

            velocity.y += gravity * Time.deltaTime;
        }
    }
}