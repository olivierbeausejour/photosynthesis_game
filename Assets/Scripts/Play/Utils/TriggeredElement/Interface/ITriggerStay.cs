//Authors:
//Charles Tremblay
//Anthony Dodier

using UnityEngine;

namespace Game
{
    public interface ITriggerStay
    {
        void OnTriggerStayDetected(Collider2D other);
    }
}