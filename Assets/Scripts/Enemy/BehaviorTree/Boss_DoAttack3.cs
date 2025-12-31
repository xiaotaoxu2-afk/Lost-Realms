using BehaviorDesigner.Runtime.Tasks;

public class Boss_DoAttack3 : Action
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
            _started = _boss.DoAttack3_Start();
            return _started ? TaskStatus.Running : TaskStatus.Failure;
        }

        // 你原注释是等动画事件 FinishAttack() 解锁 :contentReference[oaicite:6]{index=6}
        return _boss.IsBusy ? TaskStatus.Running : TaskStatus.Success;
    }
}
