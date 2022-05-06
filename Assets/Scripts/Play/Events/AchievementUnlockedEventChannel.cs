//Authors:
//Olivier Beauséjour

using Harmony;
using UnityEngine;

namespace Game
{
    [Findable(R.S.Tag.GameController)]
    public class AchievementUnlockedEventChannel : MonoBehaviour
    {
        public event AchievementUnlockedEventHandler OnAchievementUnlocked;

        public void NotifyAchievementUnlocked(string achievementName, string achievementDescription)
        {
            if (OnAchievementUnlocked != null)
                OnAchievementUnlocked(achievementName, achievementDescription);
        }
    }

    public delegate void AchievementUnlockedEventHandler(string achievementName, string achievementDescription);
}