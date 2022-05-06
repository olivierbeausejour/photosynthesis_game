using System.Collections.Generic;
using UnityEngine;


namespace Game
{
    public class Rope : MonoBehaviour
    {
        [SerializeField] [Range(0f, 1f)] private float ropeTensionPercentage = 0.8f;
        [SerializeField] private int numberOfSimulation = 30;
        [SerializeField] private int segmentCount = 35;
        [SerializeField] private Transform startPoint;
        [SerializeField] private Transform endPoint;
        [SerializeField] private Vector2 ropeGravity = Vector2.down;

        private PlayerController playerController;
        private SpriteRenderer grappleRenderer;
        private LineRenderer ropeRenderer;

        private readonly List<RopeSegment> ropeSegments = new List<RopeSegment>();
        public float ropeLength;
        private float ropeSegmentLength;
        private Vector3[] ropePositions;

        private void Awake()
        {
            ropeRenderer = GetComponent<LineRenderer>();
            grappleRenderer = endPoint.GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            ropePositions = new Vector3[segmentCount];
            Vector3 ropeStartPoint = transform.position;
            ropeRenderer.enabled = false;

            for (int i = 0; i < segmentCount; i++)
            {
                ropeSegments.Add(new RopeSegment(ropeStartPoint));
                ropeStartPoint.y -= ropeSegmentLength;
            }

            SetRopeLength(0f);
            SetSegmentLength();
            Simulate();
        }

        public void SetActive(bool isActive)
        {
            if (isActive)
            {
                ropeRenderer.enabled = true;
                grappleRenderer.enabled = true;
            }
            else
            {
                ropeRenderer.enabled = false;
                grappleRenderer.enabled = false;
            }
        }

        public void SetRopeLength(float ropeLength)
        {
            this.ropeLength = ropeLength;
        }

        private void Update()
        {
            SetSegmentLength();
            Simulate();
            DrawRope();
        }

        // Uses Verlet Integration to simulate where the Rope parts should be
        private void Simulate()
        {
            // Simulation
            for (int i = 0; i < segmentCount; i++)
            {
                RopeSegment firstSegment = ropeSegments[i];
                Vector2 velocity = firstSegment.currentPosition - firstSegment.previousPosition;
                firstSegment.previousPosition = firstSegment.currentPosition;
                firstSegment.currentPosition += velocity;
                firstSegment.currentPosition += ropeGravity * Time.deltaTime;
                ropeSegments[i] = firstSegment;
            }

            // Constraints
            for (int i = 0; i < numberOfSimulation; i++)
            {
                ApplyConstraint();
            }
        }

        private void ApplyConstraint()
        {
            RopeSegment startSegment = ropeSegments[0];
            startSegment.currentPosition = startPoint.position;
            ropeSegments[0] = startSegment;

            RopeSegment endSegment = ropeSegments[segmentCount - 1];
            endSegment.currentPosition = endPoint.position;
            ropeSegments[segmentCount - 1] = endSegment;

            for (int i = 0; i < segmentCount - 1; i++)
            {
                RopeSegment firstSegment = ropeSegments[i];
                RopeSegment secondSegment = ropeSegments[i + 1];

                float distance = (firstSegment.currentPosition - secondSegment.currentPosition).magnitude;
                float error = distance - ropeSegmentLength;
                Vector2 changeDir = (firstSegment.currentPosition - secondSegment.currentPosition).normalized;
                Vector2 changeAmount = changeDir * error;

                if (i != 0)
                {
                    firstSegment.currentPosition -= changeAmount * 0.5f;
                    ropeSegments[i] = firstSegment;
                    secondSegment.currentPosition += changeAmount * 0.5f;
                    ropeSegments[i + 1] = secondSegment;
                }
                else
                {
                    secondSegment.currentPosition += changeAmount;
                    ropeSegments[i + 1] = secondSegment;
                }
            }
        }

        private void SetSegmentLength()
        {
            ropeSegmentLength = ropeLength * ropeTensionPercentage / segmentCount;
        }

        private void DrawRope()
        {
            for (int i = 0; i < segmentCount; i++)
            {
                ropePositions[i] = ropeSegments[i].currentPosition;
            }

            ropeRenderer.positionCount = ropePositions.Length;
            ropeRenderer.SetPositions(ropePositions);
        }

        private struct RopeSegment
        {
            public Vector2 currentPosition;
            public Vector2 previousPosition;

            public RopeSegment(Vector2 position)
            {
                previousPosition = position;
                currentPosition = position;
            }
        }
    }
}
