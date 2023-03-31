using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 YieldInstruction을 캐싱해 필요한 곳에서 사용할 수 있도록 하기 위한 클래스..
/// </summary>
public static class YieldInstructionStore
{
	private static Dictionary<float, WaitForSeconds> _waitForSeconds = new Dictionary<float, WaitForSeconds>();

	public static WaitForSeconds GetWaitForSec(float second)
	{
		if( false == _waitForSeconds.ContainsKey(second) )
			_waitForSeconds.Add(second, new WaitForSeconds(second));

		return _waitForSeconds[second];
	}
}