//Authors:
//Charles Tremblay

using Harmony;
using UnityEngine;

namespace Game
{
    [Findable(R.S.Tag.GameController)]
    public class PlayerIsDoneDashingEventChannel : MonoBehaviour
    {
        public event PlayerIsDoneDashingEventHandler OnPlayerDoneDashing;

        public void NotifyPlayerIsDoneDashing()
        {
            if (OnPlayerDoneDashing != null)
                OnPlayerDoneDashing();
        }
    }
    
    public delegate void PlayerIsDoneDashingEventHandler();
}