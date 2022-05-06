// Author : Derek Pouliot

namespace Game
{
    public class WaitingState : BaseState
    {
        private BossController boss;
        
        public WaitingState(BossController boss)
        {
            this.boss = boss;
        }

        public override void Enter()
        {
            boss.HasRespawned = false;
        }

        public override IState Update()
        {
            if (boss.IsActivate)
                return new AttackingState(boss);
            
            boss.Fall();

            return this;
        }
    }
}