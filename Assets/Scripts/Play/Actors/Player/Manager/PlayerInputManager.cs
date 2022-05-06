//Authors:
//Jonathan Mathieu
//Olivier Beauséjour
//Charles Tremblay

using System;
using Harmony;
using UnityEngine;

namespace Game
{
    [Findable(R.S.Tag.Player)]
    public class PlayerInputManager : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private float movementDirectionDeadZone = 0.3f;
        [SerializeField] private float gamepadZeroDeadZone = 0.1f;

        [Header("Jump keys")] [Header("Keyboard Inputs")] [Header("_________________")] 
        [SerializeField] private KeyCode jumpKey1 = KeyCode.Space;
        [SerializeField] private KeyCode jumpKey2 = KeyCode.V;

        [Header("Dash keys")] 
        [SerializeField] private KeyCode dashKey1 = KeyCode.LeftShift;
        [SerializeField] private KeyCode dashKey2 = KeyCode.RightShift;

        [Header("Pull keys")] 
        [SerializeField] private KeyCode pullKey1 = KeyCode.LeftControl;
        [SerializeField] private KeyCode pullKey2 = KeyCode.RightControl;

        [Header("Right keys")] [Header("Horizontal movement Axis")] 
        [SerializeField] private KeyCode rightKey1 = KeyCode.D;
        [SerializeField] private KeyCode rightKey2 = KeyCode.RightArrow;

        [Header("Left keys")] 
        [SerializeField] private KeyCode leftKey1 = KeyCode.A;
        [SerializeField] private KeyCode leftKey2 = KeyCode.LeftArrow;

        [Header("Up keys")] [Header("Vertical movement Axis")] 
        [SerializeField] private KeyCode upKey1 = KeyCode.W;
        [SerializeField] private KeyCode upKey2 = KeyCode.UpArrow;

        [Header("Down keys")] 
        [SerializeField] private KeyCode downKey1 = KeyCode.S;
        [SerializeField] private KeyCode downKey2 = KeyCode.DownArrow;

        [Header("Fire keys")] 
        [SerializeField] private KeyCode fireKey1 = KeyCode.Mouse0;
        [SerializeField] private KeyCode fireKey2 = KeyCode.Mouse1;

        [Header("Gamepad Inputs")] [Header("_________________")]
        
        [Header("Fire button")] 
        [SerializeField] private GamepadManager.Button gamepadFireKey = GamepadManager.Button.RightTrigger;

        [Header("Jump button")] 
        [SerializeField] private GamepadManager.Button gamepadJumpKey = GamepadManager.Button.LeftShoulder;

        [Header("Dash button")] 
        [SerializeField] private GamepadManager.Button gamePadDashKey = GamepadManager.Button.RightShoulder;

        [Header("Pull button")] 
        [SerializeField] private GamepadManager.Button gamePadPullKey = GamepadManager.Button.LeftTrigger;
        
        [Header("Right button")] 
        [SerializeField] private GamepadManager.Button gamePadRightKey = GamepadManager.Button.RightPad;
        
        [Header("Left button")] 
        [SerializeField] private GamepadManager.Button gamePadLeftKey = GamepadManager.Button.LeftPad;
        
        [Header("Up button")] 
        [SerializeField] private GamepadManager.Button gamePadUpKey = GamepadManager.Button.UpPad;
        
        [Header("Down button")] 
        [SerializeField] private GamepadManager.Button gamePadDownKey = GamepadManager.Button.DownPad;

        [Header("Gamepad horizontal movement axis")] [SerializeField] 
        private GamepadManager.Axis horizontalMovementAxis = GamepadManager.Axis.LeftThumbStickX;

        [Header("Gamepad vertical movement axis")] [SerializeField]
        private GamepadManager.Axis verticalMovementAxis = GamepadManager.Axis.LeftThumbStickY;

        [Header("Gamepad Dash Aim axis Horizontal")] [SerializeField]
        private GamepadManager.Axis horizontalDashAimAxis = GamepadManager.Axis.RightThumbStickX;

        [Header("Gamepad Dash Aim axis Vertical")] [SerializeField]
        private GamepadManager.Axis verticalDashAimAxis = GamepadManager.Axis.RightThumbStickY;
        
        [Header("Gamepad Grapple Aim axis Horizontal")] [SerializeField]
        private GamepadManager.Axis horizontalGrappleAimAxis = GamepadManager.Axis.RightThumbStickX;

        [Header("Gamepad Grapple Aim axis Vertical")] [SerializeField]
        private GamepadManager.Axis verticalGrappleAimAxis = GamepadManager.Axis.RightThumbStickY;
        
