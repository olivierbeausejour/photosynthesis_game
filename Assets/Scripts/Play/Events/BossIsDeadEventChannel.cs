// Author : Derek Pouliot

using Harmony;
using UnityEngine;

namespace Game
{
    [Findable(R.S.Tag.GameController)]
    public class BossIsDeadEventChannel : MonoBehaviour
    {
        public event BossIsDeadEventHandler OnBossKilled;
        
        public void NotifyBossIsDead()
        {
            if (OnBossKilled != null)
                OnBossKilled();
        }
    }
    
    public delegate void BossIsDeadEventHandler();
}