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
			Debug.Log("³ª");
			target.SetActive(true);
			target.SetActive(false);
		}
	}
}
