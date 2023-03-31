namespace MH_AIFramework
{
	/// <summary>
	/// BehaviourTree의 최상위 Node..
	/// 이 Node 는 무조건 BehaviourTree의 최상위에 있어야 한다..
	/// 여기서 하는 일은 딱히 없으며 이 Node의 자식들에서 본격적인 행동을 실행한다..
	/// </summary>
	public sealed class RootNode : Node
	{
		private Node _childNode;
		private bool _isActive = true;

		public override void AddChild( Node child )
		{
			_childNode = child;
		}

		public override void RemoveChild( Node child )
		{
			_childNode = null;
		}

		public override void OnEnable()
		{
			base.OnEnable();

			_isActive = true;

			_childNode?.OnEnable();
		}

		public override void OnDisable()
		{
			base.OnDisable();

			_isActive = false;

			_childNode?.OnDisable();
		}

		protected override void OnStart()
		{
		}

		protected override void OnStop()
		{
		}

		protected override State OnUpdate()
		{
			if (false == _isActive)
				return State.Failure;

			return _childNode.Update();
		}
	}
}