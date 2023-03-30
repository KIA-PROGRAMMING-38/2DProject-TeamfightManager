using UnityEditor.SceneManagement;
using UnityEngine;

namespace MH_AIFramework
{
	public class BehaviourTree : MonoBehaviour
	{
		protected Node _rootNode = null;
		protected Blackboard _blackboard;
		public AIController aiController { get; set; }

		protected void Awake()
		{
			_blackboard = gameObject.AddComponent<Blackboard>();
		}

		protected void Start()
		{
			Debug.Assert( null != _blackboard );

			_rootNode = new RootNode();
			_rootNode.blackboard = _blackboard;
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

			newNode.blackboard = _blackboard;
			newNode.aiController = aiController;

			if ( null != parent )
				parent.AddChild( newNode );

			newNode.OnCreate();

			return newNode;
		}
	}
}