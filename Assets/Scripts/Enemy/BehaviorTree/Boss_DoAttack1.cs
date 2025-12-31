using BehaviorDesigner.Runtime.Tasks;

public class Boss_DoAttack1 : Action
{
    private BossController _boss;
    private bool _started;

    public override void OnAwake() => _boss = GetComponent<BossController>();

    public override void OnStart() => _started = false;

    public override TaskStatus OnUpdate()
    {
        if (_boss == null)
        {
            return TaskStatus.Failure;
        }

        // 忙：不要Fail，等
        if (!_started && _boss.IsBusy)
        {
            return TaskStatus.Running;
        }

        // 不忙：尝试启动一次
        if (!_started)
        {
            _started = _boss.DoAttack1();
            return _started ? TaskStatus.Running : TaskStatus.Failure;
        }

        // 已经启动：等忙结束
        return _boss.IsBusy ? TaskStatus.Running : TaskStatus.Success;
    }
}
