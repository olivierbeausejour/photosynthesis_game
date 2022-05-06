﻿// Author: Olivier Beauséjour

using Harmony;

 namespace Game
{
    public class WhatJustHappenedAchievement : BaseLevelCompleteAchievement
    {
        protected override void Awake()
        {
            Name = "What Just Happened";
            Description = "Beat the fifth level.";
            
            base.Awake();
        }
        
        protected override void OnLevelComplete(string levelCompleteName, int nbDeathsOnCurrentLevel)
        {
            if (levelCompleteName == R.S.Scene.Level5) Progression++;
        }
    }
}