using System.Collections.Generic;
using UnityEngine;

namespace MH_AIFramework
{
	[System.Serializable]
	public abstract class CompositeNode : Node
	{
		[SerializeReference] protected List<Node> _children = new List<Node>();
		[SerializeField] protected List<DecoratorNode> _decoratorChildren = new List<DecoratorNode>();
		[SerializeField] protected List<ServiceNode> _serviceChildren = new List<ServiceNode>();

		public override void AddChild( Node child )
		{
			// Decorator Node 인지 체크..
			{
				DecoratorNode decoratorNode = child as DecoratorNode;
				if ( null != decoratorNode )
				{
					_decoratorChildren.Add( decoratorNode );

					return;
				}
			}

			// Service Node 인지 체크..
			{
				ServiceNode serviceNode = child as ServiceNode;
				if ( null != serviceNode )
				{
					_serviceChildren.Add( serviceNode );

					return;
				}
			}

			// 여기까지 왔다면 평범한 노드다..
			{
				_children.Add( child );
			}
		}

		public override void RemoveChild( Node child )
		{
			// Decorator Node 인지 체크..
			{
				DecoratorNode decoratorNode = child as DecoratorNode;
				if ( null != decoratorNode )
				{
					_decoratorChildren.Remove( decoratorNode );

					return;
				}
			}

			// Service Node 인지 체크..
			{
				ServiceNode serviceNode = child as ServiceNode;
				if ( null != serviceNode )
				{
					_serviceChildren.Remove( serviceNode );

					return;
				}
			}

			// 여기까지 왔다면 평범한 노드다..
			{
				_children.Remove( child );
			}
		}

		protected override State OnUpdate()
		{
			if ( false == CheckDecoratorCondition() )
				return State.Failure;
			//if ( 0 == _children.Count )
			//	return State.Failure;

			return State.Success;
		}

		private bool CheckDecoratorCondition()
		{
			foreach ( DecoratorNode decoratorNode in _decoratorChildren )
			{
				if ( State.Failure == decoratorNode.Update() )
					return false;
			}

			return true;
		}

		protected void OnUpdateServiceNodes()
		{
			foreach ( ServiceNode serviceNode in _serviceChildren )
			{
				serviceNode.Update();
			}
		}
	}
}