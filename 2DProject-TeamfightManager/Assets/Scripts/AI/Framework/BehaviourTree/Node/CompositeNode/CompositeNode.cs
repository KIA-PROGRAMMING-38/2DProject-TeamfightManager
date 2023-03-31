using System.Collections.Generic;
using UnityEngine;

namespace MH_AIFramework
{
	/// <summary>
	/// 복합 노드(자식이 여러개인 노드)
	/// 이 클래스를 상속받아 시퀀스 셀렉터 노드를 정의함..
	/// </summary>
	public abstract class CompositeNode : Node
	{
		// 소속된 Node들(Action, Selector, Sequence 만 들어옴)과 그 개수..
		protected List<Node> _children = new List<Node>();
		protected int _childCount = 0;

		// 소속된 Decorator Node들과 그 개수..
		private List<DecoratorNode> _decoratorChildren = new List<DecoratorNode>();
		private int _decoratorChildCount = 0;

		// 소속된 Service Node들과 그 개수..
		private List<ServiceNode> _serviceChildren = new List<ServiceNode>();
		private int _serviceChildCount = 0;

		/// <summary>
		/// 자식 추가 함수(Decorator 인지 Service 인지 그 외의 Node인지 검사해 알맞게 저장)..
		/// </summary>
		/// <param name="child"> 추가할 자식 Node.. </param>
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

		/// <summary>
		/// 자식 제거 함수(Decorator 인지 Service 인지 그 외의 Node인지 검사해 알맞게 삭제)..
		/// </summary>
		/// <param name="child"> 삭제할 자식 Node.. </param>
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

		public override void OnCreate()
		{
			base.OnCreate();

			_childCount = _children.Count;
			_decoratorChildCount = _decoratorChildren.Count;
			_serviceChildCount = _serviceChildren.Count;
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