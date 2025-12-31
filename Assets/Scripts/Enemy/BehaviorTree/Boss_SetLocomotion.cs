using BehaviorDesigner.Runtime.Tasks;

public class Boss_SetLocomotion : Action
{
    private BossController _boss;
    public bool walk;
    public override void OnAwake() => _boss = GetComponent<BossController>();

    public override TaskStatus OnUpdate()
    {
        if (_boss == null || _boss.IsBusy || _boss.IsDead)
        {
            return TaskStatus.Success; // 忙/死亡时不修改状态
        }

        _boss.SetWalk(walk);
        return TaskStatus.Success;
    }
}
