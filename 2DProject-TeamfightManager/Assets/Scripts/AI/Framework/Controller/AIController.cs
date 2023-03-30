using UnityEngine;
using UnityEngine.PlayerLoop;

namespace MH_AIFramework
{
	public class AIController : MonoBehaviour
	{
		protected BehaviourTree _behaviourTree = null;

		protected void Awake()
		{
			Debug.Assert( null != _behaviourTree );

			_behaviourTree.aiController = this;
		}

		protected void Update()
		{
			if(null != _behaviourTree)
			{
				_behaviourTree.Run();
			}
		}
	}
}
