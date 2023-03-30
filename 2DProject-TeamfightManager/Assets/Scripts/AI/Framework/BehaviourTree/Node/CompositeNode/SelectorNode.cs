using JetBrains.Annotations;
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

			for (int i = 0; i < _childCount; ++i)
			{
				Node child = _children[i];

				switch (child.Update())
				{
					case State.Running:
						_state = State.Running;

						return _state;
					case State.Success:
						_state = State.Success;

						return _state;
				}
			}

			_state = State.Failure;
			return _state;
		}
	}
}