using BehaviorDesigner.Runtime.Tasks;

public class Boss_FacePlayer : Action
{
    private BossController _boss;
    public override void OnAwake() => _boss = GetComponent<BossController>();

    public override TaskStatus OnUpdate()
    {
        if (_boss == null)
        {
            return TaskStatus.Failure;
        }

        _boss.FacePlayer();
        return TaskStatus.Success;
    }
}
