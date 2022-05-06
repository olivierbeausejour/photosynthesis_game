//Authors:
//Charles Tremblay

using Harmony;
using UnityEngine;

namespace Game
{
    [Findable(R.S.Tag.GameController)]
    public class ShootingEnemyShootEventChannel : MonoBehaviour
    {
        public event ShootingEnemyShootEventHandler OnEnemyShot;

        public void NotifyEnemyShot()
        {
            if (OnEnemyShot != null)
                OnEnemyShot();
        }
    }

    public delegate void ShootingEnemyShootEventHandler();
}