using System.Collections.Generic;
using UnityEngine;

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