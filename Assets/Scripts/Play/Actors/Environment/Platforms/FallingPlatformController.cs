//Author:Anthony Dodier

using System.Collections;
using Game;
using Harmony;
using UnityEngine;

public class FallingPlatformController : MonoBehaviour, ITriggerEnter
{
    [SerializeField] private SoundEnum fallingSound;
    [SerializeField] private float timeBeforePlatformFall = 3;
    [SerializeField] private float timeBeforeDestroy =2;
    [SerializeField] private float timeBeforeRespawn =2;
    [SerializeField] private float downSpeed = 5;
    [SerializeField] private bool respawnOnPlayerDeathOnly;
    
    private PlayerRespawnEventChannel playerRespawnEventChannel;
    
    private BoxCollider2D fallingTrigger;
    private Vector2 currentPosition;
    private AudioManager audioManager;
    private AudioSource audioSource;
    private bool isNotFalling;
    
    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        audioSource = GetComponentInChildren<AudioSource>();
        audioSource.clip = audioManager.GetAudioClip(fallingSound);
        currentPosition = transform.position;
        isNotFalling = false;
        playerRespawnEventChannel = Finder.PlayerRespawnEventChannel;
    }

    private void OnEnable()
    {
        playerRespawnEventChannel.OnPlayerRespawn += ResetPlatform;
    }

    private void OnDisable()
    {
        playerRespawnEventChannel.OnPlayerRespawn += ResetPlatform;
    }

    private void DeactivatePlatform()
    {
        foreach (Transform child in transform)     
        {  
            child.gameObject.SetActive(false);   
        }
    }
    
    private void ResetPlatform()
    {
        transform.position = currentPosition;
        isNotFalling = false;
        foreach (Transform child in transform)     
        {  
            child.gameObject.SetActive(true);   
        }
    }

    public void OnTriggerDetected(Collider2D other)
    {
        if (!isNotFalling)
        {
            isNotFalling = true;
            StartCoroutine(TriggerCoroutine());
        }
        
    }

    private IEnumerator TriggerCoroutine()
    {
        yield return new WaitForSeconds(timeBeforePlatformFall);
        audioSource.Play();
        float timer = 0f;
        while (timer < timeBeforeDestroy)
        {
            transform.Translate(Time.deltaTime * downSpeed * Vector2.down);
            timer += Time.deltaTime;
            yield return null;
        }
        DeactivatePlatform();
        if (!respawnOnPlayerDeathOnly)
        {
            yield return new WaitForSeconds(timeBeforeRespawn);
            ResetPlatform();
        }
    }
}
