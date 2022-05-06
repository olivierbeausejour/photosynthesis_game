//Author:Anthony Dodier

using Harmony;
using UnityEngine;

namespace Game
{
    [Findable(R.S.Tag.MainMenu)]
    public class MenuInputManager : MonoBehaviour
    {
        private GamepadManager gamepadManager;
        
        [Header("Exit key")] 
        [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
        [SerializeField] private GamepadManager.Button gamepadPauseButton = GamepadManager.Button.Start;
        
        [Header("Confirm key")]
        [SerializeField] private KeyCode confirmKey = KeyCode.Return;
        [SerializeField] private GamepadManager.Button gamepadConfirmButton = GamepadManager.Button.A;
        
        [Header("Down key")]
        [SerializeField] private KeyCode downKey = KeyCode.DownArrow;
        [SerializeField] private GamepadManager.Button gamepadDownButton = GamepadManager.Button.DownPad;
        
        [Header("Up key")]
        [SerializeField] private KeyCode upKey = KeyCode.UpArrow;
        [SerializeField] private GamepadManager.Button gamepadUpButton = GamepadManager.Button.UpPad;

        [Header("Return key")] 
        [SerializeField] private KeyCode returnKey = KeyCode.Escape;
        [SerializeField] private GamepadManager.Button gamepadReturnButton1 = GamepadManager.Button.B;
        [SerializeField] private GamepadManager.Button gamepadReturnButton2 = GamepadManager.Button.Start;

        public bool CanUseMenuControls { get; set; } = true;
        public bool isInMainMenu = false;
        public bool isInPauseMenu = false;

        // Exit
        public bool ExitKeyDown => Input.GetKeyDown(pauseKey) || gamepadManager.GetButtonDown(gamepadPauseButton);

        // ConfirmKey
        public bool ConfirmKey => CanUseMenuControls && Input.GetKeyDown(confirmKey) || 
                                  gamepadManager.GetButtonDown(gamepadConfirmButton);
        
        // DownKey
        public bool DownKey => CanUseMenuControls && Input.GetKeyDown(downKey) || 
                               gamepadManager.GetButtonDown(gamepadDownButton);
        
        // UpKey
        public bool UpKey => CanUseMenuControls && Input.GetKeyDown(upKey)|| 
                             gamepadManager.GetButtonDown(gamepadUpButton);
        
        // ReturnKey
        public bool ReturnKey => CanUseMenuControls && Input.GetKeyDown(returnKey) ||
                                 gamepadManager.GetButtonDown(gamepadReturnButton1) ||
                                 gamepadManager.GetButtonDown(gamepadReturnButton2);

        private void Awake()
        {
            gamepadManager = GetComponent<GamepadManager>();
        }

        public KeyCode MouseButtonToKeyCode(int mouseButton)
        {
            switch (mouseButton)
            {
                case 0: return KeyCode.Mouse0;
                case 1: return KeyCode.Mouse1;
                case 2: return KeyCode.Mouse2;
                case 3: return KeyCode.Mouse3;
                case 4: return KeyCode.Mouse4;
                case 5: return KeyCode.Mouse5;
                case 6: return KeyCode.Mouse6;
            }

            return KeyCode.None;
        }
    }
}