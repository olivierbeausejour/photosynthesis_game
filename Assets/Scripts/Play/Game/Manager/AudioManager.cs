﻿//Authors:
 //Anthony Dodier
 //Charles Tremblay
 //Olivier Beauséjour
 
 using System;
 using Harmony;
 using UnityEngine;

namespace Game
{
    [Findable("AudioManager")]
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private Sound[] sounds;
        private AudioSource specificSound;
        private Camera camera;

        public void Awake()
        {
            foreach (Sound sound in sounds)
            {
                sound.source = gameObject.AddComponent<AudioSource>();
                sound.source.clip = sound.clip;
                sound.source.volume = sound.volume;
                sound.source.spatialBlend = sound.spatialBlend;
                sound.source.rolloffMode = AudioRolloffMode.Linear;
                sound.source.maxDistance = sound.maxDistance;
                sound.source.minDistance = sound.minDistance;
                sound.source.loop = sound.loop;
            }
            camera= Camera.main;
        }

        private void Play(string soundName)
        {
            Sound soundToPlay = Array.Find(sounds, sound => sound.name == soundName);
            if(soundToPlay == null)
            {
                Debug.LogWarning("Sound: " + soundName + " not found");
                return;
            }
            soundToPlay.source.Play();
        }
        public void Play(SoundEnum soundEnum,Transform soundPosition)
        {
            SetSoundPosition(soundPosition);
            string soundToPlay = FindSoundNameWithEnum(soundEnum);
            Play(soundToPlay);
        }

        public AudioClip GetAudioClip(SoundEnum soundEnum)
        {
            string soundToReturn = FindSoundNameWithEnum(soundEnum);
            return Array.Find(sounds, sound => sound.name == soundToReturn).clip;
        }

