// Author : Derek Pouliot

using Harmony;
using UnityEngine;

namespace Game
{
    [Findable(R.S.Tag.GameController)]
    public class PlatformHasCrushedPlayerEventChannel : MonoBehaviour
    {
        public event PlatformHasCrushedPlayerEventHandler OnPlayerCrushedByPlatform;

        public void NotifyPlatformHasCrushedPlayer()
        {
            if (OnPlayerCrushedByPlatform != null)
                OnPlayerCrushedByPlatform();
        }
    }
    
    public delegate void PlatformHasCrushedPlayerEventHandler();
}