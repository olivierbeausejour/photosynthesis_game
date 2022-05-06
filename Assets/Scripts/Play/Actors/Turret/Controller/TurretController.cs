//Author:Anthony Dodier
using System;
using System.Collections;
using System.Collections.Generic;
using Harmony;
using UnityEngine;

namespace Game
{
    public class TurretController : MonoBehaviour, ITriggerEnter,ITriggerExit,ITriggerStay
    {
        [SerializeField] private float timeBetweenFire = 0.2f;
        [SerializeField] private float timeBetweenBurst = 3f;
        [SerializeField] private int nbAmmoInBurst = 3;
        [SerializeField] private Transform gunBarrel;
        [SerializeField] private Animator animator;
        [SerializeField] private bool shootRightAlways;
        [SerializeField] private bool ShootLeftAlways;

        private static Quaternion rotationToShootRight = Quaternion.Euler(0, 180, 0);
        private static Quaternion rotationToShootLeft = Quaternion.Euler(0, 0, 0);
        private Spawner spawner;
        private PlayerController playerController;
        
        private bool shootRight;
        float t = 0;
        private bool isShooting;
        private bool isInView;
        private int nbAmmoShot;
        
        public bool IsShooting
        {
            get => isShooting;
            set => isShooting = value;
        }
        
        public bool IsInView
        {
            get => isInView;
            set => isInView = value;
        }
        
        public bool CanShoot { get; set; }

        public bool ShootRight => shootRight;

        private void Start()
        {
            nbAmmoShot = 0;
            if (shootRight)
            {
                gunBarrel.rotation = rotationToShootRight;
            }
            spawner = GetComponentInChildren<Spawner>();
        }

        private void Awake()
        {
            CanShoot = true;
            StartCoroutine(FireCoroutine());
        }

        private IEnumerator FireCoroutine()
        {
            while (isActiveAndEnabled)
            {
                if (isInView && CanShoot)
                { 
                    if (gameObject.transform.parent.name.Contains(R.S.Prefab.Boss))
                        Fire();
                    else
                        FireAnimation();
                    
                    nbAmmoShot++;
                }
                yield return new WaitForSeconds(timeBetweenFire);
                if (nbAmmoShot >= nbAmmoInBurst || !isInView)
                {
                    isShooting = false;
                    nbAmmoShot = 0;
                    yield return new WaitForSeconds(timeBetweenBurst);
                }
            }
        }
        
        private void Update()
        {
            ManageShootingDirection();
        }

        private void ManageShootingDirection()
        {
            if (shootRight)
                gunBarrel.rotation = rotationToShootRight;
            else
                gunBarrel.rotation = rotationToShootLeft;
        }

        public void Fire()
        {
            GameObject projectileToShoot = spawner.Spawn();
            if (projectileToShoot == null) return;
            projectileToShoot.transform.position = gunBarrel.position;
            projectileToShoot.transform.rotation = gunBarrel.rotation;
            projectileToShoot.SetActive(true);
        }

        private void FireAnimation()
        {
            isShooting = true;
        }

        public void OnTriggerDetected(Collider2D other)
        {
            if (!isInView)
            {
                isInView = true;
            }
        }

        public void OnTriggerExitDetected(Collider2D other)
        {
            isInView = false;
            nbAmmoShot = 0;
        }

        public void OnTriggerStayDetected(Collider2D other)
        {
            if (shootRightAlways)
            {
                shootRight = true;
                return;
            }
            else if (ShootLeftAlways)
            {
                shootRight = false;
                return;
            }
            
            if (other.transform.position.x > transform.position.x)
                shootRight = true;
            else
                shootRight = false;
        }
    }
}