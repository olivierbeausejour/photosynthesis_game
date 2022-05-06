// Author: Olivier Beauséjour

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    public class ControlsMenuController : MonoBehaviour
    {
        [Header("Key binding options")]
        [SerializeField] private KeyCode cancelKeyBindKeyCode = KeyCode.Escape;
        [SerializeField] private GamepadManager.Button cancelKeyBindButton = GamepadManager.Button.Start;
        [SerializeField] private float gamepadBindingWaitingTime = 0.01f;
        
        [Header("Sounds")]
        [SerializeField] private SoundEnum selectItemSound;
        [SerializeField] private AudioSource selectItemSoundAudioSource;
        
        [Header("UI")]
        [SerializeField] private PopupMessage popupMessage;

        private MainMenuController mainMenuController;
        private PauseMenuController pauseMenuController;
        private MenuInputManager menuInputManager;
        private AudioManager audioManager;
        private PlayerControlsData playerControlsData;
        private GamepadManager gamepadManager;
        private EventSystem eventSystem;
        [CanBeNull] private PlayerInputManager playerInputManager;
        private static Canvas controlsMenuCanvas;
        
        private List<KeyBindButton> keyBindButtons;
        private List<BooleanButton> booleanButtons;
        private Button firstSelectedButton;
        
        private int selectedKeyIndex = -1;
        private bool coroutineRunning;
        private bool canBindGamepadButtons;

        private KeyBindButton selectedKeyBindButton;

        private void Awake()
        {
            audioManager = Finder.AudioManager;
            
            selectItemSoundAudioSource.clip = audioManager.GetAudioClip(selectItemSound);
            
            menuInputManager = this.GetComponentInSiblings<MenuInputManager>();
            playerControlsData = new PlayerControlsData();
            gamepadManager = GetComponent<GamepadManager>();
            eventSystem = GetComponentInChildren<EventSystem>();
            controlsMenuCanvas = GetComponent<Canvas>();
            mainMenuController = Finder.MainMenuController;
            pauseMenuController = Finder.PauseMenuController;
            menuInputManager = Finder.MenuInputManager;

            keyBindButtons = GetComponentsInChildren<KeyBindButton>().ToList();
            booleanButtons = GetComponentsInChildren<BooleanButton>().ToList();

            firstSelectedButton = eventSystem.firstSelectedGameObject.GetComponent<Button>();

            try
            {
                playerInputManager = Finder.PlayerInputManager;
            }
            catch (Exception e)
            {
                // ignored
            }

            controlsMenuCanvas.enabled = false;
            controlsMenuCanvas.sortingOrder = 0;
        }

        private void Update()
        {
            if(menuInputManager.ReturnKey)
                ReturnToMainMenu();
        }

        [UsedImplicitly]
        public void ActivateControlsMenu()
        {
            controlsMenuCanvas.enabled = true;
            controlsMenuCanvas.sortingOrder = 1;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            var savedControlsData = PlayerControlsSaver.LoadSavedControls();
            
            if (savedControlsData == null) ResetControlsToDefault(false);
            
            else
            {
                foreach (var serializableKeyBindAssociation in savedControlsData.SavedKeyBinds)
                {
                    var associatedKeyBind = keyBindButtons.Find(k => k.Type == serializableKeyBindAssociation.type
                                                                     && k.Property == serializableKeyBindAssociation
                                                                         .property);

                    associatedKeyBind.CurrentKeyCode = serializableKeyBindAssociation.keyCode;
                    associatedKeyBind.CurrentGamepadButton = serializableKeyBindAssociation.button;
                    associatedKeyBind.CurrentGamepadAxis = serializableKeyBindAssociation.axis;
                }

                foreach (var serializableBooleanButtonAssociation in savedControlsData.SavedBooleanButtons)
                {
                    var associatedBooleanButton =
                        booleanButtons.Find(b => b.Type == serializableBooleanButtonAssociation.type);

                    associatedBooleanButton.Value = serializableBooleanButtonAssociation.value;
                }
            }
            
            firstSelectedButton.Select();
        }

        private void SetActiveUnusedKeyBinds(bool value)
        {
            foreach (var keyBindButton in keyBindButtons.Where(k => k.CurrentIndexInMenu != selectedKeyIndex)) 
                keyBindButton.Button.interactable = value;
        }
        
        [UsedImplicitly]
        public void BindKeyCode(KeyBindButton keyBindButton)
        {
            selectItemSoundAudioSource.Play();
            
            keyBindButtons.Find(k => k.CurrentIndexInMenu == keyBindButton.CurrentIndexInMenu).UpdateTextToBindingState();
            selectedKeyIndex = keyBindButton.CurrentIndexInMenu;
            
            SetActiveUnusedKeyBinds(false);

            menuInputManager.CanUseMenuControls = false;
        }

        private bool BindGamepad(KeyBindButton keyBindButton)
        {
            if (selectedKeyIndex != -1) return false;
            
            selectedKeyBindButton = keyBindButtons.Find(k => k.CurrentIndexInMenu == keyBindButton.CurrentIndexInMenu);
            
            if (!gamepadManager.PlayerIndexSet)
            {
                selectedKeyBindButton.UpdateTextToPreviousBind();
                selectedKeyIndex = -1;
                
                popupMessage.ShowPopup(StringConstants.NO_CONTROLLER_CONNECTED_WARNING, PopupMessage.PopupSoundType.Warning, 
                    PopupMessage.FADE_TIME, PopupMessage.NORMAL_SHOWING_TIME, 
                    PopupMessage.FADE_TIME);
                
                return false;
            }

            selectItemSoundAudioSource.Play();
            
            selectedKeyBindButton.UpdateTextToBindingState();
            selectedKeyIndex = keyBindButton.CurrentIndexInMenu;
            SetActiveUnusedKeyBinds(false);

            return true;
        }
        
        [UsedImplicitly]
        public void BindGamepadButton(KeyBindButton keyBindButton)
        {
            if (!BindGamepad(keyBindButton)) return;
            StartCoroutine(ListenForGamepadInput());
        }

        private IEnumerator ListenForGamepadInput()
        {
            if (coroutineRunning) yield break;

            yield return new WaitForSeconds(gamepadBindingWaitingTime);

            GamepadManager.Button gamepadPressedButton;
            coroutineRunning = true;
            
            while ((gamepadPressedButton = gamepadManager.ReadButtonDown()) == GamepadManager.Button.None) yield return null;

            selectedKeyBindButton = FindSelectedKeyBind();
            
            if (gamepadPressedButton == cancelKeyBindButton || 
                selectedKeyBindButton.CurrentGamepadButton == GamepadManager.Button.None)
            {
                selectedKeyBindButton.UpdateTextToPreviousBind();
                
                SetActiveUnusedKeyBinds(true);
                menuInputManager.CanUseMenuControls = true;
                
                yield break;
            }

            while (isActiveAndEnabled)
            {
                if (gamepadManager.GetButtonUp(gamepadPressedButton)) break;
                yield return null;
            }
            
            selectedKeyBindButton.CurrentGamepadButton = gamepadPressedButton;
            selectedKeyIndex = -1;
            SetActiveUnusedKeyBinds(true);

            menuInputManager.CanUseMenuControls = true;
            coroutineRunning = false;
        }
        
        [UsedImplicitly]
        public void BindGamepadAxis(KeyBindButton keyBindButton)
        {
            if (!BindGamepad(keyBindButton)) return;
            StartCoroutine(ListenForAxisInput());
        }
        
        private IEnumerator ListenForAxisInput()
        {
            if (coroutineRunning) yield break;

            coroutineRunning = true;
            var gamepadPressedAxis = GamepadManager.Axis.None;
            
            while (isActiveAndEnabled)
            {
                gamepadPressedAxis = gamepadManager.ReadAxis();
                if (gamepadPressedAxis != GamepadManager.Axis.None) break;
                yield return null;
            }

            selectedKeyBindButton = FindSelectedKeyBind();
            
            if (gamepadManager.ReadButton() == cancelKeyBindButton || 
                selectedKeyBindButton.CurrentGamepadAxis == GamepadManager.Axis.None)
            {
                selectedKeyBindButton.UpdateTextToPreviousBind();
                
                SetActiveUnusedKeyBinds(true);
                menuInputManager.CanUseMenuControls = true;
                
                yield break;
            }

            while (isActiveAndEnabled)
            {
                if (Math.Abs(gamepadManager.GetAxis(gamepadPressedAxis)) < gamepadManager.CenterDeadZone) break;
                yield return null;
            }
            
            selectedKeyBindButton.CurrentGamepadAxis = gamepadPressedAxis;
            selectedKeyIndex = -1;
            SetActiveUnusedKeyBinds(true);

            menuInputManager.CanUseMenuControls = true;
            coroutineRunning = false;
        }
        
        [UsedImplicitly]
        public void ApplyControls()
        {
            if (KeyBindsContainDuplicates())
            {
                popupMessage.ShowPopup(StringConstants.KEY_BINDING_DUPLICATES_WARNING, PopupMessage.PopupSoundType.Warning, 
                    PopupMessage.FADE_TIME, PopupMessage.NORMAL_SHOWING_TIME, 
                    PopupMessage.FADE_TIME);
                return;
            }
            
            playerControlsData.Reset();
            
            foreach (var keyBindButton in keyBindButtons)
            {
                playerControlsData.AddKeyBindData(keyBindButton.Property, keyBindButton.Type, keyBindButton.CurrentKeyCode, 
                    keyBindButton.CurrentGamepadButton, keyBindButton.CurrentGamepadAxis);
            }
            
            foreach (var booleanButton in booleanButtons)
            {
                playerControlsData.AddBooleanButtonData(booleanButton.Type, booleanButton.Value);
            }

            PlayerControlsSaver.SaveControls(playerControlsData);

            if (playerInputManager != null)
                playerInputManager.ApplyKeyBinds();

            popupMessage.ShowPopup(StringConstants.CONTROLS_APPLIED_SUCCESS, PopupMessage.PopupSoundType.Success, 
                PopupMessage.FADE_TIME, PopupMessage.NORMAL_SHOWING_TIME, 
                PopupMessage.FADE_TIME);
        }

        private bool KeyBindsContainDuplicates()
        {
            var keyCodeButtonsToCheck = keyBindButtons.Where(k =>
                !k.CanBeDuplicate && k.Property == KeyBindButton.KeyProperty.Main ||
                k.Property == KeyBindButton.KeyProperty.Second).ToList();
            if (keyCodeButtonsToCheck.Any(keyBindButton =>
                keyCodeButtonsToCheck.Count(k => k.CurrentKeyCode == keyBindButton.CurrentKeyCode) > 1))
            {
                return true;
            }

            var gamepadButtonsToCheck = keyBindButtons.Where(k =>
                !k.CanBeDuplicate && k.Property == KeyBindButton.KeyProperty.GamepadButton).ToList();
            if (gamepadButtonsToCheck.Any(keyBindButton =>
                gamepadButtonsToCheck.Count(k => k.CurrentGamepadButton == keyBindButton.CurrentGamepadButton) > 1))
            {
                return true;
            }

            var gamepadAxisToCheck = keyBindButtons.Where(k =>
                !k.CanBeDuplicate && k.Property == KeyBindButton.KeyProperty.GamepadAxis).ToList();
            if (gamepadAxisToCheck.Any(keyBindButton =>
                gamepadAxisToCheck.Count(k => k.CurrentGamepadAxis == keyBindButton.CurrentGamepadAxis) > 1))
            {
                return true;
            }

            return false;
        }
        
        [UsedImplicitly]
        public void OnBooleanButtonClick(BooleanButton booleanButton)
        {
            selectItemSoundAudioSource.Play();
            booleanButton.Switch();
        }
        
        [UsedImplicitly]
        public void ResetControlsToDefault(bool buttonClick)
        {
            if (buttonClick) selectItemSoundAudioSource.Play();
            
            foreach (var keyBind in keyBindButtons)
            {
                keyBind.ResetToDefault();
                keyBind.UpdateText();
            }

            foreach (var booleanButton in booleanButtons)
            {
                booleanButton.ResetToDefault();
                booleanButton.UpdateText();
            }
        }
        
        [UsedImplicitly]
        public void ReturnToMainMenu()
        {
            selectItemSoundAudioSource.Play();
            
            controlsMenuCanvas.enabled = false;
            controlsMenuCanvas.sortingOrder = 0;
            
            if(menuInputManager.isInMainMenu)
                mainMenuController.ShowMainMenu();
            else if(menuInputManager.isInPauseMenu)
                pauseMenuController.ShowPauseMenu();
        }

        private void OnGUI()
        {
            if (!controlsMenuCanvas.enabled || selectedKeyIndex == -1 || menuInputManager.CanUseMenuControls) return;
            selectedKeyBindButton = FindSelectedKeyBind();
            
            var e = Event.current;
            if (!e.isKey && !e.isMouse && !e.isScrollWheel && !e.shift) return;

            if (e.shift) e.keyCode = Input.GetKey(KeyCode.LeftShift) ? KeyCode.LeftShift : KeyCode.RightShift;
            
            else if (e.isMouse && Input.GetMouseButtonDown(e.button)) 
                e.keyCode = menuInputManager.MouseButtonToKeyCode(e.button);
            
            if (e.keyCode == cancelKeyBindKeyCode || e.keyCode == KeyCode.None ||
                selectedKeyBindButton.CurrentKeyCode == KeyCode.None)
            {
                selectedKeyBindButton.UpdateTextToPreviousBind();
                selectedKeyIndex = -1;
                
                SetActiveUnusedKeyBinds(true);
                menuInputManager.CanUseMenuControls = true;
                
                return;
            }

            StartCoroutine(WaitForKeyUp(e.keyCode));
        }

        private IEnumerator WaitForKeyUp(KeyCode keyCode)
        {
            while (isActiveAndEnabled)
            {
                if (!Input.GetKey(keyCode)) break;
                yield return null;
            }
            
            selectedKeyBindButton.CurrentKeyCode = keyCode;
            selectedKeyIndex = -1;
            
            SetActiveUnusedKeyBinds(true);
            menuInputManager.CanUseMenuControls = true;
        }

        private KeyBindButton FindSelectedKeyBind()
        {
            return keyBindButtons.Find(k => k.CurrentIndexInMenu == selectedKeyIndex);
        }
    }
}