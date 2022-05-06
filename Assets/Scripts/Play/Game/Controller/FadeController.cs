// Author: Anthony Dodier

using System.Collections;
using Harmony;
using UnityEngine;

namespace Game
{
    public class FadeController : MonoBehaviour
    {
        private const string FADE_OUT_LEVEL_END = "FadeOut";
        private const string FADE_OUT_PLAYER_DEATH = "FadeOutPlayerDeath";
        private LevelCompleteEventChannel levelCompleteEventChannel;
        private PlayerDeathEventChannel playerDeathEventChannel;
        private Animator animator;

        private void Awake()
        {
            levelCompleteEventChannel = Finder.LevelCompleteEventChannel;
            playerDeathEventChannel = Finder.PlayerDeathEventChannel;
            animator = GetComponent<Animator>();
        }
        
        private void OnEnable()
        {
            levelCompleteEventChannel.OnLevelComplete += FadeToLevel;
            playerDeathEventChannel.OnPlayerDeath += FadeToRespawn;
        }

        private void OnDisable()
        {
            levelCompleteEventChannel.OnLevelComplete -= FadeToLevel;
            playerDeathEventChannel.OnPlayerDeath -= FadeToRespawn;
        }

        private void FadeToRespawn()
        {
            animator.SetBool(FADE_OUT_PLAYER_DEATH, true);
            StartCoroutine(WaitForAnimationToFinish());
        }

        private void FadeToLevel(string levelCompleteName, int nbDeathsonCurrentLevel)
        {
            animator.SetBool(FADE_OUT_LEVEL_END, true);
        }
        
        private IEnumerator WaitForAnimationToFinish()
        {
            while (isActiveAndEnabled)
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName(FADE_OUT_PLAYER_DEATH))
                {
                    animator.SetBool(FADE_OUT_PLAYER_DEATH, false);
                    yield break;
                }
                yield return null;
            }
        }
    }

}

