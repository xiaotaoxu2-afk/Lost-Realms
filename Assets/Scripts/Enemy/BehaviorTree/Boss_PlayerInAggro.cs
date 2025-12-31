using BehaviorDesigner.Runtime.Tasks;

public class Boss_PlayerInAggro : Conditional
{
    private BossController _boss;
    public override void OnAwake() => _boss = GetComponent<BossController>();

    public override TaskStatus OnUpdate()
    {
        if (_boss == null)
        {
            return TaskStatus.Failure;
        }

        return _boss.PlayerInAggro() ? TaskStatus.Success : TaskStatus.Failure;
    }
}
