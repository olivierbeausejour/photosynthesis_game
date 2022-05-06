// Author: Olivier Beauséjour

namespace Game
{
    public class FullOnAdventurerAchievement : BaseGameCompleteAchievement
    {
        protected override void Awake()
        {
            Name = "Full-On Adventurer";
            Description = "Beat all the levels.";
            
            base.Awake();
        }

        protected override void OnGameComplete(int totalNbDeaths)
        {
            Progression++;
        }
    }
}