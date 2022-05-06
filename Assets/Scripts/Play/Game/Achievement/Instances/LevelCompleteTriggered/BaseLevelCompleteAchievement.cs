// Author: Olivier Beauséjour

using Harmony;

namespace Game
{
    public abstract class BaseLevelCompleteAchievement : BaseAchievement
    {
        protected LevelCompleteEventChannel levelCompleteEventChannel;

        protected override void Awake()
        {
            GoalValue = 1;
            levelCompleteEventChannel = Finder.LevelCompleteEventChannel;
            
            base.Awake();
        }

        private void OnEnable()
        {
            levelCompleteEventChannel.OnLevelComplete += OnLevelComplete;
        }

        private void OnDisable()
        {
            levelCompleteEventChannel.OnLevelComplete -= OnLevelComplete;
        }

        protected abstract void OnLevelComplete(string levelCompleteName, int nbDeathsOnCurrentLevel);
    }
}