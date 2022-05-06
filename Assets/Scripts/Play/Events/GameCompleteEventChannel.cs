//Authors:
//Olivier Beauséjour

using Harmony;
using UnityEngine;

namespace Game
{
    [Findable(R.S.Tag.GameController)]
    public class GameCompleteEventChannel : MonoBehaviour
    {
        public event GameCompleteEventHandler OnGameComplete;

        public void NotifyGameComplete(int totalNbDeaths)
        {
            if (OnGameComplete != null)
                OnGameComplete(totalNbDeaths);
        }
    }

    public delegate void GameCompleteEventHandler(int totalNbDeaths);
}