using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game
{
    public class AudioPlayer : MonoBehaviour
    {
        private Camera camera;
        private bool isFirstUpdate;
        
        private void Awake()
        {
            isFirstUpdate = true;
            camera = Camera.main;
           
        }

        // Update is called once per frame
        private void Update()
        {
            SetSoundPosition();
        }

        public void SetSoundPosition()
        {
            var audioPlayerPosition = transform.position;
            audioPlayerPosition.z = camera.transform.position.z;
            transform.position = audioPlayerPosition;
        }
    }
}
