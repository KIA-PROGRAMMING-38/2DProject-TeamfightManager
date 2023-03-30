using System.Collections.Generic;
using UnityEngine;

namespace MH_AIFramework
{
	public abstract class CompositeNode : Node
	{
		protected List<Node> _children = new List<Node>();
		protected int _childCount = 0;

		private List<DecoratorNode> _decoratorChildren = new List<DecoratorNode>();
		private int _decoratorChildCount = 0;

		private List<ServiceNode> _serviceChildren = new List<ServiceNode>();
		private int _serviceChildCount = 0;

		public override void AddChild( Node child )
		{
			// Decorator Node 인지 체크..
			{
				DecoratorNode decoratorNode = child as DecoratorNode;
				if ( null != decoratorNode )
				{
					_decoratorChildren.Add( decoratorNode );
					++_decoratorChildCount;

					return;
				}
			}

			// Service Node 인지 체크..
			{
				ServiceNode serviceNode = child as ServiceNode;
				if ( null != serviceNode )
				{
					_serviceChildren.Add( serviceNode );
					++_serviceChildCount;

					return;
				}
			}

			// 여기까지 왔다면 평범한 노드다..
			{
				_children.Add( child );
				++_childCount;
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
					_decoratorChildCount = _decoratorChildren.Count;

					return;
				}
			}

			// Service Node 인지 체크..
			{
				ServiceNode serviceNode = child as ServiceNode;
				if ( null != serviceNode )
				{
					_serviceChildren.Remove( serviceNode );
					_serviceChildCount = _serviceChildren.Count;

					return;
				}
			}

			// 여기까지 왔다면 평범한 노드다..
			{
				_children.Remove( child );
				_childCount = _children.Count;
			}
		}

		public override void OnDisable()
		{
			base.OnDisable();

			for( int i = 0; i < _childCount; ++i)
			{
				_children[i].OnDisable();
			}

			for (int i = 0; i < _decoratorChildCount; ++i)
			{
				_decoratorChildren[i].OnDisable();
			}

			for (int i = 0; i < _serviceChildCount; ++i)
			{
				_serviceChildren[i].OnDisable();
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
			for(int i = 0; i < _decoratorChildCount; ++i)
			{
				if (State.Failure == _decoratorChildren[i].Update())
					return false;
			}

			return true;
		}

		protected void OnUpdateServiceNodes()
		{
			for (int i = 0; i < _serviceChildCount; ++i)
			{
				_serviceChildren[i].Update();
			}
		}
	}
}