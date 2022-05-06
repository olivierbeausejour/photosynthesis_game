//Authors:
//Charles Tremblay

using Harmony;
using UnityEngine;

namespace Game
{
    [Findable(R.S.Tag.GameController)]
    public class PlayerRespawnEventChannel : MonoBehaviour
    {
        public event PlayerRespawnEventHandler OnPlayerRespawn;

        public void NotifyPlayerRespawn()
        {
            if (OnPlayerRespawn != null)
                OnPlayerRespawn();
        }
    }

    public delegate void PlayerRespawnEventHandler();
}