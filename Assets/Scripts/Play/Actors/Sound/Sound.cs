//Author:Anthony Dodier
using System;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class Sound
    {
        [SerializeField]public string name;
        [SerializeField]public AudioClip clip;
        [Range(0f,1f)]
        [SerializeField]public float spatialBlend;
        [Range(0f,1f)]
        [SerializeField]public float volume;
        [SerializeField] [Range(0f,20f)] public float maxDistance;
        [SerializeField] [Range(0f,4f)] public float minDistance;
        [SerializeField]public bool playOnAwake;
        [SerializeField]public bool loop;
        

        [HideInInspector]
        public AudioSource source;
    }
}