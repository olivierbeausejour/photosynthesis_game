//Author: Olivier Beauséjour

using System;
using System.Linq;
using Harmony;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class AchievementSlot : MonoBehaviour
    {
        [Header("Achievement slot UI elements")]
        [SerializeField] private Text achievementNameText;
        [SerializeField] private Text achievementDescriptionText;
        [SerializeField] private Text achievementProgressionText;

        public void SetAchievementName(string value)
        {
            achievementNameText.text = value;
        }
        
        public void SetAchievementDescription(string value)
        {
            achievementDescriptionText.text = value;
        }
        
        public void SetAchievementProgression(int progression, int goalValue)
        {
            achievementProgressionText.text = progression + " / " + goalValue;
        }
    }
}