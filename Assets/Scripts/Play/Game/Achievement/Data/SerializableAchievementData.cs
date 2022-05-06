// Author : Olivier Beauséjour

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class SerializableAchievementData
    {
        public List<SerializableAchievement> Achievements { get; private set; }

        public SerializableAchievementData()
        {
            Achievements = new List<SerializableAchievement>();
        }

        public static SerializableAchievementData MakeSerializableAchievementData(IReadOnlyList<BaseAchievement> achievements)
        {
            var serializableAchievementData = new SerializableAchievementData();

            foreach (var achievement in achievements)
            {
                serializableAchievementData.Achievements.Add(new SerializableAchievement(achievement.Name, 
                    achievement.Description, achievement.Progression, achievement.GoalValue));
            }

            return serializableAchievementData;
        }
    }
}