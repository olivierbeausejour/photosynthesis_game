//Authors:
//Charles Tremblay

using Harmony;
using UnityEngine;

namespace Game
{
    [Findable(R.S.Tag.GameController)]
    public class EnemyIsDoneBeingPulledEventChannel : MonoBehaviour
    {
        public event EnemyIsDoneBeingPulledEventHandler OnEnemyDoneBeingPulled;

        public void NotifyEnemyDoneBeingPulled()
        {
            if (OnEnemyDoneBeingPulled != null)
                OnEnemyDoneBeingPulled();
        }
    }

    public delegate void EnemyIsDoneBeingPulledEventHandler();
}