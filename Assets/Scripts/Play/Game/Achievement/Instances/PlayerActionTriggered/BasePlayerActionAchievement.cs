// Author: Olivier Beauséjour

using Harmony;

namespace Game
{
    public abstract class BasePlayerActionAchievement : BaseAchievement
    {
        protected PlayerData playerData;

        private void Start()
        {
            playerData = Finder.GameController.CurrentPlayerData;
        }
    }
}