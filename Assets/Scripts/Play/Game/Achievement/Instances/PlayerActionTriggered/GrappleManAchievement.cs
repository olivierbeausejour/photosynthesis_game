// Author: Olivier Beauséjour

using Harmony;

namespace Game
{
    public class GrappleManAchievement : BasePlayerActionAchievement
    {
        private const int NB_GRAPPLE_DEPLOYS_TO_UNLOCK_ACHIEVEMENT = 100;
        
        private PlayerDeployingGrappleEventChannel playerDeployingGrappleEventChannel;

        protected override void Awake()
        {
            Name = "Grapple Man";
            Description = "Grapple 100 times.";
            GoalValue = NB_GRAPPLE_DEPLOYS_TO_UNLOCK_ACHIEVEMENT;
            
            playerDeployingGrappleEventChannel = Finder.PlayerDeployingGrappleEventChannel;
            
            base.Awake();
        }
        
        private void OnEnable()
        {
            playerDeployingGrappleEventChannel.OnPlayerDeployingGrapple += OnPlayerDeployingGrapple;
        }

        private void OnDisable()
        {
            playerDeployingGrappleEventChannel.OnPlayerDeployingGrapple -= OnPlayerDeployingGrapple;
        }

        private void OnPlayerDeployingGrapple()
        {
            Progression++;
        }
    }
}