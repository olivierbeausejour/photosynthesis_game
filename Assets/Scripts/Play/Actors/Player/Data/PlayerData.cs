// Author : Derek Pouliot
// Author : Olivier Beauséjour

using System;
using System.Collections.Generic;
using Harmony;

namespace Game
{
    [Serializable]
    public class PlayerData
    {
        private int lastCheckpointEncounteredId;
        private int totalFilmRollsPicked;
        private int nbTotalDashes;
        private int nbTotalGrapplingHookDeployments;
        private int nbTotalDeaths;
        private string currentLevelName;
        private readonly Dictionary<string, List<string>> filmRollsCollected;
        private readonly Dictionary<string, List<string>> keysUsed;
        private readonly Dictionary<string, List<string>> switchesUsed;

        public Dictionary<string, List<string>> SwitchesUsed => switchesUsed;

        public Dictionary<string, List<string>> KeysUsed => keysUsed;

        public Dictionary<string, List<string>> FilmRollsCollected => filmRollsCollected;

        public int LastCheckpointEncounteredId
        {
            get => lastCheckpointEncounteredId;
            set => lastCheckpointEncounteredId = value;
        }

        public int TotalFilmStocksPicked
        {
            get => totalFilmRollsPicked;
            set => totalFilmRollsPicked = value;
        }

        public string CurrentLevelName
        {
            get => currentLevelName;
            set => currentLevelName = value;
        }

        public int NbTotalDashes
        {
            get => nbTotalDashes;
            set => nbTotalDashes = value;
        }

        public int NbTotalGrapplingHookDeployments
        {
            get => nbTotalGrapplingHookDeployments;
            set => nbTotalGrapplingHookDeployments = value;
        }

        public int NbTotalDeaths
        {
            get => nbTotalDeaths;
            set => nbTotalDeaths = value;
        }

        public PlayerData()
        {
            lastCheckpointEncounteredId = -1;
            totalFilmRollsPicked = 0;
            nbTotalDashes = 0;
            nbTotalGrapplingHookDeployments = 0;
            currentLevelName = "";
            filmRollsCollected = new Dictionary<string, List<string>>();
            keysUsed = new Dictionary<string, List<string>>();
            switchesUsed = new Dictionary<string, List<string>>();
        }
    }
}