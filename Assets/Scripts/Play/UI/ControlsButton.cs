using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public abstract class ControlsButton : MonoBehaviour
    {
        protected static int totalIndexInMenu = 0;
        protected int currentIndexInMenu;

        protected Button button;
        protected Text shownText;
        protected string previousShownText;

        public int CurrentIndexInMenu => currentIndexInMenu;

        public Button Button => button;

        private void Awake()
        {
            currentIndexInMenu = totalIndexInMenu++;
            
            button = GetComponent<Button>();
            shownText = GetComponentInChildren<Text>();

            ResetToDefault();
        }

        public abstract void ResetToDefault();
        public abstract void UpdateText();
    }
}