// Author: Olivier Beauséjour

using Harmony;

namespace Game
{
    public class JustTheBeginningAchievement : BaseLevelCompleteAchievement
    {
        protected override void Awake()
        {
            Name = "Just The Beginning";
            Description = "Beat the first level.";
            
            base.Awake();
        }
        
        protected override void OnLevelComplete(string levelCompleteName, int nbDeathsOnCurrentLevel)
        {
            if (levelCompleteName == R.S.Scene.Level1) Progression++;
        }
    }
}