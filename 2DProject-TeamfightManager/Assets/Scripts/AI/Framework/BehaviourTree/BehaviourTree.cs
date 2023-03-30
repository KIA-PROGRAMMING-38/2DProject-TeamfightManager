using UnityEngine;

namespace MH_AIFramework
{
	public class BehaviourTree : MonoBehaviour
	{
		protected Node _rootNode = null;
		public Blackboard blackboard { get; set; }
		public AIController aiController { get; set; }

		protected void Start()
		{
			Debug.Assert( null != blackboard );

			_rootNode = new RootNode();
			_rootNode.blackboard = blackboard;
			_rootNode.aiController = aiController;

			_rootNode.AddChild( new SelectorNode() );
		}

		protected void OnEnable()
		{
			_rootNode?.OnEnable();
		}

		protected void OnDisable()
		{
			_rootNode?.OnDisable();
		}

		public void Run()
		{
			if ( null != _rootNode )
				_rootNode.Update();
		}

		protected Node AddNode( Node newNode, Node parent )
		{
			// 유효한 값인지 검사..
			Debug.Assert( null != newNode );
			Debug.Assert( null != parent );

			newNode.blackboard = blackboard;
			newNode.aiController = aiController;

			if ( null != parent )
				parent.AddChild( newNode );

			newNode.OnCreate();

			return newNode;
		}
	}
}