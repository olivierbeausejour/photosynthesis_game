//Authors:
//Olivier Beauséjour

using Harmony;
using UnityEngine;

namespace Game
{
    [Findable(R.S.Tag.GameController)]
    public class LevelCompleteEventChannel : MonoBehaviour
    {
        public event LevelCompleteEventHandler OnLevelComplete;

        public void NotifyLevelComplete(string levelCompleteName, int nbDeathsOnCurrentLevel)
        {
            if (OnLevelComplete != null)
                OnLevelComplete(levelCompleteName, nbDeathsOnCurrentLevel);
        }
    }

    public delegate void LevelCompleteEventHandler(string levelCompleteName, int nbDeathsOnCurrentLevel);
}