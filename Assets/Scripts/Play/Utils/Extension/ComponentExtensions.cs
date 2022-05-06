using UnityEngine;

namespace Game
{
    public static class ComponentExtensions
    {
        public static bool IsDestroyed(this Component component)
        {
            return !component;
        }
        
        public static bool IsNotDestroyed(this Component component)
        {
            return !component.IsDestroyed();
        }
    }
}