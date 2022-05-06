// Author: Olivier Beauséjour

using Harmony;

namespace Game
{
    public class PostApocalypticRaiderAchievement : BaseLevelCompleteAchievement
    {
        protected override void Awake()
        {
            Name = "Post Apocalyptic Raider";
            Description = "Beat the third level."; 
            
            base.Awake();
        }

        protected override void OnLevelComplete(string levelCompleteName, int nbDeathsOnCurrentLevel)
        {
            if (levelCompleteName == R.S.Scene.Level3) Progression++;
        }
    }
}