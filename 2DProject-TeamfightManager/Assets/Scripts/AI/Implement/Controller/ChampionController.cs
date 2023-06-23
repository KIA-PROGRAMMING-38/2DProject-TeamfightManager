using MH_AIFramework;

/// <summary>
/// ChampionBT�� �����ϰ� ������Ѿ� �� BehaviourTree�� �ִ� ������ ���δ�..
/// �������� AIController�� �˾Ƽ� �Ѵ�..
/// </summary>
public class ChampionController : AIController
{
    // Start is called before the first frame update
    new private void Awake()
    {
        _behaviourTree = new ChampionBT(gameObject, blackboard);

		base.Awake();
    }
}
