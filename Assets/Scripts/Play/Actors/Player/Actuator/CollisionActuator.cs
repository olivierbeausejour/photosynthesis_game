using UnityEngine;


namespace Game
{
    /// <summary>
    /// Author: Jonathan Mathieu
    /// Works for vertical and horizontal collision in 2D
    /// Pass velocity in LimitVelocity to return a new one that stops object from going through scene's horizontal
    /// or vertical colliders parts and takes effect on colliders included in the collisionMask.
    ///
    /// ShowDebugRays shows the velocity currently passed and modified from the raycast shooting points.
    /// </summary>
    public class CollisionActuator : MonoBehaviour
    {
        private Collider2D boxCollider;

        [SerializeField] private float skinWidth = 0.02f;
        [SerializeField] private int verticalRayCount = 4;
        [SerializeField] private int horizontalRayCount = 4;
        [SerializeField] private LayerMask collisionMask;
        [SerializeField] private bool showDebugRays = true;

        private RaycastOrigins raycastOrigins;
        private Bounds bounds;
        private float horizontalRaySpacing;
        private float verticalRaySpacing;
        private CollisionInformation collisions;
        private PlayerController playerController;

        public CollisionInformation Collisions => collisions;

        public float angle;

        public float SkinWidth
        {
            set => skinWidth = value;
        }

        private void Awake()
        {
            boxCollider = GetComponentInChildren<Collider2D>();
            playerController = GetComponent<PlayerController>();
            bounds = boxCollider.bounds;
            bounds.Expand(skinWidth * -1);
            CalculateRaySpacing();
        }

        public Vector2 LimitVelocity(Vector2 velocity)
        {
            UpdateRaycastOrigins();
            collisions.Reset();

            if (velocity.x != 0f)
            {
                ManageHorizontalCollisions(ref velocity);
            }

            if (velocity.y != 0f)
            {
                ManageVerticalCollisions(ref velocity);
            }
            return velocity;
        }

        private void ManageVerticalCollisions(ref Vector2 velocity)
        {
            float directionX = Mathf.Sign(velocity.x);
            float directionY = Mathf.Sign(velocity.y);
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY < 0) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
                var hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
                
                if (hit && (Vector2.Angle(hit.normal, Vector2.up) == 180f || 
                            Vector2.Angle(hit.normal, Vector2.up) == 0f))
                {
                    angle = Vector2.Angle(hit.normal, Vector2.up);
                    
                    velocity.y = (hit.distance - skinWidth) * directionY;
                    rayLength = hit.distance;

                    collisions.below = directionY < 0;
                    collisions.above = directionY > 0;
                    collisions.grounded = collisions.below;
                }

                ManagePotentialFall(directionX, i, rayOrigin, directionY);
                if (showDebugRays) Debug.DrawRay(rayOrigin, directionY * rayLength * Vector2.up, Color.white);
            }
        }

        private void ManagePotentialFall(float directionX, int i, Vector2 rayOrigin, float directionY)
        {
            if (directionX > 0 && i == verticalRayCount - 1)
            {
                var hitRight = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, skinWidth + 0.1f, collisionMask);
                if (!hitRight)
                {
                    collisions.belowRight = true;
                }
            }
            else if (directionX < 0 && i == 0)
            {
                var hitLeft = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, skinWidth + 0.1f, collisionMask);
                if (!hitLeft)
                {
                    collisions.belowLeft = true;
                }
            }
        }

        private void ManageHorizontalCollisions(ref Vector2 moveDistance)
        {
            float directionX = Mathf.Sign(moveDistance.x);
            float rayLength = Mathf.Abs(moveDistance.x) + skinWidth;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX < 0) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                var hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

                if (hit && Vector2.Angle(hit.normal, Vector2.up) == 90f)
                {
                    moveDistance.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    collisions.left = directionX < 0;
                    collisions.right = directionX > 0;
                }

                if (showDebugRays) Debug.DrawRay(rayOrigin, directionX * rayLength * Vector2.right, Color.white);
            }
        }

        private void UpdateRaycastOrigins()
        {
            bounds.center = boxCollider.bounds.center;
            raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
            raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        }

        private void CalculateRaySpacing()
        {
            horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
            verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);
            horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
            verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
        }

        struct RaycastOrigins
        {
            public Vector2 topLeft, topRight;
            public Vector2 bottomLeft, bottomRight;
        }

        public struct CollisionInformation
        {
            public bool above, below, left, right;
            public bool belowLeft,belowRight;
            public bool grounded;

            public void Reset()
            {
                above = below = left = right = false;
                belowLeft = belowRight = false;
                grounded = false;
            }
        }
    }
}