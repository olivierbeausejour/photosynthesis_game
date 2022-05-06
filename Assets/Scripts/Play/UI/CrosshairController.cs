// Author: Jonathan Mathieu

using UnityEngine;

namespace Game
{
    public class CrosshairController : MonoBehaviour
    {
        [SerializeField] private float crosshairDistance = 4f;
        [SerializeField] private GameObject visual;

        private SpriteRenderer spriteRenderer;
        private PlayerInputManager inputManager;

        private void Awake()
        {
            inputManager = GetComponentInParent<PlayerInputManager>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }

        private void Start()
        {
            visual.transform.localPosition = Vector2.right * crosshairDistance;
        }

        private void Update()
        {
            if (inputManager.IsUsingGamepad)
            {
                if (inputManager.GrappleAimDirection != Vector2.zero)
                {
                    visual.transform.position = (Vector2)transform.position + (inputManager.GrappleAimDirection * crosshairDistance);
                    visual.transform.right = transform.position - visual.transform.position;
                }

            }
            else
            {
                visual.transform.position = inputManager.MouseScreenPosition;
            }
            
        }
        
    }
}