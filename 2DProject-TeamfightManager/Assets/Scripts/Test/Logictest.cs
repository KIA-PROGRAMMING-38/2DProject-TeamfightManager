using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logictest : MonoBehaviour
{
    public int TestParam;

	public void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			GetComponent<Animator>().SetInteger("Test", TestParam);
		}
	}

	public void TestAnimEvent(string value)
    {
		Debug.Log($"�ִϸ��̼� �̺�Ʈ : {value}");

		GetComponent<Animator>().SetInteger("Test", 0);
    }
}
