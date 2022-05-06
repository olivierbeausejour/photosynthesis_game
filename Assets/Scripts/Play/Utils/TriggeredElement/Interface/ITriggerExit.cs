//Authors:
//Charles Tremblay
//Anthony Dodier

using UnityEngine;

namespace Game
{
    public interface ITriggerExit
    {
        void OnTriggerExitDetected(Collider2D other);
    }
}