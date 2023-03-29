using UnityEngine;

namespace MH_AIFramework
{
	[System.Serializable]
	public abstract class ActionNode : Node
	{
		public override sealed void AddChild( Node child )
		{
			throw new System.ArgumentException();
		}

		public override sealed void RemoveChild( Node child )
		{
			throw new System.ArgumentException();
		}
	}
}