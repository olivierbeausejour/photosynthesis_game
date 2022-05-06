// Author: Olivier Beauséjour

using Harmony;

namespace Game
{
    public class OofAchievement : BasePlayerActionAchievement
    {
        private const int NB_DEATHS_TO_UNLOCK_ACHIEVEMENT = 1000;
        
        private PlayerDeathEventChannel playerDeathEventChannel;

        protected override void Awake()
        {
            Name = "OOF";
            Description = "Die 1000 times.";
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