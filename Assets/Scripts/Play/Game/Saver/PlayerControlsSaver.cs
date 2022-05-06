// Author: Olivier Beauséjour

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Game
{
    public class PlayerControlsSaver
    {
        private const string SAVE_FILE_NAME = "/savedControls";
        private const string SAVE_FILE_EXTENSION = ".dat";
        
        private static string SaveFilePath => Application.persistentDataPath + SAVE_FILE_NAME;
        
        public static void SaveControls(PlayerControlsData playerControlsData)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            using (FileStream saveFile = File.Open(SaveFilePath + SAVE_FILE_EXTENSION, FileMode.OpenOrCreate))
            {
                binaryFormatter.Serialize(saveFile, playerControlsData);
            }
        }
        
        public static PlayerControlsData LoadSavedControls()
        {
            if (CheckIfFileExists())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                PlayerControlsData playerControlsData;

                using (FileStream saveFile = File.Open(SaveFilePath + SAVE_FILE_EXTENSION, FileMode.Open))
                { 
                    playerControlsData = (PlayerControlsData) binaryFormatter.Deserialize(saveFile);
                }

                return playerControlsData;
            }

            return null;
        }
        
        public static bool CheckIfFileExists()
        {
            return File.Exists(SaveFilePath + SAVE_FILE_EXTENSION);
        }
    }
}