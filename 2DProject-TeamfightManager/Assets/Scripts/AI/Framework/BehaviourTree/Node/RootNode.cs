using UnityEngine;

namespace MH_AIFramework
{
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