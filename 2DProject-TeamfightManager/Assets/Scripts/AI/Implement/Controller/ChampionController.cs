using MH_AIFramework;

/// <summary>
/// ChampionBT를 생성하고 실행시켜야 할 BehaviourTree로 넣는 역할이 전부다..
/// 나머지는 AIController가 알아서 한다..
/// </summary>
public class ChampionController : AIController
{
    // Start is called before the first frame update
    new private void Awake()
    {
        _behaviourTree = new ChampionBT(this);

		base.Awake();
    }
}
