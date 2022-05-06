// Author: Olivier Beauséjour

using System;
using System.Collections;
using Harmony;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class PopupMessage : MonoBehaviour
    {
        public static float NORMAL_SHOWING_TIME = 3f;
        public static float LONG_SHOWING_TIME = 6f;
        public static float FADE_TIME = 0.5f;
        
        [Header("Popup sounds")]
        [SerializeField] private SoundEnum successPopupSound;
        [SerializeField] private SoundEnum warningPopupSound;
        [SerializeField] private SoundEnum neutralPopupSound;
        [SerializeField] private AudioSource successPopupSoundAudioSource;
        [SerializeField] private AudioSource warningPopupSoundAudioSource;
        [SerializeField] private AudioSource neutralPopupSoundAudioSource;
        
        private CanvasGroup popupBox;
        private Text message;
        private AudioManager audioManager;

        private bool poppedUp;

        private void Awake()
        {
            popupBox = GetComponentInChildren<CanvasGroup>();
            message = GetComponentInChildren<Text>();

            audioManager = Finder.AudioManager;

            popupBox.alpha = 0;
            successPopupSoundAudioSource.clip = audioManager.GetAudioClip(successPopupSound);
            warningPopupSoundAudioSource.clip = audioManager.GetAudioClip(warningPopupSound);
            neutralPopupSoundAudioSource.clip = audioManager.GetAudioClip(neutralPopupSound);
        }

        public void ShowPopup(string text, PopupSoundType soundType, float fadeInTime, float stayTime, float fadeOutTime)
        {
            switch (soundType)
            {
                case PopupSoundType.None: break;
                case PopupSoundType.Success: successPopupSoundAudioSource.Play(); break;
                case PopupSoundType.Warning: warningPopupSoundAudioSource.Play(); break;
                case PopupSoundType.Neutral: neutralPopupSoundAudioSource.Play(); break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(soundType), soundType, soundType + " popup type does not exist.");
            }
            
            if (poppedUp) return;

            StartCoroutine(ShowPopupCoroutine(text, fadeInTime, stayTime, fadeOutTime));
        }

        private IEnumerator ShowPopupCoroutine(string text, float fadeInTime, float stayTime, float fadeOutTime)
        {
            message.text = text;
            
            StartCoroutine(FadeCoroutine(popupBox, popupBox.alpha, 1, fadeInTime));
            poppedUp = true;
            
            yield return new WaitForSecondsRealtime(stayTime);
            
            StartCoroutine(FadeCoroutine(popupBox, popupBox.alpha, 0, fadeOutTime));
            poppedUp = false;
        }

        private IEnumerator FadeCoroutine(CanvasGroup canvasGroup, float startOpacity, float endOpacity,
            float totalFadeTime = 1f)
        {
            var timeStartedFade = Time.realtimeSinceStartup;

            while (isActiveAndEnabled)
            {
                var timeElapsed = Time.realtimeSinceStartup - timeStartedFade;
                var percentageComplete = timeElapsed / totalFadeTime;

                canvasGroup.alpha = Mathf.Lerp(startOpacity, endOpacity, percentageComplete);;

                if (percentageComplete >= 1) break;
                
                yield return new WaitForEndOfFrame();
            }
        }

        public enum PopupSoundType
        {
            None,
            Success,
            Warning,
            Neutral
        }
    }
}