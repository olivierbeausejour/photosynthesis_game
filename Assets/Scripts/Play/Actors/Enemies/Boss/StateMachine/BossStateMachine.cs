// Author : Derek Pouliot

namespace Game
{
    public class BossStateMachine
    {
        private IState currentState;

        public IState CurrentState => currentState;

        public BossStateMachine(IState startState)
        {
            currentState = startState;
            currentState.Enter();
        }

        public void Update()
        {
            var nextState = currentState.Update();

            if (nextState != currentState)
            {
                currentState?.Leave();
                currentState = nextState;
                currentState?.Enter();
            }
        }
    }
}