// Author: Derek Pouliot

using Harmony;
using UnityEngine;

namespace Game
{
    [Findable(R.S.Tag.GameController)]
    public class DashOnBossWeakSpotEventChannel : MonoBehaviour
    {
        public event DashOnBossWeakSpotEventHandler OnDashOnBossWeakSpot;
    
        public void NotifyDashOnBossWeakSpot()
        {
            if (OnDashOnBossWeakSpot != null)
                OnDashOnBossWeakSpot();
        }
    }

    public delegate void DashOnBossWeakSpotEventHandler();
}