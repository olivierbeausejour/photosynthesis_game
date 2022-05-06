// Author: Olivier Beauséjour

using Harmony;

namespace Game
{
    public class PrehistoricTravellerAchievement : BaseLevelCompleteAchievement
    {
        protected override void Awake()
        {
            Name = "Prehistoric Traveller";
            Description = "Beat the second level.";
            
            base.Awake();
        }
        
        protected override void OnLevelComplete(string levelCompleteName, int nbDeathsOnCurrentLevel)
        {
            if (levelCompleteName == R.S.Scene.Level2) Progression++;
        }
    }
}