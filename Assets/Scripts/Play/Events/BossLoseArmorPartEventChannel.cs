using Harmony;
using UnityEngine;

// Author : Charles Tremblay
namespace Game
{
    [Findable(R.S.Tag.GameController)]
    public class BossLoseArmorPartEventChannel : MonoBehaviour
    {
        public event BossLoseArmorPartEventHandler OnBossLoseArmorPart;
        
        public void NotifyBossLoseArmorPart()
        {
            if (OnBossLoseArmorPart != null)
                OnBossLoseArmorPart();
        }
    }
    
    public delegate void BossLoseArmorPartEventHandler();
}