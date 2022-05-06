// Author: Olivier Beauséjour

using UnityEngine;

namespace Game
{
    public class KeyBindButton : ControlsButton
    {
        [SerializeField] private KeyCode defaultKeyCode;
        [SerializeField] private GamepadManager.Button defaultGamepadButton;
        [SerializeField] private GamepadManager.Axis defaultGamepadAxis;
        
        [SerializeField] private KeyProperty property;
        [SerializeField] private KeyActionType type;

        [SerializeField] private bool canBeDuplicate;
        
        private KeyCode currentKeyCode;
        private GamepadManager.Button currentGamepadButton;
        private GamepadManager.Axis currentGamepadAxis;

        public bool CanBeDuplicate => canBeDuplicate;

        public KeyCode DefaultKeyCode => defaultKeyCode;
        public GamepadManager.Button DefaultGamepadButton => defaultGamepadButton;
        public GamepadManager.Axis DefaultGamepadAxis => defaultGamepadAxis;

        public KeyProperty Property => property;
        public KeyActionType Type => type;

        public KeyCode CurrentKeyCode
        {
            get => currentKeyCode;
            set
            {
                currentKeyCode = value;
                UpdateText();
            }
        }

        public GamepadManager.Button CurrentGamepadButton
        {
            get => currentGamepadButton;
            set
            {
                currentGamepadButton = value;
                UpdateText();
            } 
        }

        public GamepadManager.Axis CurrentGamepadAxis
        {
            get => currentGamepadAxis;
            set
            {
                currentGamepadAxis = value;
                UpdateText();
            }
        }

        public override void ResetToDefault()
        {
            currentKeyCode = defaultKeyCode;
            currentGamepadButton = defaultGamepadButton;
            currentGamepadAxis = defaultGamepadAxis;
            
            UpdateText();
        }
        
        public override void UpdateText()
        {
            if (currentKeyCode != KeyCode.None) shownText.text = previousShownText = currentKeyCode.ToString();
            else if (currentGamepadButton != GamepadManager.Button.None) shownText.text = previousShownText =
                GamepadManager.ButtonEnumToString(currentGamepadButton);
            else if (currentGamepadAxis != GamepadManager.Axis.None) shownText.text =  previousShownText =
                GamepadManager.AxisEnumToStrung(currentGamepadAxis);
        }

        public void UpdateTextToBindingState()
        {
            previousShownText = shownText.text;
            shownText.text = "[  ]";
        }

        public void UpdateTextToPreviousBind()
        {
            shownText.text = previousShownText;
        }
        
        public enum KeyProperty
        {
            None,
            Main,
            Second,
            GamepadButton,
            GamepadAxis
        }
        
        public enum KeyActionType
        {
            None,
            MoveRight,
            MoveLeft,
            MoveUp,
            MoveDown,
            Jump,
            Dash,
            Fire,
            Pull,
            VerticalMovementAxis,
            HorizontalMovementAxis,
            VerticalDashAimAxis,
            HorizontalDashAimAxis,
            VerticalGrappleAimAxis,
            HorizontalGrappleAimAxis
        }
    }
}