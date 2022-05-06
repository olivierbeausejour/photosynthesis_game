using System;
using Harmony;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game
{
    public class Parallax : MonoBehaviour
    {
        [SerializeField] private float parallaxEffect;
        [SerializeField] private GameObject cam;
        [SerializeField] private float zPos = -1f;
        
        private Renderer spriteRenderer;
        
        private float length;
        private float startPosX;
        private float startPosY;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>() ? spriteRenderer = GetComponent<SpriteRenderer>() : spriteRenderer = GetComponent<TilemapRenderer>();
        }

        private void Start()
        {
            startPosX = transform.position.x;
            startPosY = transform.position.y;
            length = spriteRenderer.bounds.size.x;
        }

        private void Update()
        {
            ParallaxInXAxis();
        }
        

        private void ParallaxInXAxis()
        {
            var camPosition = cam.transform.position;

            var distanceTravelled = camPosition.x * (1 - parallaxEffect);
            var distanceX = camPosition.x * parallaxEffect;

            transform.position = new Vector3(startPosX + distanceX, transform.position.y, zPos);

            if (distanceTravelled > startPosX + length) startPosX += length;
            else if (distanceTravelled < startPosX - length) startPosX -= length;
        }
    }
}