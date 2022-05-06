// Author: Olivier Beauséjour

using Harmony;

namespace Game
{
    public class TheFirstOfManyAchievement : BasePlayerActionAchievement
    {
        private const int NB_DEATHS_TO_UNLOCK_ACHIEVEMENT = 1;
        
        private PlayerDeathEventChannel playerDeathEventChannel;

        protected override void Awake()
        {
            Name = "The First Of Many";
            Description = "Die once.";
            GoalValue = NB_DEATHS_TO_UNLOCK_ACHIEVEMENT;
            
            playerDeathEventChannel = Finder.PlayerDeathEventChannel;
            
            base.Awake();
        }

        private void OnEnable()
        {
            playerDeathEventChannel.OnPlayerDeath += OnPlayerDeath;
        }

        private void OnDisable()
        {
            playerDeathEventChannel.OnPlayerDeath -= OnPlayerDeath;
        }
        
        private void OnPlayerDeath()
        {
            Progression++;
        }
    }
}