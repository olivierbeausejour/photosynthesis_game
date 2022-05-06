using UnityEngine;

//Useful website to understand what's going on: https://www.desmos.com/calculator/xjx15igpsf


namespace Game
{
    public class GrappleSwayActuator : MonoBehaviour
    {
        [SerializeField] private float maximumSwayAngle;
        [SerializeField] private float maximumGrappleLength = 10f;
        [SerializeField] private float playerSwayInfluence = 5f;
        [SerializeField] private float playerInfluenceToStopInPercentage = 0.7f;
        [SerializeField] private float InfluenceThreshold = 0.5f;
        [SerializeField] private float swaySpeed = 5f;
        [SerializeField] private float grappleLengthSpeedInfluence = 0.5f;
        [SerializeField] private float swayReturnToCenterSpeed = 5f;
        [SerializeField] private bool showDebugRays;
        [SerializeField] private float swayCenterThreshold = 0.000001f;
        [SerializeField] private float centerAngleResetThreshold = 3f;
        [SerializeField] private float sineAHeightResetThreshold = 0.1f;
        
        public float rotationAngle;
        private Ray2D grappleRay;
        public float grappleLength;
        private Vector2 oldVelocity;
        public float sineFunctionX;
        public float sineFunctionA;
        public float sineASmoothTime;

        public void InitialiseGrappleSway(Vector2 position)
        {
            sineFunctionA = 1f;
            sineFunctionX = 0f;
            grappleRay.origin = position;
            grappleLength = Vector2.Distance(transform.position, position);
        }
        
        public Vector2 Move (float playerInputX)
        {
            Vector2 velocity;

            float grappleLengthRatio = grappleLength / maximumGrappleLength;
            sineFunctionX += swaySpeed * (grappleLengthRatio + grappleLengthRatio * grappleLengthSpeedInfluence) * Time.deltaTime;

            if (sineFunctionX > -Mathf.PI / 2f  + InfluenceThreshold && sineFunctionX < Mathf.PI / 2 - InfluenceThreshold)
            {
                sineFunctionA -= Mathf.SmoothDamp(0, sineFunctionA, ref sineASmoothTime, swayReturnToCenterSpeed);

                if(playerInputX > 0) sineFunctionA += Mathf.Abs(playerInputX) * playerSwayInfluence * Time.deltaTime;
                if(playerInputX < 0) sineFunctionA -= Mathf.Abs(playerInputX) * playerSwayInfluence * playerInfluenceToStopInPercentage * Time.deltaTime;
            }

            if (sineFunctionX < -Mathf.PI / 2f - InfluenceThreshold || sineFunctionX > Mathf.PI / 2 + InfluenceThreshold)
            {
                sineFunctionA -= Mathf.SmoothDamp(0, sineFunctionA, ref sineASmoothTime, swayReturnToCenterSpeed);

                if(playerInputX > 0) sineFunctionA -= Mathf.Abs(playerInputX) * playerSwayInfluence * playerInfluenceToStopInPercentage * Time.deltaTime;
                if(playerInputX < 0) sineFunctionA += Mathf.Abs(playerInputX) * playerSwayInfluence * Time.deltaTime;
            }

            if (sineFunctionA < sineAHeightResetThreshold && rotationAngle > -centerAngleResetThreshold &&
                rotationAngle < centerAngleResetThreshold)
            {
                if (playerInputX > 0)
                {
                    sineFunctionX = 0;
                }

                if (playerInputX < 0)
                {
                    sineFunctionX = -Mathf.PI;
                }
            }
            
            if (sineFunctionX == 0)
            {
                if (playerInputX < 0)
                {
                    sineFunctionX -= Mathf.PI;
                    sineFunctionA += Mathf.Abs(playerInputX) * playerSwayInfluence * Time.deltaTime;
                }

                if (playerInputX > 0)
                {
                    sineFunctionX = 0;
                    sineFunctionA += Mathf.Abs(playerInputX) * playerSwayInfluence * Time.deltaTime;
                }
            }
            
            if (sineFunctionA < swayCenterThreshold && sineFunctionA > - swayCenterThreshold) sineFunctionA = 0;
            if (sineFunctionA > 1f) sineFunctionA = 1f;
            if (sineFunctionA < 0f) sineFunctionA = 0f;
            
            Debug.DrawRay(grappleRay.origin, Vector3.up * sineFunctionA * 2f, Color.white);
            Debug.DrawRay(grappleRay.origin, new Vector2(1,1) * sineFunctionX, Color.green);
            print(sineFunctionA);
            if(showDebugRays) Debug.DrawRay(grappleRay.origin, Vector2.right * SineFunctionY(ref sineFunctionX, sineFunctionA), Color.red);
            rotationAngle = maximumSwayAngle * SineFunctionY(ref sineFunctionX, sineFunctionA);

            return GetPosition();
        }

        private float SineFunctionY(ref float x, float a)
        {
            if (x > Mathf.PI) x = -Mathf.PI;
            else if (x < -Mathf.PI) x = Mathf.PI;
            
            return a * Mathf.Sin(x);
        }

        private Vector2 GetPosition()
        {
            Quaternion currentRotation = Quaternion.Euler(0f, 0f, rotationAngle);
            grappleRay.direction = currentRotation * Vector2.down;
            
            if(showDebugRays) Debug.DrawLine(grappleRay.origin, grappleRay.GetPoint(grappleLength), Color.blue);

            return grappleRay.GetPoint(grappleLength);
        }
    }
}