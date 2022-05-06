//Authors:
//Charles Tremblay
//Anthony Dodier

using UnityEngine;

namespace Game
{
    public interface ITriggerEnter
    {
        void OnTriggerDetected(Collider2D other);
    }
}