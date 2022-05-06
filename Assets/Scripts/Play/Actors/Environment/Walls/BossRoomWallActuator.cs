// Author : Derek Pouliot

using System;
using Harmony;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game 
{
    public class BossRoomWallActuator : MonoBehaviour
    {
        private TilemapCollider2D wallCollider;
        private TilemapRenderer wallRenderer;
        
        private BossIsDeadEventChannel bossIsDeadEventChannel;
        private BossHasBeenActivatedEventChannel bossHasBeenActivatedEventChannel;
        private PlayerRespawnEventChannel playerRespawnEventChannel;

        private void Awake()
        {
            wallCollider = GetComponent<TilemapCollider2D>();
            wallRenderer = GetComponent<TilemapRenderer>();
            
            bossIsDeadEventChannel = Finder.BossIsDeadEventChannel;
            bossHasBeenActivatedEventChannel = Finder.BossHasBeenActivatedEventChannel;
            playerRespawnEventChannel = Finder.PlayerRespawnEventChannel;
        }

        private void OnEnable()
        {
            bossIsDeadEventChannel.OnBossKilled += Disappear;
            bossHasBeenActivatedEventChannel.OnBossActivated += Appear;
            playerRespawnEventChannel.OnPlayerRespawn += Disappear;
        }

        private void OnDisable()
        {
            bossIsDeadEventChannel.OnBossKilled -= Disappear;
            bossHasBeenActivatedEventChannel.OnBossActivated -= Appear;
            playerRespawnEventChannel.OnPlayerRespawn -= Disappear;
        }

        private void Start()
        {
            Disappear();
        }

        private void Appear()
        {
            wallCollider.enabled = true;
            wallRenderer.enabled = true;
        }

        private void Disappear()
        {
            wallCollider.enabled = false;
            wallRenderer.enabled = false;
        }
    }
}