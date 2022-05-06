// Author : Olivier Beauséjour

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Game
{
    public static class AchievementSaver
    {
        private const string ACHIEVEMENT_DATA_SAVE_FILE_PATH = "/achievementProgression";
        private const string ACHIEVEMENT_DATA_SAVE_FILE_EXTENSION = ".dat";
        
        public static void SaveAchievements(SerializableAchievementData serializableAchievementData, int saveSlotId)
        {
            var binaryFormatter = new BinaryFormatter();
            
            var saveFile = File.Open(Application.persistentDataPath + ACHIEVEMENT_DATA_SAVE_FILE_PATH + 
                                     saveSlotId + ACHIEVEMENT_DATA_SAVE_FILE_EXTENSION, FileMode.OpenOrCreate);
            binaryFormatter.Serialize(saveFile, serializableAchievementData);
            
            saveFile.Close();
        }
        
        public static SerializableAchievementData LoadAchievements(int saveSlotId)
        {
            if (!CheckIfFileExists(saveSlotId)) return null;
            
            var binaryFormatter = new BinaryFormatter();
            
            var saveFile = File.Open(Application.persistentDataPath + ACHIEVEMENT_DATA_SAVE_FILE_PATH + 
                                     saveSlotId + ACHIEVEMENT_DATA_SAVE_FILE_EXTENSION, FileMode.Open);
            
            var achievementData = (SerializableAchievementData) binaryFormatter.Deserialize(saveFile);
            
            saveFile.Close();

            return achievementData;

        }

        private static bool CheckIfFileExists(int saveSlotId)
        {
            return File.Exists(Application.persistentDataPath + ACHIEVEMENT_DATA_SAVE_FILE_PATH + 
                               saveSlotId + ACHIEVEMENT_DATA_SAVE_FILE_EXTENSION);
        }

        // Author : Derek Pouliot
        public static void DeleteAchievementData(int saveSlotId)
        {
            if (CheckIfFileExists(saveSlotId))
                File.Delete(Application.persistentDataPath + ACHIEVEMENT_DATA_SAVE_FILE_PATH + 
                            saveSlotId + ACHIEVEMENT_DATA_SAVE_FILE_EXTENSION);
        }
    }
}