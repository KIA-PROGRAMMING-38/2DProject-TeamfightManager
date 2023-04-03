using System.IO;
using UnityEngine;

public class Logictest : MonoBehaviour
{
	private string DefaultPath;
	public string FileName;
	public 

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Return))
		{
			string filePath = Path.Combine(DefaultPath, FileName);

			File.CreateText(filePath);
		}
	}
}
