// Author : Derek Pouliot

using Harmony;
using UnityEngine;

namespace Game
{
    [Findable(R.S.Tag.GameController)]
    public class BossHasBeenActivatedEventChannel : MonoBehaviour
    {
        public event BossHasBeenActivatedEventHandler OnBossActivated;

        public void NotifyBossHasBeenActivated()
        {
            if (OnBossActivated != null)
                OnBossActivated();
        }
    }
    
    public delegate void BossHasBeenActivatedEventHandler();
}