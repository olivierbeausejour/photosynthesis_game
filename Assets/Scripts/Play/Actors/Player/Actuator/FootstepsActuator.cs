using System;
using UnityEngine;

namespace Game
{
    public class FootstepsActuator : MonoBehaviour
    {
        [SerializeField] private AudioSource runSoundAudioSource;
        [SerializeField] private SoundEnum runSound;

        private AudioManager audioManager;


        private void Awake()
        {
            audioManager = FindObjectOfType<AudioManager>();
            
            runSoundAudioSource.clip = audioManager.GetAudioClip(runSound);
        }

        private void PlayFootstep()
        {
            runSoundAudioSource.Play();
        }
    }
}