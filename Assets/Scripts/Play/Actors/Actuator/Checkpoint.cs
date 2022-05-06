// Authors:
// Olivier Beauséjour
// Derek Pouliot

using Harmony;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace Game
{
    public class Checkpoint : MonoBehaviour
    {
        [Header("Checkpoint visuals")]
        [SerializeField] private Sprite offSprite;
        [SerializeField] private Sprite onSprite;
        
        [Header("Sounds")]
        [SerializeField] private SoundEnum checkpointSound;
        [SerializeField] private AudioSource checkpointSoundAudioSource;
        
        [Header("Respawn camera options")]
        [SerializeField] private float cameraOrthographicSize;
        
        private SpriteRenderer spriteRenderer;
        private Sensor sensor;
        private GameController gameController;
        private AudioManager audioManager;
        
        private ISensor<PlayerController> playerSensors;
        
        private int checkpointId;
        private static int currentCheckpointId;

        public float CameraOrthographicSize => cameraOrthographicSize;
        
        public int CheckpointId
        {
            get => checkpointId;
            set => checkpointId = value;
        }

        private void Awake()
        {
            spriteRenderer = GetComponentInParent<SpriteRenderer>();
            sensor = GetComponent<Sensor>();
            gameController = Finder.GameController;

            playerSensors = sensor.For<PlayerController>();
            audioManager = FindObjectOfType<AudioManager>();
            checkpointId = ++currentCheckpointId;

            checkpointSoundAudioSource.clip = audioManager.GetAudioClip(checkpointSound);
        }

        private void Start()
        {
            if (gameController.CurrentCheckpoint.checkpointId == checkpointId)
                spriteRenderer.sprite = onSprite;
            else 
                spriteRenderer.sprite = offSprite;
        }

        private void OnDestroy()
        {
            currentCheckpointId = 0;
        }

        private void OnEnable()
        {
            playerSensors.OnSensedObject += SetNewCheckpoint;
            playerSensors.OnUnsensedObject += RemoveSensedObject;
        }

        private void OnDisable()
        {
            playerSensors.OnSensedObject -= SetNewCheckpoint;
            playerSensors.OnUnsensedObject -= RemoveSensedObject;
        }

        private void SetNewCheckpoint(PlayerController player)
        {
            if (gameController.CurrentCheckpoint.checkpointId != checkpointId)
            {
                spriteRenderer.sprite = onSprite;
                checkpointSoundAudioSource.Play();
                gameController.SetCheckpoint(this);
            }
        }
        
        private void RemoveSensedObject(PlayerController player)
        {
        }
    }
}