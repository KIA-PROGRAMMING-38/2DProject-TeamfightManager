using UnityEngine;

namespace MH_AIFramework
{
	[System.Serializable]
	public sealed class RootNode : Node
	{
		[SerializeReference] private Node _childNode;

		public override void AddChild( Node child )
		{
			_childNode = child;
		}

		public override void RemoveChild( Node child )
		{
			_childNode = null;
		}

		protected override void OnStart()
		{
		}

		protected override void OnStop()
		{
		}

		protected override State OnUpdate()
		{
			return _childNode.Update();
		}
	}
}