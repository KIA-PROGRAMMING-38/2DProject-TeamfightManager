using MH_AIFramework;
using UnityEngine;

[RequireComponent(typeof(ChampionBT))]
[RequireComponent(typeof(ChampionBlackboard))]
public class ChampionController : AIController
{
    // Start is called before the first frame update
    new private void Awake()
    {
        _behaviourTree = GetComponent<BehaviourTree>();
        _blackboard = GetComponent<Blackboard>();

		base.Awake();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }
}
