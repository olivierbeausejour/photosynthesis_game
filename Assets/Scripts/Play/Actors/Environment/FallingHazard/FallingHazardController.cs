//Authors:
//Charles Tremblay

using System.Collections;
using Harmony;
using UnityEngine;

namespace Game
{
    public class FallingHazardController : MonoBehaviour, ITriggerEnter
    {
        [SerializeField] private SoundEnum fallingHazardSound;
        [SerializeField] private float secondsBeforeHazardFalls = 1.5f;
        [SerializeField] private float maxFallingTime = 3f;
        [SerializeField] private float fallingSpeedAcceleration = 2f;
        
        private PlayerDeathEventChannel playerDeathEventChannel;
        private PlayerRespawnEventChannel playerRespawnEventChannel;
        private GameController gameController;
        private AudioManager audioManager;
        private AudioSource audioSource;
        
        private bool isTriggered;
        private Vector2 startPosition;

        private void Awake()
        {
            isTriggered = false;
            startPosition = transform.position;
            playerDeathEventChannel = Finder.PlayerDeathEventChannel;
            playerRespawnEventChannel = Finder.PlayerRespawnEventChannel;
            gameController = Finder.GameController;
            audioManager = FindObjectOfType<AudioManager>();
            audioSource = GetComponentInChildren<AudioSource>();
            audioSource.clip = audioManager.GetAudioClip(fallingHazardSound);
        }

        private void OnEnable()
        {
            playerDeathEventChannel.OnPlayerDeath += Deactivate;
            playerRespawnEventChannel.OnPlayerRespawn += Respawn;
        }

        private void OnDisable()
        {
            playerDeathEventChannel.OnPlayerDeath -= Deactivate;
            playerRespawnEventChannel.OnPlayerRespawn -= Respawn;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isTriggered) return;
            
            if (other.gameObject.layer != LayerMask.NameToLayer(R.S.Layer.Player) &&
                other.gameObject.layer != LayerMask.NameToLayer(R.S.Layer.TriggerBox) && gameController.IsPlayerAlive)
            {
                Deactivate();
            }
                
        }

        private void Deactivate()
        {
            isTriggered = false;
            SetChildrenActive(false);
        }

        private void Respawn()
        {
            SetChildrenActive(true);
            transform.position = startPosition;
        }

        private void SetChildrenActive(bool value)
        {
            foreach (GameObject sibling in gameObject.Parent().Children())
            {
                foreach (GameObject child in sibling.Children())
                {
                    child.SetActive(value);
                }
            }
        }

        public void OnTriggerDetected(Collider2D other)
        {
            if (!isTriggered)
            {
                isTriggered = true;
                StartCoroutine(TriggerCoroutine());
            }
        }

        private IEnumerator TriggerCoroutine()
        {
            float acceleration = 0f;
            
            yield return new WaitForSeconds(secondsBeforeHazardFalls);
            float t = 0f;
            audioSource.Play();
            while (isTriggered)
            {
                
                acceleration += fallingSpeedAcceleration;
                transform.Translate(Time.deltaTime * acceleration * Vector2.down);
                t += Time.deltaTime;
                if (t > maxFallingTime)
                {
                    break;
                }
                yield return null;
            }
        }
    }
}