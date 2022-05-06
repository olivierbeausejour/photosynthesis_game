// Author: Olivier Beauséjour

using System;
using Harmony;

namespace Game
{
    public abstract class BaseGameCompleteAchievement : BaseAchievement
    {
        protected GameCompleteEventChannel gameCompleteEventChannel;
        
        protected override void Awake()
        {
            GoalValue = 1;
            gameCompleteEventChannel = Finder.GameCompleteEventChannel;
            
            base.Awake();
        }

        private void OnEnable()
        {
            gameCompleteEventChannel.OnGameComplete += OnGameComplete;
        }

        private void OnDisable()
        {
            gameCompleteEventChannel.OnGameComplete -= OnGameComplete;
        }

        protected abstract void OnGameComplete(int totalNbDeaths);
    }
}