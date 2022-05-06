// Author: Olivier Beauséjour

using System;

namespace Game
{
    [Serializable]
    public class SerializableAchievement
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        
        public int Progression { get; set; }
        
        public int GoalValue { get; set; }

        public SerializableAchievement(string name, string description, int progression, int goalValue)
        {
            Name = name;
            Description = description;
            Progression = progression;
            GoalValue = goalValue;
        }
    }
}