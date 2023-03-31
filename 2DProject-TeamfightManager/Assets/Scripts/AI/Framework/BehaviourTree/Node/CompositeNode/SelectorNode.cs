using JetBrains.Annotations;
using UnityEngine;

namespace MH_AIFramework
{
	/// <summary>
	/// 자식들 중 하나라도 성공이라면 성공을 반환..
	/// </summary>
	public sealed class SelectorNode : CompositeNode
	{
		private int _curIndex = 0;

		protected override void OnStart()
		{
			_curIndex = 0;
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

			while(_curIndex < _childCount)
			{
				Node child = _children[_curIndex];

				switch (child.Update())
				{
					case State.Running:
						_state = State.Running;

						return _state;
					case State.Success:
						_state = State.Success;

						return _state;
				}

				++_curIndex;
			}

			_state = State.Failure;
			return _state;
		}
	}
}