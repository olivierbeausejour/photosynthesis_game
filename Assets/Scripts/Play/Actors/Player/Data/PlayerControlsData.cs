using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class PlayerControlsData
    {
        private List<SerializableKeyBindAssociation> savedKeyBinds;
        private List<SerializableBooleanButtonAssociation> savedBooleanButtons;

        public IReadOnlyList<SerializableKeyBindAssociation> SavedKeyBinds => savedKeyBinds;
        public List<SerializableBooleanButtonAssociation> SavedBooleanButtons => savedBooleanButtons;

        public PlayerControlsData()
        {
            savedKeyBinds = new List<SerializableKeyBindAssociation>();
            savedBooleanButtons = new List<SerializableBooleanButtonAssociation>();
        }

        public void AddKeyBindData(KeyBindButton.KeyProperty property, KeyBindButton.KeyActionType type, KeyCode keyCode,
            GamepadManager.Button button, GamepadManager.Axis axis)
        {
            savedKeyBinds.Add(new SerializableKeyBindAssociation(property, type, keyCode, button, axis));
        }

        public void AddBooleanButtonData(BooleanButton.KeyActionType type, bool value)
        {
            savedBooleanButtons.Add(new SerializableBooleanButtonAssociation(type, value));
        }

        public void Reset()
        {
            savedKeyBinds.Clear();
        }

        [Serializable]
        public class SerializableKeyBindAssociation
        {
            public KeyBindButton.KeyProperty property;
            public KeyBindButton.KeyActionType type;
            public KeyCode keyCode;
            public GamepadManager.Button button;
            public GamepadManager.Axis axis;
            
            public SerializableKeyBindAssociation(KeyBindButton.KeyProperty property, KeyBindButton.KeyActionType type, KeyCode keyCode,
                GamepadManager.Button button, GamepadManager.Axis axis)
            {
                this.property = property;
                this.type = type;
                this.keyCode = keyCode;
                this.button = button;
                this.axis = axis;
            }
        }

        [Serializable]
        public class SerializableBooleanButtonAssociation
        {
            public BooleanButton.KeyActionType type;
            public bool value;

            public SerializableBooleanButtonAssociation(BooleanButton.KeyActionType type, bool value)
            {
                this.type = type;
                this.value = value;
            }
        }
    }
}