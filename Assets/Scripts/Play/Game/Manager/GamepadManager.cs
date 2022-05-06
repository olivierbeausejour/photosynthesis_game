//Authors:
//Jonathan Mathieu
//Olivier Beauséjour

using System;
using Harmony;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

namespace Game
{
    public class GamepadManager : MonoBehaviour
    {
        [SerializeField] private float triggersAsButtonDeadZone = 0.4f;
        [SerializeField] private float centerDeadZone = 0.05f;
        
        public bool isUsingGamepad;
        
        private int MAX_NUMBER_OF_CONTROLLER = 4;
        
        private bool isCameraNotNull;
        private bool playerIndexSet;
        private PlayerIndex playerIndex;
        private GamePadState state;
        private GamePadState previousState;

        public bool IsUsingGamepad => isUsingGamepad;

        public bool PlayerIndexSet => playerIndexSet;

        public float CenterDeadZone => centerDeadZone;

        private void Awake()
        {
            // Find any controllers
            FindControllerIndex();
            if(playerIndexSet)

                state = GamePad.GetState(playerIndex);
        }

        private void Update()
        {
            // Find a controller and use it if none connected
            if (!playerIndexSet || !previousState.IsConnected)
            {
                FindControllerIndex();
            }

            previousState = state;
            state = GamePad.GetState(playerIndex);
        }

        public bool GetConnectionStatus()
        {
            return state.IsConnected;
        }

        public bool GetButton(Button button)
        {
            return GetButtonFromState(button, false);
        }

        public bool GetButtonDown(Button button)
        {
            return GetButtonFromState(button, false) && !GetButtonFromState(button, true);
        }

        public bool GetButtonUp(Button button)
        {
            return !GetButtonFromState(button, false) && GetButtonFromState(button, true);
        }
        
        public Button ReadButton()
        {
            foreach (var button in (Button[]) Enum.GetValues(typeof(Button)))
            {
                if (GetButtonFromState(button, false) && button != Button.None) return button;
            }

            return Button.None;
        }
        
        public Button ReadButtonDown()
        {
            foreach (var button in (Button[]) Enum.GetValues(typeof(Button)))
            {
                if (GetButtonFromState(button, false) && !GetButtonFromState(button, true) && 
                    button != Button.None) return button;
            }

            return Button.None;
        }
        
        public Button ReadButtonUp()
        {
            foreach (var button in (Button[]) Enum.GetValues(typeof(Button)))
            {
                if (GetButtonFromState(button, false) && GetButtonFromState(button, true) && 
                    button != Button.None) return button;
            }

            return Button.None;
        }

        public Axis ReadAxis()
        {
            foreach (var axis in (Axis[]) Enum.GetValues(typeof(Axis)))
            {
                if (GetAxis(axis) > centerDeadZone) return axis;
            }
            
            return Axis.None;
        }

        private void OnGUI()
        {
            if (isUsingGamepad) SetIfUsingMouseKeyboard();
            else SetIfUsingGamepad();
        }

        private void SetIfUsingMouseKeyboard()
        {
            // Keyboard keys
            if (Event.current.isKey ||
                Event.current.isMouse)
            {
                isUsingGamepad = false;
                return;
            }
            
            // mouse movement
            if( Input.GetAxis("Mouse X") != 0.0f ||
                Input.GetAxis("Mouse Y") != 0.0f )
            {
                isUsingGamepad = false;
            }
        }

        private void SetIfUsingGamepad()
        {
            // Buttons
            foreach (Button button in Enum.GetValues(typeof(Button)))
            {
                if (GetButton(button))
                {
                    isUsingGamepad = true;
                    return;
                }
            }
            
            foreach (Axis axis in Enum.GetValues(typeof(Axis)))
            {
                if ( Mathf.Abs(GetAxis(axis)) > centerDeadZone)
                {
                    isUsingGamepad = true;
                    return;
                }
            }
        }

