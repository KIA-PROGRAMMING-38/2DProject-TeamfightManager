namespace MH_AIFramework
{
	// Sequence Node : 자식 노드들 중 하나라도 실패를 반환하기 전까지 실행..
	public sealed class SequenceNode : CompositeNode
	{
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

						break;
					case State.Success:
						if (State.Running != _state)
							_state = State.Success;

						break;
					case State.Failure:
						return _state;
					default:
						throw new System.ArgumentException();
				}
			}

			return _state;
		}
	}
}