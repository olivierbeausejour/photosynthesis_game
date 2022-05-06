//Authors:
//Charles Tremblay

using Harmony;
using UnityEngine;

namespace Game
{
    [Findable(R.S.Tag.GameController)]
    public class ActorBouncedEventChannel : MonoBehaviour
    {
        public event ActorBouncedEventHandler OnActorBounced;
        
        public void NotifyActorBounced()
        {
            if (OnActorBounced != null)
                OnActorBounced();
        }
    }
    
    public delegate void ActorBouncedEventHandler();
}