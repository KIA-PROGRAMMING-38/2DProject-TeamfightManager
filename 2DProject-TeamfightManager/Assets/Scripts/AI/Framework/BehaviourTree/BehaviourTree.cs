using UnityEngine;

namespace MH_AIFramework
{
	/// <summary>
	/// AI를 위한 행동 트리..
	/// 여기에는 각 AI의 행동을 위한 Node들이 트리의 형태로 구성되어 있다..
	/// 얘는 그저 실행만 시키는 역할이며 조건, 행동 같은 것들은 Node들이 담당한다..
	/// </summary>
	public class BehaviourTree
	{
		protected Node rootNode { get; private set; }
		public Blackboard blackboard { get; private set; }
		public GameObject gameObject { get; private set; }

		public BehaviourTree(GameObject ownerGameObject, Blackboard blackboard)
		{
			//this.aiController = aiController;
			this.gameObject = ownerGameObject;

			this.blackboard = blackboard;
			rootNode = new RootNode();

			rootNode.blackboard = blackboard;
			rootNode.behaviourTree = this;
		}

		public void Run()
		{
			if (null != rootNode)
				rootNode.Update();
		}

		public virtual void OnEnable()
		{
			rootNode?.OnEnable();
		}

		public virtual void OnDisable()
		{
			rootNode?.OnDisable();
		}

		protected Node AddNode( Node newNode, Node parent, string name = "")
		{
#if UNITY_EDITOR
			// 유효한 값인지 검사..
			Debug.Assert( null != newNode );
			Debug.Assert( null != parent );
#endif

			newNode.blackboard = blackboard;
			newNode.behaviourTree = this;
			newNode.nodeName = name;

			if ( null != parent )
				parent.AddChild( newNode );

			newNode.OnCreate();

			return newNode;
		}
	}
}