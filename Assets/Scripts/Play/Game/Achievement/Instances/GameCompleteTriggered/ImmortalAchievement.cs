// Author: Olivier Beauséjour

namespace Game
{
    public class ImmortalAchievement : BaseGameCompleteAchievement
    {
        private const int NB_DEATHS_TO_UNLOCK_ACHIEVEMENT = 0;
        
        protected override void Awake()
        {
            Name = "Immortal";
            Description = "Beat all the levels without dying.";
            
            base.Awake();
        }

        protected override void OnGameComplete(int totalNbDeaths)
        {
            if (totalNbDeaths == NB_DEATHS_TO_UNLOCK_ACHIEVEMENT) Progression++;
        }
    }
}