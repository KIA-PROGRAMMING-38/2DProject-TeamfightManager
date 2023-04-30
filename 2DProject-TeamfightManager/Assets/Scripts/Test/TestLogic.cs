using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestLogic : MonoBehaviour
{
	public Collider[] colliders;

	private void Awake()
	{
		colliders = new Collider[2];
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			int a = Physics.OverlapSphereNonAlloc(transform.position, 1000f, colliders);
			Debug.Log(a);
		}
	}
}
