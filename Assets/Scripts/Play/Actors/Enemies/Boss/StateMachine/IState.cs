// Author : Derek Pouliot

namespace Game
{
    public interface IState
    {
        void Enter();
        void Leave();
        IState Update();
    }
}