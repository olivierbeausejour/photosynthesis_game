// Author: Olivier Beauséjour

using Harmony;
using UnityEngine;

namespace Game
{
    public abstract class BaseAchievement : MonoBehaviour
    {
        private AchievementUnlockedEventChannel achievementUnlockedEventChannel;
        private int progression;

        public string Name { get; protected set; }

        public string Description { get; protected set; }

        public int Progression
        {
            get => progression;
            set
            {
                if (IsCompleted) return;

                progression = value;
                
                if (IsCompleted) achievementUnlockedEventChannel.NotifyAchievementUnlocked(Name, Description);
            }
        }

        public int GoalValue { get; protected set; }

        public bool IsCompleted => Progression >= GoalValue;

        protected virtual void Awake()
        {
            achievementUnlockedEventChannel = Finder.AchievementUnlockedEventChannel;
        }
    }
}