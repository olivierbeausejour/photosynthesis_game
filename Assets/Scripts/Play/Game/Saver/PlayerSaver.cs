// Author : Derek Pouliot

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Game
{
    public static class PlayerSaver
    {
        private const string SAVE_FILE_NAME = "/savedGame";
        private const string SAVE_FILE_EXTENSION = ".dat";

        private static string SaveFilePath => Application.persistentDataPath + SAVE_FILE_NAME;

        public static void SaveGame(PlayerData playerData, int slotGameId)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            using (FileStream saveFile = File.Open(SaveFilePath + slotGameId + SAVE_FILE_EXTENSION, FileMode.OpenOrCreate))
            {
                binaryFormatter.Serialize(saveFile, playerData);
            }
        }

        public static PlayerData LoadGame(int slotGameId)
        {
            if (CheckIfFileExists(slotGameId))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                PlayerData playerData;

                using (FileStream saveFile = File.Open(SaveFilePath + slotGameId + SAVE_FILE_EXTENSION, FileMode.Open))
                { 
                    playerData = (PlayerData) binaryFormatter.Deserialize(saveFile);
                }

                return playerData;
            }

            return null;
        }

        public static bool CheckIfFileExists(int slotGameId)
        {
            return File.Exists(SaveFilePath + slotGameId + SAVE_FILE_EXTENSION);
        }

        public static void DeleteGameSlot(int slotGameId)
        {
            if (CheckIfFileExists(slotGameId))
            {
                File.Delete(SaveFilePath + slotGameId + SAVE_FILE_EXTENSION);
            }
        }
    }
}