        [Header("Input settings")]
        [SerializeField] private bool isDashMouseDirection = false;

        private bool isController;
        private Camera camera;
        private bool isCameraNotNull;
        private GamepadManager gamepad;

        // Jump
        public bool JumpKey => Input.GetKey(jumpKey1) || Input.GetKey(jumpKey2) || gamepad.GetButton(gamepadJumpKey);

        public bool JumpKeyDown => Input.GetKeyDown(jumpKey1) || Input.GetKeyDown(jumpKey2) ||
                                   gamepad.GetButtonDown(gamepadJumpKey);

        public bool JumpKeyUp =>
            Input.GetKeyUp(jumpKey1) || Input.GetKeyUp(jumpKey2) || gamepad.GetButtonUp(gamepadJumpKey);

        // Dash
        public bool DashKey => Input.GetKey(dashKey1) || Input.GetKey(dashKey2) || gamepad.GetButton(gamePadDashKey);

        public bool DashKeyDown => Input.GetKeyDown(dashKey1) || Input.GetKeyDown(dashKey2) ||
                                   gamepad.GetButtonDown(gamePadDashKey);

        public bool DashKeyUp =>
            Input.GetKeyUp(dashKey1) || Input.GetKeyUp(dashKey2) || gamepad.GetButtonUp(gamePadDashKey);

        // Pull
        public bool PullKey => Input.GetKey(pullKey1) || Input.GetKey(pullKey2) || gamepad.GetButton(gamePadPullKey);

        public bool PullKeyDown => Input.GetKeyDown(pullKey1) || Input.GetKeyDown(pullKey2) ||
                                   gamepad.GetButtonDown(gamePadPullKey);

        public bool PullKeyUp =>
            Input.GetKeyUp(pullKey1) || Input.GetKeyUp(pullKey2) || gamepad.GetButtonUp(gamePadPullKey);

        // Right
        public bool RightKey =>
            Input.GetKey(rightKey1) || Input.GetKey(rightKey2) || gamepad.GetButton(gamePadRightKey);
        public bool RightKeyDown =>
            Input.GetKeyDown(rightKey1) || Input.GetKeyDown(rightKey2) || gamepad.GetButtonDown(gamePadRightKey);
        public bool RightKeyUp =>
            Input.GetKeyUp(rightKey1) || Input.GetKeyUp(rightKey2) || gamepad.GetButtonUp(gamePadRightKey);

        // Left
        public bool LeftKey => Input.GetKey(leftKey1) || Input.GetKey(leftKey2) || gamepad.GetButton(gamePadLeftKey);
        public bool LeftKeyDown =>
            Input.GetKeyDown(leftKey1) || Input.GetKeyDown(leftKey2) || gamepad.GetButtonDown(gamePadLeftKey);
        public bool LeftKeyUp =>
            Input.GetKeyUp(leftKey1) || Input.GetKeyUp(leftKey2) || gamepad.GetButtonUp(gamePadLeftKey);

        // Up
        public bool UpKey => Input.GetKey(upKey1) || Input.GetKey(upKey2) || gamepad.GetButton(gamePadUpKey);
        public bool UpKeyDown => 
            Input.GetKeyDown(upKey1) || Input.GetKeyDown(upKey2) || gamepad.GetButtonDown(gamePadUpKey);
        public bool UpKeyUp => Input.GetKeyUp(upKey1) || Input.GetKeyUp(upKey2) || gamepad.GetButtonUp(gamePadUpKey);

        // Down
        public bool DownKey => Input.GetKey(downKey1) || Input.GetKey(downKey2) || gamepad.GetButton(gamePadDownKey);
        public bool DownKeyDown => 
            Input.GetKeyDown(downKey1) || Input.GetKeyDown(downKey2) || gamepad.GetButtonDown(gamePadDownKey);
        public bool DownKeyUp => 
            Input.GetKeyUp(downKey1) || Input.GetKeyUp(downKey2) || gamepad.GetButtonUp(gamePadDownKey);

        // Fire
        public bool FireKey => Input.GetKey(fireKey1) || Input.GetKey(fireKey2) || gamepad.GetButton(gamepadFireKey);

        public bool FireKeyDown => Input.GetKeyDown(fireKey1) || Input.GetKeyDown(fireKey2) || gamepad.GetButtonDown(gamepadFireKey);

        public bool FireKeyUp =>
            Input.GetKeyUp(fireKey1) || Input.GetKeyUp(fireKey2) || gamepad.GetButtonUp(gamepadFireKey);
        
        // Dash cardinal
        public bool IsDashMouseDirection => isDashMouseDirection;


