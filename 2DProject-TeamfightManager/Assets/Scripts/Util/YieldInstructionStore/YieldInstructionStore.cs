using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 YieldInstruction을 캐싱해 필요한 곳에서 사용할 수 있도록 하기 위한 클래스..
/// </summary>
public static class YieldInstructionStore
{
	private static readonly Dictionary<float, WaitForSeconds> _waitForSeconds = new Dictionary<float, WaitForSeconds>();

	public static WaitForSeconds GetWaitForSec(float second)
	{
		WaitForSeconds outValue = null;

		if (false == _waitForSeconds.TryGetValue(second, out outValue))
		{
			_waitForSeconds.Add(second, outValue = new WaitForSeconds(second));
		}

		return outValue;
	}

	class FloatComparer : IEqualityComparer<float>
	{
		bool IEqualityComparer<float>.Equals(float x, float y)
		{
			return x == y;
		}
		int IEqualityComparer<float>.GetHashCode(float obj)
		{
			return obj.GetHashCode();
		}
	}
}