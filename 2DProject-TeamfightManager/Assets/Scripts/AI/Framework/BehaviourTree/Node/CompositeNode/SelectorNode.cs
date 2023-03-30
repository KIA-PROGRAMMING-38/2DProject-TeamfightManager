using UnityEngine;

namespace MH_AIFramework
{
	// Selector Node : 자식들 중 하나라도 성공이라면 성공을 반환..
	[System.Serializable]
	public sealed class SelectorNode : CompositeNode
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

				_state = child.Update();
				switch ( _state )
				{
					case State.Running:
					case State.Success:
						_curUpdateChildIndex = 0;
						return _state;
					case State.Failure:
						_state = State.Failure;
						_curUpdateChildIndex = (_curUpdateChildIndex + 1 == _childCount) ? 0 : _curUpdateChildIndex + 1;
						break;
					default:
						throw new System.ArgumentException();
				}
			}

			return _state;
		}
	}
}