        public bool IsUsingGamepad => gamepad.isUsingGamepad;

        private void Awake()
        {
            camera = Camera.main;
            isCameraNotNull = camera != null;
            gamepad = Finder.GameController.gameObject.GetComponent<GamepadManager>();
            
            ApplyKeyBinds();
        }

        public void ApplyKeyBinds()
        {
            var savedControlsData = PlayerControlsSaver.LoadSavedControls();
            if (savedControlsData == null) return;
            
            foreach (var savedKeyBind in savedControlsData.SavedKeyBinds)
            {
                switch (savedKeyBind.type)
                {
                    case KeyBindButton.KeyActionType.MoveRight:
                        switch (savedKeyBind.property)
                        {
                            case KeyBindButton.KeyProperty.Main: rightKey1 = savedKeyBind.keyCode; break;
                            case KeyBindButton.KeyProperty.Second: rightKey2 = savedKeyBind.keyCode; break;
                            case KeyBindButton.KeyProperty.GamepadButton: gamePadRightKey = savedKeyBind.button; break;
                            default: throw new ArgumentOutOfRangeException(
                                    savedKeyBind.property + " not valid in " + savedKeyBind.type);
                        }
                        break;

                    case KeyBindButton.KeyActionType.MoveLeft:
                        switch (savedKeyBind.property)
                        {
                            case KeyBindButton.KeyProperty.Main: leftKey1 = savedKeyBind.keyCode; break;
                            case KeyBindButton.KeyProperty.Second: leftKey2 = savedKeyBind.keyCode; break;
                            case KeyBindButton.KeyProperty.GamepadButton: gamePadLeftKey = savedKeyBind.button; break;
                            default: throw new ArgumentOutOfRangeException(
                                savedKeyBind.property + " not valid in " + savedKeyBind.type);
                        }
                        break;
                    
                    case KeyBindButton.KeyActionType.MoveUp:
                        switch (savedKeyBind.property)
                        {
                            case KeyBindButton.KeyProperty.Main: upKey1 = savedKeyBind.keyCode; break;
                            case KeyBindButton.KeyProperty.Second: upKey2 = savedKeyBind.keyCode; break;
                            case KeyBindButton.KeyProperty.GamepadButton: gamePadUpKey = savedKeyBind.button; break;
                            default: throw new ArgumentOutOfRangeException(
                                savedKeyBind.property + " not valid in " + savedKeyBind.type);
                        }
                        break;
                    
                    case KeyBindButton.KeyActionType.MoveDown:
                        switch (savedKeyBind.property)
                        {
                            case KeyBindButton.KeyProperty.Main: downKey1 = savedKeyBind.keyCode; break;
                            case KeyBindButton.KeyProperty.Second: downKey2 = savedKeyBind.keyCode; break;
                            case KeyBindButton.KeyProperty.GamepadButton: gamePadDownKey = savedKeyBind.button; break;
                            default: throw new ArgumentOutOfRangeException(
                                savedKeyBind.property + " not valid in " + savedKeyBind.type);
                        }
                        break;
                    
                    case KeyBindButton.KeyActionType.Jump:
                        switch (savedKeyBind.property)
                        {
                            case KeyBindButton.KeyProperty.Main: jumpKey1 = savedKeyBind.keyCode; break;
                            case KeyBindButton.KeyProperty.Second: jumpKey2 = savedKeyBind.keyCode; break;
                            case KeyBindButton.KeyProperty.GamepadButton: gamepadJumpKey = savedKeyBind.button; break;
                            default: throw new ArgumentOutOfRangeException(
                                savedKeyBind.property + " not valid in " + savedKeyBind.type);
                        }
                        break;
                    
                    case KeyBindButton.KeyActionType.Dash:
                        switch (savedKeyBind.property)
                        {
                            case KeyBindButton.KeyProperty.Main: dashKey1 = savedKeyBind.keyCode; break;
                            case KeyBindButton.KeyProperty.Second: dashKey2 = savedKeyBind.keyCode; break;
                            case KeyBindButton.KeyProperty.GamepadButton: gamePadDashKey = savedKeyBind.button; break;
                            default: throw new ArgumentOutOfRangeException(
                                savedKeyBind.property + " not valid in " + savedKeyBind.type);
                        }
                        break;
                    
                    case KeyBindButton.KeyActionType.Fire:
                        switch (savedKeyBind.property)
                        {
                            case KeyBindButton.KeyProperty.Main: fireKey1 = savedKeyBind.keyCode; break;
                            case KeyBindButton.KeyProperty.Second: fireKey2 = savedKeyBind.keyCode; break;
                            case KeyBindButton.KeyProperty.GamepadButton: gamepadFireKey = savedKeyBind.button; break;
                            default: throw new ArgumentOutOfRangeException(
                                savedKeyBind.property + " not valid in " + savedKeyBind.type);
                        }
                        break;
                    
                    case KeyBindButton.KeyActionType.Pull:
                        switch (savedKeyBind.property)
                        {
                            case KeyBindButton.KeyProperty.Main: pullKey1 = savedKeyBind.keyCode; break;
                            case KeyBindButton.KeyProperty.Second: pullKey2 = savedKeyBind.keyCode; break;
                            case KeyBindButton.KeyProperty.GamepadButton: gamePadPullKey = savedKeyBind.button; break;
                            default: throw new ArgumentOutOfRangeException(
                                savedKeyBind.property + " not valid in " + savedKeyBind.type);
                        }
                        break;
                    
                    case KeyBindButton.KeyActionType.VerticalMovementAxis:
                        verticalMovementAxis = savedKeyBind.axis; break;
                    
                    case KeyBindButton.KeyActionType.HorizontalMovementAxis:
                        horizontalMovementAxis = savedKeyBind.axis; break;
                    
                    case KeyBindButton.KeyActionType.VerticalDashAimAxis:
                        verticalDashAimAxis = savedKeyBind.axis; break;
                    
                    case KeyBindButton.KeyActionType.HorizontalDashAimAxis:
                        horizontalDashAimAxis = savedKeyBind.axis; break;
                    
                    case KeyBindButton.KeyActionType.VerticalGrappleAimAxis:
                        verticalGrappleAimAxis = savedKeyBind.axis; break;
                    
                    case KeyBindButton.KeyActionType.HorizontalGrappleAimAxis:
                        horizontalGrappleAimAxis = savedKeyBind.axis; break;
                    
                    default:
                        throw new ArgumentOutOfRangeException(savedKeyBind.type + " should not be a KeyBind");
                }
            }
            
            foreach (var savedBooleanButton in savedControlsData.SavedBooleanButtons)
            {
                switch (savedBooleanButton.type)
                {
                    case BooleanButton.KeyActionType.MouseAimDash:
                        isDashMouseDirection = savedBooleanButton.value; break;
                    default:
                        throw new ArgumentOutOfRangeException(savedBooleanButton.type + " should not be a usable type");
                }
            }
        }
        
