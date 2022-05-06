// Author : Derek Pouliot

namespace Game
{
    public class BaseState : IState
    {
        public virtual void Enter()
        {
            // Empty on purpose
        }

        public virtual void Leave()
        {
            // Empty on purpose
        }

        public virtual IState Update()
        {
            return this;
        }
    }
}