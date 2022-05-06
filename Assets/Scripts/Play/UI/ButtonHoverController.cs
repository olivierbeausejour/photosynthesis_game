// Author: Olivier Beauséjour

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    public class ButtonHoverController : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField] private SoundEnum soundToPlay;
        [SerializeField] private AudioSource soundToPlayAudioSource;
        
        private Selectable currentButton;
        private AudioManager audioManager;

        private void Awake()
        {
            currentButton = GetComponent<Selectable>();
            audioManager = FindObjectOfType<AudioManager>();
            
            soundToPlayAudioSource.clip = audioManager.GetAudioClip(soundToPlay);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (currentButton.interactable)
                soundToPlayAudioSource.Play();
        }
    }
}