        private bool GetButtonFromState(Button button, bool isPreviousState)
        {        
            switch (button)
            {
                case Button.A:
                    if (isPreviousState ? previousState.Buttons.A == ButtonState.Pressed : state.Buttons.A == ButtonState.Pressed) return true;
                    break;
                case Button.X:
                    if (isPreviousState ? previousState.Buttons.X == ButtonState.Pressed : state.Buttons.X == ButtonState.Pressed) return true;
                    break;
                case Button.B:
                    if (isPreviousState ? previousState.Buttons.B == ButtonState.Pressed : state.Buttons.B == ButtonState.Pressed) return true;
                    break;
                case Button.Y:
                    if (isPreviousState ? previousState.Buttons.Y == ButtonState.Pressed : state.Buttons.Y == ButtonState.Pressed) return true;
                    break;
                case Button.LeftPad:
                    if (isPreviousState ? previousState.DPad.Left == ButtonState.Pressed : state.DPad.Left == ButtonState.Pressed) return true;
                    break;
                case Button.UpPad:
                    if (isPreviousState ? previousState.DPad.Up == ButtonState.Pressed : state.DPad.Up == ButtonState.Pressed) return true;
                    break;
                case Button.RightPad:
                    if (isPreviousState ? previousState.DPad.Right == ButtonState.Pressed : state.DPad.Right == ButtonState.Pressed) return true;
                    break;
                case Button.DownPad:
                    if (isPreviousState ? previousState.DPad.Down == ButtonState.Pressed : state.DPad.Down == ButtonState.Pressed) return true;
                    break;
                case Button.LeftJoystickClick:
                    if (isPreviousState ? previousState.Buttons.LeftStick == ButtonState.Pressed : state.Buttons.LeftStick == ButtonState.Pressed) return true;
                    break;
                case Button.RightJoyStickClick:
                    if (isPreviousState ? previousState.Buttons.RightStick == ButtonState.Pressed : state.Buttons.RightStick == ButtonState.Pressed) return true;
                    break;
                case Button.LeftShoulder:
                    if (isPreviousState ? previousState.Buttons.LeftShoulder == ButtonState.Pressed : state.Buttons.LeftShoulder == ButtonState.Pressed) return true;
                    break;
                case Button.RightShoulder:
                    if (isPreviousState ? previousState.Buttons.RightShoulder == ButtonState.Pressed : state.Buttons.RightShoulder == ButtonState.Pressed) return true;
                    break;
                case Button.LeftTrigger:
                    if (isPreviousState ? previousState.Triggers.Left > triggersAsButtonDeadZone : state.Triggers.Left > triggersAsButtonDeadZone) return true;
                    break;
                case Button.RightTrigger:
                    if (isPreviousState ? previousState.Triggers.Right > triggersAsButtonDeadZone : state.Triggers.Right > triggersAsButtonDeadZone) return true;
                    break;
                case Button.Start:
                    if (isPreviousState ? previousState.Buttons.Start == ButtonState.Pressed : state.Buttons.Start == ButtonState.Pressed) return true;
                    break;
                case Button.Back:
                    if (isPreviousState ? previousState.Buttons.Back == ButtonState.Pressed : state.Buttons.Back == ButtonState.Pressed) return true;
                    break;
                case Button.Guide:
                    if (isPreviousState ? previousState.Buttons.Guide == ButtonState.Pressed : state.Buttons.Guide == ButtonState.Pressed) return true;
                    break;
                case Button.None:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(button), button, null);
            }

            return false;
        }

        public float GetAxis(Axis axis)
        {
            float value;
            
            switch (axis)
            {
                case Axis.LeftTrigger:
                    value = state.Triggers.Left;
                    break;
                case Axis.RightTrigger:
                    value = state.Triggers.Right;
                    break;
                case Axis.RightThumbStickX:
                    value = state.ThumbSticks.Right.X;
                    break;
                case Axis.RightThumbStickY:
                    value = state.ThumbSticks.Right.Y;
                    break;
                case Axis.LeftThumbStickX:
                    value = state.ThumbSticks.Left.X;
                    break;
                case Axis.LeftThumbStickY:
                    value = state.ThumbSticks.Left.Y;
                    break;
                case Axis.None:
                    value = 0f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
            
            return value;
        }
        
        private void FindControllerIndex()
        {
            for (int i = 0; i < MAX_NUMBER_OF_CONTROLLER; i++)
            {
                PlayerIndex playerIndexTest = (PlayerIndex) i;
                GamePadState testState = GamePad.GetState(playerIndexTest);

                if (testState.IsConnected)
                {
                    playerIndex = playerIndexTest;
                    playerIndexSet = true;
                }
            }
        }

        public static string ButtonEnumToString(Button button)
        {
            switch (button)
            {
                case Button.A: return "A";
                case Button.X: return "X";
                case Button.B: return "B";
                case Button.Y: return "Y";
                case Button.LeftPad: return "LPad";
                case Button.UpPad: return "UPad";
                case Button.RightPad: return "RPad";
                case Button.DownPad: return "DPad";
                case Button.LeftJoystickClick: return "LSBC";
                case Button.RightJoyStickClick: return "RSBC";
                case Button.LeftShoulder: return "LB";
                case Button.RightShoulder: return "RB";
                case Button.LeftTrigger: return "LT";
                case Button.RightTrigger: return "RT";
                case Button.Start: return "Start";
                case Button.Back: return "Back";
                case Button.Guide: return "Guide";
                case Button.None: return "None";
                default:
                    throw new ArgumentOutOfRangeException(nameof(button), button, "Button doesn't exist.");
            }
        }

        public static string AxisEnumToStrung(Axis axis)
        {
            switch (axis)
            {
                case Axis.LeftTrigger: return "LT";
                case Axis.RightTrigger: return "RT";
                case Axis.RightThumbStickX: return "XRSB";
                case Axis.RightThumbStickY: return "YRSB";
                case Axis.LeftThumbStickX: return "XLSB";
                case Axis.LeftThumbStickY: return "YLSB";
                case Axis.None: return "None";
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, "Axis doesn't exist.");
            }
        }

         public enum Button
        {
            A, X, B, Y,
            LeftPad, UpPad, RightPad, DownPad,
            LeftJoystickClick, RightJoyStickClick,
            LeftShoulder, RightShoulder,
            LeftTrigger, RightTrigger,
            Start, Back, Guide,
            None
        }

        public enum Axis
        {
            LeftTrigger, RightTrigger,
            RightThumbStickX, RightThumbStickY,
            LeftThumbStickX, LeftThumbStickY,
            None
        }
    }
}