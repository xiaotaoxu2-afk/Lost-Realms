using BehaviorDesigner.Runtime.Tasks;

public class Boss_PlayerInAttack1Range : Conditional
{
    private BossController _boss;
    public override void OnAwake() => _boss = GetComponent<BossController>();

    public override TaskStatus OnUpdate() =>
        _boss != null && _boss.PlayerInAttack1Range() ? TaskStatus.Success : TaskStatus.Failure;
}