        // Directional input vectors
        public Vector2 MovementAxisInput
        {
            get
            {
                Vector2 direction = GetAxisInputVector(false, horizontalMovementAxis, verticalMovementAxis);
                
                if(gamepad.IsUsingGamepad)
                    if (direction.x > movementDirectionDeadZone)
                        direction.x = Mathf.Sign(direction.x);

                return direction;
            }
        }

        public Vector2 MovementAxisInputNormalized => GetAxisInputVector(true, horizontalMovementAxis, verticalMovementAxis);

        public Vector2 DashCardinalDirection =>
            gamepad.IsUsingGamepad
                ? GetAxisInputVector(true, horizontalDashAimAxis, verticalDashAimAxis)
                : MovementAxisInputNormalized;

        public Vector2 DashFreeDirection
        {
            get
            {
                if(gamepad.IsUsingGamepad)
                    return GetAxisInputVector(true, horizontalDashAimAxis, verticalDashAimAxis);
                else
                    return (MouseScreenPosition - (Vector2) transform.position).normalized;
            }
        }

        public Vector2 GrappleAimDirection
        {
            get
            {
                if(gamepad.IsUsingGamepad)
                    return GetAxisInputVector(true, horizontalGrappleAimAxis, verticalGrappleAimAxis);
                else
                    return (MouseScreenPosition - (Vector2) transform.position).normalized;
            }
        }

        private Vector2 GetAxisInputVector(bool isNormalized, GamepadManager.Axis horizontalAxis, GamepadManager.Axis verticalAxis)
        {
            Vector2 axis = Vector2.zero;

            if (gamepad.IsUsingGamepad)
            {
                axis.x = gamepad.GetAxis(horizontalAxis);
                axis.y = gamepad.GetAxis(verticalAxis);
            }
            else
            {
                if (RightKey) axis.x++;
                if (LeftKey) axis.x--;
                if (UpKey) axis.y++;
                if (DownKey) axis.y--;
            }
            return isNormalized ? axis.normalized : axis;
        }

        public Vector2 MouseScreenPosition =>
            isCameraNotNull ? (Vector2) camera.ScreenToWorldPoint(Input.mousePosition) : Vector2.zero;
    }
}