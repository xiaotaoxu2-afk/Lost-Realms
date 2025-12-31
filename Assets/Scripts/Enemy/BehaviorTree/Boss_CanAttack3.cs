using BehaviorDesigner.Runtime.Tasks;

public class Boss_CanAttack3 : Conditional
{
    private BossController _boss;
    public override void OnAwake() => _boss = GetComponent<BossController>();

    public override TaskStatus OnUpdate() =>
        _boss != null && _boss.CanAttack3() ? TaskStatus.Success : TaskStatus.Failure;
}
