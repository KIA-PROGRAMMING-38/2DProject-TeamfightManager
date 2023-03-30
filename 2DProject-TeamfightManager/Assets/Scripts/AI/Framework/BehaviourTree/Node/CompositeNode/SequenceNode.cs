using UnityEngine;

namespace MH_AIFramework
{
	// Sequence Node : 자식 노드들 중 하나라도 실패를 반환하기 전까지 실행..
	[System.Serializable]
	public sealed class SequenceNode : CompositeNode
	{
		[HideInInspector] private int _curUpdateChildIndex = 0;

		protected override void OnStart()
		{

		}

		protected override void OnStop()
		{
			
		}

		protected override State OnUpdate()
		{
			// Decorator Node 들의 조건을 만족하는지 검사..
			if ( State.Failure == base.OnUpdate() )
				return State.Failure;

			// 모든 Service Node 들 Update..
			OnUpdateServiceNodes();

			if (_childCount > 0 )
			{
				Node child = _children[_curUpdateChildIndex];

				switch ( child.Update() )
				{
					case State.Running:
						_state = State.Running;
						break;
					case State.Success:
						_state = State.Success;
						_curUpdateChildIndex = (_curUpdateChildIndex + 1 == _childCount) ? 0 : _curUpdateChildIndex + 1;
						break;
					case State.Failure:
						_state = State.Failure;
						_curUpdateChildIndex = 0;
						return _state;
					default:
						_state = State.Failure;
						_curUpdateChildIndex = 0;
						return _state;
				}
			}

			return _state;
		}
	}
}