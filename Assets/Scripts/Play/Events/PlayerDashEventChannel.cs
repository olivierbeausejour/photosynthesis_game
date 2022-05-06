//Authors:
//Olivier Beauséjour

using Harmony;
using UnityEngine;

namespace Game
{
    [Findable(R.S.Tag.GameController)]
    public class PlayerDashEventChannel : MonoBehaviour
    {
        public event PlayerDashEventHandler OnPlayerDash;
            
        public void NotifyPlayerDash()
        {
            if (OnPlayerDash != null)
                OnPlayerDash();
        }
    }

    public delegate void PlayerDashEventHandler();
}