        private string FindSoundNameWithEnum(SoundEnum soundEnum)
        {
            switch (soundEnum)
            {
                case SoundEnum.level1Theme:
                    return "Level1-Theme";
                case SoundEnum.level2Theme:
                    return "Level2-Theme";
                case SoundEnum.level3Theme:
                    return "Level3-Theme";
                case SoundEnum.level4Theme:
                    return "Level4-Theme";
                case SoundEnum.level5Theme:
                    return "Level5-Theme";
                case SoundEnum.bossTheme:
                    return "";
                case SoundEnum.mainMenuTheme:
                    return "MainMenu-Theme";
                case SoundEnum.creditsTheme:
                    return "Credits-Theme";
                case SoundEnum.deathPlayerSound:
                    return "PlayerDeathSound";
                case SoundEnum.runPlayerSound:
                    return "PlayerRunSound";
                case SoundEnum.jumpPlayerSound:
                    return "PlayerJumpSound";
                case SoundEnum.landPlayerSound:
                    return "PlayerLandSound";
                case SoundEnum.dashPlayerSound:
                    return "PlayerDashSound";
                case SoundEnum.hookDeploySound:
                    return "GrapplingHookFiringDeploymentSound";
                case SoundEnum.hookImpactSound:
                    return "GrapplingHookImpactSound";
                case SoundEnum.hookRetractSound:
                    return "GrapplingHookRetractSound";
                case SoundEnum.hookSwaySound:
                    return "GrapplingHookSwaySound";
                case SoundEnum.destroyableWalls:
                    return "DestroyableWallsSound";
                case SoundEnum.keysCollected:
                    return "KeySound";
                case SoundEnum.doorOpen:
                    return "DoorSound";
                //TODO if levels share enemy sounds : remove cases that are not necessary anymore
                case SoundEnum.level1BaseEnemyDeath:
                    return "Level1BaseEnemyDeathSound";
                case SoundEnum.level1BaseEnemyIdle:
                    return "Level1BaseEnemyIdleSound";
                case SoundEnum.level1JumpingEnemyDeath:
                    return "Level1JumpingEnemySound";
                case SoundEnum.level1JumpingEnemyIdle:
                    return "Level1JumpingEnemyIdleSound";
                case SoundEnum.level1JumpingEnemyJump:
                    return "Level1JumpingEnemyJumpSound";
                case SoundEnum.level1ShootingEnemyDeath:
                    return "Level1ShootingEnemyDeathSound";
                case SoundEnum.level1ShootingEnemyIdle:
                    return "Level1ShootingEnemyIdleSound";
                case SoundEnum.level1ShootingEnemyShoot:
                    return "Level1ShootingEnemyShootSound";
                case SoundEnum.level2BaseEnemyDeath:
                    return "Level2BaseEnemyDeathSound";
                case SoundEnum.level2BaseEnemyIdle:
                    return "Level2BaseEnemyIdleSound";
                case SoundEnum.level2JumpingEnemyDeath:
                    return "Level2JumpingEnemyDeathSound";
                case SoundEnum.level2JumpingEnemyIdle:
                    return "Level2JumpingEnemyIdleSound";
                case SoundEnum.level2JumpingEnemyJump:
                    return "Level2JumpingEnemyJumpSound";
                case SoundEnum.level2ShootingEnemyDeath:
                    return "Level2ShootingEnemyDeathSound";
                case SoundEnum.level2ShootingEnemyIdle:
                    return "Level2ShootingEnemyIdleSound";
                case SoundEnum.level2ShootingEnemyShoot:
                    return "Level2ShootingEnemyShootSound";
                case SoundEnum.level3BaseEnemyDeath:
                    return "Level3BaseEnemyDeathSound";
                case SoundEnum.level3BaseEnemyIdle:
                    return "Level3BaseEnemyIdleSound";
                case SoundEnum.level3JumpingEnemyDeath:
                    return "Level3JumpingEnemySound";
                case SoundEnum.level3JumpingEnemyIdle:
                    return "Level3JumpingEnemyIdleSound";
                case SoundEnum.level3JumpingEnemyJump:
                    return "Level3JumpingEnemyJumpSound";
                case SoundEnum.level3ShootingEnemyDeath:
                    return "Level3ShootingEnemyDeathSound";
                case SoundEnum.level3ShootingEnemyIdle:
                    return "Level3ShootingEnemyIdleSound";
                case SoundEnum.level3ShootingEnemyShoot:
                    return "Level3ShootingEnemyShootSound";
                case SoundEnum.level4BaseEnemyDeath:
                    return "Level4BaseEnemyDeathSound";
                case SoundEnum.level4BaseEnemyIdle:
                    return "Level4BaseEnemyIdleSound";
                case SoundEnum.level4JumpingEnemyDeath:
                    return "Level4JumpingEnemySound";
                case SoundEnum.level4JumpingEnemyIdle:
                    return "Level4JumpingEnemyIdleSound";
                case SoundEnum.level4JumpingEnemyJump:
                    return "Level4JumpingEnemyJumpSound";
                case SoundEnum.level4ShootingEnemyDeath:
                    return "Level4ShootingEnemyDeathSound";
                case SoundEnum.level4ShootingEnemyIdle:
                    return "Level4ShootingEnemyIdleSound";
                case SoundEnum.level4ShootingEnemyShoot:
                    return "Level4ShootingEnemyShootSound";
                case SoundEnum.level5BaseEnemyDeath:
                    return "Level5BaseEnemyDeathSound";
                case SoundEnum.level5BaseEnemyIdle:
                    return "Level5BaseEnemyIdleSound";
                case SoundEnum.level5JumpingEnemyDeath:
                    return "Level5JumpingEnemySound";
                case SoundEnum.level5JumpingEnemyIdle:
                    return "Level5JumpingEnemyIdleSound";
                case SoundEnum.level5JumpingEnemyJump:
                    return "Level5JumpingEnemyJumpSound";
                case SoundEnum.level5ShootingEnemyDeath:
                    return "Level5ShootingEnemyDeathSound";
                case SoundEnum.level5ShootingEnemyIdle:
                    return "Level5ShootingEnemyIdleSound";
                case SoundEnum.level5ShootingEnemyShoot:
                    return "Level5ShootingEnemyShootSound";
                case SoundEnum.fallingPlatform:
                    return "FallingPlatformSound";
                case SoundEnum.filmRollCollected:
                    return "FilmRollPickUpSound";
                case SoundEnum.checkpointSound:
                    return "CheckpointSound";
                case SoundEnum.fallingHazard:
                    return "FallingHazardSound";
                case SoundEnum.movingPlatform:
                    return "MovingPlatformSound";
                case SoundEnum.switchSound:
                    return "SwitchSound";
                case SoundEnum.pauseUnpauseSound:
                    return "PauseUnpauseSound";
                case SoundEnum.hoverItemSound:
                    return "HoverItemSound";
                case SoundEnum.selectItemSound:
                    return "SelectItemSound";
                case SoundEnum.selectSaveSound:
                    return "SelectSaveSound";
                case SoundEnum.bossJump:
                    return "BossJumpSound";
                case SoundEnum.bossHurt:
                    return "BossHurtSound";
                case SoundEnum.bossDeath:
                    return "BossDeathSound";
                case SoundEnum.bossReload:
                    return "BossReloadSound";
            }
            return null;
        }
        
        private void SetSoundPosition(Transform soundPosition)
        {
            var audioManagerPosition = soundPosition.position;
            audioManagerPosition.z = camera.transform.position.z;
            transform.position = audioManagerPosition;
        }
    }
}