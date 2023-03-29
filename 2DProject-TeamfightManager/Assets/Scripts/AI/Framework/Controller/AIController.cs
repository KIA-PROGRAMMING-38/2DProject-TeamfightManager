using UnityEngine;
using UnityEngine.PlayerLoop;

namespace MH_AIFramework
{
	[System.Serializable]
	public class AIController : MonoBehaviour
	{
		protected BehaviourTree _behaviourTree = null;
		protected Blackboard _blackboard = null;

		public Blackboard blackboard { get { return _blackboard; } }

		protected void Awake()
		{
			Debug.Assert( null != _behaviourTree );
			Debug.Assert( null != _blackboard );

			_behaviourTree.blackboard = _blackboard;
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
