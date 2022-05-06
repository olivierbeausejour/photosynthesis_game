// Author: Olivier Beauséjour

using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class BooleanButton : ControlsButton
    {
        [SerializeField] private bool defaultState;
        [SerializeField] private KeyActionType type;

        [SerializeField] private string trueText = "Yes";
        [SerializeField] private string falseText = "No";

        private bool value;
        
        public KeyActionType Type => type;

        public bool Value
        {
            get => this.value;
            
            set
            {
                this.value = value;
                UpdateText();
            }
        }

        private void Awake()
        {
            currentIndexInMenu = totalIndexInMenu++;
            
            button = GetComponent<Button>();
            shownText = GetComponentInChildren<Text>();

            ResetToDefault();
        }

        public void Switch()
        {
            value = !value;
            UpdateText();
        }

        public override void ResetToDefault()
        {
            value = defaultState;
            UpdateText();
        }
        
        public override void UpdateText()
        {
            shownText.text = value ? trueText : falseText;
        }
        
        public enum KeyActionType
        {
            None,
            MouseAimDash
        }
    }
}