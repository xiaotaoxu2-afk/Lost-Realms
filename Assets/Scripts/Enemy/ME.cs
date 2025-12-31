namespace Enemys
{
    public class ME : Enemy
    {
        protected override void Awake()
        {
            base.Awake();
            patrolstate = new MEPatrolState();
            chasestate = new MEChaseState();
        }
    }
}