//Authors:
//Charles Tremblay

using Harmony;
using UnityEngine;

namespace Game
{
    public class MovingPlatformTriggerBoxController : MonoBehaviour
    {
        private MovingPlatformController movingPlatformController;
        private PlatformHasCrushedPlayerEventChannel platformHasCrushedPlayerEventChannel;

        private void Awake()
        {
            movingPlatformController = GetComponentInParent<MovingPlatformController>();
            platformHasCrushedPlayerEventChannel = Finder.PlatformHasCrushedPlayerEventChannel;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(R.S.Layer.Player))
            {
                var player = other.transform.parent;
                
                if(player.GetComponent<PlayerController>().BaseActuator.Grounded && transform.gameObject.layer == LayerMask.NameToLayer(R.S.Layer.KillTriggerBox))
                    platformHasCrushedPlayerEventChannel.NotifyPlatformHasCrushedPlayer();
                
                player.SetParent(transform.parent.transform);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(R.S.Layer.Player))
            {
                movingPlatformController.DetachPlayerFromPlatform(other.transform.parent);
            }
        }
    }
}