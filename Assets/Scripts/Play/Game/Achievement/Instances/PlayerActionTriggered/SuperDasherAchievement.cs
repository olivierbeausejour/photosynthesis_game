// Author: Olivier Beauséjour

using Harmony;

namespace Game
{
    public class SuperDasherAchievement : BasePlayerActionAchievement
    {
        private const int NB_DASHES_TO_UNLOCK_ACHIEVEMENT = 100;
        
        private PlayerDashEventChannel playerDashEventChannel;

        protected override void Awake()
        {
            Name = "Super Dasher";
            Description = "Dash 100 times.";
            GoalValue = NB_DASHES_TO_UNLOCK_ACHIEVEMENT;
            
            playerDashEventChannel = Finder.PlayerDashEventChannel;
            
            base.Awake();
        }

        private void OnEnable()
        {
            playerDashEventChannel.OnPlayerDash += OnPlayerDash;
        }

        private void OnDisable()
        {
            playerDashEventChannel.OnPlayerDash -= OnPlayerDash;
        }

        private void OnPlayerDash()
        {
            Progression++;
        }
    }
}