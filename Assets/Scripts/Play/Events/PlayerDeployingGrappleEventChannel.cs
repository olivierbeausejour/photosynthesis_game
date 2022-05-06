//Authors:
//Olivier Beauséjour

using Harmony;
using UnityEngine;

namespace Game
{
    [Findable(R.S.Tag.GameController)]
    public class PlayerDeployingGrappleEventChannel : MonoBehaviour
    {
        public event PlayerDeployingGrappleEventHandler OnPlayerDeployingGrapple;
            
        public void NotifyPlayerDeployingGrapple()
        {
            if (OnPlayerDeployingGrapple != null)
                OnPlayerDeployingGrapple();
        }
    }

    public delegate void PlayerDeployingGrappleEventHandler();
}