// Author: Olivier Beauséjour

namespace Game
{
    public class HardOneAchievement : BaseLevelCompleteAchievement
    {
        private const int NB_DEATHS_TO_UNLOCK_ACHIEVEMENT = 100;
        
        protected override void Awake()
        {
            Name = "Hard One";
            Description = "Die At Least 100 times trying to complete a level.";
            
            base.Awake();
        }

        protected override void OnLevelComplete(string levelCompleteName, int nbDeathsOnCurrentLevel)
        {
            if (nbDeathsOnCurrentLevel >= NB_DEATHS_TO_UNLOCK_ACHIEVEMENT) Progression++;
        }
    }
}