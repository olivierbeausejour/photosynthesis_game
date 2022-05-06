// Author: Olivier Beauséjour

using Harmony;

namespace Game
{
    public class SpaceManAchievement : BaseLevelCompleteAchievement
    {
        protected override void Awake()
        {
            Name = "Space Man";
            Description = "Beat the fourth level.";
            
            base.Awake();
        }
        
        protected override void OnLevelComplete(string levelCompleteName, int nbDeathsOnCurrentLevel)
        {
            if (levelCompleteName == R.S.Scene.Level4) Progression++;
        }
    }
}