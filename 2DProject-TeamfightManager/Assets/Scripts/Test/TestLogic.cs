using UnityEngine;

public class TestLogic : MonoBehaviour
{
	public GameObject target;

	private void Start()
	{
		
	}

	public void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			Debug.Log("��");
			target.SetActive(true);
			target.SetActive(false);
		}
	}
}
