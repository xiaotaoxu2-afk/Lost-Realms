using BehaviorDesigner.Runtime.Tasks;

public class Boss_DoAttack2 : Action
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

        if (!_started && _boss.IsBusy)
        {
            return TaskStatus.Running;
        }

        if (!_started)
        {
            _started = _boss.DoAttack2();
            return _started ? TaskStatus.Running : TaskStatus.Failure;
        }

        return _boss.IsBusy ? TaskStatus.Running : TaskStatus.Success;
    }
}
