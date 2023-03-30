using MH_AIFramework;
using UnityEngine;

public class ChampionController : AIController
{
    // Start is called before the first frame update
    new private void Awake()
    {
        _behaviourTree = gameObject.AddComponent<ChampionBT>();

		base.Awake();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }
}
